using UnityEngine;
using Services;

namespace Mechanics.Merge
{
    public class GameplayManager : MonoBehaviour
    {
        /// <summary>
        /// This class is the only entry point to the game. Passes the necessary dependencies
        /// </summary>

        [SerializeField] private InputController _inputController;
        [SerializeField] private ObjectPool _obbjectPool;
        [SerializeField] private FloatingTextSpawner _floatingTextSpawner;
        [SerializeField] private ScoreController _scoreController;
        [SerializeField] private UIController _uiController;
        [SerializeField] private SpawnerItems _spawnerItems;
        [SerializeField] private MergeSystem _mergeSystem;

        void Start()
        {
            _inputController.Initialize();
            _obbjectPool.InitializePools();
            _floatingTextSpawner.Initialize(_obbjectPool);
            _scoreController.Initialize(_floatingTextSpawner);
            _uiController.Initialize(_scoreController);
            _spawnerItems.Initialize(_inputController, _obbjectPool, _scoreController);
            _mergeSystem.Initialize(_spawnerItems, _scoreController);
        }

    }
}
