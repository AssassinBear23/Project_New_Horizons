using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
public class SwipePowerSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public UnityEvent CooldownComplete;
    public void StartSlider(float cooldown)
    {
        StartCoroutine(SliderTime(cooldown));
    }
    private IEnumerator SliderTime(float cooldown)
    {
        float timePassed = 0;
        slider.value = 0;
        while (timePassed < cooldown)
        {
            yield return null;
            timePassed += Time.deltaTime;
            slider.value = Mathf.Clamp01(timePassed / cooldown);
        }
        CooldownComplete?.Invoke();
    }
}