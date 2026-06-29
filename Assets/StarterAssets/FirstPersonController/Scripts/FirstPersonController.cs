using Baloon;
using Baloon.SaveSystem;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
#endif

namespace StarterAssets
{

    public enum PlayerDeadType { KillerWind, BoilerExplosion, CatwalkCollapsing }

    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour//, ISavable
	{
		

		public delegate void DeadDelegate(PlayerDeadType deadType);
		public static DeadDelegate OnDead;

		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

        [SerializeField]
        bool onBaloon = false;
		public bool OnBaloon => onBaloon;
		
		float baloonGround = 0f;
		
		

        // cinemachine
        private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		public bool IsRunning => _input.sprint;

		public bool IsCrouching => false;

		[SerializeField]
		GameObject bloodUI;

		[SerializeField]
		AudioSource bloodAudioSource;

		[SerializeField]
		AudioSource bodyFallAudioSource;

		
		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

        [Header("In-Basket Movement")]
        [SerializeField] private float acceleration = 8f; // How fast you reach max speed
        [SerializeField] private float wallDetectionDistance = 0.4f;
        [SerializeField] private LayerMask wallLayer;
		[SerializeField] private Collider onBasketCollider;
		[SerializeField] private Transform playerTarget;
        private Vector3 currentLocalVelocity;

		bool dead = false;

        [SerializeField]
        float mouseSens = 1f;

		public bool JawDisabled = false;
		public bool PitchDisabled = false;
		public bool MoveDisabled = false;

        // Set true if something is going to kill the player in order to avoid any other killer routine (set true as soon as the routine start);
        // For example when killer wind destroy the balloon, before the player actually dies, tentacles start shaking the balloon, and then we must call Doomed = true at that moment.
        public bool Doomed { get; set; }

        public string SaveId => "Player";

        class Data
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			onBasketCollider.enabled = false;
			bloodUI.SetActive(false);
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			// Check save game
			if(SaveManager.Instance.DataEntryExists(SaveId))
			{
				
				var jsonData = SaveManager.Instance.GetRawJsonData(SaveId);
				var data = JsonUtility.FromJson<Data>(jsonData);

                Debug.Log($"TEST - Entry found - SaveID:{SaveId}, rawData:{jsonData}");

                ForcePosition(data.position);
				transform.rotation = data.rotation;
				
			}
		}



		private void Update()
		{
            
            

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.P))
				Time.timeScale = Time.timeScale == 0 ? 1 : 0;
