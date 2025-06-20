using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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

        [Header("Internal")]
        [SerializeField] private List<TreeController> m_treeSegments = new List<TreeController>();
        public PlayerControls PlayerControls { get; set; }

        [SerializeField] private TMP_Text fpsIndicator;
        [SerializeField, Range(0, 2f)] private float m_fpsUpdateIntervalTime = 1f;

        [Header("Events")]
        [SerializeField] private UnityEvent onSetupFinished;
        #endregion

        private void Awake()
        {
            // Set the fps target to the current screen's refresh rate
            Application.targetFrameRate = QualitySettings.vSyncCount >= 1 ? 60 : (int)Screen.currentResolution.refreshRateRatio.value * 2;

            GetFpsIndicator();

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            onSetupFinished?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ToggleAllowPause()
        {
            AllowPause = !AllowPause;
        }

        private float m_timeCounter = 0f;
        private float m_lastFramerate = 0f;
        private int m_frameCount = 0;

        private void Update()
        {
            if (m_timeCounter < m_fpsUpdateIntervalTime)
            {
                m_timeCounter += Time.deltaTime;
                m_frameCount++;
            }
            else
            {
                m_lastFramerate = (float)m_frameCount / m_timeCounter;
                fpsIndicator.text = $"{m_lastFramerate:F0} FPS";
                m_timeCounter = 0f;
                m_frameCount = 0;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void GetFpsIndicator()
        {
            if (fpsIndicator == null)
            {
                List<TMP_Text> textElements = FindObjectsByType<TMP_Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
                foreach (TMP_Text textElement in textElements)
                {
                    if (textElement.name.Contains("FPS", StringComparison.OrdinalIgnoreCase))
                    {
                        fpsIndicator = textElement;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartGamePlay()
        {
            if (m_treeSegments.Count == 0)
                throw new Exception("No tree segments available to start gameplay.");

            PlayerControls.enabled = true;
            int count = 0;
            foreach (TreeController treeSegment in m_treeSegments)
            {
                count++;
                Debug.Log($"Tree Segment {count} enabled: {treeSegment.name}");
                treeSegment.enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopGameplay()
        {
            PlayerControls.enabled = false;
            foreach (TreeController treeSegment in m_treeSegments)
            {
                treeSegment.enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeSegment"></param>
        public void AddTreeSegment(TreeController treeSegment)
        {
            m_treeSegments.Add(treeSegment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeSegment"></param>
        public void RemoveTreeSegment(TreeController treeSegment)
        {
            m_treeSegments.Remove(treeSegment);
        }

        /// <summary>
        /// 
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quiting Application");
#endif
            StaticMethods.QuitApplication();
        }
    }
}