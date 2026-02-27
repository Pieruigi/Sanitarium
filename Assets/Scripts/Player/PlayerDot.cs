using SNT.Interfaces;
using System;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace SNT.UI
{
    public class PlayerDot : Singleton<PlayerDot>
    {
        [SerializeField]
        Image dot;

        bool mouseOver = false;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //ShowDot(true);
        }

        // Update is called once per frame
        void Update()
        {
            if(!dot.enabled) return;

            var pos = Input.mousePosition;
            dot.rectTransform.position = pos;

            if(PlayerInteractor.Instance.IsBusy())
            {
                dot.color = new Color(0,0,0,0);
            }
            
        }

        private void OnEnable()
        {
            PlayerInteractor.OnMouseEnter += HandleOnMouseEnter;
            PlayerInteractor.OnMouseExit += HandleOnMouseExit;
        }

        private void OnDisable()
        {
            PlayerInteractor.OnMouseEnter -= HandleOnMouseEnter;
            PlayerInteractor.OnMouseExit -= HandleOnMouseExit;
        }

        private void HandleOnMouseEnter(IInteractable interactable)
        {
            if (mouseOver) return;
            SetMouseOver(true, interactable);
        }

        private void HandleOnMouseExit(IInteractable interactable)
        {
            if(!mouseOver) return;
            SetMouseOver(false, null);
        }

        public void ShowDot(bool value)
        {
            if (dot.enabled == value) return;

            dot.enabled = value;
        }

        public void SetMouseOver(bool value, IInteractable interactable = null)
        {
            if (mouseOver == value) return;
            mouseOver = value;

         
            dot.color = value ? Color.red : Color.white;
        }
    }
}