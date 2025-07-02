using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using System;
using UnityEngine;

/// <summary>
/// Singleton MonoBehaviour for managing application settings such as sound and controls.
/// Provides methods for saving and loading settings using Unity's PlayerPrefs.
/// </summary>
public class Settings : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the Settings class.
    /// </summary>
    public static Settings Instance { get; private set; }

    /// <summary>
    /// Settings related to sound.
    /// </summary>
    [field: SerializeField] public BaseSettings SoundSettings { get; private set; } = new("Sound");

    /// <summary>
    /// Settings related to controls.
    /// </summary>
    [field: SerializeField] public BaseSettings ControlSettings { get; private set; } = new("Control");

    /// <summary>
    /// Ensures only one instance of Settings exists and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    [Button]
    public void DestroyPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// Saves all settings when the application quits.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    /// <summary>
    /// Saves all sound and control settings to PlayerPrefs.
    /// </summary>
    public void SaveSettings()
    {
        SoundSettings.SaveAllSettings();
        ControlSettings.SaveAllSettings();
    }
}

/// <summary>
/// Serializable class for storing and managing different types of settings (float, bool, int).
/// Uses SerializedDictionary for Unity serialization support.
/// </summary>
[Serializable]
public class BaseSettings
{
    [SerializeField] private string _prefix = "";
    [SerializeField] private SerializedDictionary<string, float> m_FloatValues = new();
    [SerializeField] private SerializedDictionary<string, bool> m_BoolValues = new();
    [SerializeField] private SerializedDictionary<string, int> m_intValues = new();

    public BaseSettings(string prefix = "")
    {
        _prefix = prefix;
    }

    private string GetPrefixedKey(string key) => $"{_prefix}_{key}";

    /// <summary>
    /// Gets a float setting by key.
    /// </summary>
    /// <param name="key">The key of the float setting.</param>
    /// <returns>The float value if found, otherwise null.</returns>
    public float? GetFloat(string key)
    {
        if (m_FloatValues.TryGetValue(key, out var value))
            return value;
        else if (LoadSetting(key, typeof(float), out var result))
        {
            SetFloat(key, (float)result); // Cache the value for future use
            return (float)result;
        }
        return null;
    }

    /// <summary>
    /// Sets a float setting by key.
    /// </summary>
    /// <param name="key">The key of the float setting.</param>
    /// <param name="value">The float value to set.</param>
    public void SetFloat(string key, float value)
    {
        m_FloatValues[key] = value;
    }

    /// <summary>
    /// Gets a bool setting by key.
    /// </summary>
    /// <param name="key">The key of the bool setting.</param>
    /// <returns>The bool value if found, otherwise null.</returns>
    public bool? GetBool(string key)
    {
        if (m_BoolValues.TryGetValue(key, out var value))
            return value;
        else if (LoadSetting(key, typeof(bool), out var result))
        {
            SetBool(key, (bool)result);
            return (bool)result;
        }
        return null;
    }

    /// <summary>
    /// Sets a bool setting by key.
    /// </summary>
    /// <param name="key">The key of the bool setting.</param>
    /// <param name="value">The bool value to set.</param>
    public void SetBool(string key, bool value)
    {
        m_BoolValues[key] = value;
    }

    /// <summary>
    /// Gets an int setting by key.
    /// </summary>
    /// <param name="key">The key of the int setting.</param>
    /// <returns>The int value if found, otherwise null.</returns>
    public int? GetInt(string key)
    {
        if (m_intValues.TryGetValue(key, out var value))
            return value;
        else if (LoadSetting(key, typeof(int), out var result))
        {
            SetInt(key, (int)result);
            return (int)result;
        }
        return null;
    }

    /// <summary>
    /// Sets an int setting by key.
    /// </summary>
    /// <param name="key">The key of the int setting.</param>
    /// <param name="value">The int value to set.</param>
    public void SetInt(string key, int value)
    {
        m_intValues[key] = value;
    }

    /// <summary>
    /// Saves all float, bool, and int settings to PlayerPrefs.
    /// </summary>
    public void SaveAllSettings()
    {
        foreach (var kvp in m_FloatValues)
        {
            PlayerPrefs.SetFloat(GetPrefixedKey(kvp.Key), kvp.Value);
        }
        foreach (var kvp in m_BoolValues)
        {
            PlayerPrefs.SetInt(GetPrefixedKey(kvp.Key), kvp.Value ? 1 : 0);
        }
        foreach (var kvp in m_intValues)
        {
            PlayerPrefs.SetInt(GetPrefixedKey(kvp.Key), kvp.Value);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads a setting from PlayerPrefs with the specified key and type.
    /// </summary>
    /// <param name="key">The key of the setting to load.</param>
    /// <param name="type">The type of the setting to load (int, float, bool, string).</param>
    /// <param name="result">The loaded setting as an object, or null if the type is unsupported.</param>
    /// <returns>
    /// True if the setting was successfully loaded and the type is supported; otherwise, false.
    /// </returns>
    public bool LoadSetting(string key, Type type, out object result)
    {
        key = GetPrefixedKey(key);
        if (type == typeof(int))
        {
            result = PlayerPrefs.GetInt(key);
            return true;
        }
        if (type == typeof(float))
        {
            result = PlayerPrefs.GetFloat(key);
            return true;
        }
        if (type == typeof(bool))
        {
            result = PlayerPrefs.GetInt(key) != 0; // Bool doesn't exist in PlayerPrefs, so we use int 0/1
            return true;
        }
        if (type == typeof(string))
        {
            result = PlayerPrefs.GetString(key);
            return true;
        }
        // Add more types as needed
        result = null;
        return false;
    }
}