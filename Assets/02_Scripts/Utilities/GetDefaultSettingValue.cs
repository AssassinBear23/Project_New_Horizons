using UnityEngine;
using UnityEngine.UI;

public class GetDefaultSettingValue : MonoBehaviour
{
    [SerializeField, Tooltip("The name of the setting to retrieve the default value for. You can find this in the code of the related Manager.")]
    private string settingName = "Setting Name Here";
    [SerializeField, Tooltip("The type of value you want to get.\n"
                             + "The options are the following:\n\t"
                             + "- Float\n\t"
                             + "- Int\n\n"
                             + "The type is not case sensitive")]
    private string settingType = "Setting Type Here"; // e.g., "Sound", "Graphics", etc.
    [SerializeField] 
    private Slider selectable = null;

    private void Start()
    {
        if (TryGetComponent(out Slider slider))
            selectable = slider;
    }

    public void OnEnable()
    {

        if (selectable == null)
            return;

        if(settingType.Equals("Float", System.StringComparison.OrdinalIgnoreCase))
            selectable.value = Settings.Instance.SoundSettings.GetFloat(settingName).GetValueOrDefault();
        else if (settingType.Equals("Int", System.StringComparison.OrdinalIgnoreCase))
            selectable.value = Settings.Instance.SoundSettings.GetInt(settingName).GetValueOrDefault();
        else
            Debug.LogWarning($"Unknown setting type: {settingType} for setting: {settingName}");
    }
}
