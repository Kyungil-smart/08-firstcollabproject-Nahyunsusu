using System;
using UnityEngine;

public enum Language { Korean, English }

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    public static event Action<Language> OnLanguageChanged;

    public Language Current { get; private set; }

    private const string PrefKey = "Language";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Current = (Language)PlayerPrefs.GetInt(PrefKey, (int)Language.Korean);
    }

    public void SetLanguage(Language language)
    {
        if (Current == language) return;

        Current = language;
        PlayerPrefs.SetInt(PrefKey, (int)language);
        OnLanguageChanged?.Invoke(language);
    }
}