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

    void Start()
    {
        float alturaTotal = posicaoFinal.position.y - posicaoComeco.position.y;

        int quantidade = Mathf.FloorToInt(alturaTotal / dificuldade);

        // eixo X fixo (linha reta pra cima)
        float eixoX = posicaoComeco.position.x;

        for (int i = 0; i <= quantidade; i++)
        {
            float y = posicaoComeco.position.y + (i * dificuldade);

            // pequena variacao horizontal
            float variacao = Random.Range(variacaoMin, variacaoMax);

            float x;

            // alterna esquerda/direita mas sempre perto do mesmo X
            if (i % 2 == 0)
                x = eixoX + variacao;
            else
                x = eixoX - variacao;

            Vector3 posFinal = new Vector3(
                x,
                y,
                posicaoComeco.position.z
            );

            Instantiate(pedra, posFinal, Quaternion.identity);
        }
    }
}