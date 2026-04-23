using UnityEngine;
using System.Collections.Generic;

public class GoniometriaClimb : MonoBehaviour
{
    [Header("Referências")]
    public Transform handRight;
    public Transform handLeft;

    [Header("Calibração")]
    public float tempoRelaxado = 2f;
    public float tempoMaximo = 3f;

    public bool calibrando = false;
    public bool calibrado = false;

    public float progressoCalibracao = 0f;
    public string faseAtual = "";

    public float alcanceMaximo = 0f;

    // estados
    float timer = 0f;
    int fase = 0;

    float maxRight = 0f;
    float maxLeft = 0f;

    // tempo real
    public float alcanceAtualDir { get; private set; }
    public float alcanceAtualEsq { get; private set; }

    public Transform head;

    void Update()
    {
        if (calibrando)
        {
            CalibracaoGuiada();
            return;
        }

        if (!calibrado) return;

        alcanceAtualDir = Vector3.Distance(head.position, handRight.position);
        alcanceAtualEsq = Vector3.Distance(head.position, handLeft.position);
    }

    // ─────────────────────────────
    // CALIBRAÇÃO GUIADA
    // ─────────────────────────────
    public void IniciarCalibracaoManual()
    {
        calibrando = true;
        calibrado = false;

        fase = 0;
        timer = 0f;

        maxRight = 0f;
        maxLeft = 0f;

        Debug.Log("▶ Iniciando calibração guiada");
    }

    void CalibracaoGuiada()
    {
        timer += Time.deltaTime;

        // FASE 0: RELAXADO
        if (fase == 0)
        {
            faseAtual = "Deixe os braços relaxados";

            progressoCalibracao = timer / tempoRelaxado;

            if (timer >= tempoRelaxado)
            {
                timer = 0f;
                fase = 1;
            }
        }

        // FASE 1: MÁXIMO
        else if (fase == 1)
        {
            faseAtual = "Levante os braços ao máximo";

            progressoCalibracao = timer / tempoMaximo;

            float r = Vector3.Distance(head.position, handRight.position);
            float l = Vector3.Distance(head.position, handLeft.position);

            if (r > maxRight) maxRight = r;
            if (l > maxLeft) maxLeft = l;

            if (timer >= tempoMaximo)
            {
                alcanceMaximo = Mathf.Max(maxRight, maxLeft);

                calibrando = false;
                calibrado = true;

                Debug.Log("✔ Calibrado: " + (alcanceMaximo * 100f) + " cm");
            }
        }
    }

    // ─────────────────────────────
    // TEMPO REAL
    // ─────────────────────────────
    public float GetUsoAtualDir()
    {
        if (!calibrado) return 0;
        return (alcanceAtualDir / alcanceMaximo) * 100f;
    }

    public float GetUsoAtualEsq()
    {
        if (!calibrado) return 0;
        return (alcanceAtualEsq / alcanceMaximo) * 100f;
    }

    // ─────────────────────────────────────────────
    // RESULTADOS
    // ─────────────────────────────────────────────
    public struct ResultadoSessao
    {
        public float percDireito;
        public float percEsquerdo;
        public string diagnostico;
    }

    public ResultadoSessao GetResultados()
    {
        ResultadoSessao r;

        float percDir = GetUsoAtualDir();
        float percEsq = GetUsoAtualEsq();

        r.percDireito = Mathf.Clamp(percDir, 0f, 100f);
        r.percEsquerdo = Mathf.Clamp(percEsq, 0f, 100f);

        r.diagnostico = GerarDiagnostico(r.percDireito, r.percEsquerdo);

        return r;
    }

    string GerarDiagnostico(float dir, float esq)
    {
        float diff = Mathf.Abs(dir - esq);

        if (diff > 15f)
            return "⚠ Assimetria funcional detectada";

        if (dir < 70f || esq < 70f)
            return "⚠ Amplitude abaixo do ideal";

        return "✔ Movimento dentro do esperado";
    }
}