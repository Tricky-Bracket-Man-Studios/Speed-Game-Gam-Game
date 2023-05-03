using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpeedGame.Core.Player
{
    public partial class PlayerController2 : MonoBehaviour
    {
        // Definition:
        // This class will handle the inventory code for the player.
        
        private void Awake() 
        {
            //Invoke(nameof(Activate), 0.5f);
            Instance = this;
            //_playerInventory = gameObject.GetComponent<PlayerInventory>();
            playerControls = new PlayerControls();
            _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            CalculateWalk();
            CalculateGravity();
            MoveCharacter();
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if(other.gameObject.layer == _groundLayer)
            {
                _colDown = true;
            }
        }

        #region Input
        
        public PlayerControls playerControls;
        private InputAction _movement;
        private InputAction _jump;
        private InputAction _click;
        private InputAction _interact;
        private bool _jumpUp;
        private bool _jumpDown;
        public static PlayerController2 Instance;

        private void OnEnable() 
        {
            _movement = playerControls.ActionMapPlayer.Movement;
            _movement.Enable();
            _jump = playerControls.ActionMapPlayer.Jump;
            _jump.Enable();
            _click = playerControls.ActionMapPlayer.Click;
            _click.Enable();
            _interact = playerControls.ActionMapPlayer.Interact;
            _interact.Enable();

        }
        private void OnDisable() 
        {
            _movement.Disable();
            _jump.Disable();
            _click.Disable();
            _interact.Disable();

        }
        private void Start() 
        {
            // Jump Bool's to replace GetButtonDown
            playerControls.ActionMapPlayer.Jump.started += context => 
            {
                //Debug.Log("Started");
                _jumpUp = false;
                _jumpDown = true;
            };
            playerControls.ActionMapPlayer.Jump.performed += context =>
            {
            //Debug.Log("Preformed");
                _jumpDown = false;  

            };
            playerControls.ActionMapPlayer.Jump.canceled += context =>
            {
                //Debug.Log("Canceled");
                _jumpDown = false;
                _jumpUp = true;
            };

            //_oldJumpHeight = _jumpHeight;
            //_jumpHeightHalf = _jumpHeight/2;
        }
        #endregion

        #region walk
        [Header("Walk")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _X;
        [SerializeField] private float _currentHorizontalSpeed, _currentVerticalSpeed;
        [SerializeField] private float _acceleration = 90;
        [SerializeField] private float _moveClamp = 13;
        [SerializeField] private float _deAcceleration = 60f;
        [SerializeField] private float _apexBonus = 2;
        [SerializeField] public Vector2 _velocity;


        private void CalculateWalk()
        {
            _X = _movement.ReadValue<float>();

            _velocity.x = _rigidbody.velocity.x;

            if(_X != 0)
            {
                // Set horizontal move speed
                _currentHorizontalSpeed += _X * _acceleration * Time.deltaTime;
                // clamped by max frame movement
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);
            }
            else 
            {
                // No input. Let's slow the character down
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }
            /*if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft) 
            {
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
            }*/
        }
        #endregion

        #region Gravity
        [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        [SerializeField] private bool _colDown;
        [SerializeField] private float _fallSpeed;
        private void CalculateGravity() 
        {
            if (_colDown) 
            {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else 
            {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = /*_endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier :*/ _fallSpeed;
                // Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;
                // Clamp
                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }
        #endregion

        #region Move
        [Header("Move")]
        [SerializeField] private int _freeColliderIterations = 10;
        [SerializeField] private bool isFacingRight = true;
        [SerializeField] private Vector3 RawMovement;
        [SerializeField] private LayerMask _groundLayer;
        // We cast our bounds before moving to avoid future collisions
        private void MoveCharacter()
        {
            var pos = transform.position;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
            var move = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;
            // flipping the characters sprite to match the direction its moving in
            if(_X > Vector2.zero.x && !isFacingRight)
            {
                Flip();
            }
            else if (_X < Vector2.zero.x && isFacingRight)
            {
                Flip();
            }
            // check furthest movement. If nothing hit, move and don't do extra checks
            var hit = Physics2D.OverlapBox(furthestPoint, new Vector2(transform.localScale.x, transform.localScale.y), 0, _groundLayer);
            if (!hit) 
            {
                transform.position += move;
                return;
            }
            // otherwise increment away from current pos; see what closest position we can move to
            var positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++) 
            {
                // increment to check all but furthestPoint - we did that already
                var t = (float)i / _freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);
                if (Physics2D.OverlapBox(posToTry, new Vector2(transform.localScale.x, transform.localScale.y), 0, _groundLayer)) 
                {
                    transform.position = positionToMoveTo;
                    // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                    if (i == 1) 
                    {
                        if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                        var dir = transform.position - hit.transform.position;
                        transform.position += dir.normalized * move.magnitude;
                    }
                    return;
                }
                positionToMoveTo = posToTry;
            }
            // optimizing out flip-age
            void Flip()
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x *= -1;
                transform.localScale = currentScale;

                isFacingRight = !isFacingRight;
            }
        }
        #endregion

        public bool GetPlayerColDown()
        {
            return true;
        }
    }
}
