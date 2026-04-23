using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI_Jogo : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // CONFIGURAÇÃO DE DIFICULDADE
    // ─────────────────────────────────────────────
    [Header("Configuração de Dificuldade")]
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;

    // ─────────────────────────────────────────────
    // COMPONENTES DO JOGO
    // ─────────────────────────────────────────────
    [Header("Componentes do Jogo")]
    public Degrais degrais;
    public GoniometriaClimb goniometria;

    // ─────────────────────────────────────────────
    // CALIBRAÇÃO
    // ─────────────────────────────────────────────
    [Header("Calibração")]
    public TextMeshProUGUI statusCalibracao;
    public Slider barraProgressoCalibracao;

    // ─────────────────────────────────────────────
    // TEMPO REAL
    // ─────────────────────────────────────────────
    [Header("Tempo Real")]
    public TextMeshProUGUI usoAtualDireito;
    public TextMeshProUGUI usoAtualEsquerdo;

    // ─────────────────────────────────────────────
    // RESULTADOS
    // ─────────────────────────────────────────────
    [Header("Resultados Clínicos")]
    public TextMeshProUGUI resultadoDireito;
    public TextMeshProUGUI resultadoEsquerdo;
    public TextMeshProUGUI diagnostico;

    // ─────────────────────────────────────────────
    // CENAS
    // ─────────────────────────────────────────────
    [Header("Cenas")]
    public string cenaMenu = "Menu";
    public string cenaJogo = "EscaladaPrototipo";

    // ─────────────────────────────────────────────
    // INIT
    // ─────────────────────────────────────────────
    void Start()
    {
        if (SceneManager.GetActiveScene().name != cenaJogo) return;

        if (goniometria == null)
            goniometria = FindObjectOfType<GoniometriaClimb>();

        // slider contínuo
        difficultySlider.minValue = 0.4f; // fácil
        difficultySlider.maxValue = 1f;   // difícil
        difficultySlider.wholeNumbers = false;

        difficultySlider.value = 0.7f; // padrão médio

        AtualizarTextoDificuldade();
    }

    void Update()
    {
        if (goniometria == null) return;

        AtualizarStatus();
        AtualizarTempoReal();
    }

    // ─────────────────────────────────────────────
    // CALIBRAÇÃO
    // ─────────────────────────────────────────────
    public void CalibrarPaciente()
    {
        goniometria.IniciarCalibracaoManual();
    }

    void AtualizarStatus()
    {
        if (goniometria.calibrando)
        {
            statusCalibracao.text = goniometria.faseAtual;
        }
        else if (goniometria.calibrado)
        {
            statusCalibracao.text = $"✔ Calibrado ({goniometria.alcanceMaximo * 100f:F0} cm)";
        }
        else
        {
            statusCalibracao.text = "⚠ Não calibrado";
        }
    }

    // ─────────────────────────────────────────────
    // TEMPO REAL
    // ─────────────────────────────────────────────
    void AtualizarTempoReal()
    {
        if (!goniometria.calibrado) return;

        usoAtualDireito.text = $"Dir: {goniometria.GetUsoAtualDir():F0}%";
        usoAtualEsquerdo.text = $"Esq: {goniometria.GetUsoAtualEsq():F0}%";
    }

    // ─────────────────────────────────────────────
    // DIFICULDADE DINÂMICA
    // ─────────────────────────────────────────────
    public void OnSliderChanged()
    {
        AtualizarTextoDificuldade();
        AtualizarDegraus();
    }

    void AtualizarTextoDificuldade()
    {
        float valor = difficultySlider.value;

        int porcentagem = Mathf.RoundToInt(valor * 100f);
        difficultyText.text = $"Dificuldade: {porcentagem}%";
    }

    void AtualizarDegraus()
    {
        if (!goniometria.calibrado) return;

        float alcance = goniometria.alcanceMaximo;

        float dificuldade = difficultySlider.value;

        // 🔥 aqui é o segredo
        float distanciaFinal = alcance * dificuldade;

        degrais.distanciaVertical = distanciaFinal;
        degrais.GerarDegraus();
    }

    // ─────────────────────────────────────────────
    // RESULTADOS
    // ─────────────────────────────────────────────
    public void FinalizarSessao()
    {
        var r = goniometria.GetResultados();

        resultadoDireito.text = $"Direito: {r.percDireito:F1}%";
        resultadoEsquerdo.text = $"Esquerdo: {r.percEsquerdo:F1}%";
        diagnostico.text = r.diagnostico;
    }

    // ─────────────────────────────────────────────
    // BOTÃO EXTRA (OPCIONAL)
    // ─────────────────────────────────────────────
    public void RegenerarDegraus()
    {
        AtualizarDegraus();
    }
}