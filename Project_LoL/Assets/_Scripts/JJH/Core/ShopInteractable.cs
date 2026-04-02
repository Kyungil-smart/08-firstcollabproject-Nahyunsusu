using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInteractable : MonoBehaviour
{
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private GameObject _interactUI;

    private bool _playerInRange;

    private void Awake()
    {
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