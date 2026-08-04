using System;
using UnityEngine;
using UnityEngine.UI;

namespace Baloon.UI
{
    public class DotUI : MonoBehaviour
    {
        [SerializeField]
        Image dotImage;

        [SerializeField]
        Sprite emptySprite, fullSprite;

        [SerializeField]
        float emptyAlpha, fullAlpha;

        CanvasGroup group;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            SetEmptySprite();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            Interactor.OnHint += HandleOnHint;
        }

        private void OnDisable()
        {
            Interactor.OnHint -= HandleOnHint;
        }

        private void HandleOnHint(Interactor interactor, bool interactable)
        {
            if(interactable) SetFullSprite();
            else SetEmptySprite();
        }

        void SetEmptySprite()
        {
            dotImage.sprite = emptySprite;
            group.alpha = emptyAlpha;
        }

        void SetFullSprite()
        {
            dotImage.sprite = fullSprite;
            group.alpha = fullAlpha;
        }
    }
}