using UnityEngine;

public class DoorObject : MonoBehaviour
{
    [SerializeField] private float _colliderThickness = 1f;
    [SerializeField] private float _spriteSpacing = 2f;

    private BoxCollider2D _collider;
    private GameObject[] _activeSprites;
    private Transform[] _spriteTransforms;
    private Transform _spriteContainer;

    private void Awake()
    {
        _collider = GetComponentInChildren<BoxCollider2D>(true);
        _spriteContainer = transform.Find("SpriteContainer");

        _spriteTransforms = new Transform[3];
        for (int i = 1; i <= 3; i++)
            _spriteTransforms[i - 1] = transform.Find($"SpriteContainer/DoorSprite_{i}");
    }

    public void Setup(int width, DoorDir dir)
    {
        if (_spriteContainer != null)
            _spriteContainer.rotation = Quaternion.identity;

        bool isVertical = dir == DoorDir.Up || dir == DoorDir.Down;
        float sign = (dir == DoorDir.Up || dir == DoorDir.Left) ? -1f : 1f;

        if (_collider != null)
        {
            if (isVertical)
            {
                _collider.size = new Vector2(width, _colliderThickness);
                _collider.offset = new Vector2(sign * (width - 1) / 2f, 0f);
            }
            else
            {
                _collider.size = new Vector2(_colliderThickness, width);
                _collider.offset = new Vector2(0f, sign * (width - 1) / 2f);
            }
            _collider.gameObject.SetActive(false);
        }

        int spriteCount = Mathf.CeilToInt(width / 2f);
        _activeSprites = new GameObject[spriteCount];

        for (int i = 1; i <= 3; i++)
        {
            Transform spriteObj = _spriteTransforms[i - 1];
            if (spriteObj == null) continue;

            bool active = i <= spriteCount;
            spriteObj.gameObject.SetActive(false);
            if (!active) continue;

            float localX = (i * _spriteSpacing) - (_spriteSpacing / 2f) - 0.5f;

            spriteObj.localPosition = isVertical
                ? new Vector3(sign * localX, 0f, 0f)
                : new Vector3(0f, sign * localX, 0f);

            _activeSprites[i - 1] = spriteObj.gameObject;
        }
    }

    public void Close()
    {
        if (_collider != null)
            _collider.gameObject.SetActive(true);

        if (_activeSprites == null) return;
        foreach (var s in _activeSprites)
            if (s != null) s.SetActive(true);
    }

    public void Open()
    {
        if (_collider != null)
            _collider.gameObject.SetActive(false);

        if (_activeSprites == null) return;
        foreach (var s in _activeSprites)
            if (s != null) s.SetActive(false);
    }
}