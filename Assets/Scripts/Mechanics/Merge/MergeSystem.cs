using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Mechanics.Merge
{
    public class MergeSystem : MonoBehaviour
    {
        [SerializeField] private float _mergeRadius = 1.5f;

        private List<MergeItem> _subscribedItems = new List<MergeItem>();
        private SpawnerItems _spawnerItems;
        private ScoreController _scoreController;
        private CancellationTokenSource _mergeCancellationTokenSource;

        public void Initialize(SpawnerItems spawnerItems, ScoreController scoreController)
        {
            _spawnerItems = spawnerItems;
            _scoreController = scoreController;
            _mergeCancellationTokenSource = new CancellationTokenSource();

            Observable.EveryUpdate()
                .Subscribe(_ => SubscribeToNewItems())
                .AddTo(this);
        }



        private void SubscribeToNewItems()
        {
            var allItems = _scoreController.ActiveItems;
            foreach (var item in allItems)
            {
                if (!_subscribedItems.Contains(item))
                {
                    item.OnDragEnded
                        .Subscribe(OnItemDragEnded)
                        .AddTo(this);

                    _subscribedItems.Add(item);
                }
            }
        }

        private void OnItemDragEnded(MergeItem draggedItem)
        {
            CheckForMerge(draggedItem);
        }

        private void CheckForMerge(MergeItem draggedItem)
        {
            if (draggedItem.Data == null) return;

            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
                draggedItem.transform.position,
                _mergeRadius
            );

            foreach (var collider in nearbyColliders)
            {
                MergeItem otherItem = collider.GetComponent<MergeItem>();
                if (IsValidMergeTarget(draggedItem, otherItem))
                {
                    PerformMerge(draggedItem, otherItem);
                    return;
                }
            }
        }

        private bool IsValidMergeTarget(MergeItem item1, MergeItem item2)
        {
            return item2 != null &&
                   item1 != item2 &&
                   item1.Data != null &&
                   item2.Data != null &&
                   item1.Data.Level == item2.Data.Level &&
                   item1.Data.BranchId == item2.Data.BranchId &&
                   item1.Data.NextLevelItem != null &&
                   item1._canMerge && item2._canMerge;
        }

        private async UniTaskVoid PerformMerge(MergeItem item1, MergeItem item2)
        {
            Vector2 mergePosition = (item1.transform.position + item2.transform.position) * 0.5f;
            item1.PlayMergeAnimation(mergePosition);
            item2.PlayMergeAnimation(mergePosition);

            await UniTask.Delay(500, cancellationToken: _mergeCancellationTokenSource.Token);

            IMergeItem newItemData = item1.Data.NextLevelItem;
            CreateNewItem(newItemData, mergePosition);
        }


        private void CreateNewItem(IMergeItem itemData, Vector2 position)
        {
            var newItem = _spawnerItems.CreateMergeItem(itemData, position);

            if (!_subscribedItems.Contains(newItem))
            {
                newItem.OnDragEnded
                    .Subscribe(OnItemDragEnded)
                    .AddTo(this);
                _subscribedItems.Add(newItem);
            }
        }

        private void OnDestroy()
        {
            _mergeCancellationTokenSource?.Cancel();
            _mergeCancellationTokenSource?.Dispose();
        }
    }
}