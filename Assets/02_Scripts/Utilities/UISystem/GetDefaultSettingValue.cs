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
    private string settingType = "Setting Type Here";
    [SerializeField, Tooltip("The category of the setting.\n"
                             + "Options are Control or Sound")]
    private string settingCategory = "Setting Category Here";
    [SerializeField]
    private Slider selectable = null;
    private GetMinMax getMinMax = null;

    private void Start()
    {
        if (TryGetComponent(out Slider slider))
            selectable = slider;
        if (TryGetComponent<GetMinMax>(out getMinMax))
            getMinMax.SetValues();
        GetValue();
    }

    private void OnEnable()
    {
        if (selectable == null)
        {
            Debug.LogError("Selectable component not found on this GameObject.");
            return;
        }
        GetValue();
    }

    private void GetValue()
    {
        //Debug.Log($"Getting value for setting: {settingName} of type: {settingType}");
        switch (settingCategory.ToLower())
        {
            case "control":
                if (settingType.Equals("Float", System.StringComparison.OrdinalIgnoreCase) && Settings.Instance.ControlSettings.GetFloat(settingName).HasValue)
                    selectable.value = Settings.Instance.ControlSettings.GetFloat(settingName).Value;
                else if (settingType.Equals("Int", System.StringComparison.OrdinalIgnoreCase) && Settings.Instance.ControlSettings.GetInt(settingName).HasValue)
                    selectable.value = Settings.Instance.ControlSettings.GetInt(settingName).Value;
                else
                    Debug.LogWarning($"Unknown setting type: {settingType} for setting: {settingName}");
                break;
            case "sound":
                if (settingType.Equals("Float", System.StringComparison.OrdinalIgnoreCase) && Settings.Instance.SoundSettings.GetFloat(settingName).HasValue)
                    selectable.value = Settings.Instance.SoundSettings.GetFloat(settingName).Value;
                else if (settingType.Equals("Int", System.StringComparison.OrdinalIgnoreCase) && Settings.Instance.SoundSettings.GetInt(settingName).HasValue)
                    selectable.value = Settings.Instance.SoundSettings.GetInt(settingName).Value;
                else
                    Debug.LogWarning($"Unknown setting type: {settingType} for setting: {settingName}");
                break;
            default:
                Debug.LogWarning($"Unknown setting category: {settingCategory}. Defaulting to Control category.");
                settingName = $"Control_{settingName}";
                break;
        }
        

        //Debug.Log($"Value for setting: {settingName} of type: {settingType} is set to: {selectable.value}");
    }
}
