using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSlot : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;

    [SerializeField] private TextMeshProUGUI       _priceText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void SetItem(EquipmentData_SO data)
    {
        if (data == null) return;

        _descriptionText.text = $"<b>{data.EquipName}</b>\n{data.EquipText}";

        _priceText.text = $"{data.EquipPrice}";


        Debug.Log("장비 세팅됨");

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => Debug.Log($"{data.EquipName} 구매 시도!"));
    }

    public void SetSkill(SkillDataSO skillDataSO, int diceValue = 1)
    {
        if (skillDataSO == null) return;

        SkillData data = skillDataSO.Get(diceValue);

        _descriptionText.text = $"<b>{data.SkillName}</b>\n{data.SkillDescription}";
        _priceText.text = $"{data.Price}";

        if (_image != null) _image.sprite = data.SkillImage;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => Debug.Log($"{data.SkillName} 스킬 구매!"));
    }
}
