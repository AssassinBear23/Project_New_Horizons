using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Video;
using System.Collections;
public class SkipAnimationScene : MonoBehaviour
{
    public PlayerInput playerInput;
    public UnityEvent onClickEvent;
    public VideoPlayer video;
    bool started = false;
    public void OnClick()
    {
        onClickEvent?.Invoke();
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5);
        started = true;
    }
    private void Update()
    {
        if (!video.isPlaying && started)
        {
            onClickEvent?.Invoke();
        }
    }        
}