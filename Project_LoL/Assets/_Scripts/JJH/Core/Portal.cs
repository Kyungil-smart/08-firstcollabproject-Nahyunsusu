using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform _destination;
    [SerializeField] private GameObject _interactUI;

    private bool _playerInRange;
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (_playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (_destination != null && _player != null)
                _player.transform.position = _destination.position;
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