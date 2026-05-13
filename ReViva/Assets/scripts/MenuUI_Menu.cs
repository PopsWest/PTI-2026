using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI_Menu : MonoBehaviour
{
    [Header("Cenas")]
    public string cenaMenu = "Menu";
    public string cenaJogo = "EscaladaPrototipo";

    [Header("Loading UI")]
    public GameObject loadingUI;

    // ─────────────────────────────────────────────
    // INSTAGRAM
    // ─────────────────────────────────────────────
    public void ReViva()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
        {
            Application.OpenURL(
                "https://www.instagram.com/revivavr/?utm_source=ig_web_button_share_sheet"
            );
        }
    }

    // ─────────────────────────────────────────────
    // QUIT
    // ─────────────────────────────────────────────
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

    // ─────────────────────────────────────────────
    // START GAME
    // ─────────────────────────────────────────────
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
        {
            StartCoroutine(CarregarJogoAsync());
        }
    }

    // ─────────────────────────────────────────────
    // LOAD ASYNC
    // ─────────────────────────────────────────────
    IEnumerator CarregarJogoAsync()
    {
        // ativa loading na tela
        if (loadingUI != null)
            loadingUI.SetActive(true);

        // começa carregar
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(cenaJogo);

        // espera terminar
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}