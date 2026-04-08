using UnityEngine;
using UnityEngine.InputSystem;

public class BossRewardChest : MonoBehaviour
{
    [SerializeField] private GameObject _visuals;
    [SerializeField] private Animator _animator;
    [SerializeField] private EquipmentList _equipmentList;
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private InteractUI _interactUI;

    private bool _playerInRange;
    private bool _opened;

    private void Awake()
    {
        _dataManager = FindAnyObjectByType<DataManager>();
        _equipmentList = FindAnyObjectByType<EquipmentList>();
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