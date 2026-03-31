using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class Skill_List : MonoBehaviour
{
    [SerializeField] private List<Skill> _weaponList = new List<Skill>(4);

    [SerializeField] private List<Skill_UI> _slots;

    private bool _isFirstSet = true;

    // InputAction
    private PlayerInput _playerInput;

    private InputAction        _tabAction;
    private InputAction  _leftClickAction;
    private InputAction _rightClickAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput != null)
        {
            // TODO -> InputAction 작성
                   _tabAction = _playerInput.actions.FindAction("Tab");
             _leftClickAction = _playerInput.actions.FindAction("Attack");
            _rightClickAction = _playerInput.actions.FindAction("AttackRight");
        }
    }

    private void Start()
    {
        UpdateWeaponSet();
    }

    private void OnEnable()
    {
        if        (_tabAction != null)        _tabAction.performed +=   OnTabInput;
        if  (_leftClickAction != null)  _leftClickAction.performed +=  OnLeftClick; 
        if (_rightClickAction != null) _rightClickAction.performed += OnRightClick; 
    }

    private void OnDisable()
    {
        if        (_tabAction != null)        _tabAction.performed -=   OnTabInput;
        if  (_leftClickAction != null)  _leftClickAction.performed -=  OnLeftClick; 
        if (_rightClickAction != null) _rightClickAction.performed -= OnRightClick; 
    }

    private void OnTabInput(InputAction.CallbackContext ctx)
    {
        _isFirstSet = !_isFirstSet;
        Debug.Log($"Tab 입력됨");
        UpdateWeaponSet();
    }

    private void  OnLeftClick(InputAction.CallbackContext ctx) => OnFire(0);
    private void OnRightClick(InputAction.CallbackContext ctx) => OnFire(1);

    private void UpdateWeaponSet()
    {
        int startIndex = _isFirstSet ? 0 : 2;

        for (int i = 0; i < _weaponList.Count; i++)
        {
            if (_weaponList[i] == null) continue;

            bool isSelectedSet = (i == startIndex || i == startIndex + 1);

            _weaponList[i].gameObject.SetActive(isSelectedSet);

            if (i < _slots.Count && _slots[i] != null)
            {
                _slots[i].UpdateUI(_weaponList[i], isSelectedSet);
            }
        }
    }
        
    private void OnFire(int type)
    {
        int weaponIndex = (_isFirstSet ? 0 : 2) + type;

        if (weaponIndex < _weaponList.Count && _weaponList[weaponIndex] != null)
        {
            if (_slots[weaponIndex].isCoolingDown) return;

            _weaponList[weaponIndex].Attack();

            if (weaponIndex < _slots.Count)
            {
                int currentAmmo = _weaponList[weaponIndex].CurrentAmmo;

                Debug.Log($"{weaponIndex}번 무기 남은 탄수: {currentAmmo}");

                _slots[weaponIndex].UpdateAmmoOnly(currentAmmo);

                if (currentAmmo <= 0)
                {
                    _slots[weaponIndex].StartCoolDown(3.0f, _weaponList[weaponIndex]);
                }
            }
        }
    }
}
