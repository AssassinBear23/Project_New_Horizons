    using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    using Terrain;

    /// <summary>
    /// Manages the overall game state, including player controls, UI, tree segments, and debugging utilities.
    /// Provides singleton access and controls game start, stop, and quitting logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region ManagerReferences
        /// <summary>
        /// Singleton instance of the <see cref="GameManager"/>.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Terrain.TreeManager"/> instance responsible for managing tree-related operations.
        /// </summary>
        public TreeManager TreeManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Managers.InputManager"/> instance for handling player input and movement.
        /// </summary>
        public InputManager InputManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Managers.SoundManager"/> instance for managing sound effects and music in the game.
        /// </summary>
        public SoundManager SoundManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Managers.UIManager"/> instance for managing the game's user interface.
        /// </summary>
        public UIManager UIManager { get; set; }

        public PowerUpManager PowerUpManager;

        #endregion ManagerReferences

        #region Variables
        [field: Header("Game state")]
        /// <summary>
        /// Gets or sets a value indicating whether the game can be paused.
        /// </summary>
        [Tooltip("Whether or not the game can be paused")]
        [field: SerializeField] public bool AllowTogglePause { get; set; }

        /// <summary>
        /// Gets a value indicating whether the game is currently paused.
        /// </summary>
        [field: SerializeField] public bool IsPaused { get; private set; } = true;

        [Header("Gameplay")]
        /// <summary>
        /// The current score of the active run.
        /// </summary>
        private float m_currentScore = 0f;
        [SerializeField]
        private float m_scoreMultiplier = 100f;

        [Header("Internal")]
        /// <summary>
        /// Gets or sets the player controls component.
        /// </summary>
        public PlayerControls PlayerControls { get; set; }

        /// <summary>
        /// The player GameObject reference.
        /// </summary>
        [SerializeField] private GameObject m_playerObject;

        [Header("Debugging")]
        /// <summary>
        /// Whether to show the FPS indicator in the UI.
        /// </summary>
        [SerializeField] private bool m_showFpsIndicator = false;

        /// <summary>
        /// Reference to the TMP_Text component used for displaying FPS.
        /// </summary>
        [SerializeField] private TMP_Text m_fpsIndicator;

        /// <summary>
        /// The interval in seconds at which the FPS indicator updates.
        /// </summary>
        [SerializeField, Range(0, 2f)] private float m_fpsUpdateIntervalTime = 1f;

        [Header("Events")]
        /// <summary>
        /// Event invoked when setup is finished.
        /// </summary>
        [SerializeField] private UnityEvent onSetupFinished;

        /// <summary>
        /// Event invoked when gameplay is stopped.
        /// </summary>
        [SerializeField] private UnityEvent onStopGameplay;

        /// <summary>
        /// Event invoked when gameplay is started.
        /// </summary>
        [SerializeField] private UnityEvent onStartGameplay;

        /// <summary>
        /// Event invoked when the game is over.
        /// </summary>
        [SerializeField] private UnityEvent onGameOver;
        #endregion Variables

        #region Methods
        /// <summary>
        /// Unity Awake callback. Initializes singleton instance, sets target frame rate, and invokes setup event.
        /// </summary>
        private void Awake()
        {
            // Set the fps target to the current screen's refresh rate
            Application.targetFrameRate = QualitySettings.vSyncCount >= 1 ? (int)Screen.currentResolution.refreshRateRatio.value * QualitySettings.vSyncCount : (int)Screen.currentResolution.refreshRateRatio.value * 2;

            GetFpsIndicator();

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            onSetupFinished?.Invoke();
        }

        /// <summary>
        /// Toggles the ability to pause the game.
        /// </summary>
        public void ToggleAllowPause()
        {
            AllowTogglePause = !AllowTogglePause;
        }

        /// <summary>
        /// Time counter for FPS calculation.
        /// </summary>
        private float m_timeCounter = 0f;

        /// <summary>
        /// Last calculated framerate.
        /// </summary>
        private float m_lastFramerate = 0f;

        /// <summary>
        /// Frame count for FPS calculation.
        /// </summary>
        private int m_frameCount = 0;

        /// <summary>
        /// Unity Update callback. Updates the FPS indicator if enabled.
        /// </summary>
        private void Update()
        {
            if (m_showFpsIndicator)
                UpdateFpsIndicator();
        }

        /// <summary>
        /// Gets the current score of the game.
        /// </summary>
        /// <returns>The score of the currently active run.</returns>
        public float GetCurrentScore()
        {
            return m_currentScore;
        }

        /// <summary>
        /// Adds a specified amount to the current score.
        /// </summary>
        /// <param name="scoreToAdd">The amount of score to add to the score of the current run.</param>
        public void AddToCurrentScore(float scoreToAdd)
        {
            m_currentScore += scoreToAdd * m_scoreMultiplier;
        }

        /// <summary>
        /// Saves the current score as the high score if it exceeds the existing high score.
        /// </summary>
        public void SaveScore()
        {
            Debug.Assert(m_currentScore > GetHighscore(), "Current score is greater than high score, proceeding to save!");
            if (m_currentScore > GetHighscore())
            {
                PlayerPrefs.SetFloat("HighScore", m_currentScore);
                Debug.Log($"New high score saved: {GetHighscore()}");
            }
        }

        /// <summary>
        /// Unity callback that is invoked when the application is quitting.
        /// Ensures that all PlayerPrefs are saved before the application closes.
        /// </summary>
        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets the saved high score from PlayerPrefs.
        /// </summary>
        /// <returns>The saved high score, or <see cref="Mathf.Infinity"/> if not set.</returns>
        public float GetHighscore()
        {
            return PlayerPrefs.GetFloat("HighScore", 0);
        }

        /// <summary>
        /// Updates the FPS indicator text based on the current frame rate.
        /// </summary>
        private void UpdateFpsIndicator()
        {
            if (m_timeCounter < m_fpsUpdateIntervalTime)
            {
                m_timeCounter += Time.deltaTime;
                m_frameCount++;
            }
            else
            {
                m_lastFramerate = (float)m_frameCount / m_timeCounter;
                m_fpsIndicator.text = $"{m_lastFramerate:F0} FPS";
                m_timeCounter = 0f;
                m_frameCount = 0;
            }
        }

        /// <summary>
        /// Finds and assigns the FPS indicator text element if not already set.
        /// </summary>
        private void GetFpsIndicator()
        {
            if (m_fpsIndicator == null)
            {
                List<TMP_Text> textElements = FindObjectsByType<TMP_Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
                foreach (TMP_Text textElement in textElements)
                {
                    if (textElement.name.Contains("FPS", StringComparison.OrdinalIgnoreCase))
                    {
                        m_fpsIndicator = textElement;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Starts gameplay and invokes the start gameplay event.
        /// </summary>
        private void StartGameplay()
        {
            onStartGameplay?.Invoke();
        }

        /// <summary>
        /// Stops gameplay and invokes the stop gameplay event.
        /// </summary>
        private void StopGameplay()
        {
            onStopGameplay?.Invoke();
        }

        /// <summary>
        /// Toggles the state of the game between paused and playing.
        /// </summary>
        /// <remarks>
        /// Might as well be combined with the <see cref="TogglePause"/> method. But no, cause YOLO.
        /// </remarks>
        public void ToggleGameplayState()
        {
            if (!AllowTogglePause)
                return;

            if (IsPaused)
                StartGameplay();
            else if (!IsPaused)
                StopGameplay();
        }

        /// <summary>
        /// Toggles the pause value and calls the <see cref="UIManager.TogglePause(bool)"/> method on the <see cref="Managers.UIManager"/>.
        /// </summary>
        /// <remarks>
        /// I don't really know why I did it like this, but it works, so we ball.
        /// </remarks>
        public void TogglePause()
        {
            IsPaused = !IsPaused;
            UIManager.TogglePause(IsPaused);
        }

        /// <summary>
        /// Triggers the game over state, pauses the game, and invokes the game over event.
        /// </summary>
        public void GameOver()
        {
            IsPaused = true;
            onGameOver?.Invoke();
        }
        public GameObject GetPlayer()
        {
            return m_playerObject;
        }
        #endregion
    }
}