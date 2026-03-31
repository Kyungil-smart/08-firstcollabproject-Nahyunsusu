using UnityEngine;
 
public class DoorObject : MonoBehaviour
{
    [SerializeField] private float _colliderThickness = 1f;
    [SerializeField] private float _spriteSpacing = 2f;
 
    private BoxCollider2D _collider;
    private GameObject[] _activeSprites;
 
    private void Awake()
    {
        _collider = GetComponentInChildren<BoxCollider2D>(true);
    }
 
    public void Setup(int width)
    {
        if (_collider != null)
        {
            _collider.size = new Vector2(width, _colliderThickness);
            _collider.offset = new Vector2((width - 1) / 2f, 0f);
            _collider.gameObject.SetActive(false);
        }
 
        int spriteCount = Mathf.CeilToInt(width / 2f);
        _activeSprites = new GameObject[spriteCount];
 
        for (int i = 1; i <= 3; i++)
        {
            Transform spriteObj = transform.Find($"DoorSprite_{i}");
            if (spriteObj == null) continue;
 
            bool active = i <= spriteCount;
            spriteObj.gameObject.SetActive(false);
 
            if (!active) continue;
 
            float localX = (i * _spriteSpacing) - (_spriteSpacing / 2f) - 0.5f;
            spriteObj.localPosition = new Vector3(localX, 0f, 0f);
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