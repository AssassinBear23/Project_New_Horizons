using UnityEngine;

/// <summary>
/// Abstract base class for sound components in the sound system.
/// Provides a contract for playing audio clips.
/// </summary>
public abstract class AbstractSound : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the <see cref="AudioSource"/> component used for playing the sound.
    /// </summary>
    [field:SerializeField] public AudioSource SourceComponent { get; set; }

    /// <summary>
    /// Plays the assigned <see cref="SoundClip"/>.
    /// Implementations should define how the sound is played.
    /// </summary>
    public abstract void PlaySound(AudioClip audioClip);
}
