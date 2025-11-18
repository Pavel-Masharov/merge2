using Services;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Mechanics.Merge
{
    public class SpawnerItems : MonoBehaviour
    {
        public MergeBranch Data;
        [SerializeField] private MergeItem _prefabItem;
        [SerializeField] private Transform _parentItems;

        private int _maxCountItems = 10;
        private int _countItems = 0;

        private Coroutine _coroutineCreateItems;
        private WaitForSeconds _intervalSpawnItems = new WaitForSeconds(3);
        private CancellationTokenSource _cancellationTokenSource;

        private InputController _inputController;
        private ObjectPool _objectPool;
        private ScoreController _scoreController;

        private float _minPosX = -2.5f;
        private float _maxPosX = 2.5f;
        private float _minPosY = -4.5f;
        private float _maxPosY = 4.5f;

        public void Initialize(InputController inputController, ObjectPool objectPool, ScoreController scoreController)
        {
            _inputController = inputController;
            _objectPool = objectPool;
            _scoreController = scoreController;
            _coroutineCreateItems = StartCoroutine(CreateItems());
        }

        public MergeItem CreateMergeItem(IMergeItem itemData, Vector2 position)
        {
            var item = _objectPool.SpawnFromPool("itemMerge").GetComponent<MergeItem>();
            item.transform.position = position;
            var firstData = itemData;
            item.InitializeWithInput(firstData, _inputController, DecrimentCoyntItems, ReturnItemToPool, RemoveItemAtScore);
            _countItems++;
            _scoreController?.AddItem(item);
            return item;
        }

        private void CreateItem()
        {
            var item = _objectPool.SpawnFromPool("itemMerge").GetComponent<MergeItem>();
            item.transform.position = GetPositionItem();
            var firstData = Data.GetFirstLevelItem();
            item.InitializeWithInput(firstData, _inputController, DecrimentCoyntItems, ReturnItemToPool, RemoveItemAtScore);
            _countItems++;
            _scoreController?.AddItem(item);
        }

        private IEnumerator CreateItems()
        {
            while (true)
            {
                if (_countItems < _maxCountItems)
                    CreateItem();

                yield return _intervalSpawnItems;
            }
        }

        private Vector2 GetPositionItem()
        {
            float posX = Random.Range(_minPosX, _maxPosX);
            float posY = Random.Range(_minPosY, _maxPosY);
            return new Vector2(posX, posY);
        }

        private void DecrimentCoyntItems() => _countItems--;
        private void ReturnItemToPool(GameObject gameObject) => _objectPool.ReturnToPool("itemMerge", gameObject);
        private void RemoveItemAtScore(MergeItem item) => _scoreController?.RemoveItem(item);


        private void OnDestroy()
        {
            StopCoroutine(_coroutineCreateItems);
        }
    }
}