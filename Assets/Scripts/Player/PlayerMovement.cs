using Assets.Scripts.Player.Player_State_Machine.States;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.UI.Selectable;

public class PlayerMovement: MonoBehaviour
{
    #region Player State Machine Declerations

    public PlayerStateMachine stateMachine { get; set; }

    public PlayerIdle idleState { get; set; }

    public PlayerJumpingFalling jumpFallState { get; set; }

    public PlayerWalk walkState { get; set; }

    public PlayerDash dashState { get; set; }

    public PlayerGroundCollide groundCollide { get; set; }

    public PlayerTrampolineCollide trampolineCollide { get; set; }
    
    public PlayerWallCollide wallCollide { get; set; }

    public void initializationOfStates()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdle(this, stateMachine);
        jumpFallState = new PlayerJumpingFalling(this, stateMachine);
        walkState = new PlayerWalk(this, stateMachine);
        dashState = new PlayerDash(this, stateMachine);
        groundCollide = new PlayerGroundCollide(this, stateMachine);
        wallCollide = new PlayerWallCollide(this, stateMachine);
        trampolineCollide = new PlayerTrampolineCollide(this, stateMachine);
        stateMachine.Initialize(jumpFallState);
    }

    #endregion

    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] public Collider2D body;
    [SerializeField] private Collider2D feet;
    [SerializeField] private Collider2D head;
    [SerializeField] public Transform playerTransform;
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Movement Variables")]
    private Vector2 moveVelocity;
    public bool isFacingRight;

    [Header("Collision Check Variables")]
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    public bool _isGrounded;
    private bool _bumpedHead;

    [Header("Jump Variables")]
    public float VerticalVelocity { get; set; }
    public bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    [Header("Apex Variables")]
    private float _apexPoint;
    private float _timePastApexThreshold;
    public bool _isPastApexThreshold;

    [Header("Jump Buffer Variables")]
    private float _jumpBufferTimer;
    private bool _jumpReleaseDuringBuffer;

    [Header("Coyote Time Variables")]
    public float _coyoteTimer;

    [Header("Camera Variables")]
    [SerializeField] private GameObject cameraFollowGO;
    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    [Header("Dash Variables")]
    public float DASHSPEED = 30;
    public float DASHTIME = 0.15f;
    public float dashCD = 1f;
    public float dashDir = 1f;

    [Header("LayerMasks")]
    public LayerMask groundLayer;

    [Header("Collision Variables")]
    public float wallStunTime = 0.4f;
    public float floorStunTime = 0.3f;
    public float trampolineDelay = 0.1f;


    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cameraFollowObject = cameraFollowGO.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
        initializationOfStates();
    }

    private void Update()
    {
        CountTimers();
        stateMachine.CurrentPlayerState.FrameUpdate();
        animator.SetFloat("SpeedY", VerticalVelocity);
        animator.SetFloat("SpeedX", rb.linearVelocityX);
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        stateMachine.CurrentPlayerState.PhysicsUpdate();
    }

    #region Camera

    public void yCameraDamping()
    {
        if (rb.linearVelocityY < fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.lerpYDamping(true);
        }

        if (rb.linearVelocityY >= 0 && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.lerpYDamping(false);
        }
    }

    #endregion

    #region Movement

    public void Move(Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            moveVelocity = new Vector2(moveInput.x, 0f) * MoveStats.moveSpeed;
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }

        else if (moveInput == Vector2.zero)
        {
            moveVelocity = new Vector2(0f, 0f);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }

    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }

        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion

    #region Jump

    public void JumpChecks()
    {
        if (InputManager.instance.jumpJustPressed)
        {
            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleaseDuringBuffer = false;
        }

        if (InputManager.instance.jumpReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleaseDuringBuffer = true;
            }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //Jump w/ jump buffering and coyote time
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);
            animator.SetTrigger("Jump");

            if (_jumpReleaseDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        //Double jump
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);
            animator.SetTrigger("DoubleJump");
        }

        //Air jump after coyote time lapsed
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            animator.SetTrigger("DoubleJump");
            _isFastFalling = false;
        }

        //Landed
        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;

            VerticalVelocity = 0;
        }
    }

    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }


        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    public void Jump()
    {
        //Apply gravity when jumping
        if (_isJumping)
        {
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            //Gravity on ascending
            if (VerticalVelocity >= 0f)
            {
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }

                //Gravity on ascending not past apex
                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }

            //Gravity on descending
            else if (!_isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        //Jump cut
        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, -1f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        //Normal gravity without jumping
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }

        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

        rb.linearVelocityY = VerticalVelocity;
    }

    #endregion

    #region Collision Checks

    private void isGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feet.bounds.center.x, feet.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feet.bounds.size.x, MoveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, groundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(head.bounds.center.x, head.bounds.max.y);
        Vector2 boxCastSize = new Vector2(head.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, groundLayer);
        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else { _bumpedHead = false; }
    }

    private void CollisionChecks()
    {
        isGrounded();
        BumpedHead();
    }

    #endregion

    #region Timers

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;
        dashCD -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else { _coyoteTimer = MoveStats.JumpCoyoteTime; }
    }


    #endregion

    #region Animation Triggers
    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        stateMachine.CurrentPlayerState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        Idle,
        Walk,
        Jump,
        Fall,
    }

    #endregion

    #region Debug/Visualizations

    private void OnDrawGizmos()
    {
        if (MoveStats.ShowJumpArc)
        {
            DrawJumpArc(MoveStats.moveSpeed, Color.white);
        }
    }

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(feet.bounds.center.x, feet.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }
        Vector2 velocity = new Vector2(speed, MoveStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MoveStats.TimeTillJumpApex / MoveStats.ArcResolution;

        for (int i = 0; i < MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MoveStats.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveStats.Gravity) * simulationTime * simulationTime;
            }
            else if (simulationTime < MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime)
            {
                float apexTime = simulationTime - MoveStats.TimeTillJumpApex;
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }
            else
            {
                float descendTime = simulationTime - (MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime);
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveStats.ApexHangTime;
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, MoveStats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPosition + displacement;

            if (MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), groundLayer);
                if (hit.collider != null)
                {
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }

    #endregion
}

