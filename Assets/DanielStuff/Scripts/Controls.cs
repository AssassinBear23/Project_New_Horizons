using System.Collections;
using UnityEngine;
public class Controls : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100;
    [SerializeField] private float stunTimer = 0.2f;
    [SerializeField, Range(0f,1f)] private float mouseThreshold = 0.1f;
    public static Controls instance;
    // Internal
    private float m_direction;
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
        transform.localEulerAngles =
            new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + moveSpeed * m_direction,
            transform.localEulerAngles.z);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && isEnabled)
        {
            if (Input.mousePositionDelta.x < -mouseThreshold)
            {
                m_direction = Mathf.Clamp01(Mathf.Abs(Input.mousePositionDelta.x));
            }

            else if (Input.mousePositionDelta.x > mouseThreshold)
            {
                m_direction = Mathf.Clamp(-Input.mousePositionDelta.x, -1, 0);
            }

            else m_direction = 0;
        }
        else 
            m_direction = 0;
    }
    public IEnumerator PlayerBounce(float direction)
    {
        isEnabled = false;
        m_direction = direction * 1.5f;
        yield return new WaitForSeconds(stunTimer);
        isEnabled = true;
    }
}