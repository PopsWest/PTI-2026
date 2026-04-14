using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI_Jogo : MonoBehaviour
{
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;

    public TMP_InputField alcanceInput;

    public Degrais degrais;


    void Start()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;



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

            // carrega alcance salvo
            alcanceInput.text = GameSettings.Instance.alcanceMaximoCM.ToString("0");

    }

    public void OnSliderChanged()
    {
       

        AtualizarTexto();
    }

    void AtualizarTexto()
    {


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

    // 🔥 atualiza alcance digitado
    public void AtualizarAlcance()
    {


        float valor;

        if (float.TryParse(alcanceInput.text, out valor))
        {
            GameSettings.Instance.alcanceMaximoCM = valor;
        }
    }

    // 🔥 botão pra regenerar
    public void RegenerarDegraus()
    {
        Debug.Log("BOTÃO FUNCIONOU");

        AtualizarAlcance();
        degrais.GerarDegraus();
    }
}
