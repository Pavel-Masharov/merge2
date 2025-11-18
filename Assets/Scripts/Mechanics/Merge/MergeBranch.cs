using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Merge
{
    [CreateAssetMenu(fileName = "MergeBranch", menuName = "Merge Game/Merge Branch")]
    public class MergeBranch : ScriptableObject
    {
        [SerializeField] private string branchId;
        [SerializeField] private string branchName;
        [SerializeField] private List<LinearMergeItemData> itemsByLevel;

        public string BranchId => branchId;
        public string BranchName => branchName;

        public IReadOnlyList<LinearMergeItemData> ItemsByLevel => itemsByLevel;

        public LinearMergeItemData GetItemByLevel(int level)
        {
            if (level >= 0 && level < itemsByLevel.Count)
                return itemsByLevel[level];
            return null;
        }

        public LinearMergeItemData GetFirstLevelItem()
        {
            return itemsByLevel.Count > 0 ? itemsByLevel[0] : null;
        }

        public bool IsMaxLevel(int level)
        {
            return level >= itemsByLevel.Count - 1;
        }
    }
}