using UnityEngine;

namespace Mechanics.Merge
{
    public abstract class BaseMergeItemData : ScriptableObject, IMergeItem
    {
        [Header("Basic Settings")]
        [SerializeField] protected int level;
        [SerializeField] protected string branchId;

        [Header("Visuals")]
        [SerializeField] protected Sprite icon;

        [Header("Merge Settings")]
        [SerializeField] protected int mergeScore = 10;

        public int Level => level;
        public string BranchId => branchId;
        public Sprite Icon => icon;
        public int MergeScore => mergeScore;
        public abstract IMergeItem NextLevelItem { get; }

        public virtual bool CanMergeWith(IMergeItem other)
        {
            return other != null &&
                   other.Level == Level &&
                   other.BranchId == BranchId;
        }
    }
}