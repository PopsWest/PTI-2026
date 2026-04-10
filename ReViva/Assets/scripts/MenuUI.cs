using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;
    public Degrais degrais;
    void Start()
    {
        difficultySlider.wholeNumbers = true;
        difficultySlider.minValue = 0;
        difficultySlider.maxValue = 2;

        // valor inicial (caso já tenha algo salvo)
        float saved = GameSettings.Instance.difficulty;

        if (saved < 0.33f)
            difficultySlider.value = 0;
        else if (saved < 0.66f)
            difficultySlider.value = 1;
        else
            difficultySlider.value = 2;

        AtualizarTexto();
    }

    public void OnSliderChanged()
    {
        AtualizarTexto();
        degrais.GerarDegraus();
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
}