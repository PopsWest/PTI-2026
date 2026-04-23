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
    public float proporcaoBraco = 0.5f;

    [Header("DEBUG VISUAL")]
    public GameObject particlePrefab;

    GameObject headMarker;
    GameObject shoulderRMarker;
    GameObject shoulderLMarker;
    GameObject elbowRMarker;
    GameObject elbowLMarker;

    private List<float> rightPeaks = new List<float>();
    private List<float> leftPeaks = new List<float>();

    private bool rightGrabbing = false;
    private bool leftGrabbing = false;

    private float currentRightPeak = 0f;
    private float currentLeftPeak = 0f;

    float alcanceMaximo;

    public struct ResultadoSessao
    {
        public float percDireito;
        public float percEsquerdo;
        public string diagnostico;
    }

    void Start()
    {
        alcanceMaximo = GameSettings.Instance.alcanceMaximoCM / 100f;

        // cria marcadores
        headMarker = Instantiate(particlePrefab);
        shoulderRMarker = Instantiate(particlePrefab);
        shoulderLMarker = Instantiate(particlePrefab);
        elbowRMarker = Instantiate(particlePrefab);
        elbowLMarker = Instantiate(particlePrefab);
    }

    void Update()
    {
        Vector3 shoulderR = GetShoulder(true);
        Vector3 shoulderL = GetShoulder(false);

        Vector3 elbowR = GetElbow(shoulderR, handRight.position);
        Vector3 elbowL = GetElbow(shoulderL, handLeft.position);

        // -------- ATUALIZA PARTICULAS --------
        headMarker.transform.position = head.position;

        shoulderRMarker.transform.position = shoulderR;
        shoulderLMarker.transform.position = shoulderL;

        elbowRMarker.transform.position = elbowR;
        elbowLMarker.transform.position = elbowL;

        // -------- LÓGICA NORMAL --------
        ProcessHand(handRight, true, ref rightGrabbing, ref currentRightPeak, rightPeaks);
        ProcessHand(handLeft, false, ref leftGrabbing, ref currentLeftPeak, leftPeaks);
    }

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

    Vector3 GetElbow(Vector3 shoulder, Vector3 hand)
    {
        Vector3 mid = Vector3.Lerp(shoulder, hand, proporcaoBraco);

        Vector3 dir = (hand - shoulder).normalized;
        Vector3 perpendicular = Vector3.Cross(dir, Vector3.up);

        float curvatura = 0.1f;

        return mid + perpendicular * curvatura;
    }

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

    public ResultadoSessao GetResultados()
    {
        float mediaDir = Media(rightPeaks);
        float mediaEsq = Media(leftPeaks);

        float percDir = (mediaDir / alcanceMaximo) * 100f;
        float percEsq = (mediaEsq / alcanceMaximo) * 100f;

        ResultadoSessao r;

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