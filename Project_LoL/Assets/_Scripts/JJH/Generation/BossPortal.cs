using UnityEngine;
using UnityEngine.InputSystem;

public class BossPortal : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;
    [SerializeField] private float _interactHoldTime = 2f;
    [SerializeField] private InteractUI _interactUI;

    private bool _playerInRange;
    private float _holdTimer;
    private SceneTransitionInteractable _transition;
    private Collider2D _collider;

    private void Awake()
    {
        _transition = GetComponent<SceneTransitionInteractable>();
        _collider = GetComponentInChildren<Collider2D>();
        BossDieState.OnBossDied += Activate;
    }

    private void Start()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
            canvas.worldCamera = Camera.main;

        if (_visuals != null) _visuals.SetActive(false);
        if (_interactUI != null) _interactUI.SetVisible(false);
        if (_collider != null) _collider.enabled = false;
    }

    private void OnDestroy()
    {
        BossDieState.OnBossDied -= Activate;
    }

    private void Activate()
    {
        if (_visuals != null) _visuals.SetActive(true);
        if (_collider != null) _collider.enabled = true;
    }

    private void Update()
    {
        if (!_playerInRange) return;

        if (Keyboard.current.fKey.isPressed)
        {
            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _interactHoldTime)
                _transition?.Transit();
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
        if (_interactUI != null) _interactUI.SetVisible(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInRange = false;
        _holdTimer = 0f;
        if (_interactUI != null) _interactUI.SetVisible(false);
    }
}