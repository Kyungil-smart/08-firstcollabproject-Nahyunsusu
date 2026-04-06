using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private List<SkillDataSO> _haveSkillList = new List<SkillDataSO>();
    [SerializeField] private List<Image> _skillImageList = new List<Image>();

    [SerializeField] private PlayerSkillHandler _skillHandler;

    private void Start()
    {
        RefreshSkillUI();
    }

    public void RefreshSkillUI()
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
}
