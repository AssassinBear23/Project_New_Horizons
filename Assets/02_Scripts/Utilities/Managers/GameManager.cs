using System.Collections;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public InputManager InputManager { get; set; }
        public UIManager UIManager { get; set; }

        #region Variables
        [Header("Game state options")]
        /// <summary>
        /// Whether or not the game can be paused.
        /// </summary>
        [Tooltip("Whether or not the game can be paused")]
        public bool AllowPause { get; set; }


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

        public void ToggleAllowPause()
        {
            AllowPause = !AllowPause;
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quiting Application");
#endif
            StaticMethods.QuitApplication();
        }
    }
}