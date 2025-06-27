using UnityEngine;

/// <summary>
/// Represents a sound that can be played in a loop using an <see cref="AudioClip"/>.
/// Inherits from <see cref="AbstractSound"/>.
/// </summary>
public class LoopingSound : AbstractSound
{
    #region Variables
    /// <summary>
    /// Gets or sets the <see cref="AudioClip"/> to be played in a loop.
    /// </summary>
    public AudioClip LoopingSoundClip { private get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the sound should loop infinitely.
    /// If set to <c>true</c>, the sound will loop indefinitely.
    /// </summary>
    public bool IsInfiniteLooping { get; set; } = true;

    private int m_loopCount = 0;

    /// <summary>
    /// Gets or sets the number of times the sound should loop.
    /// If <see cref="IsInfiniteLooping"/> is <c>true</c>, this value is ignored.
    /// </summary>
    public int LoopCount
    {
        get
        {
            return m_loopCount;
        }
        set
        {
            if (!IsInfiniteLooping)
                Debug.LogWarning($"LoopCount will not be used if {nameof(IsInfiniteLooping)} is set to {false}.");
            m_loopCount = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }
    #endregion Variables

    /// <summary>
    /// Plays the looping sound using the assigned <see cref="LoopingSoundClip"/>.<br/>
    /// If <see cref="IsInfiniteLooping"/> is <c>true</c>, the sound will loop indefinitely.
    /// Otherwise, it will play for the specified <see cref="LoopCount"/> times.
    /// </summary>
    public override void PlaySound(AudioClip audioClip)
    {
        // Guard block
        if (LoopingSoundClip == null || audioClip == null)
            return;
    }
}
