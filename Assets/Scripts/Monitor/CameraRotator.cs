using Baloon;
using UnityEngine;

namespace SNT
{
    using System;
    using UnityEngine;

    public class CameraRotator : MonoBehaviour
    {
        [SerializeField] HoldButton leftButton, rightButton;

        [Header("Rotation Settings")]
        [SerializeField] float maxAngle = 15f;    // Limite rotazione (es. da -15 a +15)
        [SerializeField] float rotationSpeed = 30f; // Gradi al secondo
        [SerializeField] float smoothTime = 0.1f;   // Tempo di smorzamento

        private float currentYAngle = 0f;  // Angolo target
        private float velocity = 0f;       // Usata internamente da SmoothDamp
        private int rotationDirection = 0; // -1 sinistra, 1 destra, 0 fermo

        void Update()
        {
            // 1. Calcoliamo la direzione target basata sui tasti
            float targetAngle = currentYAngle + (rotationDirection * rotationSpeed * Time.deltaTime);

            // 2. Clampiamo l'angolo per non superare i limiti
            currentYAngle = Mathf.Clamp(targetAngle, -maxAngle, maxAngle);

            // 3. Applichiamo la rotazione locale in modo smussato
            // Usiamo SmoothDampAngle per evitare scatti e avere un feeling organico
            float smoothedAngle = Mathf.SmoothDampAngle(transform.parent.localEulerAngles.y, currentYAngle, ref velocity, smoothTime);

            transform.parent.localRotation = Quaternion.Euler(transform.parent.localEulerAngles.x, smoothedAngle, 0);
        }

        private void OnEnable()
        {
            leftButton.OnPushed += HandleOnLeftDown;
            leftButton.OnReleased += HandleOnLeftUp;
            rightButton.OnPushed += HandleOnRightDown;
            rightButton.OnReleased += HandleOnRightUp;
            CameraSwitcher.OnSwitch += HandleOnCameraSwitch;
        }

        private void OnDisable()
        {
            leftButton.OnPushed -= HandleOnLeftDown;
            leftButton.OnReleased -= HandleOnLeftUp;
            rightButton.OnPushed -= HandleOnRightDown;
            rightButton.OnReleased -= HandleOnRightUp;
            CameraSwitcher.OnSwitch -= HandleOnCameraSwitch;
        }

        private void HandleOnCameraSwitch()
        {
            float y = transform.parent.localEulerAngles.y;

            // Converte l'angolo da 0...360 a -180...180
            if (y > 180) y -= 360;
            currentYAngle = y;
            velocity = 0;
        }

        // Gestione degli stati di pressione
        private void HandleOnLeftDown() => rotationDirection = 1;
        private void HandleOnLeftUp() => rotationDirection = 0; 

        private void HandleOnRightDown() => rotationDirection = -1;
        private void HandleOnRightUp() => rotationDirection = 0;
    }
}