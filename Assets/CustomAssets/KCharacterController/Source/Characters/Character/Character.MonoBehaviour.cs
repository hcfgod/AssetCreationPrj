using System.Collections;
using UnityEngine;

namespace KCharacterControler
{
    public partial class Character : MonoBehaviour
    {
        #region MONOBEHAVIOUR
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void Reset()
        {
            _rotationMode = RotationMode.OrientRotationToMovement;
            _rotationRate = 540.0f;

            _startingMovementMode = MovementMode.Walking;

            _maxWalkSpeed = 5.0f;
            _minAnalogWalkSpeed = 0.0f;
            _maxAcceleration = 20.0f;
            _brakingDecelerationWalking = 20.0f;
            _groundFriction = 8.0f;
            
            _canEverCrouch = true;
            _crouchedHeight = 1.25f;
            _unCrouchedHeight = 2.0f;
            _maxWalkSpeedCrouched = 3.0f;

            _maxFallSpeed = 40.0f;
            _brakingDecelerationFalling = 0.0f;
            _fallingLateralFriction = 0.3f;
            _airControl = 0.3f;
            
            _canEverJump = true;
            _canJumpWhileCrouching = true;
            _jumpMaxCount = 1;
            _jumpImpulse = 5.0f;
            _jumpMaxHoldTime = 0.0f;
            _jumpMaxPreGroundedTime = 0.0f;
            _jumpMaxPostGroundedTime = 0.0f;

            _maxFlySpeed = 10.0f;
            _brakingDecelerationFlying = 0.0f;
            _flyingFriction = 1.0f;

            _maxSwimSpeed = 3.0f;
            _brakingDecelerationSwimming = 0.0f;
            _swimmingFriction = 0.0f;
            _buoyancy = 1.0f;

            _gravity = new Vector3(0.0f, -9.81f, 0.0f);
            _gravityScale = 1.0f;
            
            _useRootMotion = false;
            
            _impartPlatformVelocity = false;
            _impartPlatformMovement = false;
            _impartPlatformRotation = false;

            _enablePhysicsInteraction = false;
            _applyPushForceToCharacters = false;
            _applyStandingDownwardForce = false;
            
            _mass = 1.0f;
            _pushForceScale = 1.0f;
            _standingDownwardForceScale = 1.0f;
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void OnValidate()
        {
            rotationRate = _rotationRate;
            
            maxWalkSpeed = _maxWalkSpeed;
            minAnalogWalkSpeed = _minAnalogWalkSpeed;
            maxAcceleration = _maxAcceleration;
            brakingDecelerationWalking = _brakingDecelerationWalking;
            groundFriction = _groundFriction;
            
            crouchedHeight = _crouchedHeight;
            unCrouchedHeight = _unCrouchedHeight;
            maxWalkSpeedCrouched = _maxWalkSpeedCrouched;

            maxFallSpeed = _maxFallSpeed;
            brakingDecelerationFalling = _brakingDecelerationFalling;
            fallingLateralFriction = _fallingLateralFriction;
            airControl = _airControl;
            
            jumpMaxCount = _jumpMaxCount;
            jumpImpulse = _jumpImpulse;
            jumpMaxHoldTime = _jumpMaxHoldTime;
            jumpMaxPreGroundedTime = _jumpMaxPreGroundedTime;
            jumpMaxPostGroundedTime = _jumpMaxPostGroundedTime;

            maxFlySpeed = _maxFlySpeed;
            brakingDecelerationFlying = _brakingDecelerationFlying;
            flyingFriction = _flyingFriction;

            maxSwimSpeed = _maxSwimSpeed;
            brakingDecelerationSwimming = _brakingDecelerationSwimming;
            swimmingFriction = _swimmingFriction;
            buoyancy = _buoyancy;

            gravityScale = _gravityScale;
            
            useRootMotion = _useRootMotion;

            if (_characterMovement == null)
                _characterMovement = GetComponent<CharacterMovement>();
            
            impartPlatformVelocity = _impartPlatformVelocity;
            impartPlatformMovement = _impartPlatformMovement;
            impartPlatformRotation = _impartPlatformRotation;

            enablePhysicsInteraction = _enablePhysicsInteraction;
            applyPushForceToCharacters = _applyPushForceToCharacters;
            applyPushForceToCharacters = _applyPushForceToCharacters;
            
            mass = _mass;
            pushForceScale = _pushForceScale;
            standingDownwardForceScale = _standingDownwardForceScale;
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void Awake()
        {
            // Cache components
            
            CacheComponents();
            
            // Initialize physics handlers
            
            InitializePhysicsHandlers();
            
            // Initialize movement handlers
            
            InitializeMovementHandlers();
            
            // Set starting movement mode
            
            SetMovementMode(_startingMovementMode);
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void OnEnable()
        {
            // Subscribe to CharacterMovement events
            
            characterMovement.Collided += OnCollided;
            characterMovement.FoundGround += OnFoundGround;
            
            // If enabled, start LateFixedUpdate coroutine to perform auto simulation
            
            if (_enableAutoSimulation)
            {
                InitializePhysicsHandlers();
                _autoSimulationHandler.SetEnabled(true);
            }
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void OnDisable()
        {
            // Unsubscribe from CharacterMovement events
            
            characterMovement.Collided -= OnCollided;
            characterMovement.FoundGround -= OnFoundGround;
            
            // If enabled, stops LateFixedUpdate coroutine to disable auto simulation
            
            if (_enableAutoSimulation)
            {
                InitializePhysicsHandlers();
                _autoSimulationHandler.SetEnabled(false);
            }
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void Start()
        {
            // Force a ground check to update CM ground state,
            // Otherwise character will change to falling, due characterMovement updating its ground state until next Move call.
            
            if (_startingMovementMode == MovementMode.Walking)
            {
                characterMovement.SetPosition(transform.position, true);
            }
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            AddPhysicsVolume(other);
        }
        
        /// <summary>
        /// If overriden, base method MUST be called.
        /// </summary>
        
        protected virtual void OnTriggerExit(Collider other)
        {
            RemovePhysicsVolume(other);
        }
        
        /// <summary>
        /// If enableAutoSimulation is true, this coroutine is used to perform character simulation.
        /// </summary>
        
        private IEnumerator LateFixedUpdate()
        {
            // Deprecated: moved to DefaultCharacterAutoSimulationHandler
            yield break;
        }
        
        #endregion
    }
}


