using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem instance;

    [SerializeField] private GameObject _tooltipWindow;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Vector2 _offset = new Vector2(20f, -20f);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Hide();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
    }

    public void Show(string content)
    {
        _text.text = content;
          _tooltipWindow.SetActive(true);
        _text.gameObject.SetActive(true);

    }

    public void Hide()
    {
          _tooltipWindow.SetActive(false);
        _text.gameObject.SetActive(false);
    }
}

