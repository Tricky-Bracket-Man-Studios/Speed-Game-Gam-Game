using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SpeedGame.Core.Player {
/// <summary>
/// Hey!
/// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
/// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
/// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
/// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
/// </summary>


public class PlayerController : MonoBehaviour, IPlayerController {
#region mattsCode
// This is Code I added to implement it with my input system:
// My Variables I added
public PlayerControls playerControls;

private InputAction _movement;
private InputAction _jump;
private InputAction _click;
private InputAction _interact;
private bool _jumpUp;
private bool _jumpDown;
public static PlayerController Instance;


void Awake() {
    Invoke(nameof(Activate), 0.5f);
    playerControls = new PlayerControls();
    Instance = this;
    _playerInventory = gameObject.GetComponent<PlayerInventory>();
}

void OnEnable() {
    _movement = playerControls.ActionMapPlayer.Movement;
    _movement.Enable();
    _jump = playerControls.ActionMapPlayer.Jump;
    _jump.Enable();
    _click = playerControls.ActionMapPlayer.Click;
    _click.Enable();
    _interact = playerControls.ActionMapPlayer.Interact;
    _interact.Enable();

}
public void OnDisable() {
    _movement.Disable();
    _jump.Disable();
    _click.Disable();
    _interact.Disable();

}

private void Start() {
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

    _oldJumpHeight = _jumpHeight;
    _jumpHeightHalf = _jumpHeight/2;
}
        // END OF MY ADDITIONS(I did change the inputGather Function/Method)
#endregion

// Public for external hooks
public Vector3 Velocity { get; private set; }
public FrameInput Input { get; private set; }
public bool JumpingThisFrame { get; private set; }
public bool LandingThisFrame { get; private set; }
public Vector3 RawMovement { get; private set; }
public bool Grounded => _colDown;
private Vector3 _lastPosition;
private float _currentHorizontalSpeed, _currentVerticalSpeed;
// This is horrible, but for some reason colliders are not fully established when update starts...
private bool _active;
//void Awake() => Invoke(nameof(Activate), 0.5f);
void Activate() => _active = true;
private void Update() {
if(!_active) return;
if(_isDashing) return;
// Calculate velocity
Velocity = (transform.position - _lastPosition) / Time.deltaTime;
_lastPosition = transform.position;

if (!_canDash && _colDown && _playerInventory.GetCurrentItem() == PlayerInventory.Items.Powerups_PunchingGlove)
{
    _canDash = true;
}

GatherInput();
RunCollisionChecks();
RunInteractions(); // Player item interactions
BoostJump(); // Boost the jump
DashCharacter(); // Dash Movement
CalculateWalk(); // Horizontal movement
CalculateJumpApex(); // Affects fall speed, so calculate before gravity
CalculateGravity(); // Vertical movement
CalculateJump(); // Possibly overrides vertical
MoveCharacter(); // Actually perform the axis movement
}
#region Gather Input
private void GatherInput() {
Input = new FrameInput {
//JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
JumpDown = _jumpDown,
//JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
JumpUp = _jumpUp,
// X = UnityEngine.Input.GetAxisRaw("Horizontal")
X = _movement.ReadValue<float>()
};
if (Input.JumpDown) {
_lastJumpPressed = Time.time;
}
}
#endregion
#region Collisions
[Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
[SerializeField] private LayerMask _groundLayer;
[SerializeField] private LayerMask _objectLayer;
[SerializeField] private int _detectorCount = 3;
[SerializeField] private float _detectionRayLength = 0.1f;
[SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground
private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
private bool _colUp, _colRight, _colDown, _colLeft;
private float _timeLeftGrounded;
// We use these raycast checks for pre-collision information
private void RunCollisionChecks() 
{
    // Generate ray ranges.
    CalculateRayRanged();
    // Ground
    LandingThisFrame = false;
    var groundedCheck = RunGroundDetection(_raysDown) || RunObjectDetection(_raysDown);
    if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
    else if (!_colDown && groundedCheck) 
    {
        _coyoteUsable = true; // Only trigger when first touching
        LandingThisFrame = true;
    }
    _colDown = groundedCheck;
    // The rest
    _colUp = RunGroundDetection(_raysUp)|| RunObjectDetection(_raysUp);
    _colLeft = RunGroundDetection(_raysLeft);
    _colRight = RunGroundDetection(_raysRight);
    bool RunGroundDetection(RayRange range) 
    {
        return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
    }
    // Object detection:
    bool RunObjectDetection(RayRange range) 
    {
        return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _objectLayer));
    }
}

public bool GetPlayerColRight()
{
    return _colRight;
}
public bool GetPlayerColLeft()
{
    return _colLeft;
}
public bool GetPlayerColDown()
{
    return _colDown;
}

private void CalculateRayRanged() {
// This is crying out for some kind of refactor.
var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);
_raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
_raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
_raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
_raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
}
private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
for (var i = 0; i < _detectorCount; i++) {
var t = (float)i / (_detectorCount - 1);
yield return Vector2.Lerp(range.Start, range.End, t);
}
}
private void OnDrawGizmos() {
// Bounds
Gizmos.color = Color.yellow;
Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);
// Rays
if (!Application.isPlaying) {
CalculateRayRanged();
Gizmos.color = Color.blue;
foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft }) {
foreach (var point in EvaluateRayPositions(range)) {
Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
}
}
}
if (!Application.isPlaying) return;
// Draw the future position. Handy for visualizing gravity
Gizmos.color = Color.red;
var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
Gizmos.DrawWireCube(transform.position + _characterBounds.center + move, _characterBounds.size);
}
#endregion
#region Walk
[Header("WALKING")] [SerializeField] private float _acceleration = 90;
[SerializeField] private float _moveClamp = 13;
[SerializeField] private float _deAcceleration = 60f;
[SerializeField] private float _apexBonus = 2;
private void CalculateWalk() 
{
    if (Input.X != 0) 
    {
        // Set horizontal move speed
        _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;
        // clamped by max frame movement
        _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);
        // Apply bonus at the apex of a jump
        var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
        _currentHorizontalSpeed += apexBonus * Time.deltaTime;
    }
    else 
    {
        // No input. Let's slow the character down
        _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
    }
    if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft) 
    {
        // Don't walk through walls
        _currentHorizontalSpeed = 0;
    }
}
#endregion
#region Gravity
[Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
[SerializeField] private float _minFallSpeed = 80f;
[SerializeField] private float _maxFallSpeed = 120f;
private float _fallSpeed;
private void CalculateGravity() {
if (_colDown) {
// Move out of the ground
if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
}
else {
// Add downward force while ascending if we ended the jump early
var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;
// Fall
_currentVerticalSpeed -= fallSpeed * Time.deltaTime;
// Clamp
if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
}
}
#endregion
#region Jump
[Header("JUMPING")] [SerializeField] private float _jumpHeight = 30;
[SerializeField] private float _jumpApexThreshold = 10f;
[SerializeField] private float _coyoteTimeThreshold = 0.1f;
[SerializeField] private float _jumpBuffer = 0.1f;
[SerializeField] private float _jumpEndEarlyGravityModifier = 3;
private bool _coyoteUsable;
private bool _endedJumpEarly = true;
private float _apexPoint; // Becomes 1 at the apex of a jump
private float _lastJumpPressed;
private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
private bool HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;
private void CalculateJumpApex() {
if (!_colDown) {
// Gets stronger the closer to the top of the jump
_apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
_fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
}
else {
_apexPoint = 0;
}
}
private void CalculateJump() {
// Jump if: grounded or within coyote threshold || sufficient jump buffer
if (Input.JumpDown && CanUseCoyote || HasBufferedJump) {
_currentVerticalSpeed = _jumpHeight;
_endedJumpEarly = false;
_coyoteUsable = false;
_timeLeftGrounded = float.MinValue;
JumpingThisFrame = true;
}
else {
JumpingThisFrame = false;
}
// End the jump early if button released
if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0) 
{
    // _currentVerticalSpeed = 0;
    _endedJumpEarly = true;
}
if (_colUp) {
if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
}
}
#endregion
#region Move
[Header("MOVE")] [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
private int _freeColliderIterations = 10;
private bool isFacingRight = true;
// We cast our bounds before moving to avoid future collisions
private void MoveCharacter() 
{
    var pos = transform.position + _characterBounds.center;
    RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
    var move = RawMovement * Time.deltaTime;
    var furthestPoint = pos + move;
    // flipping the characters sprite to match the direction its moving in
    if(RawMovement.x > Vector2.zero.x && !isFacingRight)
    {
        Flip();
    }
    else if (RawMovement.x < Vector2.zero.x && isFacingRight)
    {
        Flip();
    }
    // check furthest movement. If nothing hit, move and don't do extra checks
    var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
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
        if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer)) {
        transform.position = positionToMoveTo;
        // We've landed on a corner or hit our head on a ledge. Nudge the player gently
        if (i == 1) {
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
#region Dash
[Header("DASH")] [SerializeField, Tooltip("This controls the dash physics")]
// Variables
private bool _canDash = false;
[SerializeField] private bool _isDashing = false;
[SerializeField] private float _dashingPower = 24f;
[SerializeField] private float _dashingTime = 0.2f;
//[SerializeField] private float _dashingCooldown = 1f;

[SerializeField] private Rigidbody2D _rigidbody;
[SerializeField] private TrailRenderer _trailRenderer;
private PlayerInventory _playerInventory;
private void DashCharacter()
{
    playerControls.ActionMapPlayer.Click.started += context => 
    {
        if(_canDash)
        {
            StartCoroutine(Dash());

        }
        
        
    };
    IEnumerator Dash()
    {
        // Updating player dash bool's
        _canDash = false;
        _isDashing = true;
        _colDown = false;
        // Saving the players original states
        float originalGravity = _rigidbody.gravityScale;
        Vector2 originalVelocity = _rigidbody.velocity;
        // preforming the dash
        _rigidbody.gravityScale = 0f;
        _rigidbody.velocity = new Vector2(transform.localScale.x * _dashingPower, 0f);
        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        // reverting the dash effects
        _trailRenderer.emitting = false;
        _rigidbody.gravityScale = originalGravity;
        _rigidbody.velocity = originalVelocity;
        _isDashing = false;
        
    }

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
#endregion

#region Interact
[Header("Interact")] 
// Variables
[SerializeField, Tooltip("This controls the powerup physics")]
private bool PlayerIsInteracting;
private void RunInteractions()
{
    playerControls.ActionMapPlayer.Interact.started += context => 
    {
        StartCoroutine(Interact());
    };
    IEnumerator Interact()
    {
        PlayerIsInteracting = true;
        yield return new WaitForSeconds(0f);
        PlayerIsInteracting = false;
    }
    
}
public bool GetPlayerIsInteracting()
{
    return PlayerIsInteracting;
}
#endregion
#region Dash
[Header("Jump Boost")] [SerializeField, Tooltip("This controls the dash physics")]
// Variables
private bool _canBoostJump = false;
private bool _ifLeadBoots = false;
private float _jumpHeightHalf;
[SerializeField] private float _BoostPower = 12f;
[SerializeField] private float _oldJumpHeight;

private void BoostJump()
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
}
}