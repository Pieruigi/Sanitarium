using StarterAssets;
using UnityEngine;

namespace Baloon
{
    public class BaloonDestroyer : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rbBasket, rbBoiler, rbBalloon;

        private void Awake()
        {
            rbBasket.isKinematic = true;
            rbBoiler.isKinematic = true;
            rbBalloon.isKinematic = true;

            rbBasket.useGravity = false;
            rbBoiler.useGravity = false;
            rbBalloon.useGravity = false;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            FirstPersonController.OnDead += HandleOnDead;
        }
        private void OnDisable()
        {
            FirstPersonController.OnDead -= HandleOnDead;
        }

        private void HandleOnDead(PlayerDeadType deadType)
        {

            if (deadType != PlayerDeadType.KillerWind) return;
            
            // Destroy the balloon
            // Activate all rigidbodies
            rbBasket.isKinematic = false;
            rbBoiler.isKinematic = false;
            rbBalloon.isKinematic = false;
            rbBalloon.useGravity = true;
            rbBoiler.useGravity = true;
            rbBasket.useGravity = true;

            Vector3 dir = GetRandomDir();

            rbBasket.AddForce(dir * 3, ForceMode.VelocityChange);
            rbBasket.AddTorque(dir, ForceMode.VelocityChange);

            dir = GetRandomDir();
            rbBoiler.AddForce(dir * 3, ForceMode.VelocityChange);
            rbBoiler.AddTorque(dir, ForceMode.VelocityChange);

            dir = GetRandomDir();
            rbBalloon.AddForce(dir * 3, ForceMode.VelocityChange);
            rbBalloon.AddTorque(dir, ForceMode.VelocityChange);
        }

        Vector3 GetRandomDir()
        {
            return (Random.Range(0, 2) == 0 ? Vector3.right : Vector3.left) * Random.Range(1f, 2f) +
                          (Random.Range(0, 2) == 0 ? Vector3.up : Vector3.down) * Random.Range(1f, 2f) +
                          (Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back) * Random.Range(1f, 2f);
        }
    }
}