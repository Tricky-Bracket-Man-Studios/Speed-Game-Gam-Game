using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SpeedGame.Core.Player
{
    public class PlayerMovementControls : MonoBehaviour
    {
        // Definition:
        // This class will handle the player's movement
    #region Events:

        public event EventHandler OnInteractionPressed;

    #endregion
    #region Fields:
        public static PlayerMovementControls Instance;

        private PlayerControls _playerControls;
        private PlayerInput _playerInput;
        private InputAction _movement;
        private InputAction _jump;
        private InputAction _dash;
        private InputAction _interact;

        private Rigidbody2D _rigidbody2D;
        private Vector2 _currentVelocity;
        private float _currentGravity;
        private float _originalGravity;
        private bool _playerIsInteracting;

        [Header("COLLISION")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private BoxCollider2D _aboveTrigger;
        [SerializeField] private BoxCollider2D _belowTrigger;
        [SerializeField] private BoxCollider2D _leftTrigger;
        [SerializeField] private BoxCollider2D _rightTrigger;
        [SerializeField] private float _boxCastDistance = 0.1f;
        private float _boxCaseAngle;
        private bool _isCollidingPlayerAbove; 
        private bool _isCollidingPlayerBelow; 
        private bool _isCollidingPlayerLeft;
        private bool _isCollidingPlayerRight;

        [Header("GRAVITY")]

        [SerializeField] private float _fallGravityModifier = 1.5f;
        [SerializeField] private float _maxFallSpeed = 40f;
        [SerializeField] private float jumpHangTimeThreshold = 0.1f;
        [SerializeField] private float jumpHangGravityMultiplier = 0.5f;

        [Header("WALKING")]
        [SerializeField] private float _acceleration = 40f;
        [SerializeField] private float _deceleration = 100f;
        [SerializeField] private float _moveSpeedClamp = 5f;
        //[SerializeField] private float _apexBonus = 2f;
        private float _currentHorizontalSpeed;
        private float _horizontalInput;

        [Header("JUMPING")]
        [SerializeField] private float _jumpHeight = 11f;
        [SerializeField] private bool _isGrounded;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 11f/2f;
        //private bool _jumpingThisFrame;
        public bool _endedJumpEarly;
        private float _currentVerticalSpeed;
        private float _verticalInput;
        private float _jumpInput;
        private float fallSpeed;

        [Header("DASHING")]
        [SerializeField] private bool _isDashing = false;
        [SerializeField] private float _dashingPower = 24f;
        [SerializeField] private float _dashingTime = 0.2f;
        [SerializeField] private float _dashingCooldown = 0f;
        private bool _canDash;
        private TrailRenderer _trailRenderer;
        private PlayerInventory _playerInventory;
        private PlayerAnimation _playerAnimator;

        [Header("Jump Boost")]
        [SerializeField] private float _BoostPower = 12f;
        [SerializeField] private float _oldJumpHeight;
        private bool _canBoostJump = false;
        private bool _ifLeadBoots = false;
        private float _jumpHeightHalf;
        

    #endregion

    #region Private Methods:

        private void Awake()
        {
            Instance = this;

            _playerControls = new PlayerControls();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _playerInventory = GetComponent<PlayerInventory>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _playerAnimator = GetComponentInChildren<PlayerAnimation>();
        }

        private void Start()
        {
            _oldJumpHeight = _jumpHeight;
            _jumpHeightHalf = _jumpHeight/2;
            _boxCaseAngle = 0f; 
            
            _playerControls.Enable();
            _jump = _playerControls.ActionMapPlayer.Jump;
            _originalGravity = _rigidbody2D.gravityScale;
        }

        private void FixedUpdate() 
        {
            _horizontalInput = _playerControls.ActionMapPlayer.Movement.ReadValue<Vector2>().x;
            _verticalInput = _playerControls.ActionMapPlayer.Movement.ReadValue<Vector2>().y;
            _currentVelocity = _rigidbody2D.velocity;
            _currentGravity = _rigidbody2D.gravityScale;

            if(_isDashing) return;

            if (!_canDash && _isGrounded && _playerInventory.GetCurrentItem() == PlayerInventory.Items.Powerups_PunchingGlove)
            {
                _canDash = true;
            }
        
            HandleCollision();
            HandleVariableJumpHeight();
            HandleBetterGravity();
            MakePlayerWalk();
            MakePlayerBoostJump();
        }
        
        private void HandleCollision()
        {
            _isGrounded = _isCollidingPlayerBelow;
            _isCollidingPlayerAbove = Physics2D.BoxCast(_aboveTrigger.bounds.center, _aboveTrigger.bounds.size, _boxCaseAngle, Vector2.up, _boxCastDistance, groundLayer);
            _isCollidingPlayerBelow = Physics2D.BoxCast(_belowTrigger.bounds.center, _belowTrigger.bounds.size, _boxCaseAngle, Vector2.down, _boxCastDistance, groundLayer);
            _isCollidingPlayerLeft = Physics2D.BoxCast(_leftTrigger.bounds.center, _leftTrigger.bounds.size, _boxCaseAngle, Vector2.left, _boxCastDistance, groundLayer);
            _isCollidingPlayerRight = Physics2D.BoxCast(_rightTrigger.bounds.center, _rightTrigger.bounds.size, _boxCaseAngle, Vector2.right, _boxCastDistance, groundLayer);
        }

        private void HandleVariableJumpHeight()
        {
            if(_endedJumpEarly && _rigidbody2D.velocity.y > 0f)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y - _jumpEndEarlyGravityModifier);
                _endedJumpEarly = false;
            }
            if(_isGrounded)
            {
                _endedJumpEarly = false;
            }
        }

        private void HandleBetterGravity()
        {
            if(_currentVelocity.y < 0)
            {
                SetPlayerGravity(_originalGravity * _fallGravityModifier);
                //Debug.Log(_rigidbody2D.gravityScale);
            }
            // Reset Gravity:
            else if(_isGrounded)
            {
                _rigidbody2D.gravityScale = _originalGravity;
            }
            // Max Fall Speed:
            if(_currentVelocity.y < _maxFallSpeed)
            {
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, Mathf.Max(_rigidbody2D.velocity.y, -_maxFallSpeed));
            }
            // Bonus air time:
            if(!_isGrounded && Mathf.Abs(_rigidbody2D.velocity.y) < jumpHangTimeThreshold)
            {
                SetPlayerGravity(_originalGravity * jumpHangGravityMultiplier);
                //Debug.Log(_rigidbody2D.gravityScale);
            }
            
        }
       
        private void MakePlayerWalk()
        {
            // Moving player:
            if(_horizontalInput != 0)
            {
                _currentHorizontalSpeed += _horizontalInput * _acceleration;
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveSpeedClamp, _moveSpeedClamp);
            }
            // Slowing player:
            else 
            {
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deceleration);
            }
            // Stopping Player from sticking to walls:
            if (_currentHorizontalSpeed > 0 && _isCollidingPlayerRight  || _currentHorizontalSpeed < 0 && _isCollidingPlayerLeft )
            {
                _currentHorizontalSpeed = 0;
            }
            
            _rigidbody2D.velocity = new Vector2( _currentHorizontalSpeed, _rigidbody2D.velocity.y);
        }
        
        public void MakePlayerJump(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                //_jumpingThisFrame = true;
            }
            if(context.performed)
            {
                _currentVerticalSpeed = _jumpHeight;

                if(_isGrounded)
                {
                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _currentVerticalSpeed);
                    _isGrounded = false;
                }
            }
            if(context.canceled)
            {
                if(!_isGrounded)
                {
                    _endedJumpEarly = true;
                }
            }
        }

        public void MakePlayerDash(InputAction.CallbackContext context)
        {
            if(_canDash && context.performed)
            {
                StartCoroutine(Dash());
            
                IEnumerator Dash()
                {
                    // Updating player dash bool's
                    _canDash = false;
                    _isDashing = true;
                    _isGrounded = false;
                    // Saving the players original states
                    float originalGravity = _rigidbody2D.gravityScale;
                    Vector2 originalVelocity = _rigidbody2D.velocity;
                    // preforming the dash
                    _rigidbody2D.gravityScale = 0f;
                    _rigidbody2D.velocity = new Vector2(_playerAnimator.GetIsFacingRight() * _dashingPower, 0f);
                    _trailRenderer.emitting = true;
                    yield return new WaitForSeconds(_dashingTime);
                    // reverting the dash effects
                    _trailRenderer.emitting = false;
                    _rigidbody2D.gravityScale = originalGravity;
                    _rigidbody2D.velocity = originalVelocity;
                    yield return new WaitForSeconds(_dashingCooldown);
                    _isDashing = false;
                }
            }
        }

        public void MakePayerInteract(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnInteractionPressed?.Invoke(this, EventArgs.Empty);
                
            }
            if(context.canceled)
            {
                _playerIsInteracting = false;
            }
        }

        private void MakePlayerBoostJump()
        {
            if(_canBoostJump)
            {
                _jumpHeight = _BoostPower;
            }
            else if (_ifLeadBoots)
            {
                _jumpHeight = _jumpHeightHalf;
            }
            else
            {
                _jumpHeight = _oldJumpHeight;
            }
        }

        


    #endregion

    #region Getters:
        public Vector2 GetPlayerVelocity()
            {
                return _currentVelocity;
            }
            public bool GetPlayerIsGrounded()
            {
                return _isGrounded;
            }
            public float GetPlayerInputX()
            {
                return _horizontalInput;
            }
            public bool GetPlayerIsInteracting()
            {
                return _playerIsInteracting;
            }

            public bool GetIsDashing()
            {
                return _isDashing;
            }
            public bool GetCanDash()
            {
                return _canDash;
            }
            public void EnableDash()
            {
                _canDash = true;
            }
            public void DisableDash()
            {
                _canDash = false;
            }

            public void EnableBoostJump()
            {
                _canBoostJump = true;
            }
            public void DisableBoostJump()
            {
                _canBoostJump = false;
            }
            public void EnableLeadJump()
            {
                _ifLeadBoots = true;
            }
            public void DisableLeadJump()
            {
                _ifLeadBoots = false;
            }
    #endregion

    #region Setters:
        private void SetPlayerGravity(float value)
        {
            _rigidbody2D.gravityScale = value; 
        }
    #endregion

    }
}
