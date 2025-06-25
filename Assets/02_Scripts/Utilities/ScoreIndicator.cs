using Managers;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ScoreIndicator : MonoBehaviour
{
    private GameManager m_Gm;
    private TMP_Text m_textComponent;

    void Start()
    {
        m_Gm = GameManager.Instance;
        m_textComponent = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        float currentScore = m_Gm.GetCurrentScore();
        string text = GetFormatedText(currentScore);
        m_textComponent.text = text;
    }

    private string GetFormatedText(float currentScore)
    {
        string formatedScore = GetFormatedScore(currentScore);
        string modifier = GetTextModifier(currentScore);

        return formatedScore + modifier;
    }

    private readonly (float threshold, float divisor)[] ranges = new[]
        {
            (10000f, 1000f),                    // K
            (1000000f, 1000000f),               // Million
            (1000000000f, 1000000000f),         // Billion
            (1000000000000f, 1000000000000f)    // Trillion
        };

    private string GetFormatedScore(float currentScore)
    {
        foreach (var (threshold, divisor) in ranges)
        {
            if (currentScore < threshold)
                return (currentScore / divisor).ToString("F0");
        }
        return currentScore.ToString("F0");
    }

    /// <summary>
    /// Determines the text modifier to add based on the passed score.
    /// </summary>
    /// <remarks>
    /// Remarks:<br/>
    /// Text modifiers are used to format large numbers into more readable forms, such as "K" for thousands, "Million" for millions, etc.
    /// </remarks>
    /// <param name="currentScore">The score to evaluate.</param>
    /// <returns>The determined modifier. Returns a empty string if none is needed.</returns>
    private string GetTextModifier(float currentScore)
    {
        return currentScore switch
        {
            < 10000 => "K",
            < 1000000 => "M",
            < 1000000000 => "B",
            < 1000000000000 => "T",
            _ => string.Empty,
        };
    }
}
