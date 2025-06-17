using UnityEngine;
public class Controls : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100;

    // Internal
    private float direction;
    private bool holding = false;
    private void Start()
    {
        Input.simulateMouseWithTouches = true;
    }
    private void FixedUpdate()
    {
        //Debug.Log("Holding: " + holding + "\nDirection: " + direction);
        if (holding) transform.localEulerAngles =
            new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + moveSpeed * direction,
            transform.localEulerAngles.z);
    }
    private void Update()
    {
        holding = false;
        if (Input.GetMouseButton(0)) holding = true;

        switch (Input.mousePositionDelta.x)
        {
            case < 0:
                direction = 1;
                break;
            case > 0:
                direction = -1;
                break;
            case 0:
                direction = 0;
                break;
        }
    }
}