using UnityEngine;

public class SortingOrderByY : MonoBehaviour
{
    [SerializeField] private bool _isStatic = false;
    private const int _baseOffset = 500;

    private SpriteRenderer[] _renderers;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (_isStatic)
            UpdateSortingOrder();
    }

    private void LateUpdate()
    {
        if (!_isStatic)
            UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        int order = Mathf.RoundToInt(-transform.position.y) + _baseOffset;
        foreach (var sr in _renderers)
            sr.sortingOrder = order;
    }
}