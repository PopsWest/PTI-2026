using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
   

    public string cenaMenu = "Menu";
    public string cenaJogo = "EscaladaPrototipo";

    

    // MENU

    public void ReViva()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
        {
            Application.OpenURL("https://www.instagram.com/revivavr/?utm_source=ig_web_button_share_sheet");
        }
    }

    public void QuitGame()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
        {
            SceneManager.LoadScene(cenaJogo);
        }
    }
}