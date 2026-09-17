using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Look Ahead")]
    [SerializeField] private bool useLookAhead = true;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSmoothTime = 0.3f;

    [Header("Bounds")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private Vector3 followVelocity;
    private float lookAheadVelocity;
    private float currentLookAheadX;
    private float lastTargetX;

    #endregion

    #region Unity Methods

    private void LateUpdate()
    {
        if (target == null)
            return;

        UpdateLookAhead();

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x += currentLookAheadX;

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothTime);
    }

    #endregion

    #region Look Ahead

    private void UpdateLookAhead()
    {
        if (!useLookAhead)
        {
            currentLookAheadX = 0f;
            return;
        }

        float deltaX = target.position.x - lastTargetX;

        float targetDirection = Mathf.Abs(deltaX) > 0.001f ? Mathf.Sign(deltaX) : 0f;

        float desiredLookAhead = targetDirection * lookAheadDistance;

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, desiredLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);

        lastTargetX = target.position.x;
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (!useBounds)
            return;

        Gizmos.color = Color.cyan;

        Vector3 bottomLeft = new Vector3(minX, minY, 0f);
        Vector3 bottomRight = new Vector3(maxX, minY, 0f);
        Vector3 topLeft = new Vector3(minX, maxY, 0f);
        Vector3 topRight = new Vector3(maxX, maxY, 0f);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

    #endregion
}
