using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 7f;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
    public LayerMask groundLayer;
    public bool autoFitColliderToSprite = true;
    [Range(0.1f, 1f)]
    public float colliderWidthRatio = 0.65f;
    [Range(0.1f, 1f)]
    public float colliderHeightRatio = 0.95f;
    public bool restartOnFall = true;
    public float fallRestartY = -8f;
    public AudioClip jumpSound;
    [SerializeField]
    private bool forceJumpOnLanding;

    private Rigidbody2D rb;
    private BoxCollider2D bodyCollider;
    [SerializeField]
    private SpriteRenderer sr;
    [SerializeField]
    private Transform visualRoot;
    [SerializeField]
    private bool visualFacesRight = true;
    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    private float fallAnimationVelocityThreshold = -0.5f;
    [SerializeField]
    private float fallAnimationDelay = 0.1f;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isRestarting;
    private Vector3 visualRootDefaultScale;
    private float characterAnimatorDefaultSpeed = 1f;
    private float lastGroundedTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        if (visualRoot != null)
        {
            visualRootDefaultScale = visualRoot.localScale;
        }
        if (characterAnimator != null)
        {
            characterAnimatorDefaultSpeed = characterAnimator.speed;
        }
        rb.freezeRotation = true;
        FitColliderToSprite();
    }

    void Update()
    {
        if (GameClearManager.Instance != null && GameClearManager.Instance.IsCleared)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        CheckFallRestart();
        CheckGround();
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        bool jumpedThisFrame = false;
        if (forceJumpOnLanding && isGrounded && !wasGrounded)
        {
            Jump();
            jumpedThisFrame = true;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            UpdateAnimation(false);
            wasGrounded = isGrounded;
            return;
        }

        float move = 0f;
        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)
        {
            move -= 1f;
            SetFacingDirection(true);
        }

        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed)
        {
            move += 1f;
            SetFacingDirection(false);
        }

        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (!jumpedThisFrame &&
            (keyboard.spaceKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame) &&
            isGrounded)
        {
            Jump();
        }

        UpdateAnimation(!Mathf.Approximately(move, 0f));
        wasGrounded = isGrounded;
    }

    private void SetFacingDirection(bool faceLeft)
    {
        if (visualRoot == null)
        {
            sr.flipX = faceLeft;
            return;
        }

        bool shouldFlip = visualFacesRight ? faceLeft : !faceLeft;
        visualRoot.localScale = new Vector3(
            Mathf.Abs(visualRootDefaultScale.x) * (shouldFlip ? -1f : 1f),
            visualRootDefaultScale.y,
            visualRootDefaultScale.z);
    }

    private void UpdateAnimation(bool hasMovementInput)
    {
        if (characterAnimator != null)
        {
            bool landedThisFrame = isGrounded && !wasGrounded;
            bool isFalling = !isGrounded &&
                rb.linearVelocity.y < fallAnimationVelocityThreshold &&
                Time.time - lastGroundedTime >= fallAnimationDelay;
            characterAnimator.SetBool("IsFalling", isFalling);
            characterAnimator.speed = isFalling || hasMovementInput || landedThisFrame
                ? characterAnimatorDefaultSpeed
                : 0f;
        }
    }

    public void SetForceJumpOnLanding(bool isEnabled)
    {
        forceJumpOnLanding = isEnabled;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);

        if (jumpSound != null)
        {
            AudioSource.PlayClipAtPoint(jumpSound, transform.position, GameSettings.SfxVolume);
        }
    }

    void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    void CheckFallRestart()
    {
        if (!restartOnFall || isRestarting || transform.position.y >= fallRestartY)
        {
            return;
        }

        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            bodyCollider = GetComponent<BoxCollider2D>();
            FitColliderToSprite();
        }
    }

    void FitColliderToSprite()
    {
        if (!autoFitColliderToSprite || bodyCollider == null)
        {
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        bodyCollider.size = new Vector2(
            spriteBounds.size.x * colliderWidthRatio,
            spriteBounds.size.y * colliderHeightRatio);
        bodyCollider.offset = spriteBounds.center;

        if (groundCheck != null)
        {
            float localHalfHeight = groundCheckSize.y * 0.5f / Mathf.Max(Mathf.Abs(transform.localScale.y), 0.0001f);
            groundCheck.localPosition = new Vector3(
                bodyCollider.offset.x,
                bodyCollider.offset.y - (bodyCollider.size.y * 0.5f) - localHalfHeight,
                groundCheck.localPosition.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
