using UnityEngine;
using Services;

namespace Mechanics.Merge
{
    public class FloatingTextSpawner : MonoBehaviour
    {
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private Transform floatingTextParent;

        private ObjectPool _objectPool;

        public void Initialize(ObjectPool objectPool)
        {
            _objectPool = objectPool;
        }

        public void SpawnFloatingText(string text, Vector3 worldPosition, Color color)
        {
            var floatingText = _objectPool.SpawnFromPool("floatingText").GetComponent<FloatingText>();
            floatingText.transform.position = worldPosition;
            floatingText.Initialize(text, color, ReturnItemToPool);
        }

        private void ReturnItemToPool(GameObject gameObject)
        {
            _objectPool.ReturnToPool("floatingText", gameObject);
        }
    }
}