#endif

			if (dead) return;

			//if (onBaloon)
			//	Physics.SyncTransforms();
			if (!onBaloon)
			{
                JumpAndGravity();
                GroundedCheck();
                Move();
            }
			else
			{
				MoveOnBalloon();

            }
			

            //if (onBaloon) AdjustOnBaloon();
        }

		private void LateUpdate()
		{
            if (dead) return;

			if (onBaloon)
			{
				var pos = transform.localPosition;
				pos.y = baloonGround;
				transform.localPosition = pos;

            }

			
            CameraRotation();
        }

        private void FixedUpdate()
        {
           
        }

        private void OnEnable()
        {
			SaveManager.OnUpdateDataEntry += HandleOnUpdateDataEntry;
        }

        private void OnDisable()
        {
            SaveManager.OnUpdateDataEntry -= HandleOnUpdateDataEntry;
        }

        private void HandleOnUpdateDataEntry()
        {
            // Generate save data
			var rawJson = GenerateSaveData();
			// Create or update save entry
			SaveManager.Instance.CreateOrUpdateDataEntry(SaveId, rawJson);
        }

        string GenerateSaveData()
        {
            var data = new Data();
            data.position = transform.position;
            data.rotation = transform.rotation;
            var rawJson = JsonUtility.ToJson(data);
            return rawJson;
        }


        private void MoveOnBalloon()
        {
            if (MoveDisabled)
                _input.move = Vector2.zero;

            // 2. Smoothly update velocity
            float targetSpeed = 0;
			if (_input.move.magnitude > 0) targetSpeed = MoveSpeed;

			if(_speed != targetSpeed)
			{
				if(targetSpeed > 0)
					_speed = Mathf.Min(MoveSpeed, _speed + acceleration * Time.deltaTime);
				else
                    _speed = Mathf.Max(0, _speed - acceleration * Time.deltaTime);
            }

			var direction = transform.right * _input.move.x + transform.forward * _input.move.y;
			direction = Vector3.ProjectOnPlane(direction, transform.up).normalized;

			var velocity = direction * _speed;
			
			if(_speed > 0)
			{
                // Collisions
                float sphereRadius = _controller.radius;
                Vector3 origin = transform.position + Vector3.up * 0.4f;
                Vector3 castDirection = velocity.normalized;
				// La distanza del cast deve coprire lo spostamento di questo frame + un piccolo margine
				float castDistance = (_speed * Time.deltaTime) + wallDetectionDistance;


                // Prendiamo TUTTE le collisioni davanti a noi in questo frame
                RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, castDirection, castDistance, wallLayer);

                if (hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
						
						// Casta una sfera quindi se sono leggermente entrato in collisione per colpa per esempio dello shaking mi casta anche sulla collisione nella quale sono entrato;
						// per risolvere basta che verifico che la il DOT tra direzione cast e velocità sia > 0
						var hitDir = Vector3.ProjectOnPlane(hit.point - transform.position, Vector3.up);
						if (Vector3.Dot(hitDir, velocity) < 0) continue;

						var normal = Vector3.ProjectOnPlane(hit.normal, transform.up).normalized;
						//normal.y = 0;
                        float dot = Vector3.Dot(velocity, normal);

                        // Se stiamo spingendo contro questa specifica faccia
                        if (dot < 0)
                        {
                            // Sottraiamo la componente normale di QUESTO urto
                            Vector3 normalComponent = dot * normal;
                            velocity -= normalComponent;
                        }
                    }

                    // Dopo aver "pulito" la velocity contro tutti i muri trovati, aggiorniamo la speed
                    //_speed = velocity.magnitude;
                }

            }

			transform.position += velocity * Time.deltaTime;


           
        }


        void AdjustOnBaloon()
		{
		    _controller.Move(new Vector3(BaloonController.Instance.CurrentVelocity.x, 0f, BaloonController.Instance.CurrentVelocity.z) * Time.deltaTime);
            var pos = transform.localPosition;
            pos.y = baloonGround;
            transform.localPosition = pos;
	    }

        private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		

		private void CameraRotation()
		{
			if (Time.timeScale == 0) return;
            //if (onBaloon)
            //{
            //    transform.Rotate(Vector3.up * BaloonController.Instance.GetComponent<Rigidbody>().angularVelocity.y * Mathf.Rad2Deg * Time.deltaTime);

            //}

			if(JawDisabled) _input.look.x = 0f;
			if(PitchDisabled) _input.look.y = 0f;	

            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
			{
				mouseSens = SettingsManager.Instance.MouseSpeed;

				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier * mouseSens * (SettingsManager.Instance.VerticalMouse ? -1f : 1f);
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier * mouseSens;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}

            
        }

		private void Move()
		{
			
			
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			//float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			//if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			//{
			//	// creates curved result rather than a linear one giving a more organic speed change
			//	// note T in Lerp is clamped, so we don't need to clamp our speed
			//	_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			//	// round speed to 3 decimal places
			//	_speed = Mathf.Round(_speed * 1000f) / 1000f;
			//}
			//else
			//{
			//	_speed = targetSpeed;
			//}

			
			



			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			if (onBaloon)
			{
                _verticalVelocity = 0.0f;
			}

			// move player
			var velocity = ComputeVelocity();
            _controller.Move(velocity * Time.deltaTime );

		}

		Vector3 ComputeVelocity()
		{
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero || MoveDisabled) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            //if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            //{
            //	// creates curved result rather than a linear one giving a more organic speed change
            //	// note T in Lerp is clamped, so we don't need to clamp our speed
            //	_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            //	// round speed to 3 decimal places
            //	_speed = Mathf.Round(_speed * 1000f) / 1000f;
            //}
            //else
            //{
            //	_speed = targetSpeed;
            //}
            if (_speed != targetSpeed)
            {
                if (targetSpeed > 0f)
                {
                    _speed += acceleration * Time.deltaTime;
                    if (_speed > targetSpeed) _speed = targetSpeed;
                }
                else
                {
                    _speed -= acceleration * Time.deltaTime;
                    if (_speed < targetSpeed) _speed = targetSpeed;
                }
            }
            //_speed = targetSpeed;


            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

          
            if (onBaloon) _verticalVelocity = 0.0f;
	        
                


			return inputDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f);
        }

		private void JumpAndGravity()
		{
			_input.jump = false; // Avoid jumping

            if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}


		public void EnterBaloon(Transform baloon)
		{
			transform.parent = playerTarget;
			baloonGround = transform.localPosition.y;
			onBaloon = true;
			_controller.enabled = false;
			onBasketCollider.enabled = true;
            _speed = 0;
        }

		public void ExitBaloon()
		{
			transform.parent = null;
			onBaloon = false;
			_controller.enabled = true;
            onBasketCollider.enabled = false;
			_speed = 0;
        }

		public void ForcePosition(Vector3 position)
		{
			_controller.enabled = false;
			transform.position = position;
			_controller.enabled = true;
		}

		public float GetSpeed()
		{
			return _speed;
		}

		public void ForceCameraPitch(float pitch)
		{
			_cinemachineTargetPitch = pitch;
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        }

		public void ForceRotation(Quaternion rotation)
		{
			transform.rotation = rotation;
		}

		public void DisableAndLookForSeconds(Vector3 target, float time = .5f)
		{
			StartCoroutine(DoDisable());

            IEnumerator DoDisable()
            {
                // Disable player input
                JawDisabled = true;
                PitchDisabled = true;
                MoveDisabled = true;

                // Force pitch and jaw
                var lookDir = target - transform.position;

                ForceRotation(Quaternion.LookRotation(lookDir.normalized, Vector3.up));
                ForceCameraPitch(0);

                yield return new WaitForSeconds(time);

                // Disable player input
                JawDisabled = false;
                PitchDisabled = false;
                MoveDisabled = false;
            }
        }

		public void Die(PlayerDeadType deadType)
		{

			switch (deadType)
			{
				case PlayerDeadType.KillerWind:
                    StartCoroutine(DoKillerWindDead());
                    break;

				case PlayerDeadType.BoilerExplosion:
					StartCoroutine(DoExplosionDead());
					break;
				case PlayerDeadType.CatwalkCollapsing:
					StartCoroutine(DoCatwalkCollapsing());
					break;
			}

			IEnumerator DoKillerWindDead()
			{
				
				yield return new WaitForSeconds(3f);

                dead = true;

                // Free parenting
                transform.parent = null;
                // Set non kinematic rigidbody
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

                // Remove collision
                onBasketCollider.enabled = false;

                // Get a random direction
                //var dir = Vector3.right * Random.Range(1f, 2f) + Vector3.forward * Random.Range(1f, 2f) + Vector3.up * Random.Range(1f, 2f);
                //if (Random.Range(0, 2) == 0) dir.x *= -1;
                //if (Random.Range(0, 2) == 0) dir.z *= -1;

                // Apply a force to the rigidbody
                //rb.AddForce(dir * 3f, ForceMode.VelocityChange);
                rb.AddTorque(Random.onUnitSphere * Random.Range(1f, 6f), ForceMode.VelocityChange);

                OnDead?.Invoke(deadType);

                yield return new WaitForSeconds(3f);
            }

			//
			// Only reset thing but don't apply any force here
			IEnumerator DoExplosionDead()
			{
                dead = true;

                // Free parenting
                transform.parent = null;
                // Set non kinematic rigidbody
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

				// Remove collision

				//onBasketCollider.enabled = false;

				DisableAllBalloonCollisions();
                onBasketCollider.isTrigger = false;


                OnDead?.Invoke(deadType);


                yield return new WaitForSeconds(5f);

                GameManager.Instance.ReportPlayerDeath();
            }

			IEnumerator DoCatwalkCollapsing()
			{
				

                // Set non kinematic rigidbody
                var rb = GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = true;

				yield return new WaitForSeconds(1.5f);
                
				dead = true;
                OnDead?.Invoke(deadType);
				

            }

			
			
        }

		void DisableAllBalloonCollisions()
		{
			var colls = BaloonController.Instance.GetComponentsInChildren<Collider>();
			foreach (Collider coll in colls)
			{
				Physics.IgnoreCollision(onBasketCollider, coll, true);
			}
		}

        private void OnCollisionEnter(Collision collision)
        {
			if(!dead) return;

			GetComponent<Rigidbody>().isKinematic = true;

            bodyFallAudioSource.Play();
			bloodAudioSource.Play();
			bloodUI.SetActive(true);

            Debug.Log("TEST - Dead player collision:"+collision.gameObject.name);
        }


    }
}