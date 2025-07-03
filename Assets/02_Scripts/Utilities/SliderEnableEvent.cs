using Managers;
using UnityEngine;
using UnityEngine.Events;

public class SliderEnableEvent : MonoBehaviour
{
    [SerializeField] private string playerPrefsKey;
    private bool onEnableCalled = false;
    public UnityEvent<Transform, string> doSomething;
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

        doSomething?.Invoke(transform, playerPrefsKey);
        //GameManager.Instance.SoundManager.SetStoredVolume(transform, playerPrefsKey);
    }
}
