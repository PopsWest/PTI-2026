using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;
    public Degrais degrais;

    // nomes das cenas (troca pelos seus nomes reais)
    public string cenaMenu = "Menu";
    public string cenaJogo = "EscaladaPrototipo";

    void Start()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;

        // Só roda isso na cena do jogo
        if (cenaAtual == cenaJogo)
        {
            difficultySlider.wholeNumbers = true;
            difficultySlider.minValue = 0;
            difficultySlider.maxValue = 2;

            float saved = GameSettings.Instance.difficulty;

            if (saved < 0.33f)
                difficultySlider.value = 0;
            else if (saved < 0.66f)
                difficultySlider.value = 1;
            else
                difficultySlider.value = 2;

            AtualizarTexto();
        }
    }

    public void OnSliderChanged()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;

        if (cenaAtual != cenaJogo) return;

        AtualizarTexto();
        degrais.GerarDegraus();
    }

    void AtualizarTexto()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;

        if (cenaAtual != cenaJogo) return;

        int diff = (int)difficultySlider.value;

        string nivel = "";

        switch (diff)
        {
            case 0:
                nivel = "Fácil";
                GameSettings.Instance.difficulty = 0f;
                break;

            case 1:
                nivel = "Médio";
                GameSettings.Instance.difficulty = 0.5f;
                break;

            case 2:
                nivel = "Difícil";
                GameSettings.Instance.difficulty = 1f;
                break;
        }

        difficultyText.text = nivel;
    }

    // Essas funções só funcionam na cena do menu
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
            SceneManager.LoadScene("EscaladaPrototipo");
        }
    }
}