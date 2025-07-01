using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private InputAction m_moveAction;

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

        [Header("Swipe Power")]
        [Tooltip("The time window in which the player needs to reach the swipe threshold to activate the power")]
        [SerializeField] private float swipeWindow = 0.2f;
        [Tooltip("The distance the player needs to swipe down to activate the power")]
        [SerializeField] private float swipeThreshold = 2f;
        [HideInInspector] public bool swiped = false;
        private bool canSwipe = true;
        private bool isSwiping = true;
        private float SwipeMovement = 0;
        public UnityEvent OnSwipeEvent;

        [Header("Sensitivity Settings")]
        [SerializeField] private float m_sensitivity = 1f;
        [SerializeField] private float m_minSensitivity = 0.1f;
        [SerializeField] private float m_maxSensitivity = 10f;
        /// <summary>
        /// The sensitivity of the movement inputs. Clamped between <see cref="m_minSensitivity">m_minSensitivity</see> and <see cref="m_maxSensitivity">m_maxSensitivity</see>.
        /// </summary>
        public float Sensitivity
        {
            private get => m_sensitivity;

            set
            {
                if (value < m_minSensitivity || value > m_maxSensitivity)
                {
                    Debug.LogWarning($"Sensitivity value must be between {m_minSensitivity} and {m_maxSensitivity}. Clamping to valid range.");
                    m_sensitivity = Mathf.Clamp(value, m_minSensitivity, m_maxSensitivity);
                }
                else
                {
                    m_sensitivity = value;
                }
                Settings.Instance.ControlSettings.SetFloat("Sensitivity", m_sensitivity);
            }
        }

        [Header("Events")]
        [SerializeField] private UnityEvent m_pausePressed;
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
        /// Unity Awake callback. Initializes input actions and filtered accelerometer value.
        /// </summary>
        private void Awake()
        {
            m_moveAction = inputActions.FindActionMap("Player").FindAction("Move");
            m_filteredAccel = Vector3.zero;
        }

        private void Start()
        {
            Sensitivity = Settings.Instance.ControlSettings.GetFloat("Sensitivity").GetValueOrDefault(.5f);
        }
         #endregion Setup Methods

        #region Input Handling

        public void OnMove(InputValue input)
        {
            if (!IsInputEnabled)
                return;
            Vector2 inputValue = input.Get<Vector2>();
            RotationMovement = InputCleanUp(inputValue);
        }

        #region SwipePowerLogic
        public void OnSwipe(InputValue input)
        {
            if (!IsInputEnabled || swiped || !canSwipe)
                return;
            
            float inputValue = input.Get<Vector2>().y * m_sensitivity;

            if (isSwiping)
            {
                SwipeMovement += inputValue;
                if (SwipeMovement <= -swipeThreshold)
                {
                    DidSwipe();
                    isSwiping = false;
                }
            }

            // Start swiping if not swiping yet
            else if (inputValue <= 0)
            {
                isSwiping = true;
                SwipeMovement = inputValue;
                StartCoroutine(SwipeCheck());
            }
        }
        private IEnumerator SwipeCheck()
        {
            yield return new WaitForSeconds(swipeWindow);
            if (SwipeMovement <= -swipeThreshold && canSwipe)
            {
                DidSwipe();
            }
            isSwiping = false;
        }
        public IEnumerator SwipeCooldown(float time)
        {
            yield return null;
            swiped = false;
            yield return new WaitForSeconds(time - Time.deltaTime);
            canSwipe = true;
        }
        private void DidSwipe()
        {
            Debug.Log("successful swipe");
            canSwipe = false;
            swiped = true;
            OnSwipeEvent?.Invoke();
        }
        #endregion SwipePowerLogic

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
                    Debug.Log("In keyboard and mouse");
                    return inputValue.x * m_sensitivity;
                case "Phone":
                    Debug.Log("In phone");
                    return inputValue.x * m_sensitivity;
                case "PhoneGyro":
                    // Exponential moving average filter
                    m_filteredAccel = Vector3.Lerp(m_filteredAccel, m_accelerometerAmount, m_smoothingFactor);
                    m_tiltAmount = GetTilt(m_filteredAccel, true);
                    Debug.Log($"Phone Gyro: {m_tiltAmount.Item1}");
                    return m_tiltAmount.Item1 * m_sensitivity;
                default:
                    return inputValue.x * m_sensitivity;
            }
        }

        /// <summary>
        /// Sets the minimum deadzone value for the movement inputs.
        /// </summary>
        /// <param name="toSetMin">
        /// The minimum deadzone value to set. 
        /// Value must be between 0 and 1. Values outside this range will be clamped.
        /// </param>
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
                m_pausePressed?.Invoke();
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