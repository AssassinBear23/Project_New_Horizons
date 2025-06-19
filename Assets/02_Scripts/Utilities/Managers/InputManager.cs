using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public bool IsInputEnabled { get; set; } = true;
        public bool IsPausePressed { get; private set; } = false;
        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region Input Handling


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
        #endregion
    }
}