using Managers;
using TMPro;
using UnityEngine;

namespace ScoreSystem
{
    public class ScoreIndicator : ScoreDisplay
    {
        [SerializeField] bool m_constantRefresh = true;

        void Start()
        {
            m_Gm = GameManager.Instance;
            m_textComponent = GetComponent<TMP_Text>();
            UpdateText();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_constantRefresh)
                UpdateText();
        }

        private void UpdateText()
        {
            float currentScore = m_Gm.GetCurrentScore();
            currentScore *= 100;
            string text = GetFormatedText(currentScore);
            m_textComponent.text = text;
        }
    }
}