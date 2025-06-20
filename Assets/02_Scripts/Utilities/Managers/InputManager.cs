using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Managers
{
    /// <summary>
    /// Manages input-related functionality and provides a globally accessible instance.
    /// </summary>
    /// <remarks>The <see cref="InputManager"/> class is implemented as a singleton, ensuring that only one
    /// instance exists throughout the application's lifetime. The instance is automatically created and persisted
    /// across scene loads. Use the <see cref="Instance"/> property to access the singleton instance.</remarks>
    public class InputManager : MonoBehaviour
    {
        #region Variables
        public static InputManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private InputActionAsset inputActions;

        // Input Actions
        private InputAction m_moveAction;

        // Control values
        private (float, float, float) m_tiltAmount; // Roll = Z-axis Phone, Pitch = X-Axis Phone, Yaw = Y-axis Phone
        private Vector3 m_accelerometerAmount;
        private (float, float, float) m_calibrationValue; // Roll = Z-axis Phone, Pitch = X-Axis Phone, Yaw = Y-axis Phone

        // Filtering values
        [Header("Accelerometer Dampening")]
        [SerializeField][Range(0f, 1f)] private float m_smoothingFactor = 0.2f;
        private Vector3 m_filteredAccel;

        [Header("Input Settings")]
        public bool IsInputEnabled { get; set; } = true;
        public bool IsPausePressed { get; private set; } = false;
        #endregion

        #region Setup Methods
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

        private void Awake()
        {
            m_moveAction = inputActions.FindActionMap("Player").FindAction("Move");
            m_filteredAccel = Vector3.zero;
        }
        #endregion Setup Methods

        #region Input Handling
        private void FixedUpdate()
        {
            m_accelerometerAmount = m_moveAction.ReadValue<Vector3>();
            //Exponential moving average filter
            m_filteredAccel = Vector3.Lerp(m_filteredAccel, m_accelerometerAmount, m_smoothingFactor);
            m_tiltAmount = GetTilt(m_filteredAccel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed && IsInputEnabled)
            {
                IsPausePressed = true;
                // Handle pause logic here, e.g., toggle pause menu
                Debug.Log("Pause pressed");
                StartCoroutine(ResetKey(() => IsPausePressed = false));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="accel"></param>
        /// <returns></returns>
        private (float, float, float) GetTilt(Vector3 accel)
        {
            Vector3 normAccel = accel.normalized;


            float pitch = Mathf.Atan2(normAccel.y, normAccel.z) * Mathf.Rad2Deg;

            // Roll
            float roll = Mathf.Atan2(-normAccel.x, Mathf.Sqrt(normAccel.y * normAccel.y + normAccel.z * normAccel.z)) * Mathf.Rad2Deg;

            // Applying the calibrated offset
            roll -= m_calibrationValue.Item1;

            roll *= -1f;

            return (roll, pitch, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accel"></param>
        /// <param name="useCalibratedValue"></param>
        /// <returns></returns>
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
        /// Coroutine that resets the specified boolean variable at the end of the frame
        /// </summary>
        /// <param name="resetAction">The action to reset the boolean variable.</param>
        /// <returns><see cref="IEnumerator"/>, allows this function to be used as a coroutine</returns>
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