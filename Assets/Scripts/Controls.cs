using UnityEngine;
using System.Collections;
public class Controls : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100;
    public static Controls instance;
    // Internal
    private float direction;
    private bool holding = false;
    private bool isEnabled = true;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        Input.simulateMouseWithTouches = true;
    }
    private void FixedUpdate()
    {
        if (!isEnabled) return;

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
    public IEnumerator PlayerBounce(float direction)
    {
        isEnabled = false;
        if (holding) transform.localEulerAngles =
            new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + moveSpeed * direction,
            transform.localEulerAngles.z);
        yield return new WaitForSeconds(0.05f);
        isEnabled = true;
    }
}