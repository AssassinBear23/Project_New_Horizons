using System.Collections.Generic;
using UnityEngine;


namespace Managers
{
    using AYellowpaper.SerializedCollections;
    using NaughtyAttributes;
    using Unity.VisualScripting;

    /// <summary>
    /// Manages the creation and playback of audio clips, including one-shot and looping sounds.
    /// Handles logic for sustaining and controlling audio playback in the game.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        #region Variables
        [Header("Setup")]
        /// <summary>
        /// The prefab used when playing a one-shot sound.
        /// Must contain a <see cref="OneShotSound"/> script component.
        /// </summary>
        [SerializeField, Tooltip("The prefab used when playing a oneShotSound. Must contain a OneShotSound script component")]
        [Required, ValidateInput("m_oneShotPrefab != null", "One-shot prefab must be assigned.")]
        private GameObject m_oneShotPrefab;

        /// <summary>
        /// The prefab used when playing a looping sound.
        /// Must contain a <see cref="LoopingSound"/> script component.
        /// </summary>
        [SerializeField, Tooltip("The prefab used when playing a looping sound. Must contain a LoopingSound script component.")]
        [Required, ValidateInput("m_loopingPrefab != null", "Looping prefab must be assigned.")]
        private GameObject m_loopingPrefab;

        /// <summary>
        /// The parent transform for all instantiated sound objects.
        /// </summary>
        private Transform m_soundParentTransform;

        [Header("Music settings")]
        /// <summary>
        /// Serialized dictionary mapping entry sound clips to their corresponding looping music clips.
        /// The relation is one-to-one or one-to-many.
        /// </summary>
        [SerializeField, Tooltip("List of serialized dictionaries that contain the entry sound clips and their corresponding looping music clips."
                                 + "\nRelation is one to one/many")]
        private SerializedDictionary<AudioClip, List<AudioClip>> m_loopingMusic;
        #endregion Variables

        #region Methods
        /// <summary>
        /// Sets up the SoundManager reference in the GameManager.
        /// Ensures only one instance exists and destroys duplicates.
        /// </summary>
        public void SetupSoundManager()
        {
            if (GameManager.Instance.SoundManager == null)
            {
                GameManager.Instance.SoundManager = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Creates a new <see cref="LoopingSound"/> object to play a looping sound.
        /// Plays the entry clip once, then transitions to the looping sound clip.
        /// </summary>
        /// <param name="entrySoundClip">The entry clip that is played once before the looping sound.</param>
        /// <param name="loopingSoundClip">The looping audio clip that is played after the entry clip finishes.</param>
        public void PlayLoopSound(AudioClip entrySoundClip, AudioClip loopingSoundClip)
        {
            // TODO: Implement the logic to play a looping sound.
            // 
            Debug.Log("Playing looping sound.");
        }

        /// <summary>
        /// Creates a new <see cref="LoopingSound"/> object to play a looping sound with a limited loop count.
        /// Plays the entry clip once, then transitions to the looping sound clip, stopping after a specified number of loops.
        /// </summary>
        /// <param name="entrySoundClip">The entry clip that is played once before the looping sound.</param>
        /// <param name="loopingSoundClip">The looping audio clip that is played after the entry clip finishes.</param>
        /// <param name="stopAfter">The number of times to loop the <see cref="LoopingSound"/> before stopping.</param>
        public void PlayLoopSound(AudioClip entrySoundClip, AudioClip loopingSoundClip, int stopAfter)
        {
            // TODO: Implement the logic to play a looping sound with limited loop count.
            // 
            Debug.Log("Playing looping sound.");
        }

        /// <summary>
        /// Plays a one-shot sound using the specified audio clip.
        /// </summary>
        /// <param name="soundClip">The audio clip to play as a one-shot sound.</param>
        public void PlayOneShotSound(AudioClip soundClip)
        {
            Debug.Log($"Playing one-shot sound:" +
                $"\n{soundClip.name}");

            // TODO: Implement the logic to play the sound clip as a one-shot sound.
        }

        /// <summary>
        /// Plays a spatial one-shot sound at the specified position using the given audio clip.
        /// </summary>
        /// <param name="soundClip">The audio clip to play as a one-shot sound.</param>
        /// <param name="spawnPosition">The world position at which to play the sound.</param>
        /// <param name="parentTransform">The transform of the parent to attach the clip to. Defaults to <c>null</c></param>
        public void PlaySpatialOneShotSound(AudioClip soundClip, Vector3 spawnPosition, Transform parentTransform = null)
        {
            InstantiatePrefab(m_oneShotPrefab, spawnPosition, parentTransform);
            Debug.Log($"Playing one-shot sound at position:" +
                $"\n{soundClip.name}" +
                $"\n{spawnPosition}");

            // TODO: Implement the logic to play the sound clip as a spatial one-shot sound.
        }

        /// <summary>
        /// Instantiates a sound prefab at the SoundManager's position and returns its sound component.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <returns>
        /// The <see cref="OneShotSound"/> or <see cref="LoopingSound"/> component attached to the instantiated object,
        /// or <c>null</c> if neither component is found.
        /// </returns>
        private AbstractSound InstantiatePrefab(GameObject prefab)
        {
            GameObject instantiatedObject = Instantiate(prefab, transform.position, Quaternion.identity, m_soundParentTransform);

            if (instantiatedObject.TryGetComponent<OneShotSound>(out var oneShotComponent))
                return oneShotComponent;
            else if (instantiatedObject.TryGetComponent<LoopingSound>(out var loopingComponent))
                return loopingComponent;
            else
            {
                Debug.LogWarning("Prefab does not contain a OneShotSound or LoopingSound component.");
                return null;
            }
        }

        /// <summary>
        /// Instantiates a sound prefab at the specified position and parent, and returns its sound component.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="position">The world position at which to instantiate the prefab.</param>
        /// <param name="parent">The parent transform for the instantiated object. If null, uses <see cref="m_soundParentTransform"/>.</param>
        /// <returns>
        /// The <see cref="OneShotSound"/> or <see cref="LoopingSound"/> component attached to the instantiated object,
        /// or <c>null</c> if neither component is found.
        /// </returns>
        private AbstractSound InstantiatePrefab(GameObject prefab, Vector3 position, Transform parent = null)
        {
            GameObject instantiatedObject = Instantiate(prefab, position, Quaternion.identity, parent != null ? parent : m_soundParentTransform);

            if (instantiatedObject.TryGetComponent<OneShotSound>(out var oneShotComponent))
                return oneShotComponent;
            else if (instantiatedObject.TryGetComponent<LoopingSound>(out var loopingComponent))
                return loopingComponent;
            else
            {
                Debug.LogWarning("Prefab does not contain a OneShotSound or LoopingSound component.");
                return null;
            }
        }
        #endregion
    }
}