using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemSlot : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;

    [SerializeField] private TextMeshProUGUI       _priceText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    public void SetItem(Skill skill)
    {
        // TODO 정보집어넣기
    }
}
