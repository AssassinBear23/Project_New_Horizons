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
        /// Gets or sets the <see cref="Terrain.TreeManager">TerrainManager</see> instance responsible for managing tree-related operations.
        /// </summary>
        public TreeManager TreeManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Managers.InputManager">InputManager</see> instance for handling player input and movement.
        /// </summary>
        public InputManager InputManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Managers.SoundManager"/> instance for managing sound effects and music in the game.
        /// </summary>
        public SoundManager SoundManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Managers.UIManager">UIManager</see> instance for managing the game's user interface.
        /// </summary>
        public UIManager UIManager { get; set; }

        public PowerUpManager PowerUpManager;

        #endregion ManagerReferences
        #region Variables
        [field: Header("Game state")]
        /// <summary>
        /// Whether or not the game can be paused.
        /// </summary>
        [Tooltip("Whether or not the game can be paused")]
        [field: SerializeField] public bool AllowTogglePause { get; set; }
        /// <summary>
        /// True if the game is paused, false if it is playing.
        /// </summary>
        [field: SerializeField] public bool IsPaused { get; private set; } = true;

        [Header("Gameplay")]
        /// <summary>
        /// 
        /// </summary>
        private float m_currentScore = 0f;

        [Header("Internal")]
        /// <summary>
        /// Gets or sets the player controls component.
        /// </summary>
        public PlayerControls PlayerControls { get; set; }

        [SerializeField] private GameObject m_playerObject;

        [Header("Debugging")]
        [SerializeField] private bool m_showFpsIndicator = false;
        [SerializeField] private TMP_Text m_fpsIndicator;
        [SerializeField, Range(0, 2f)] private float m_fpsUpdateIntervalTime = 1f;

        [Header("Events")]
        [SerializeField] private UnityEvent onSetupFinished;
        [SerializeField] private UnityEvent onStopGameplay;
        [SerializeField] private UnityEvent onStartGameplay;
        [SerializeField] private UnityEvent onGameOver;
        #endregion Variables

        #region Methods
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

        private float m_timeCounter = 0f;
        private float m_lastFramerate = 0f;
        private int m_frameCount = 0;

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
        /// <param name="scoreToAdd">The amount of score to add to the score of the current run</param>
        public void AddToCurrentScore(float scoreToAdd)
        {
            m_currentScore += scoreToAdd;
        }

        /// <summary>
        /// Saves the current score as the high score if it exceeds the existing high score.
        /// </summary>
        public void SaveScore()
        {
            float currentHighscore = PlayerPrefs.GetFloat("HighScore");

            if (m_currentScore > currentHighscore)
            {
                PlayerPrefs.SetFloat("HighScore", m_currentScore);
            }
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

        private void StartGameplay()
        {
            onStartGameplay?.Invoke();
        }

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
        /// Toggles the pause value and calls the <see cref="UIManager.TogglePause(bool)">TogglePause</see> method on the <see cref="Managers.UIManager">UIManager</see>.
        /// </summary>
        /// <remarks>
        /// I don't really know why I did it like this, but it works, so we ball.
        /// </remarks>
        public void TogglePause()
        {
            IsPaused = !IsPaused;
            UIManager.TogglePause(IsPaused);
        }
        public void GameOver()
        {
            IsPaused = true;
            onGameOver?.Invoke();
        }
        #endregion
    }
}