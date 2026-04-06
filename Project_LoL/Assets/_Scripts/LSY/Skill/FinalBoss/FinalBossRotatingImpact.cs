using UnityEngine;

public class FinalBossRotatingImpact : FinalBossImpact
{
    private Vector3 _lastPosition;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 moveDir = transform.position - _lastPosition;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        _lastPosition = transform.position;
    }
}