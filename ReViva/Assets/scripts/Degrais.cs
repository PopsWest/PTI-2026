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

    void Start()
    {
        GerarDegraus();
    }

    public void GerarDegraus()
    {
        // apaga os antigos
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // pega dificuldade do slider
        float diffSlider = GameSettings.Instance.difficulty;

        // converte dificuldade
        dificuldade = Mathf.Lerp(dificuldadeDificil, dificuldadeFacil, diffSlider);

        float alturaTotal = posicaoFinal.position.y - posicaoComeco.position.y;

        int quantidade = Mathf.FloorToInt(alturaTotal / dificuldade);

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