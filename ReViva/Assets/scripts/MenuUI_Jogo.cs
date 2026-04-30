// MENUUI_Jogo.cs (CORRIGIDO)
// O erro acontece porque no Degrais atual NÃO EXISTE mais:
// degrais.distanciaVertical
//
// Agora quem controla isso é:
// GameSettings.Instance.alcanceMaximoCM
// + GameSettings.Instance.difficulty
//
// Então o MenuUI só precisa atualizar o slider (%)
// e mandar regenerar.

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

    void Start()
    {
        if (SceneManager.GetActiveScene().name != cenaJogo) return;

        if (goniometria == null)
            goniometria = FindObjectOfType<GoniometriaClimb>();

        // Slider contínuo de 40% a 100%
        difficultySlider.minValue = 0.4f;
        difficultySlider.maxValue = 1f;
        difficultySlider.wholeNumbers = false;

        // valor inicial
        difficultySlider.value = 0.7f;

        // salva no sistema global
        GameSettings.Instance.difficulty = difficultySlider.value;

        AtualizarTextoDificuldade();

        // Quando calibrar, gera automaticamente
        if (goniometria != null)
            goniometria.OnCalibracaoConcluida += RegenerarDegraus;
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

            if (barraProgressoCalibracao != null)
                barraProgressoCalibracao.value = goniometria.progressoCalibracao;
        }
        else if (goniometria.calibrado)
        {
            statusCalibracao.text = $"✔ Calibrado ({goniometria.alcanceMaximo * 100f:F0} cm)";

            if (barraProgressoCalibracao != null)
                barraProgressoCalibracao.value = 1f;
        }
        else
        {
            statusCalibracao.text = "⚠ Não calibrado";

            if (barraProgressoCalibracao != null)
                barraProgressoCalibracao.value = 0f;
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
        // salva dificuldade como valor real (0.4 até 1.0)
        GameSettings.Instance.difficulty = difficultySlider.value;

        AtualizarTextoDificuldade();

        // se já calibrado, regenera instantaneamente
        if (goniometria != null && goniometria.calibrado)
            RegenerarDegraus();
    }

    void AtualizarTextoDificuldade()
    {
        int porcentagem = Mathf.RoundToInt(difficultySlider.value * 100f);
        difficultyText.text = $"Dificuldade: {porcentagem}%";
    }

    // ─────────────────────────────────────────────
    // GERAR DEGRAUS
    // ─────────────────────────────────────────────
    public void RegenerarDegraus()
    {
        if (degrais == null) return;

        degrais.GerarDegraus();
    }

    // ─────────────────────────────────────────────
    // RESULTADOS
    // ─────────────────────────────────────────────
    public void FinalizarSessao()
    {
        if (goniometria == null) return;

        var r = goniometria.GetResultados();

        resultadoDireito.text =
            $"Direito: {r.percDireito:F1}% ({r.alcanceMaximoCM:F0}cm máx)";

        resultadoEsquerdo.text =
            $"Esquerdo: {r.percEsquerdo:F1}% ({r.alcanceMaximoCM:F0}cm máx)";

        diagnostico.text = r.diagnostico;
    }

    // ─────────────────────────────────────────────
    // MENU
    // ─────────────────────────────────────────────
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == cenaMenu)
            SceneManager.LoadScene(cenaJogo);
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