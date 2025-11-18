using UnityEngine;

namespace Mechanics.Merge
{
    public interface IMergeItemComponent
    {
        IMergeItem Data { get; }
        Vector2Int GridPosition { get; }
        bool IsAnimating { get; }
        bool IsDragged { get; }
        void Initialize(IMergeItem data);
        void SetGridPosition(Vector2Int gridPosition);
        void PlayMergeAnimation();
        void PlaySpawnAnimation();
        void MoveToPosition(Vector3 position, float duration = 0.3f);
    }
}