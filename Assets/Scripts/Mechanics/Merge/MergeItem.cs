using DG.Tweening;
using Services;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Merge
{
    public class MergeItem : MonoBehaviour , IPoolableObject
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _itemCollider;

        public IMergeItem Data { get; private set; }
        public bool IsAnimating { get; private set; }
        public bool IsDragged { get; private set; }

        private InputController _inputController;
        private IDisposable _dragSubscription;
        private IDisposable _tapSubscription;

        public bool _canMerge = true;

        private UnityAction _actionDecrementCount;
        private UnityAction<GameObject> _actionReturnToPool;
        private UnityAction<MergeItem> _actionRemoveItemScore;

        public Subject<MergeItem> OnDragEnded { get; private set; } = new Subject<MergeItem>();

        public void Initialize(IMergeItem data)
        {
            Data = data;
            UpdateVisuals();

            if (_itemCollider == null)
                _itemCollider = GetComponent<Collider2D>();

        }

        public void InitializeWithInput(IMergeItem data, InputController inputCtrl, UnityAction actionDecrementCount, UnityAction<GameObject> actionReturnToPool, UnityAction<MergeItem> actionRemoveItemScore)
        {
            Initialize(data);
            _inputController = inputCtrl;
            SetupInputSubscriptions();
            _actionDecrementCount = actionDecrementCount;
            _actionReturnToPool = actionReturnToPool;
            _actionRemoveItemScore = actionRemoveItemScore;
        }

        private void UpdateVisuals()
        {
            if (_spriteRenderer != null && Data != null)
            {
                _spriteRenderer.sprite = Data.Icon;
            }
        }

        private void SetupInputSubscriptions()
        {
            if (_inputController == null) return;

            _tapSubscription?.Dispose();
            _dragSubscription?.Dispose();

            _tapSubscription = _inputController.OnObjectTap
                .Where(obj => obj == gameObject)
                .Where(_ => _canMerge)
                .Subscribe(_ => OnItemTapped())
                .AddTo(this);

            var dragStartSubscription = _inputController.OnObjectDragStart
                .Where(obj => obj == gameObject)
                .Where(_ => _canMerge)
                .Subscribe(_ => OnDragStarted())
                .AddTo(this);

            _dragSubscription = _inputController.OnDrag
                .Where(_ => IsDragged && _canMerge)
                .Subscribe(position => OnDragged(position))
                .AddTo(this);

            var dragEndSubscription = _inputController.OnObjectDragEnd
                .Where(obj => obj == gameObject)
                .Where(_ => _canMerge)
                .Subscribe(_ => OnDragEndedMethod())
                .AddTo(this);
        }

        private void OnItemTapped()
        {
            transform.DOScale(1.1f, 0.1f)
                .OnComplete(() => transform.DOScale(1f, 0.1f));
        }

        private void OnDragStarted()
        {
            IsDragged = true;
            IsAnimating = true;
            transform.DOScale(1.2f, 0.2f);
            _spriteRenderer.sortingOrder = 10;

            if (_itemCollider != null)
                _itemCollider.enabled = false;
        }

        private void OnDragged(Vector2 position)
        {
            if (IsDragged)
            {
                transform.position = position;
            }
        }

        private void OnDragEndedMethod()
        {
            IsDragged = false;
            transform.DOScale(1f, 0.2f);
            _spriteRenderer.sortingOrder = 1;

            if (_itemCollider != null)
                _itemCollider.enabled = true;

            OnDragEnded.OnNext(this);
            IsAnimating = false;
        }

        public void PlayMergeAnimation(Vector2 posMerge)
        {
            _canMerge = false;

            IsAnimating = true;
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1.2f, 0.2f));
            sequence.Append(transform.DOScale(0f, 0.3f));
            sequence.Join(transform.DOMove(posMerge, 0.3f));
            sequence.OnComplete(() => {
                IsAnimating = false;
                _actionRemoveItemScore?.Invoke(this);
                _actionReturnToPool?.Invoke(gameObject);

            });
        }

        public void PlaySpawnAnimation()
        {
            IsAnimating = true;
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => IsAnimating = false);
        }

        private void OnDestroy()
        {
            _dragSubscription?.Dispose();
            _tapSubscription?.Dispose();
            OnDragEnded?.Dispose();
        }

        public void OnSpawn()
        {
            _canMerge = true;
            PlaySpawnAnimation();
        }

        public void OnDespawn() => _actionDecrementCount?.Invoke();
    }
}