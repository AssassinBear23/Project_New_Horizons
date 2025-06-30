using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace Managers
{
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
        [SerializeField]
        private Transform m_soundParentTransform;

        /// <summary>
        /// The audio mixer used for controlling audio groups and volume.
        /// </summary>
        [SerializeField]
        private AudioMixer m_audioMixer;

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
                SetMasterVolume(Settings.Instance.SoundSettings.GetFloat("MasterVolume").GetValueOrDefault(100f));
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
            Debug.Log("Playing looping sound.");

            // TODO: Implement the logic to play a looping sound.
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
            Debug.Log($"Playing x{stopAfter} looping sound.");

            // TODO: Implement the logic to play a looping sound with limited loop count.
        }

        /// <summary>
        /// Plays a one-shot sound using the specified audio clip.
        /// </summary>
        /// <param name="soundClip">The audio clip to play as a one-shot sound.</param>
        public void PlayOneShotSound(AudioClip soundClip)
        {
            Debug.Log($"Playing one-shot sound:" +
                $"\n{soundClip.name}");

            OneShotSound component = (OneShotSound)InstantiatePrefab(m_oneShotPrefab);
            component.PlaySound(soundClip);
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
            Debug.Log($"Playing one-shot sound at position:" +
                $"\n{soundClip.name}" +
                $"\n{spawnPosition}");
            OneShotSound component = (OneShotSound)InstantiatePrefab(m_oneShotPrefab, spawnPosition, parentTransform);
            component.PlaySound(soundClip);

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
                Debug.LogWarning($"Prefab does not contain a {nameof(OneShotSound)} or {nameof(LoopingSound)}.");
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

        #region Sound Control Methods

        /// <summary>
        /// Sets the master volume in the audio mixer and updates the settings.
        /// </summary>
        /// <param name="sliderValue">The new master volume value.</param>
        public void SetMasterVolume(float sliderValue)
        {
            m_audioMixer.SetFloat("MasterVolume", sliderValue);
            Settings.Instance.SoundSettings.SetFloat("MasterVolume", sliderValue);
        }

        /// <summary>
        /// Sets the music volume in the audio mixer and updates the settings.
        /// </summary>
        /// <param name="sliderValue">The new music volume value.</param>
        public void SetMusicVolume(float sliderValue)
        {
            m_audioMixer.SetFloat("MusicVolume", sliderValue);
            Settings.Instance.SoundSettings.SetFloat("MusicVolume", sliderValue);
        }

        /// <summary>
        /// Sets the SFX volume in the audio mixer and updates the settings.
        /// </summary>
        /// <param name="sliderValue">The new SFX volume value.</param>
        public void SetSFXVolume(float sliderValue)
        {
            m_audioMixer.SetFloat("SFXVolume", sliderValue);
            Settings.Instance.SoundSettings.SetFloat("SFXVolume", sliderValue);
        }

        /// <summary>
        /// Sets the volume for a specific audio mixer group and updates the settings.
        /// </summary>
        /// <param name="mixerGroup">The audio mixer group to set the volume for.</param>
        /// <param name="sliderValue">The new volume value.</param>
        public void SetGroupVolume(AudioMixerGroup mixerGroup, float sliderValue)
        {
            m_audioMixer.SetFloat(mixerGroup.name + "Volume", sliderValue);
            Settings.Instance.SoundSettings.SetFloat(mixerGroup.name + "Volume", sliderValue);
        }

        /// <summary>
        /// Stores the volume values for audio mixer groups when muting.
        /// </summary>
        private HashSet<(string, float)> storedVolumeValues = new();

        /// <summary>
        /// Toggles mute for a specific audio mixer group, restoring the previous volume if unmuted.
        /// </summary>
        /// <param name="mixerGroup">The audio mixer group to mute or unmute.</param>
        public void ToggleMute(AudioMixerGroup mixerGroup)
        {
            if (m_audioMixer.GetFloat(mixerGroup.name + "Volume", out float currentVolume))
            {
                if (currentVolume == 0f)
                {
                    // Restore the stored volume value
                    if (GetStoredVolume(mixerGroup.name, out float storedVolume))
                        m_audioMixer.SetFloat(mixerGroup.name + "Volume", storedVolume);
                }
                else
                {
                    storedVolumeValues.Add((mixerGroup.name, currentVolume));
                    m_audioMixer.SetFloat(mixerGroup.name + "Volume", 0f);
                }
            }
            else
                Debug.LogWarning($"AudioMixerGroup '{mixerGroup.name}' not found or does not have a volume parameter.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetStoredVolume(string groupName, out float value)
        {
            if (storedVolumeValues != null)
            {
                foreach (var entry in storedVolumeValues)
                {
                    if (entry.Item1 == groupName)
                    {
                        value = entry.Item2;
                        storedVolumeValues.Remove(entry);
                        return true;
                    }
                }
            }
            value = 1f;
            return false;
        }

        #endregion Sound Control Methods
        #endregion Methods
    }
}