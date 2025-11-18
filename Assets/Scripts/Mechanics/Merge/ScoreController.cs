using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Mechanics.Merge
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private float _scoreInterval = 5f;
        [SerializeField] private Color _scoreTextColor = Color.yellow;
        private FloatingTextSpawner _textSpawner;

        private int _totalScore = 0;
        private List<MergeItem> _activeItems = new List<MergeItem>();
        private IDisposable _scoreTimer;

        public ReactiveProperty<int> CurrentScore { get; private set; } = new ReactiveProperty<int>(0);
        public List<MergeItem> ActiveItems => _activeItems;

        public void Initialize(FloatingTextSpawner floatingTextSpawner)
        {
            _textSpawner = floatingTextSpawner;
            StartScoreTimer();
        }

        private void StartScoreTimer()
        {
            _scoreTimer = Observable.Interval(TimeSpan.FromSeconds(_scoreInterval))
                .Subscribe(_ => CalculateScore())
                .AddTo(this);
        }

        private void CalculateScore()
        {
            int scoreThisTick = 0;

            foreach (var item in _activeItems)
            {
                if (item != null && item.Data != null)
                {
                    int itemScore = item.Data.MergeScore;
                    scoreThisTick += itemScore;
                    ShowScoreAboveItem(itemScore, item.transform.position);
                }
            }

            if (scoreThisTick > 0)
            {
                _totalScore += scoreThisTick;
                CurrentScore.Value = _totalScore;
            }
        }

        private void ShowScoreAboveItem(int score, Vector3 itemPosition)
        {
            if (_textSpawner != null)
            {
                string scoreText = $"+{score}";
                Vector3 textPosition = itemPosition + new Vector3(0, 1f, 0);
                _textSpawner.SpawnFloatingText(scoreText, textPosition, _scoreTextColor);
            }
        }

        public void AddBonusScore(int bonus, Vector3 position)
        {
            _totalScore += bonus;
            CurrentScore.Value = _totalScore;

            ShowScoreAboveItem(bonus, position);
        }

        public void AddItem(MergeItem item)
        {
            if (item != null && !_activeItems.Contains(item))           
                _activeItems.Add(item);    
        }

        public void RemoveItem(MergeItem item)
        {
            if (item != null)            
                _activeItems.Remove(item);            
        }

        private void OnDestroy()
        {
            _scoreTimer?.Dispose();
            CurrentScore?.Dispose();
        }
    }
}