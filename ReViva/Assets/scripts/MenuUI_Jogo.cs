using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI_Jogo : MonoBehaviour
{
    [Header("Configuração")]
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;
    public TMP_InputField alcanceInput;

    public Degrais degrais;

    [Header("Goniometria")]
    public GoniometriaClimb goniometria;

    [Header("Calibração")]
    public TextMeshProUGUI statusCalibracao;

    [Header("Resultados Clínicos")]
    public TextMeshProUGUI resultadoDireito;
    public TextMeshProUGUI resultadoEsquerdo;
    public TextMeshProUGUI diagnostico;

    public string cenaMenu = "Menu";
    public string cenaJogo = "EscaladaPrototipo";

    void Start()
    {
        if (SceneManager.GetActiveScene().name != cenaJogo) return;

        goniometria = GameObject.Find("GameManager").GetComponent<GoniometriaClimb>();

        // Slider
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

        // Alcance
        alcanceInput.text = GameSettings.Instance.alcanceMaximoCM.ToString("0");

        AtualizarStatusCalibracao();
    }

    // -------- DIFICULDADE --------

    public void OnSliderChanged()
    {
        if (SceneManager.GetActiveScene().name != cenaJogo) return;

        AtualizarTexto();
    }

    void AtualizarTexto()
    {
        int diff = (int)difficultySlider.value;

        switch (diff)
        {
            case 0:
                difficultyText.text = "Fácil";
                GameSettings.Instance.difficulty = 0f;
                break;

            case 1:
                difficultyText.text = "Médio";
                GameSettings.Instance.difficulty = 0.5f;
                break;

            case 2:
                difficultyText.text = "Difícil";
                GameSettings.Instance.difficulty = 1f;
                break;
        }
    }

    private void Update()
    {
        if (goniometria != null)
        {
            if (goniometria.alcanceMaximo > 0.5f)
                statusCalibracao.text = "✔ Calibrado";
            else
                statusCalibracao.text = "⚠ Ajustando...";
        }
    }

    // -------- ALCANCE --------

    public void AtualizarAlcance()
    {
        if (float.TryParse(alcanceInput.text, out float valor))
        {
            GameSettings.Instance.alcanceMaximoCM = valor;
        }
    }

    // -------- CALIBRAÇÃO --------

    public void CalibrarPaciente()
    {
   

       

        AtualizarStatusCalibracao();
    }

    void AtualizarStatusCalibracao()
    {
        if (goniometria == null) return;

        if (goniometria.calibrado)
            statusCalibracao.text = "✔ Calibrado";
        else
            statusCalibracao.text = "⚠ Não calibrado";
    }

    // -------- GERAR DEGRAUS --------

    public void RegenerarDegraus()
    {
        if (goniometria != null && !goniometria.calibrado)
        {
            statusCalibracao.text = "⚠ Calibre antes de iniciar";
            return;
        }

        AtualizarAlcance();

        if (degrais != null)
            degrais.GerarDegraus();
    }

    // -------- FINALIZAR SESSÃO --------

    public void FinalizarSessao()
    {
        if (goniometria == null)
        {
            Debug.LogWarning("Goniometria não encontrada");
            return;
        }

        var dados = goniometria.GetResultados();

        if (dados.percDireito == 0 && dados.percEsquerdo == 0)
        {
            diagnostico.text = "Nenhum movimento detectado";
            resultadoDireito.text = "";
            resultadoEsquerdo.text = "";
            return;
        }

        resultadoDireito.text = $"Braço Direito: {dados.percDireito:F1}%";
        resultadoEsquerdo.text = $"Braço Esquerdo: {dados.percEsquerdo:F1}%";
        diagnostico.text = dados.diagnostico;
    }

    // -------- MENU --------

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
        {
            SceneManager.LoadScene(cenaJogo);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}