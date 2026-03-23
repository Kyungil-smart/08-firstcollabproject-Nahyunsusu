using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class Weapon_List : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weaponList = new List<Weapon>(4);

    private int _curSelectedNum = 0;

    // InputAction
    private PlayerInput _playerInput;
    private InputAction _inputAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            // TODO -> InputAction 작성
            _inputAction = _playerInput.actions.FindAction("WeaponSwap");
        }
    }

    private void Start()
    {
        SelectWeapon(0);
    }

    private void OnEnable()
    {
        if (_inputAction != null)
            _inputAction.performed += OnSwapInput;
    }

    private void OnDisable()
    {
        if (_inputAction != null)
            _inputAction.performed -= OnSwapInput;
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= _weaponList.Count || _weaponList[index] == null) return;
        if (index == _curSelectedNum) return;

        _weaponList[_curSelectedNum].gameObject.SetActive(false);

        _curSelectedNum = index;
        _weaponList[_curSelectedNum].gameObject.SetActive(true);
    }

    private void OnSwapInput(InputAction.CallbackContext ctx)
    {
        int inputVal = (int)ctx.ReadValue<float>();
        SelectWeapon(inputVal - 1);
    }

    public Weapon GetCurrentWeapon()
    {
        return _weaponList[_curSelectedNum];
    }
}
