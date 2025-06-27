using Managers;
using System;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    /// <summary>
    /// Displays the high score in the UI using a TextMeshPro text component.
    /// Inherits from <see cref="ScoreDisplay"/> and fetches the high score from the <see cref="GameManager"/>.
    /// </summary>
    public class HighscoreIndicator : ScoreDisplay
    {

        /// <summary>
        /// Unity Start method. Initializes references and sets the high score text on the UI.
        /// </summary>
        private void Start()
        {
            if (m_Gm == null)
                m_Gm = GameManager.Instance;
            if (m_textComponent == null)
                m_textComponent = GetComponent<TMP_Text>();

            SetText(GetHighscore());
        }

        /// <summary>
        /// Sets the high score text in the <see cref="TMP_Text"/> component.
        /// </summary>
        /// <param name="score">The high score value to display.</param>
        private void SetText(float score)
        {
            string formattedScore = GetFormatedText(score * 100); // Assuming score is in a format that needs to be multiplied by 100
            m_textComponent.text = "Highscore: " + formattedScore;
        }

        /// <summary>
        /// Retrieves the current high score from the <see cref="GameManager"/>.
        /// </summary>
        /// <returns>The high score as a <see cref="float"/>.</returns>
        private float GetHighscore()
        {
            return m_Gm.GetHighscore();
        }
    }
}