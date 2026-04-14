using UnityEngine;

public class Degrais : MonoBehaviour
{
    public Transform posicaoComeco;
    public Transform posicaoFinal;

    public GameObject pedra;

    [Header("dificuldade = distancia vertical")]
    public float dificuldade = 2f;

    [Header("pequena variacao horizontal")]
    public float variacaoMin = 0.5f;
    public float variacaoMax = 1.2f;

    [Header("ajuste dificuldade")]
    public float dificuldadeFacil = 3f;
    public float dificuldadeDificil = 0.8f;

    [Header("alcance maximo do paciente (CM)")]
    public float alcanceMaximoCM = 180f;

    [Header("multiplicadores")]
    [Range(0f, 1f)] public float facilMult = 0.4f;
    [Range(0f, 1f)] public float medioMult = 0.7f;
    [Range(0f, 1f)] public float dificilMult = 1f;

    void Start()
    {

    }

    public void GerarDegraus()
    {
        // apaga os antigos
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (posicaoComeco == null || posicaoFinal == null)
        {
            Debug.LogError("Referencias não setadas");
            return;
        }

        float diff = GameSettings.Instance.difficulty;

        float alcance = GameSettings.Instance.alcanceMaximoCM;

        float dificuldadeCM;

        if (diff == 0f)
            dificuldadeCM = alcance * facilMult;
        else if (diff == 0.5f)
            dificuldadeCM = alcance * medioMult;
        else
            dificuldadeCM = alcance * dificilMult;

        // evita zero
        dificuldadeCM = Mathf.Max(10f, dificuldadeCM);

        dificuldade = dificuldadeCM / 100f;

        float alturaTotal = posicaoFinal.position.y - posicaoComeco.position.y;

        int quantidade = Mathf.Max(1, Mathf.FloorToInt(alturaTotal / dificuldade));

        float eixoX = posicaoComeco.position.x;

        for (int i = 0; i <= quantidade; i++)
        {
            float y = posicaoComeco.position.y + (i * dificuldade);

            float variacao = Random.Range(variacaoMin, variacaoMax);

            float x = (i % 2 == 0) ? eixoX + variacao : eixoX - variacao;

            Vector3 posFinal = new Vector3(
                x,
                y,
                posicaoComeco.position.z
            );

            Instantiate(pedra, posFinal, Quaternion.identity, transform);
        }
    }
}