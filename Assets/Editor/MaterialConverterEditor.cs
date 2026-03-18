using UnityEngine;
using UnityEditor;
using System.IO;

public class RDH_MaterialConverter : EditorWindow
{
    private string folderPath = "Assets/RDH/Run Down Houses - PSX Style/RDH_Materials";
    private string sourceShaderName = "Unlit/Texture";
    private string destinationShaderName = "Retro Shaders Pro/Retro Lit";
    private string sourceMainTexParam = "_MainTex";
    private string destBaseMapParam = "_BaseMap";

    // Flag per la ricorsività
    private bool isRecursive = false;

    [MenuItem("Tools/Retro/RDH Material Converter")]
    public static void ShowWindow()
    {
        GetWindow<RDH_MaterialConverter>("RDH Converter");
    }

    private void OnGUI()
    {
        GUILayout.Label("RDH Material Converter", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        // UI per il flag ricorsivo
        isRecursive = EditorGUILayout.Toggle("Include Subfolders?", isRecursive);

        EditorGUILayout.Space();
        sourceShaderName = EditorGUILayout.TextField("Source Shader", sourceShaderName);
        destinationShaderName = EditorGUILayout.TextField("Dest Shader", destinationShaderName);

        EditorGUILayout.Space();
        sourceMainTexParam = EditorGUILayout.TextField("Source Tex Param", sourceMainTexParam);
        destBaseMapParam = EditorGUILayout.TextField("Dest Map Param", destBaseMapParam);

        if (GUILayout.Button("CONVERT MATERIALS"))
        {
            ConvertMaterials();
        }
    }

    private void ConvertMaterials()
    {
        // Verifica esistenza cartella
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"[Converter] Folder not found at: {folderPath}");
            return;
        }

        Shader destShader = Shader.Find(destinationShaderName);
        if (destShader == null)
        {
            Debug.LogError($"[Converter] Shader '{destinationShaderName}' not found!");
            return;
        }

        // Trova tutti i materiali nel percorso indicato
        string[] allGuids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        int count = 0;

        // Puliamo il percorso per il confronto
        string targetFolderClean = folderPath.TrimEnd('/');

        foreach (string guid in allGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Logica del Flag Ricorsivo
            if (!isRecursive)
            {
                // Se NON è ricorsivo, controlliamo che la cartella del file sia esattamente quella target
                string assetDirectory = Path.GetDirectoryName(assetPath).Replace('\\', '/');
                if (assetDirectory != targetFolderClean)
                {
                    continue; // Salta i file nelle sottocartelle
                }
            }

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            if (mat != null && mat.shader.name.Contains(sourceShaderName))
            {
                /* * Applying English comments as per your general rule 
                 */

                // 1. Store original texture from _MainTex
                Texture originalTex = mat.GetTexture(sourceMainTexParam);

                // 2. Store original color if available
                Color originalColor = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;

                // 3. Switch shader to Retro Lit
                mat.shader = destShader;

                // 4. Re-assign texture to the new parameter name (_BaseMap)
                if (originalTex != null)
                {
                    mat.SetTexture(destBaseMapParam, originalTex);
                }

                // 5. Re-assign color to URP/Retro standard (_BaseColor)
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", originalColor);
                }

                EditorUtility.SetDirty(mat);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[Converter] Finished! Processed {count} materials (Recursive: {isRecursive}).");
    }
}