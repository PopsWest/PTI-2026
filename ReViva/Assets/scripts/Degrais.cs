using UnityEngine;

public class Degrais : MonoBehaviour
{
    public Transform posicaoComeco;
    public Transform posicaoFinal;

    public GameObject pedra;

    [Header("Distância Vertical (vem da calibração)")]
    public float distanciaVertical = 1f;

    public float variacaoHorizontal = 0.5f;

    public void GerarDegraus()
    {
        // limpa os antigos
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Vector3 pos = posicaoComeco.position;

        float alturaFinal = posicaoFinal.position.y;

        while (pos.y < alturaFinal)
        {
            // 🔥 AQUI É O SEGREDO
            pos.y += distanciaVertical;

            // pequena variação horizontal
            float x = Random.Range(-variacaoHorizontal, variacaoHorizontal);

            Vector3 spawnPos = new Vector3(
                posicaoComeco.position.x + x,
                pos.y,
                posicaoComeco.position.z
            );

            Instantiate(pedra, spawnPos, Quaternion.identity, transform);
        }
    }
}