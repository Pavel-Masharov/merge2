using UnityEngine;

namespace Mechanics.Merge
{
    [CreateAssetMenu(fileName = "LinearMergeItem", menuName = "Merge Game/Linear Merge Item")]
    public class LinearMergeItemData : BaseMergeItemData
    {
        [SerializeField] private LinearMergeItemData nextLevelItemData;

        public override IMergeItem NextLevelItem => nextLevelItemData;

        public void SetNextLevelItem(LinearMergeItemData nextItem)
        {
            nextLevelItemData = nextItem;
        }
    }
}