using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject _interactUI;
    [SerializeField] private GameObject _targetUI;

    private bool _playerInRange;

    private void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;

        if (_targetUI != null) _targetUI.SetActive(false);
        if (_interactUI != null) _interactUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (_targetUI != null)
                _targetUI.SetActive(true);
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