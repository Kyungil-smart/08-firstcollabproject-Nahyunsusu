using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject _korean;
    [SerializeField] private GameObject _english;

    private void Awake()
    {
        Apply(LanguageManager.Instance != null ? LanguageManager.Instance.Current : Language.Korean);
        LanguageManager.OnLanguageChanged += Apply;
    }

    private void OnDestroy()
    {
        LanguageManager.OnLanguageChanged -= Apply;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    private void Apply(Language language)
    {
        if (_korean != null) _korean.SetActive(language == Language.Korean);
        if (_english != null) _english.SetActive(language == Language.English);
    }
}