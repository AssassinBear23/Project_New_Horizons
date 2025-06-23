using UnityEngine;
public enum Directions { Clockwise, Counterclockwise }
public class BirdController : MonoBehaviour
{
    public Directions moveDirection = Directions.Clockwise;
    [SerializeField] private float movementSpeed = 10;
    private void FixedUpdate()
    {
        float direction = (moveDirection == Directions.Clockwise) ? 1 : -1;
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + movementSpeed * direction,
            transform.localEulerAngles.z);
    }
}
