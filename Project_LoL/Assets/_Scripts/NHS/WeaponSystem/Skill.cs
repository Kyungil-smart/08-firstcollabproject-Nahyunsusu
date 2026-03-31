using UnityEngine;

public class Skill : MonoBehaviour
{
    public Sprite SkillIcon => _skillIcon;
    [SerializeField] private Sprite _skillIcon;

    public int CurrentAmmo => _curAmmo;
    private int _curAmmo;
    private int _maxAmmo;

    public bool isEmpty => _curAmmo <= 0;

    private void Awake()
    {
        Reload();
    }

    public void UseAmmo()
    {
        if (_curAmmo > 0)
            _curAmmo--;
    }

    public void Reload()
    {
        if (DiceSystem.instance != null)
        {
            _maxAmmo = DiceSystem.instance.RollDice();
            _curAmmo = _maxAmmo;
        }
        else
        {
            Debug.LogError("씬에 DiceSystem이 배치되지 않았습니다");
        }
    }

    public void ApplyEnhancement(int num)
    {
    }

    public void Attack()
    {
        if(!isEmpty)
            UseAmmo();
    }
}
