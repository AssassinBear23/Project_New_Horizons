using Managers;
using UnityEngine;

public class SliderEnableEvent : MonoBehaviour
{
    [SerializeField] private string playerPrefsKey;
    private bool onEnableCalled = false;

    private void Start()
    {
        OnEnable();
        onEnableCalled = true;
    }

    private void OnEnable()
    {
        if (onEnableCalled)
        {
            onEnableCalled = false;
            return;
        }

        GameManager.Instance.SoundManager.SetStoredVolume(transform, playerPrefsKey);
    }
}
