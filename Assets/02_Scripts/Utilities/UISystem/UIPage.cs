#nullable enable

using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class stores relevant information about a page of UI
/// </summary>
public class UIPage : MonoBehaviour
{
    /// <summary>
    /// The default UI to have selected when opening this page.
    /// </summary>
    [Tooltip("The default UI to have selected when opening this page")]
    public GameObject? defaultSelected;
    /// <summary>
    /// This is the page that the player/user came from. If set to null, it means that this page is the first page in the UI flow.
    /// </summary>
    [Tooltip("The page that the player/user came from. If set to null, it means that this page is the first page in the UI flow.")]
    public UIPage? OriginalCallerPage { get; private set; }

    /// <summary>
    /// Sets the selected UI selectable to the default defined by this UIPage
    /// </summary>
    public void SetSelectedUIToDefault()
    {
        if (defaultSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultSelected);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetOriginalCallerPage()
    {
        GameObject eventCaller = EventSystem.current.currentSelectedGameObject;

        if (eventCaller == null)
            return;

        if (eventCaller.TryGetComponent<UIPage>(out UIPage page))
        {
            OriginalCallerPage = page;
        }
        else
        {
            page = eventCaller.GetComponentInParent<UIPage>();
            OriginalCallerPage = page;
        }
    }

    /// <summary>
    /// Sets the original caller page for the current context.
    /// </summary>
    /// <remarks>This method assigns the provided <see cref="UIPage"/> to the internal state, allowing
    /// subsequent operations to reference the original caller page. Ensure that the <paramref name="page"/> parameter
    /// is not null before calling this method.</remarks>
    /// <param name="page">The <see cref="UIPage"/> instance representing the original caller page. Cannot be null.</param>
    public void SetOriginalCallerPage(UIPage page)
    {
        if(page == null)
            throw new NullReferenceException("The provided UIPage cannot be null.");

        OriginalCallerPage = page;
    }
}
