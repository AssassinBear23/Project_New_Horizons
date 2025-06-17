using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;
    private void FixedUpdate()
    {
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y - movementSpeed,
            transform.position.z);
    }
}