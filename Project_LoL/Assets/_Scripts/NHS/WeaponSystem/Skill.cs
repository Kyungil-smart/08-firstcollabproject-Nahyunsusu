using UnityEngine;

public class Skill : MonoBehaviour
{
    private DiceSystem _diceSystem;

    public Sprite SkillIcon => _skillIcon;
    [SerializeField] private Sprite _skillIcon;


    private int _maxAmmo;
    public int CurrentAmmo => _curAmmo;
    private int _curAmmo;

    public bool isEmpty => _curAmmo <= 0;

    private void Awake()
    {
        _diceSystem = new DiceSystem_Random();

        if(_diceSystem != null) _maxAmmo = _diceSystem.RollDice();
        else Debug.Log("다이스 시스템이 비어있습니다."); 

        _curAmmo = _maxAmmo;
    }

    public void UseAmmo()
    {
        if (_curAmmo > 0) 
            _curAmmo--;
    }

    public void Reload()
    {
        _maxAmmo = _diceSystem.RollDice();

        _curAmmo = _maxAmmo;
    }

    public void ApplyEnhancement(int num)
    {
    }

    public void Attack()
    {
        UseAmmo();
    }
}
