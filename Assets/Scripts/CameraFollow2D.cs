using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    [SerializeField]
    private float followSpeed = 8f;

    [SerializeField]
    private float minX = -12f;

    [SerializeField]
    private float maxX = 32f;

    [SerializeField]
    private bool lockY = true;

    void Awake()
    {
        if (target != null)
        {
            return;
        }

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        if (lockY)
        {
            desiredPosition.y = offset.y;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetXLimits(float minimumX, float maximumX)
    {
        minX = minimumX;
        maxX = maximumX;
    }
}
