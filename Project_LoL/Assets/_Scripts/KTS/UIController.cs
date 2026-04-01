using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void ActiveOptionUI()
    {
        SceneManager.LoadScene("OptionUI");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("게임종료");
    }
    
}



