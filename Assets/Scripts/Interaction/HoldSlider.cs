using DG.Tweening;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;


namespace Baloon
{
    using UnityEngine;

    public class HoldSlider : MonoBehaviour
    {
        [SerializeField] Interactor interactor;
        [SerializeField] GameObject handle;
        [SerializeField] Transform start, stop;

        [Range(0, 1)]
        public float sliderValue;

        Plane interactionPlane;
        bool isDragging = false;
        float offsetOnDown; // Memorizza la differenza tra mouse e posizione attuale

        private void Update()
        {
            if (isDragging) UpdateSliderPosition();
        }

        protected virtual void OnEnable()
        {
            Interactor.OnInteractionStarted += Push;
            Interactor.OnInteractionStopped += Release;
        }

        protected virtual void OnDisable()
        {
            Interactor.OnInteractionStarted -= Push;
            Interactor.OnInteractionStopped -= Release;
        }

        protected virtual void Release(Interactor interactor)
        {
            if (this.interactor != interactor) return;
            isDragging = false;
        }

        protected virtual void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            // Creiamo il piano di interazione
            interactionPlane = new Plane(-Camera.main.transform.forward, handle.transform.position);

            // Calcoliamo dove si trova il mouse rispetto allo 0-1 nel momento del click
            float tMouse = GetMouseTValue();

            // Salviamo l'offset rispetto al valore attuale dello slider
            offsetOnDown = tMouse - sliderValue;

            isDragging = true;
        }

        void UpdateSliderPosition()
        {
            float tMouse = GetMouseTValue();

            // Applichiamo il valore del mouse compensando l'offset iniziale
            sliderValue = Mathf.Clamp01(tMouse - offsetOnDown);

            // Aggiorniamo la posizione (Usando Vector3.Lerp per gestire tutti gli assi locali)
            handle.transform.localPosition = Vector3.Lerp(start.localPosition, stop.localPosition, sliderValue);
        }

        // Metodo helper per proiettare il mouse sulla retta start-stop
        float GetMouseTValue()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (interactionPlane.Raycast(ray, out float enter))
            {
                Vector3 mouseWorldPos = ray.GetPoint(enter);

                // Calcolo in coordinate locali o globali? 
                // Usiamo le globali per il calcolo del Dot, ma i punti start/stop devono essere coerenti
                Vector3 fullPath = stop.position - start.position;
                Vector3 mouseFromStart = mouseWorldPos - start.position;

                return Vector3.Dot(mouseFromStart, fullPath) / fullPath.sqrMagnitude;
            }
            return sliderValue;
        }
    }
}