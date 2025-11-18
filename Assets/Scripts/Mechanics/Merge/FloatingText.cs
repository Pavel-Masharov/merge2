using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Services;

namespace Mechanics.Merge
{
    public class FloatingText : MonoBehaviour, IPoolableObject
    {
        [SerializeField] private TMP_Text _textMesh;
        [SerializeField] private float _floatDuration = 1.5f;
        [SerializeField] private float _floatHeight = 2f;

        private UnityAction<GameObject> _actionReturnToPool;

        public void Initialize(string text, Color color, UnityAction<GameObject> actionReturnToPool)
        {
            _textMesh.text = text;
            _textMesh.color = color;
            _actionReturnToPool = actionReturnToPool;
            PlayAnimation();
        }

        public void OnDespawn()
        {

        }

        public void OnSpawn()
        {

        }

        private void PlayAnimation()
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + new Vector3(0, _floatHeight, 0);

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(targetPosition, _floatDuration));
            sequence.Join(_textMesh.DOFade(0f, _floatDuration));
            float randomOffset = Random.Range(-0.5f, 0.5f);
            sequence.Join(transform.DOMoveX(targetPosition.x + randomOffset, _floatDuration));
            sequence.Join(transform.DOScale(1.3f, 0.3f));
            sequence.Append(transform.DOScale(1f, _floatDuration - 0.3f));
            sequence.OnComplete(() => _actionReturnToPool?.Invoke(gameObject));
        }
    }
}