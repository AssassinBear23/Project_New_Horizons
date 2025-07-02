using Managers;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    public abstract class ScoreDisplay : MonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="GameManager"/> instance used to retrieve the high score.
        /// </summary>
        protected GameManager m_Gm;
        /// <summary>
        /// Reference to the <see cref="TMP_Text"/> component used to display the high score.
        /// </summary>
        protected TMP_Text m_textComponent;

        protected string GetFormatedText(float currentScore)
        {
            string formatedScore = GetFormatedScore(currentScore);
            //Debug.Log($"Current Score: {currentScore}, Formatted Score: {formatedScore}");
            string modifier = GetTextModifier(currentScore);
            //Debug.Log($"Current Score: {currentScore}, Modifier: {modifier}");

            return formatedScore + modifier;
        }

        protected readonly (float threshold, float divisor)[] ranges = new[]
            {
            (10000f, 1000f),                    // K
            (1000000f, 1000000f),               // Million
            (1000000000f, 1000000000f),         // Billion
            (1000000000000f, 1000000000000f)    // Trillion
        };

        protected string GetFormatedScore(float currentScore)
        {
            float selectedDivisor = 1;

            foreach (var (threshold, divisor) in ranges)
            {
                if (currentScore > threshold)
                    selectedDivisor = divisor;
                else
                    break;
            }

            return (currentScore / selectedDivisor).ToString("F0");
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
                < 10000 => string.Empty,
                < 1000000 => "K",
                < 1000000000 => "M",
                < 1000000000000 => "B",
                _ => "T"
            };
        }
    }
}