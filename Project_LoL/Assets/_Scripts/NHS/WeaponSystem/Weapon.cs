using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private DiceSystem _diceSystem;

    private int _maxAmmo;
    private int _curAmmo;

    public bool isEmpty => _curAmmo <= 0;

    private void Awake()
    {
        if(_diceSystem != null)
        {
            _maxAmmo = _diceSystem.RollDice();
        }
        else
        {
            Debug.Log("다이스 시스템이 비어있습니다."); 
        }

        _curAmmo = _maxAmmo;
    }

    private void UseAmmo()
    {
        if (_curAmmo > 0) 
            _curAmmo--;
    }

    private void Reload()
    {
        _curAmmo = _maxAmmo;
    }
}
