using UnityEngine;

public class GhostMove : MonoBehaviour
{
    [SerializeField] Transform point1;
    [SerializeField] Transform point2;

    private Vector3 targetPos;

    [SerializeField] float speed = 1.0f;
    [SerializeField] float rotationSpeed = 8f;

    private void Start()
    {
        targetPos = point2.position;
    }

    private void FixedUpdate()
    {
        MoveGhost();
        ChangeTargetPos();
    }

    private void MoveGhost()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void ChangeTargetPos()
    {
        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            targetPos = targetPos == point1.position ? point2.position : point1.position;
        }
    }
}
