using UnityEngine;
using UnityEngine.InputSystem;

public class BossRewardChest : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;
    [SerializeField] private Animator _animator;
    [SerializeField] private InteractUI _interactUI;

    private bool _playerInRange;
    private bool _opened;
    private Collider2D _collider;
    private EquipmentList _equipmentList;
    private DataManager _dataManager;

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider2D>();
        _dataManager = FindAnyObjectByType<DataManager>();
        _equipmentList = FindAnyObjectByType<EquipmentList>();
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
        if (_playerInRange && !_opened && Keyboard.current.fKey.wasPressedThisFrame)
            Open();
    }

    private void Open()
    {
        _opened = true;

        if (_interactUI != null) _interactUI.SetVisible(false);

        if (_animator != null)
            _animator.SetTrigger("Open");

        if (_dataManager == null || _equipmentList == null) return;

        var list = _dataManager.equipDataList;
        if (list == null || list.Count == 0) return;

        int randomId = list[Random.Range(0, list.Count)].EquipID;
        _equipmentList.AddEquip(0, randomId);
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
        if (_interactUI != null) _interactUI.SetVisible(false);
    }
}