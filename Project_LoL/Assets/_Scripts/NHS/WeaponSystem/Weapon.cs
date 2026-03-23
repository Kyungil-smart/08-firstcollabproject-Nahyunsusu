using UnityEngine;

public class Weapon : MonoBehaviour
{
    private DiceSystem _diceSystem;

    private int _maxAmmo;
    private int _curAmmo;

    public bool isEmpty => _curAmmo <= 0;

    private void Awake()
    {
        _diceSystem = new DiceSystem_Random();

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

    public void UseAmmo()
    {
        if (_curAmmo > 0) 
            _curAmmo--;
    }

    public void Reload()
    {
        // 다이스시스템에서 값 받아옴
        _maxAmmo = _diceSystem.RollDice();

        _curAmmo = _maxAmmo;
    }

    public void ApplyEnhancement(int num)
    {

    }
}
