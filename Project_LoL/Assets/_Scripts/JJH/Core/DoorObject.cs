using UnityEngine;
 
public class DoorObject : MonoBehaviour
{
    private BoxCollider2D _collider;
    private GameObject[] _activeSprites;
 
    private void Awake()
    {
        _collider = GetComponentInChildren<BoxCollider2D>(true);
    }
 
    public void Setup(int width, bool isVertical)
    {
        if (_collider != null)
        {
            _collider.size = isVertical
                ? new Vector2(width, 1f)
                : new Vector2(1f, width);
            _collider.offset = Vector2.zero;
            _collider.gameObject.SetActive(false);
        }
 
        int spriteCount = Mathf.CeilToInt(width / 2f);
        float centerOffset = (width - 1) / 2f;
 
        _activeSprites = new GameObject[spriteCount];
 
        for (int i = 1; i <= 3; i++)
        {
            Transform spriteObj = transform.Find($"DoorSprite_{i}");
            if (spriteObj == null) continue;
 
            bool active = i <= spriteCount;
            spriteObj.gameObject.SetActive(false);
 
            if (!active) continue;
 
            float pos = (i - 1) * 2f - centerOffset;
            spriteObj.localPosition = isVertical
                ? new Vector3(pos, 0f, 0f)
                : new Vector3(0f, pos, 0f);
 
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