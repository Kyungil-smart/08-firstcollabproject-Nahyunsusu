using UnityEngine;

public class SceneTransitionInteractable : MonoBehaviour
{
    [SerializeField] private int _targetStage;
    [SerializeField] private GameObject _interactUI;

    private bool _playerInRange;

    private void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.F))
            SceneLoader.LoadStage(_targetStage);
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