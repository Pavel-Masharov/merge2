using UnityEngine;

namespace Mechanics.Merge
{
    public interface IMergeItem
    {
        int Level { get; }
        string BranchId { get; }
        Sprite Icon { get; }
        int MergeScore { get; }
        IMergeItem NextLevelItem { get; }
    }
}