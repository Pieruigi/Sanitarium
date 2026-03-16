using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.Events;

namespace Baloon
{
    public class HoldSlider : MonoBehaviour
    {
        public UnityAction OnDragStarted;
        public UnityAction OnDragStopped;

        public delegate void ValueChangedDelegate(float value);
        public ValueChangedDelegate OnValueChanged;

        [Header("References")]
        [SerializeField] Interactor interactor;
        [SerializeField] GameObject handle;
        [SerializeField] Transform start, stop;

        [Header("Settings")]
        [Range(0, 1)] public float sliderValue;
        [SerializeField, Range(0, 1)] float lerpSpeed = 0.2f;
        

        private float targetSliderValue;
        private Vector3 localPath;
        private float localPathSqMagnitude;
        private Vector3 localStartPos;

        private bool isDragging = false;
        private bool locked = false;
        private float offsetOnDown;

        bool isStarting = false;

        public bool Locked { get => locked; set => locked = value; }

        Vector3 handleVelocity;

        private void Start()
        {
        

            // Inizializziamo i dati locali rispetto al "Nonno" (la cesta)
            Transform parentTransform = start.parent;
            localStartPos = parentTransform.InverseTransformPoint(start.position);
            Vector3 localStopPos = parentTransform.InverseTransformPoint(stop.position);

            localPath = localStopPos - localStartPos;
            localPathSqMagnitude = localPath.sqrMagnitude;

            targetSliderValue = sliderValue;
        }

        private void FixedUpdate()
        {
            if (isStarting)
            {
                isStarting = false;
                // RESET: Sincronizziamo il target alla posizione visiva attuale per bloccare scatti
                targetSliderValue = sliderValue;

                // Calcoliamo dove si trova il mouse RISPETTO alla manetta in questo istante
                float tMouseAtClick = GetMouseTValue();

                // Salviamo la differenza (offset) per "ancorare" il mouse alla manetta
                offsetOnDown = tMouseAtClick - targetSliderValue;
                Debug.Log("TEST - OffsetOnDown:" + offsetOnDown);

                isDragging = true;
            }

            if (isDragging)
            {
                UpdateSliderPosition();
            }

            // Il Lerp assorbe il "Noise" della camera e rende il movimento fluido
            sliderValue = Mathf.Lerp(sliderValue, targetSliderValue, lerpSpeed);
           
            float delta = 0.0001f;
            if (sliderValue < delta) sliderValue = 0f;
            else if (sliderValue > 1f - delta) sliderValue = 1f;

            var lPos = Vector3.Lerp(start.localPosition, stop.localPosition, sliderValue);
            handle.transform.localPosition = Vector3.SmoothDamp(handle.transform.localPosition, lPos, ref handleVelocity, .05f);

            //OnValueChanged?.Invoke(sliderValue);
        }

        protected virtual void Push(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            if (!locked)
            {
                handle.transform.DOKill();

                isStarting = true;

                OnDragStarted?.Invoke();
            }
            else
            {
                handle.transform.DOKill();
                handle.transform.DOShakePosition(0.2f, new Vector3(0, 0, 0.005f), 20, 90).OnComplete(() => { handle.transform.localPosition = start.localPosition; });
            }
        }

        private void UpdateSliderPosition()
        {
            float tMouseCurrent = GetMouseTValue();

            // Applichiamo il nuovo valore mantenendo l'ancoraggio iniziale
            targetSliderValue = Mathf.Clamp01(tMouseCurrent - offsetOnDown);

            //targetSliderValue = Mathf.Round(targetSliderValue * 100f) / 100f;


            OnValueChanged?.Invoke(targetSliderValue);
        }

        private float GetMouseTValue()
        {
            // Creiamo un piano virtuale sulla manetta rivolto alla camera
            // Questo piano "viaggia" con la mongolfiera perché centrato su handle.transform.position
            Plane interactionPlane = new Plane(-Camera.main.transform.forward, handle.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (interactionPlane.Raycast(ray, out float enter))
            {
                Vector3 worldHitPoint = ray.GetPoint(enter);

                // Convertiamo il punto d'impatto mondiale in coordinate locali della cesta
                Vector3 localMousePos = start.parent.InverseTransformPoint(worldHitPoint);
                Vector3 mouseFromStart = localMousePos - localStartPos;

                // Proiezione del punto sulla linea dello slider (Risultato tra 0 e 1)
                return Vector3.Dot(mouseFromStart, localPath) / localPathSqMagnitude;
            }

            return targetSliderValue;
        }

        protected virtual void Release(Interactor interactor)
        {
            if (this.interactor != interactor) return;

            isStarting = false;

            if (isDragging)
            {
                isDragging = false;
                OnDragStopped?.Invoke();
            }

            
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

        public void ResetSlider()
        {
            isDragging = false;
            targetSliderValue = 0;
            sliderValue = 0;
            handle.transform.DOKill();
            handle.transform.DOLocalMove(start.localPosition, .1f).SetEase(Ease.OutBack);
            OnValueChanged?.Invoke(sliderValue);
        }
    }
}
