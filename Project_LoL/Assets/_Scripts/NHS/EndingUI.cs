using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private List<SkillDataSO> _haveSkillList = new List<SkillDataSO>();
    [SerializeField] private List<Image> _skillImageList = new List<Image>();

    [SerializeField] private PlayerSkillHandler _skillHandler;

    [SerializeField] private Text _infoText;
    [SerializeField] private Text _statText;

    private void Start()
    {
        RefreshSkillUI();
        SetInfoText();
        SetStatText();
    }

    private void RefreshSkillUI()
    {
        if (_haveSkillList == null)
        {
            _haveSkillList = new List<SkillDataSO>();
        }

        else
        {
            _haveSkillList.Clear();
        }

        for (int i = 0; i < _skillHandler.Skills.Length; i++)
        {
            var executor = _skillHandler.Skills[i];

            if (executor != null && executor.SkillDataSO != null)
            {
                SkillDataSO currentSO = executor.SkillDataSO;
                _haveSkillList.Add(currentSO);

                if (i < _skillImageList.Count && _skillImageList[i] != null)
                {
                    _skillImageList[i].gameObject.SetActive(true);
                    _skillImageList[i].sprite = currentSO.Get(1).SkillImage;
                }
            }
            else
            {
                if (i < _skillImageList.Count && _skillImageList[i] != null)
                    _skillImageList[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetInfoText()
    {
        if (LanguageManager.Instance.currentLanguage == Language.Korean)
        {
            _infoText.text = $"최고 도달 층 수 : {1} 층\r\n최대 생존 시간 : {00: 00 : 38}\r\n처치한 적 : {1} 마리\r\n획득한 골드 : {0} Gold\r\n강화 성공 횟수 : {0} 회\r\n가장 강력한 일격 : {10}";
        }
        else
        {
            _infoText.text = $"Highest Floor: {1}\r\nMax Survival Time: {"00:00:38"}\r\nEnemies Defeated: {1}\r\nGold Earned: {0} Gold\r\nEnhancement Successes: {0}\r\nStrongest Hit: {10}";
        }
    }

    private void SetStatText()
    {
        if (LanguageManager.Instance.currentLanguage == Language.Korean)
        {
            _statText.text = $"공격력 : {10}\r\n공격 속도 : {1.0}\r\n이동 속도 : {8}\r\n치명타 확률 : {0} %\r\n치명타 피해 : {0} %";
        }
        else
        {
            _statText.text = $"Attack Power: {10}\r\nAttack Speed: {1.0}\r\nMove Speed: {8}\r\nCrit Rate: {0} %\r\nCrit Damage: {0} %";
        }
    }
}
