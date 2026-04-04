using UnityEngine;

public class SortingOrderByY : MonoBehaviour
{
    [SerializeField] private bool _isStatic = false;
    [SerializeField] private int _offset = 0;

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
        int order = Mathf.RoundToInt(-transform.position.y * 10) + _offset;
        foreach (var sr in _renderers)
            sr.sortingOrder = order;
    }
}