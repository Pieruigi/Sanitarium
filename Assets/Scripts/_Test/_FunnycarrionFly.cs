using Baloon;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class _FunnycarrionFly : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    GameObject carrion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(DoLoop());

        }


        transform.position = BaloonController.Instance.transform.position;
        transform.rotation = BaloonController.Instance.transform.rotation;
#endif


    }

    IEnumerator DoLoop()
    {
        while (true)
        {
            BaloonBoilerHealth.Instance.TryTakeSingleDamage();

            //CameraShake.Instance.PlayWrenchHit();

            yield return new WaitForSeconds(1f);
        }
    }
}
