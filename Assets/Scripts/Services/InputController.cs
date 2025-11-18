using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Services
{
    public class InputController : MonoBehaviour
    {
        public IObservable<Vector2> OnDragStart => _onDragStart;
        public IObservable<Vector2> OnDrag => _onDrag;
        public IObservable<Vector2> OnDragEnd => _onDragEnd;
        public IObservable<Vector2> OnTap => _onTap;
        public IObservable<GameObject> OnObjectTap => _onObjectTap;
        public IObservable<GameObject> OnObjectDragStart => _onObjectDragStart;
        public IObservable<GameObject> OnObjectDragEnd => _onObjectDragEnd;

        private readonly Subject<Vector2> _onDragStart = new Subject<Vector2>();
        private readonly Subject<Vector2> _onDrag = new Subject<Vector2>();
        private readonly Subject<Vector2> _onDragEnd = new Subject<Vector2>();
        private readonly Subject<Vector2> _onTap = new Subject<Vector2>();

        private readonly Subject<GameObject> _onObjectTap = new Subject<GameObject>();
        private readonly Subject<GameObject> _onObjectDragStart = new Subject<GameObject>();
        private readonly Subject<GameObject> _onObjectDragEnd = new Subject<GameObject>();

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask interactableLayerMask = -1;
        [SerializeField] private float dragThreshold = 0.1f;
        [SerializeField] private float tapTimeThreshold = 0.3f;

        private Vector2 _startDragPosition;
        private Vector2 _currentDragPosition;
        private bool _isDragging = false;
        private float _dragStartTime;
        private GameObject _currentlyDraggedObject;

        public void Initialize() => SetupInputObservables();

        private void SetupInputObservables()
        {
            var mouseDown = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0) && !IsPointerOverUI());

            var mouseUp = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonUp(0) && !IsPointerOverUI());

            var mouseDrag = Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButton(0) && !IsPointerOverUI());

            mouseDown.Subscribe(_ =>
            {
                _startDragPosition = GetWorldPosition(Input.mousePosition);
                _currentDragPosition = _startDragPosition;
                _isDragging = false;
                _dragStartTime = Time.time;
                _currentlyDraggedObject = GetObjectAtPosition(_startDragPosition);

            }).AddTo(this);

            mouseDrag.Subscribe(_ =>
            {
                Vector2 previousPosition = _currentDragPosition;
                _currentDragPosition = GetWorldPosition(Input.mousePosition);

                if (!_isDragging)
                {
                    float dragDistance = Vector2.Distance(_startDragPosition, _currentDragPosition);
                    if (dragDistance >= dragThreshold && _currentlyDraggedObject != null)
                    {
                        _isDragging = true;
                        _onDragStart.OnNext(_startDragPosition);
                        _onObjectDragStart.OnNext(_currentlyDraggedObject);
                    }
                }

                if (_isDragging)               
                    _onDrag.OnNext(_currentDragPosition);   
                
            }).AddTo(this);

            mouseUp.Subscribe(_ =>
            {
                Vector2 endPosition = GetWorldPosition(Input.mousePosition);

                if (_isDragging)
                {
                    _onDragEnd.OnNext(endPosition);
                    _onObjectDragEnd.OnNext(_currentlyDraggedObject);
                    _isDragging = false;
                    _currentlyDraggedObject = null;
                }
                else
                {
                    float tapDuration = Time.time - _dragStartTime;
                    float tapDistance = Vector2.Distance(_startDragPosition, endPosition);

                    if (tapDuration <= tapTimeThreshold && tapDistance <= dragThreshold)
                    {
                        _onTap.OnNext(_startDragPosition);
                        GameObject tappedObject = GetObjectAtPosition(_startDragPosition);
                        if (tappedObject != null)
                        {
                            _onObjectTap.OnNext(tappedObject);
                        }
                    }
                }
            }).AddTo(this);
        }

        public GameObject GetObjectAtPosition(Vector2 worldPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, interactableLayerMask);
            return hit.collider?.gameObject;
        }

        private Vector2 GetWorldPosition(Vector2 screenPosition)
        {
            return _mainCamera.ScreenToWorldPoint(screenPosition);
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

            if (Input.touchCount > 0)
            {
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void OnDestroy()
        {
            _onDragStart?.Dispose();
            _onDrag?.Dispose();
            _onDragEnd?.Dispose();
            _onTap?.Dispose();
            _onObjectTap?.Dispose();
            _onObjectDragStart?.Dispose();
            _onObjectDragEnd?.Dispose();
        }
    }

}