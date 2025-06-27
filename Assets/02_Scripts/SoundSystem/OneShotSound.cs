using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a sound component that plays an audio clip once (one-shot).
/// Inherits from <see cref="AbstractSound"/> and implements the contract for playing audio clips.
/// </summary>
public class OneShotSound : AbstractSound
{
    /// <summary>
    /// Plays the assigned <see cref="SoundClip"/> as a one-shot sound.
    /// </summary>
    public override void PlaySound(AudioClip audioClip)
    {
        SourceComponent.PlayOneShot(audioClip);
        StartCoroutine(DestroyObjectAfterSoundEnds());
    }

    /// <summary>
    /// Coroutine that waits until the <see cref="AudioSource"/> has finished playing,
    /// then destroys the associated <see cref="GameObject"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator"/> used by Unity to handle coroutine execution.
    /// </returns>
    private IEnumerator DestroyObjectAfterSoundEnds()
    {
        yield return new WaitUntil(() => !SourceComponent.isPlaying);

        Destroy(gameObject);
    }
}
