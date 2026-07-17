using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Baloon.UI
{
    [RequireComponent(typeof(Button))]
    public class WishlistFlicker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Components")]
        [SerializeField] private Button targetButton;

        [Header("Flicker Settings")]
        [SerializeField] private float minInterval = 2f;     // Minimum time between flickers
        [SerializeField] private float maxInterval = 5f;     // Maximum time between flickers
        [SerializeField] private float flickerDuration = 0.15f; // How long the dimmed flash lasts

        // RGBA: 192, 192, 192, 255 (Your default semi-transparent / grayish white)
        private readonly Color defaultNormalColor = new Color(0.753f, 0.753f, 0.753f, .3f);
        // A lower alpha or darker shade for the flicker effect
        private readonly Color flickerNormalColor = new Color(0.753f, 0.753f, 0.753f, .8f);

        private bool isMouseOver = false;
        private Coroutine flickerCoroutine;

        private void Start()
        {
            if (targetButton == null)
            {
                targetButton = GetComponent<Button>();
            }

            // Ensure the button starts with your default normal color
            SetButtonNormalColor(defaultNormalColor);

            // Start the flickering loop
            flickerCoroutine = StartCoroutine(FlickerLoop());
        }

        private IEnumerator FlickerLoop()
        {
            while (true)
            {
                // Wait for a random time before the next flicker
                yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

                // Only flicker if the mouse is NOT hovering over the button
                if (!isMouseOver)
                {
                    // Quick dimmed flash on the normal state color
                    SetButtonNormalColor(flickerNormalColor);
                    yield return new WaitForSeconds(flickerDuration);
                    SetButtonNormalColor(defaultNormalColor);

                    // Optional: second quick micro-flash for a glitchy neon feel
                    if (Random.value > 0.5f)
                    {
                        yield return new WaitForSeconds(0.05f);
                        SetButtonNormalColor(flickerNormalColor);
                        yield return new WaitForSeconds(0.05f);
                        SetButtonNormalColor(defaultNormalColor);
                    }
                }
            }
        }

        // Helper method to safely change only the normal color within Unity's ColorBlock
        private void SetButtonNormalColor(Color newColor)
        {
            ColorBlock cb = targetButton.colors;
            cb.normalColor = newColor;
            targetButton.colors = cb;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseOver = true;
            // Instantly restore full default normal color when hovered so it transitions cleanly
            SetButtonNormalColor(defaultNormalColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseOver = false;
        }

        private void OnDisable()
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
            }
        }
    }
}