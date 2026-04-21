using Scripts.Management;
using State.Models;
using TMPro;
using UnityEngine;

namespace Scripts.UIScripts
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI[] highScoreText;
        private int _score = 0;
        private int _highScore = 0;

        private void OnEnable()
        {
            EnemyController.EnemyGotHit += UpdateScoreDisplay;
            ScoreManager.OnHighScore += UpdateHighScore;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += resetScore;
        }

        private void OnDisable()
        {
            EnemyController.EnemyGotHit -= UpdateScoreDisplay;
            ScoreManager.OnHighScore -= UpdateHighScore;
            if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged += resetScore;
        }
    
        private void UpdateScoreDisplay(int points)
        {
            _score += points;
            scoreText.text = _score.ToString("D2"); // Formats as 00
        }
    
        public void UpdateHighScore(int points)
        {
            _highScore = points;
            foreach (TextMeshProUGUI highScore in highScoreText)
            {
                highScore.text = _highScore.ToString("D4"); // Formats as 0000
            }
        }
        
        private void resetScore(GameState state)
        {
            _score = (state == GameState.Transition) ? 0 : _score;
            scoreText.text = _score.ToString("D2"); // Formats as 00
        }
    }
}
