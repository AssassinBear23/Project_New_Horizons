using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class StaticMethods
{
    /// <summary>
    /// Quits the application. If running in the Unity editor, stops playing. If running in a build, quits the application.
    /// </summary>
    public static void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
