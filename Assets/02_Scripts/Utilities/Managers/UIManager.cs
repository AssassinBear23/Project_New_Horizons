using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Managers
{
    using UI.Elements;

    /// <summary>
    /// Manages the UI elements in the game.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// The pages (panels) managed by this UI Manager.
        /// </summary>
        [Header("Page Management")]
        [Tooltip("The pages (panels) managed by this UI Manager")]
        public List<UIPage> pages;

        /// <summary>
        /// The pause menu page's index.
        /// </summary>
        [Header("Pause Settings")]
        [Tooltip("The pause menu page's index\n defaults to 1")]
        [SerializeField] private UIPage m_pausePageIndex;
        [Tooltip("The gameplay UI page that is active when the game is not paused")]
        [SerializeField] private UIPage m_gameplayUI;

        // A list of all UIElements for this page.
        private List<UIElement> UIElements;

        // The event system that manages UI navigation
        [HideInInspector] public EventSystem eventSystem;
        // The input manager to list for pausing
        [SerializeField] InputManager m_inputManager;
        // The game manager instance in the scene
        [SerializeField] GameManager m_gm;
        #endregion Variables

        #region Methods
        #region SetupMethods
        /// <summary>
        /// Default Unity Method called when the script is first loaded.
        /// </summary>
        private void Start()
        {
            SetupEventSystem();
            UpdateElements();
        }

        /// <summary>
        /// Method to get all UI elements in the scene.
        /// </summary>
        void GetUIElements()
        {
            UIElements = FindObjectsByType<UIElement>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
            //Debug.Log("Amount of UI Elements:\t" + UIElements.Count);
            //for(int i = 0; i < UIElements.Count; i++)
            //{
            //    Debug.Log("Element " + i + ":\t" + UIElements[i].name);
            //}
        }

        /// <summary>
        /// Sets up the inputManager singleton reference.
        /// </summary>
        private void SetupInputManager()
        {
            if (m_inputManager == null)
            {
                m_inputManager = InputManager.Instance;
            }
            if (m_inputManager == null)
            {
                Debug.LogWarning($"There is no {nameof(m_inputManager)} in the scene. Make sure to add one to the scene otherwise you cannot pause the game");
            }
        }

        /// <summary>
        /// Sets the <see cref="eventSystem"/> variable to the EventSystem in the scene.
        /// </summary>
        private void SetupEventSystem()
        {
            eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogWarning($"There is no {nameof(eventSystem)} in the scene. Make sure to add one to the scene");
            }
        }

        /// <summary>
        /// Sets up the UIManager singleton m_gm in <see cref="GameManager.UIManager"/>.
        /// </summary>
        public void SetupUIManager()
        {
            if (GameManager.Instance != null && GameManager.Instance.UIManager == null)
            {
                GameManager.Instance.UIManager = this;
            }

            SetupInputManager();
        }
        #endregion Setup Methods
        #region FunctionalMethods

        /// <summary>
        /// Updates all UI elements in the <see cref="UIElements"/> list.
        /// </summary>
        public void UpdateElements()
        {
            GetUIElements();
            foreach (UIElement element in UIElements)
            {
                element.UpdateElement();
            }
        }

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        public void TogglePause(bool shouldPause)
        {
            if (shouldPause)
                GoToPage(m_pausePageIndex);
            else if (!shouldPause)
                GoToPage(m_gameplayUI);
        }

        /// <summary>
        /// Goes to a page by that page's index.
        /// </summary>
        /// <param name="pageIndex">The index in the page list to go to</param>
        public void GoToPage(int pageIndex)
        {
            if (pageIndex < pages.Count && pages[pageIndex] != null)
            {
                SetActiveAllPages(false);
                pages[pageIndex].gameObject.SetActive(true);
                pages[pageIndex].SetSelectedUIToDefault();
            }
            GetUIElements();
        }

        /// <summary>
        /// Goes to a page by that page's index.
        /// </summary>
        /// <param name="pageIndex">The index in the page list to go to</param>
        public void GoToPage(UIPage page)
        {
            int pageIndex = pages.IndexOf(page);


            try
            {
                if (pageIndex < pages.Count && pages[pageIndex] != null)
                {
                    SetActiveAllPages(false);
                    pages[pageIndex].gameObject.SetActive(true);
                    pages[pageIndex].SetSelectedUIToDefault();
                }
                GetUIElements();
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogError($"Error while trying to go to page {page.name} with index {pageIndex}: {e.Message}");
            }
        }

        public void GoToOriginPage(UIPage currentPage)
        {
            UIPage toGoToPage = currentPage.OriginalCallerPage;
            if (toGoToPage == null)
                return;

            UIManager uiManager = GameManager.Instance.UIManager;

            int index = uiManager.pages.IndexOf(toGoToPage);
            uiManager.GoToPage(index);
        }

        /// <summary>
        /// Turns all stored pages on or off depending on the passed parameter.
        /// </summary>
        /// <param name="activeState">The state to set all pages to, true to active them all, false to deactivate them all</param>
        private void SetActiveAllPages(bool activeState)
        {
            if (pages == null)
            {
                return;
            }
            foreach (UIPage page in pages)
            {
                if (page != null)
                {
                    page.gameObject.SetActive(activeState);
                }
            }
        }

        #endregion
        #endregion Methods
    }
}