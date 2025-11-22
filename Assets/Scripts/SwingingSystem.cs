using UnityEngine;

public class GrappleSystem : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxGrappleDistance = 50f;
    public LayerMask grappleLayer;

    [Header("Pull Settings")]
    public float pullSpeed = 25f;
    public float arrivalDistance = 2f;
    public float fallGravity = 30f;
    public float maxFallSpeed = 50f;
    public string groundTag = "ground";

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.F;

    [Header("Visual (Optional)")]
    public LineRenderer ropeRenderer;
    public Transform grappleOrigin;

    private Vector3 grapplePoint;
    private bool isGrappling;
    private bool isFalling;
    private Vector3 fallVelocity;

    private CharacterInputController inputController;
    private RootMotionControlScript rootMotionControl;
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        inputController = GetComponent<CharacterInputController>();
        rootMotionControl = GetComponent<RootMotionControlScript>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (ropeRenderer) ropeRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(grappleKey) && !isGrappling)
        {
            StartGrapple();
        }

        if (Input.GetKeyUp(grappleKey) && isGrappling)
        {
            StartFalling();
        }

        if (isGrappling)
        {
            PullTowardPoint();
            if (ropeRenderer) DrawRope();
        }
        else if (isFalling)
        {
            DoFall();
        }
    }

    void StartGrapple()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (!Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleLayer))
            return;

        grapplePoint = hit.point;
        isGrappling = true;
        isFalling = false;

        // Disable all other movement systems
        if (inputController != null)
            inputController.enabled = false;

        if (rootMotionControl != null)
            rootMotionControl.enabled = false;

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetFloat("vely", 0);
        }

        if (rb != null)
            rb.isKinematic = true;

        if (ropeRenderer) ropeRenderer.enabled = true;
    }

    void StopGrapple()
    {
        isGrappling = false;

        if (ropeRenderer) ropeRenderer.enabled = false;

        // Re-enable normal controls
        if (rb != null)
            rb.isKinematic = false;

        if (inputController != null)
            inputController.enabled = true;

        if (rootMotionControl != null)
            rootMotionControl.enabled = true;

        if (animator != null)
            animator.applyRootMotion = true;
    }

    void StartFalling()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        fallVelocity = direction * pullSpeed * 0.5f;

        isGrappling = false;
        isFalling = true;

        if (ropeRenderer) ropeRenderer.enabled = false;
    }

    void DoFall()
    {
        fallVelocity.y -= fallGravity * Time.deltaTime;
        fallVelocity.y = Mathf.Max(fallVelocity.y, -maxFallSpeed);

        Vector3 nextPos = transform.position + fallVelocity * Time.deltaTime;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                float groundY = hit.point.y;
                if (nextPos.y <= groundY + 0.1f)
                {
                    nextPos.y = groundY;
                    transform.position = nextPos;
                    Land();
                    return;
                }
            }
        }

        transform.position = nextPos;
    }

    void Land()
    {
        isFalling = false;
        fallVelocity = Vector3.zero;

        if (rb != null)
            rb.isKinematic = false;

        if (inputController != null)
            inputController.enabled = true;

        if (rootMotionControl != null)
            rootMotionControl.enabled = true;

        if (animator != null)
            animator.applyRootMotion = true;
    }

    void PullTowardPoint()
    {
        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        if (distance <= arrivalDistance)
        {
            StopGrapple();
            return;
        }

        transform.position += direction * pullSpeed * Time.deltaTime;

        Vector3 lookDir = direction;
        lookDir.y = 0;
        if (lookDir.magnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void DrawRope()
    {
        Vector3 start = grappleOrigin != null ? grappleOrigin.position : transform.position;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, start);
        ropeRenderer.SetPosition(1, grapplePoint);
    }

    public bool IsGrappling() => isGrappling;
    public bool IsFalling() => isFalling;
}