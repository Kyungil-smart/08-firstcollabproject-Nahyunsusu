using UnityEngine;
using UnityEngine.InputSystem;

public class BossPortal : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;
    [SerializeField] private float _interactHoldTime = 2f;
    [SerializeField] private GameObject _interactUI;

    private bool _playerInRange;
    private float _holdTimer;

    private void Awake()
    {
        if (_visuals != null) _visuals.SetActive(false);
        BossDieState.OnBossDied += Activate;
    }

    private void OnDestroy()
    {
        BossDieState.OnBossDied -= Activate;
    }

    private void Activate()
    {
        if (_visuals != null) _visuals.SetActive(true);
    }

    private void Update()
    {
        if (!_playerInRange) return;

        if (Keyboard.current.fKey.isPressed)
        {
            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _interactHoldTime)
                GetComponent<SceneTransitionInteractable>()?.Transit();
        }
        else
        {
            _holdTimer = 0f;
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
        _holdTimer = 0f;
        if (_interactUI != null) _interactUI.SetActive(false);
    }
}