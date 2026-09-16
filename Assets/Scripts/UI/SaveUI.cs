using Baloon.SaveSystem;
using DG.Tweening;
using System;
using UnityEngine;

namespace Baloon.UI
{
    public class SaveUI : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup group;

        private void Awake()
        {
            group.alpha = 0;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    HandleOnSave();
#endif
        }

        private void OnEnable()
        {
            SaveManager.OnUpdateDataEntry += HandleOnSave;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnSave;
        }

        private void HandleOnSave()
        {
            group.DOFade(1, .5f).SetLoops(6, LoopType.Yoyo).onComplete += ()=> { group.alpha = 0; };
        }
    }
}