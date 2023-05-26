using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedGame.Core.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        // Definition:
        // This class will handle the animation code for the player.

        // Variables:
        [SerializeField] private Animator _animation;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody;
        private PlayerMovementControls _playerController;
        [SerializeField] Vector2 _velocity;
        [SerializeField] bool isFacingRight;
        // Animations:
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walking = Animator.StringToHash("Walking");
        private static readonly int Jumping = Animator.StringToHash("Jumping");
        // Private Functions:
        private void Awake() 
        {
            _rigidbody = gameObject.GetComponentInParent<Rigidbody2D>();
            _playerController = gameObject.GetComponentInParent<PlayerMovementControls>();
        }
        private void Start()
        {
            isFacingRight = true;
        }

        private void Update() 
        {
            _velocity = _playerController.GetPlayerVelocity();

            SpriteDirection();

            if((_velocity.x >= 0.01 || _velocity.x <= -0.01) && _playerController.GetPlayerIsGrounded()) 
            {
                _animation.CrossFade(Walking, 0, 0);
            }
            else 
            {
                _animation.CrossFade(Idle, 0, 0);
            }

            if(!_playerController.GetPlayerIsGrounded()) 
            {
                _animation.speed = 1;
                if(_velocity.y > 9.25) 
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.4f);
                }
                else if(_velocity.y <= 9.25 && _velocity.y > 6.5)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.3f);
                }
                else if(_velocity.y <= 6.5 && _velocity.y > 4.25)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.2f);
                }
                else if(_velocity.y <= 4.25 && _velocity.y > 2)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0f);
                }
                else if(_velocity.y <= 2 && _velocity.y > -0.75)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.5f);
                }
                else if(_velocity.y <= -0.75 && _velocity.y > -3.5)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.7f);
                }
                else if(_velocity.y <= -3.5 && _velocity.y > -6.25)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.8f);
                }
                else if(_velocity.y <= -6.25)
                {
                    _animation.PlayInFixedTime(Jumping, 0, 0.9f);
                }
            }
            else
            {
                _animation.speed = 1;
            }
        }

        private void SpriteDirection()
        {
            if(_playerController.GetPlayerInputX() != 0f)
            {
                if(_playerController.GetPlayerInputX() > 0.1f && !isFacingRight)
                {
                    Flip();
                }
                else if(_playerController.GetPlayerInputX() < -0.1f && isFacingRight)
                {
                    Flip();
                }
            }
        }
        private void Flip()
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;

            isFacingRight = !isFacingRight;
        }

        public float GetIsFacingRight()
        {
            float direction;

            if (isFacingRight)
            {
                direction = 1f;
            }
            else
            {
                direction = -1f;
            }

            return direction;
        }
    }
}