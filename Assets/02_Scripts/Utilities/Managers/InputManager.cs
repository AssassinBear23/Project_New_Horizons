using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Managers
{
    /// <summary>
    /// Manages input-related functionality and provides a globally accessible instance.
    /// </summary>
    /// <remarks>
    /// The <see cref="InputManager"/> class is implemented as a singleton, ensuring that only one
    /// instance exists throughout the application's lifetime. The instance is automatically created and persisted
    /// across scene loads. Use the <see cref="Instance"/> property to access the singleton instance.
    /// </remarks>
    public class InputManager : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Gets the singleton instance of the <see cref="InputManager"/>.
        /// </summary>
        public static InputManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private PlayerInput playerInput;

        // Input Actions
        private InputAction m_moveAction;

        // Control values
        /// <summary>
        /// Stores the current tilt amount as a tuple (roll, pitch, yaw).
        /// Roll = Z-axis Phone, Pitch = X-Axis Phone, Yaw = Y-axis Phone.
        /// </summary>
        private (float, float, float) m_tiltAmount;
        /// <summary>
        /// Stores the current accelerometer value.
        /// </summary>
        private Vector3 m_accelerometerAmount;
        /// <summary>
        /// Stores the calibration value for tilt as a tuple (roll, pitch, yaw).
        /// </summary>
        private (float, float, float) m_calibrationValue;

        // Filtering values
        [Header("Accelerometer Dampening")]
        [SerializeField][Range(0f, 1f)] private float m_smoothingFactor = 0.2f;
        /// <summary>
        /// Stores the filtered accelerometer value.
        /// </summary>
        private Vector3 m_filteredAccel;

        [Header("Input Settings")]
        /// <summary>
        /// Gets or sets a value indicating whether input is enabled.
        /// </summary>
        public bool IsInputEnabled { get; set; } = true;
        /// <summary>
        /// Gets a value indicating whether the pause input was pressed.
        /// </summary>
        public bool IsPausePressed { get; private set; } = false;
        /// <summary>
        /// Gets the current rotation movement value.
        /// </summary>
        public float RotationMovement { get; private set; }
        #endregion

        #region Setup Methods
        /// <summary>
        /// Sets up the InputManager singleton instance and persists it across scene loads.
        /// Registers itself with the GameManager if available.
        /// </summary>
        public void SetupInputManager()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                if (GameManager.Instance == null)
                    return;

                GameManager.Instance.InputManager = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Switches the current control scheme to the specified scheme name.
        /// </summary>
        /// <param name="controlSchemeName">The name of the control scheme to switch to.</param>
        public void SwitchControlScheme(string controlSchemeName)
        {
            playerInput.SwitchCurrentControlScheme(controlSchemeName, null, null);
        }

        /// <summary>
        /// Unity Awake callback. Initializes input actions and filtered accelerometer value.
        /// </summary>
        private void Awake()
        {
            m_moveAction = inputActions.FindActionMap("Player").FindAction("Move");
            m_filteredAccel = Vector3.zero;
        }
        #endregion Setup Methods

        #region Input Handling
        /// <summary>
        /// Unity FixedUpdate callback. Reads accelerometer input, applies smoothing, and updates tilt amount.
        /// </summary>
        private void FixedUpdate()
        {
            Vector2 inputValue = m_moveAction.ReadValue<Vector2>();
            RotationMovement = InputCleanUp(inputValue);
        }

        /// <summary>
        /// Cleans up and processes the input value based on the current control scheme.
        /// </summary>
        /// <param name="inputValue">The input value to process.</param>
        /// <returns>The processed rotation movement value.</returns>
        private float InputCleanUp(Vector2 inputValue)
        {
            switch (playerInput.currentControlScheme)
            {
                case "Keyboard&Mouse":
                    return inputValue.x; // Use X-axis for keyboard and mouse input
                case "Phone":
                    return inputValue.x;
                case "PhoneGyro":
                    // Exponential moving average filter
                    m_filteredAccel = Vector3.Lerp(m_filteredAccel, m_accelerometerAmount, m_smoothingFactor);
                    m_tiltAmount = GetTilt(m_filteredAccel, true);
                    return m_tiltAmount.Item1;
                default:
                    return inputValue.x;
            }
        }

        public void SetMinDeadZone(float toSetMin)
        {
            if (toSetMin < 0f || toSetMin > 1f)
            {
                Debug.LogWarning("Deadzone value must be between 0 and 1. Clamping to valid range.");
                toSetMin = Mathf.Clamp(toSetMin, 0f, 1f);
            }

            m_moveAction.ApplyParameterOverride("StickDeadzone:min", toSetMin);
        }

        /// <summary>
        /// Handles the pause input action. Sets <see cref="IsPausePressed"/> and starts a coroutine to reset it.
        /// </summary>
        /// <param name="context">The input action callback context.</param>
        public void OnPause(InputValue context)
        {
            if (context.isPressed && IsInputEnabled)
            {
                Debug.Log("Pause pressed");
                // Handle pause logic here, e.g., toggle pause menu
            }
        }

        /// <summary>
        /// Calculates the tilt (roll, pitch, yaw) from the given accelerometer vector.
        /// Optionally applies calibration offset to roll and pitch.
        /// </summary>
        /// <param name="accel">The accelerometer vector.</param>
        /// <param name="useCalibratedValue">Whether to apply the calibration offset.</param>
        /// <returns>A tuple containing (roll, pitch, yaw).</returns>
        private (float, float, float) GetTilt(Vector3 accel, bool useCalibratedValue)
        {
            Vector3 normAccel = accel.normalized;

            // Roll
            float pitch = Mathf.Atan2(normAccel.y, normAccel.z) * Mathf.Rad2Deg;

            // Pitch
            float roll = Mathf.Atan2(-normAccel.x, Mathf.Sqrt(normAccel.y * normAccel.y + normAccel.z * normAccel.z)) * Mathf.Rad2Deg;

            // Applying the calibrated offset if specified
            if (useCalibratedValue)
            {
                roll -= m_calibrationValue.Item1;
            }

            roll *= -1f;
            pitch *= -1f;

            return (roll, pitch, 0f);
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Coroutine that resets the specified boolean variable at the end of the frame.
        /// </summary>
        /// <param name="resetAction">The action to reset the boolean variable.</param>
        /// <returns><see cref="IEnumerator"/>, allows this function to be used as a coroutine.</returns>
        public static IEnumerator ResetKey(System.Action resetAction)
        {
            /* How does this work:
             * The return type is a IEnumerator, which allows this function to be used as a coroutine.
             * 
             * The parameter is a System.Action, this means that you can pass  a lambda function.
             * System.Action is a delegate for that lambda function.
             * 
             * This allows the user to pass a lambda function that reset the desired boolean 
             * Example: StartCoroutine(ResetKey(() => pausePressed = false));
             */
            yield return new WaitForEndOfFrame();
            resetAction();
        }

        /// <summary>
        /// Calibrates the tilt by storing the current filtered accelerometer value as the calibration offset.
        /// </summary>
        /// <param name="context">The input action callback context.</param>
        public void CalibrateTilt(CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }

            m_calibrationValue = GetTilt(m_filteredAccel, false);
        }
        #endregion
    }
}