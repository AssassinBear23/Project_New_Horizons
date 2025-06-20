using Managers;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles player input and movement logic, including rotation and temporary disabling (stun) effects.
/// </summary>
public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager m_inputManager;

    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 100;
    [SerializeField] private float m_stunTimer = 0.2f;
    private float m_direction = 0f;
    private bool m_isEnabled = true;

    /// <summary>
    /// Sets up the player controls by registering this instance with the <see cref="GameManager"/>
    /// and obtaining a reference to the <see cref="InputManager"/> singleton.
    /// </summary>
    public void SetupPlayerControls()
    {
        GameManager.Instance.PlayerControls = this;
        m_inputManager = InputManager.Instance;
    }

    /// <summary>
    /// Clamps the movement input value to the range [-1, 1].
    /// </summary>
    /// <param name="movement">The raw movement input value.</param>
    /// <returns>The clamped movement value.</returns>
    private float ClampMovementSpeed(float movement)
    {
        return Mathf.Clamp(movement, -1f, 1f);
    }

    /// <summary>
    /// Unity callback for physics updates. Handles player rotation based on input.
    /// </summary>
    private void FixedUpdate()
    {
        m_direction = ClampMovementSpeed(m_inputManager.RotationMovement);

        if (m_isEnabled)
            transform.localEulerAngles =
                new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y + m_moveSpeed * m_direction,
                transform.localEulerAngles.z);
    }

    /// <summary>
    /// Temporarily disables player controls and applies a bounce effect in the specified direction.
    /// After a stun duration, re-enables controls.
    /// </summary>
    /// <param name="direction">The direction and magnitude of the bounce.</param>
    /// <returns>An enumerator for coroutine execution.</returns>
    public IEnumerator PlayerBounce(float direction)
    {
        m_isEnabled = false;
        m_direction = direction * 1.5f;
        yield return new WaitForSeconds(m_stunTimer);
        m_isEnabled = true;
    }
}