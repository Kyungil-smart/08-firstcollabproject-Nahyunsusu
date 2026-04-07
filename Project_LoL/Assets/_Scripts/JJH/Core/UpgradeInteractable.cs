using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeInteractable : MonoBehaviour
{
    [SerializeField] private GameObject _interactUI;

    private bool _playerInRange;
    private GameObject _upgradeUI;

    private void Start()
    {
        _upgradeUI = GameObject.Find("Upgrade");
        if (_upgradeUI != null) _upgradeUI.SetActive(false);
        if (_interactUI != null) _interactUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerInRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (_upgradeUI != null)
                _upgradeUI.SetActive(true);
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