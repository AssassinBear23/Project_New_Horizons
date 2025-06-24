using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    /// <summary>
    /// Manages the overall game state, including player controls, UI, tree segments, and debugging utilities.
    /// Provides singleton access and controls game start, stop, and quitting logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the <see cref="GameManager"/>.
        /// </summary>
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// Gets or sets the input manager for the game.
        /// </summary>
        public InputManager InputManager { get; set; }

        /// <summary>
        /// Gets or sets the UI manager for the game.
        /// </summary>
        public UIManager UIManager { get; set; }

        #region Variables
        [field: Header("Game state")]
        /// <summary>
        /// Whether or not the game can be paused.
        /// </summary>
        [Tooltip("Whether or not the game can be paused")]
        [field: SerializeField] public bool AllowTogglePause { get; set; }
        [field: SerializeField] public bool IsPaused { get; private set; } = true;

        [Header("Internal")]
        [SerializeField] private List<TreeController> m_treeSegments = new List<TreeController>();

        /// <summary>
        /// Gets or sets the player controls component.
        /// </summary>
        public PlayerControls PlayerControls { get; set; }

        [SerializeField] private GameObject m_playerObject;

        [Header("Debugging")]
        [SerializeField] private bool m_showFPSIndicator = false;
        [SerializeField] private TMP_Text m_fpsIndicator;
        [SerializeField, Range(0, 2f)] private float m_fpsUpdateIntervalTime = 1f;

        [Header("Events")]
        [SerializeField] private UnityEvent onSetupFinished;
        #endregion
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
            if (m_showFPSIndicator)
                UpdateFpsIndicator();
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
        /// Starts the gameplay by enabling player controls and all tree segments.
        /// </summary>
        /// <exception cref="Exception">Thrown if no tree segments are available to start gameplay.</exception>
        private void StartGameplay()
        {
            if (m_treeSegments.Count == 0)
                throw new Exception("No tree segments available to start gameplay.");

            PlayerControls.enabled = true;
            m_playerObject.SetActive(true);

            int count = 0;
            foreach (TreeController treeSegment in m_treeSegments)
            {
                count++;
                treeSegment.enabled = true;
            }
        }

        /// <summary>
        /// Stops the gameplay by disabling player controls and all tree segments.
        /// </summary>
        public void StopGameplay()
        {
            PlayerControls.enabled = false;
            foreach (TreeController treeSegment in m_treeSegments)
            {
                treeSegment.enabled = false;
            }
        }

        public void ToggleGameplayState()
        {
            if (!AllowTogglePause)
                return;

            if (IsPaused)
            {
                StartGameplay();
                IsPaused = false;
            }
            else if(!IsPaused)
            {
                StopGameplay();
                IsPaused = true;
            }

            UIManager.TogglePause();
        }

        /// <summary>
        /// Adds a tree segment to the managed list.
        /// </summary>
        /// <param name="treeSegment">The tree segment to add.</param>
        public void AddTreeSegment(TreeController treeSegment)
        {
            if (!m_treeSegments.Contains(treeSegment)) m_treeSegments.Add(treeSegment);
        }

        /// <summary>
        /// Removes a tree segment from the managed list.
        /// </summary>
        /// <param name="treeSegment">The tree segment to remove.</param>
        public void RemoveTreeSegment(TreeController treeSegment)
        {
            if (m_treeSegments.Contains(treeSegment)) m_treeSegments.Remove(treeSegment);
        }

        /// <summary>
        /// Returns a list of all the branches on the last layer of the last placed & finished tree segment
        /// </summary>
        /// <returns></returns>
        public (List<Transform>, bool) GetLastBranchList()
        {
            BranchPlacingAlgorithm reference = m_treeSegments[m_treeSegments.Count - 2].GetComponent<BranchPlacingAlgorithm>();
            List<Transform> transforms = reference.lastBranches;
            bool isBird = reference.lastWasBird;
            return (transforms, isBird);
        }
    }
}