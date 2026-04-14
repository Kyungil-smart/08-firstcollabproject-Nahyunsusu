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
        float centerOffset = width / 2f - 0.5f;

        if (_collider != null)
        {
            if (isVertical)
            {
                _collider.size = new Vector2(width, _colliderThickness);
                _collider.offset = new Vector2(centerOffset, 0f);
            }
            else
            {
                _collider.size = new Vector2(_colliderThickness, width);
                _collider.offset = new Vector2(0f, centerOffset);
            }
        }

        int spriteCount = Mathf.CeilToInt(width / 2f);
        _activeSprites = new GameObject[spriteCount];

        if (_spriteContainer != null)
        {
            float spriteMidOffset = (spriteCount - 1) * _spriteSpacing / 2f - 1.5f;
            float containerPos = centerOffset - spriteMidOffset;

            _spriteContainer.localPosition = isVertical
                ? new Vector3(containerPos, 0f, 0f)
                : new Vector3(0f, containerPos, 0f);
        }

        for (int i = 0; i < 3; i++)
        {
            if (_spriteTransforms[i] != null)
                _spriteTransforms[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < spriteCount; i++)
        {
            Transform spriteObj = _spriteTransforms[i];
            if (spriteObj == null) continue;

            float localX = (i * _spriteSpacing) - (_spriteSpacing / 2f) - 0.5f;

            spriteObj.localPosition = isVertical
                ? new Vector3(localX, 0f, 0f)
                : new Vector3(0f, localX, 0f);

            _activeSprites[i] = spriteObj.gameObject;
        }

        Open();
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