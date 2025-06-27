using System;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    public static SoundSettings SoundSettings { get; private set; } = new();
    public static ControlSettings ControlSettings { get; private set; } = new();

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

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        SoundSettings.SaveAllSettings();
        ControlSettings.SaveAllSettings();
    }

    /// <summary>
    /// Managers can call this method to load PlayerPrefs settings with a specific name and type.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <returns>The loaded setting as an object, or null if type is unsupported.</returns>
    public object LoadSettings(string key, Type type)
    {
        if (type == typeof(int))
            return PlayerPrefs.GetInt(key);
        if (type == typeof(float))
            return PlayerPrefs.GetFloat(key);
        if (type == typeof(bool))
            return PlayerPrefs.GetInt(key) != 0; // Bool doesnt exist in PlayerPrefs, so we use int 0/1
        if (type == typeof(string))
            return PlayerPrefs.GetString(key);
        // Add more types as needed
        return null;
    }
}

public class SoundSettings : BaseSettings
{
}

public class ControlSettings : BaseSettings
{
}

public class BaseSettings
{
    private static Dictionary<string, float> m_FloatValues = new();
    private static Dictionary<string, bool> m_BoolValues = new();
    private static Dictionary<string, int> m_intValues = new();

    public static float? GetFloat(string key)
    {
        if (m_FloatValues.TryGetValue(key, out var value))
        {
            return value;
        }
        return null;
    }

    public static void SetFloat(string key, float value)
    {
        m_FloatValues[key] = value;
    }

    public static bool? GetBool(string key)
    {
        if (m_BoolValues.TryGetValue(key, out var value))
        {
            return value;
        }
        return null;
    }

    public static void SetBool(string key, bool value)
    {
        m_BoolValues[key] = value;
    }

    public static int? GetInt(string key)
    {
        if (m_intValues.TryGetValue(key, out var value))
        {
            return value;
        }
        return null;
    }

    public static void SetInt(string key, int value)
    {
        m_intValues[key] = value;
    }

    public static void SaveAllSettings()
    {
        foreach (var kvp in m_FloatValues)
        {
            PlayerPrefs.SetFloat(kvp.Key, kvp.Value);
        }
        foreach (var kvp in m_BoolValues)
        {
            PlayerPrefs.SetInt(kvp.Key, kvp.Value ? 1 : 0);
        }
        foreach (var kvp in m_intValues)
        {
            PlayerPrefs.SetInt(kvp.Key, kvp.Value);
        }
        PlayerPrefs.Save();
    }
}