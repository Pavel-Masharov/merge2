using TMPro;
using UniRx;
using UnityEngine;

namespace Mechanics.Merge
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private string _scoreFormat = "Score: {0}";

        private ScoreController _scoreController;

        public void Initialize(ScoreController scoreController)
        {
            _scoreController = scoreController;
            _scoreController.CurrentScore
                    .Subscribe(UpdateScoreDisplay)
                    .AddTo(this);
        }

        private void UpdateScoreDisplay(int score) => _scoreText.text = string.Format(_scoreFormat, score);
    }
}