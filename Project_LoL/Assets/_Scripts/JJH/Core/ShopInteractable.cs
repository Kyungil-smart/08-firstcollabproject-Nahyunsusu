using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInteractable : MonoBehaviour
{
    [SerializeField] private GameObject _interactUI;

    private bool _playerInRange;
    private GameObject _shopUI;

    private void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;
        
        _shopUI = GameObject.Find("Store");
        if (_shopUI != null) _shopUI.SetActive(false);
        if (_interactUI != null) _interactUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (_shopUI != null)
                _shopUI.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = true;
        if (_interactUI != null) _interactUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
        if (_interactUI != null) _interactUI.SetActive(false);
    }
}