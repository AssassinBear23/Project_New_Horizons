using Managers;
using UnityEngine;
using UnityEngine.UI;

public class GetMinMax : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider component not found on this GameObject.");
            return;
        }
    }

    public void SetValues()
    {
        if(slider == null)
            Start();

        (float minValue, float maxValue) = GetValues();

        slider.minValue = minValue;
        slider.maxValue = maxValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private (float min, float max) GetValues()
    {
        //Debug.Log($"InputManager: {GameManager.Instance.InputManager.name}");
        float min = GameManager.Instance.InputManager.MinSensitivity;
        float max = GameManager.Instance.InputManager.MaxSensitivity;
        return (min, max);
    }
}
