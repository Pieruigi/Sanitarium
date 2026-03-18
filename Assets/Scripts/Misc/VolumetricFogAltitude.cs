using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class VolumetricFogAltitude : MonoBehaviour
{
    [SerializeField]
    Volume globalVolume;

    GameObject player;

    VolumetricFogVolumeComponent fog;

    float offset;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(globalVolume.profile.TryGet<VolumetricFogVolumeComponent>(out fog))
        {
            Debug.Log("BASEHeight:" + fog.baseHeight);
            Debug.Log("Max:" + fog.maximumHeight);
            offset = fog.maximumHeight.value; 
        }

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        var dist = player.transform.position.y - fog.baseHeight.value;
        if(Mathf.Abs(dist) > 10)
        {
            
        }

        fog.baseHeight.value = player.transform.position.y;
        fog.maximumHeight.value = fog.baseHeight.value + offset;
    }
}
