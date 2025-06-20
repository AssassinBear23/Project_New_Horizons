using Managers;
using System.Collections;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager m_inputManager;

    [Header("Movement Settings")]
    [SerializeField, Range(0f, 1f)] private float m_mouseThreshold = 0.1f;
    [SerializeField] private float m_moveSpeed = 100;
    [SerializeField] private float m_stunTimer = 0.2f;

    private float m_direction = 0f;
    private bool m_isEnabled = true;

    public void SetupPlayerControls()
    {
        GameManager.Instance.PlayerControls = this;
        m_inputManager = InputManager.Instance;
    }

    private float ClampMovementSpeed(float movement)
    {
        return Mathf.Clamp(movement, -1f, 1f);
    }

    private void FixedUpdate()
    {
        //m_direction = ClampMovementSpeed(m_inputManager.Movement);

        transform.localEulerAngles =
            new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + m_moveSpeed * m_direction,
            transform.localEulerAngles.z);
    }

    public IEnumerator PlayerBounce(float direction)
    {
        m_isEnabled = false;
        m_direction = direction * 1.5f;
        yield return new WaitForSeconds(m_stunTimer);
        m_isEnabled = true;
    }
}