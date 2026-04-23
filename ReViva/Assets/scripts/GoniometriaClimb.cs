using UnityEngine;
using System.Collections.Generic;

public class GoniometriaClimb : MonoBehaviour
{
    [Header("Referências VR")]
    public Transform head;
    public Transform handRight;
    public Transform handLeft;

    [Header("Detecção de pegada")]
    public float distanciaPegada = 0.15f;

    [Header("Configuração corporal")]
    public float ombroAlturaOffset = -0.2f;
    public float ombroLateralOffset = 0.15f;

    [Header("Calibração automática")]
    public float alcanceMaximo = 0.5f; // começa baixo e cresce
    public float suavizacao = 0.1f;

    private float melhorDir = 0f;
    private float melhorEsq = 0f;

    private float ultimoAlcanceDir = 0f;
    private float ultimoAlcanceEsq = 0f;

    private List<float> rightPeaks = new List<float>();
    private List<float> leftPeaks = new List<float>();

    private bool rightGrabbing = false;
    private bool leftGrabbing = false;

    private float currentRightPeak = 0f;
    private float currentLeftPeak = 0f;

    public bool calibrado = false;

    public struct ResultadoSessao
    {
        public float percDireito;
        public float percEsquerdo;
        public string diagnostico;
    }

    void Update()
    {
        AtualizarCalibracaoAutomatica();

        if (alcanceMaximo > 0.5f)
        {
            calibrado = true;
        }

        ProcessHand(handRight, true, ref rightGrabbing, ref currentRightPeak, rightPeaks);
        ProcessHand(handLeft, false, ref leftGrabbing, ref currentLeftPeak, leftPeaks);
    }

    // -------- AUTO CALIBRAÇÃO --------
    void AtualizarCalibracaoAutomatica()
    {
        Vector3 shoulderR = GetShoulder(true);
        Vector3 shoulderL = GetShoulder(false);

        float alcanceDir = Vector3.Distance(shoulderR, handRight.position);
        float alcanceEsq = Vector3.Distance(shoulderL, handLeft.position);

        // detectar movimento de subida
        bool subindoDir = alcanceDir > ultimoAlcanceDir;
        bool subindoEsq = alcanceEsq > ultimoAlcanceEsq;

        // só aceita se mão estiver acima do ombro (alcance real)
        if (handRight.position.y > shoulderR.y && subindoDir)
        {
            if (alcanceDir > melhorDir)
                melhorDir = Mathf.Lerp(melhorDir, alcanceDir, suavizacao);
        }

        if (handLeft.position.y > shoulderL.y && subindoEsq)
        {
            if (alcanceEsq > melhorEsq)
                melhorEsq = Mathf.Lerp(melhorEsq, alcanceEsq, suavizacao);
        }

        alcanceMaximo = Mathf.Max(melhorDir, melhorEsq);

        ultimoAlcanceDir = alcanceDir;
        ultimoAlcanceEsq = alcanceEsq;
    }

    // -------- OMBRO --------
    Vector3 GetShoulder(bool right)
    {
        Vector3 pos = head.position;

        pos.y += ombroAlturaOffset;

        if (right)
            pos += head.right * ombroLateralOffset;
        else
            pos -= head.right * ombroLateralOffset;

        return pos;
    }

    // -------- PROCESSAMENTO --------
    void ProcessHand(Transform hand, bool isRight, ref bool grabbing, ref float currentPeak, List<float> lista)
    {
        bool tocando = Physics.CheckSphere(hand.position, distanciaPegada);

        Vector3 shoulder = GetShoulder(isRight);
        float alcance = Vector3.Distance(shoulder, hand.position);

        if (tocando)
        {
            grabbing = true;

            if (alcance > currentPeak)
                currentPeak = alcance;
        }
        else
        {
            if (grabbing)
            {
                lista.Add(currentPeak);
                currentPeak = 0f;
                grabbing = false;
            }
        }
    }

    // -------- RESULTADOS --------
    public ResultadoSessao GetResultados()
    {
        ResultadoSessao r;

        float mediaDir = Media(rightPeaks);
        float mediaEsq = Media(leftPeaks);

        float percDir = Mathf.Clamp((mediaDir / alcanceMaximo) * 100f, 0f, 100f);
        float percEsq = Mathf.Clamp((mediaEsq / alcanceMaximo) * 100f, 0f, 100f);

        r.percDireito = percDir;
        r.percEsquerdo = percEsq;
        r.diagnostico = GerarDiagnostico(percDir, percEsq);

        return r;
    }

    float Media(List<float> lista)
    {
        if (lista.Count == 0) return 0;

        float soma = 0;
        foreach (float v in lista)
            soma += v;

        return soma / lista.Count;
    }

    string GerarDiagnostico(float dir, float esq)
    {
        float diff = Mathf.Abs(dir - esq);

        if (diff > 15f)
            return "⚠️ Assimetria funcional detectada";

        if (dir < 70f || esq < 70f)
            return "⚠️ Amplitude abaixo do ideal clínico";

        return "✔️ Movimento dentro do padrão esperado";
    }
}