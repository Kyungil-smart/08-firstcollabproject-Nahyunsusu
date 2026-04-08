using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("Lobby");
        PlayerPersistentData.Instance?.Clear();

    }
    public void ActiveOptionUI()
    {
        SceneManager.LoadScene("OptionUI");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }

    public void Back()
    {
        SceneManager.LoadScene("TitleUI");
    }
    
    public void SetKorean()
    {
        LanguageManager.Instance.SetLanguage(Language.Korean);
    }

    public void SetEnglish()
    {
        LanguageManager.Instance.SetLanguage(Language.English);
    }
}



