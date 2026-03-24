using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class Weapon_List : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weaponList = new List<Weapon>(4);

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
                   _tabAction = _playerInput.actions.FindAction("");
             _leftClickAction = _playerInput.actions.FindAction("");
            _rightClickAction = _playerInput.actions.FindAction("");
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
        UpdateWeaponSet();
    }

    private void OnLeftClick(InputAction.CallbackContext ctx) => OnFire(0);
    private void OnRightClick(InputAction.CallbackContext ctx) => OnFire(1);

    private void UpdateWeaponSet()
    {
        for (int i = 0; i < _weaponList.Count; i++)
        {
            if (_weaponList[i] == null) continue;
            _weaponList[i].gameObject.SetActive(false);
        }

        int startIndex = _isFirstSet ? 0 : 2;

        _weaponList[startIndex]?.    gameObject.SetActive(true);
        _weaponList[startIndex + 1]?.gameObject.SetActive(true);
    }

    private void OnFire(int type)
    {
        int weaponIndex = (_isFirstSet ? 0 : 2) + type;

        if (weaponIndex < _weaponList.Count && _weaponList[weaponIndex] != null)
        {
            _weaponList[weaponIndex].Attack();
        }
    }
}
