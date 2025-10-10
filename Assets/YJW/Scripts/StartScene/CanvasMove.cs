using UnityEngine;

public class CanvasMove : MonoBehaviour
{
    [SerializeField] GameObject ghost;

    private void Update()
    {
        transform.position = new Vector3(ghost.transform.position.x, ghost.transform.position.y + 3f,ghost.transform.position.z);
    }
}
