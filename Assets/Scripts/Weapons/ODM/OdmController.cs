using UnityEngine;

public class OdmController : MonoBehaviour
{
    [Header("Reeling")]
    public float initReelInSpeed = 50f;
    public float endReelInSpeed = 75f;
    public float reelInAccelerationTime = 1.2f;
    public float reelOutSpeed = 15f;

    [Header("Swinging")]
    public float redirectSpeed = 12f;

    [Header("Misc")]
    public Vector3 gravity = new Vector3(0, -25f, 0);

    private Rigidbody playerRigidbody;
    private Transform playerHead;

    private Vector3 _hookPoint;
    private bool _isReeling = false;
    private float _ropeDistance;
    private float _reelInTime = 0f;
    private float _initialReelSpeed;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerHead = GetComponentInChildren<Camera>().transform;
    }

    private void FixedUpdate()
    {
        if (!_isReeling) return;
        Vector3 vectorToHook = _hookPoint - playerRigidbody.position;
        float distanceToHook = vectorToHook.magnitude;

        _ropeDistance = Mathf.Min(_ropeDistance, distanceToHook);

        if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three))
        {
            _reelInTime += Time.fixedDeltaTime;
            float accelerationProgress = Mathf.Clamp01(_reelInTime / reelInAccelerationTime);
            float currentReelInSpeed = Mathf.Lerp(_initialReelSpeed, endReelInSpeed, accelerationProgress);
            Vector3 targetVelocity = vectorToHook.normalized * currentReelInSpeed;
            playerRigidbody.linearVelocity = Vector3.Lerp(playerRigidbody.linearVelocity, targetVelocity, 1 - Mathf.Exp(-10 * Time.fixedDeltaTime));
        }
        else
        {
            _reelInTime = 0f;
            playerRigidbody.linearVelocity += gravity * Time.fixedDeltaTime;

            float futureDistance = Vector3.Distance(playerRigidbody.position + playerRigidbody.linearVelocity * Time.fixedDeltaTime, _hookPoint);
            if (futureDistance > _ropeDistance)
            {
                Vector3 radialDir = vectorToHook.normalized;
                playerRigidbody.linearVelocity = Vector3.ProjectOnPlane(playerRigidbody.linearVelocity, radialDir);
            }
        }
    }

    public void StartReeling(Vector3 hookPoint)
    {
        _isReeling = true;
        _hookPoint = hookPoint;
        _reelInTime = 0f;

        Vector3 directionToHook = (hookPoint - playerRigidbody.position).normalized;
        float projectedSpeed = Vector3.Dot(playerRigidbody.linearVelocity, directionToHook);
        _initialReelSpeed = Mathf.Max(projectedSpeed, initReelInSpeed);
        _ropeDistance = Vector3.Distance(playerRigidbody.position, hookPoint);
    }

    public void StopReeling()
    {
        _isReeling = false;
        _reelInTime = 0f;
    }
}