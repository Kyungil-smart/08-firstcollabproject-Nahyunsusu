using UnityEngine;

public enum Language { Korean = 0, English = 1 }

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    [Header("Settings")]
    public Language currentLanguage = Language.Korean;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 언어 변경 함수
    public void SetLanguage(int langIndex)
    {
        currentLanguage = (Language)langIndex;
    }
}