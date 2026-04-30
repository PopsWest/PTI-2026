using UnityEngine;

public class Degrais : MonoBehaviour
{
    [Header("Referências de Cena")]
    public Transform posicaoComeco;
    public Transform posicaoFinal;
    public GameObject pedra;

    // ─────────────────────────────────────────────────────────────
    //  COMO O CÁLCULO FUNCIONA
    //
    //  alcanceMaximoCM vem de GameSettings (salvo pela calibração).
    //  Ex: paciente com 76cm de alcance.
    //
    //  Cada dificuldade define QUAL PERCENTUAL do alcance máximo
    //  é exigido para chegar na próxima pedra:
    //
    //    Fácil:  60% de 76cm = 45cm  → pedra a cada 0.45m
    //    Médio:  80% de 76cm = 61cm  → pedra a cada 0.61m
    //    Difícil: 95% de 76cm = 72cm → pedra a cada 0.72m
    //
    //  A posição HORIZONTAL é FIXA e alternada (esquerda/direita),
    //  nunca aleatória. Isso garante que toda pedra está sempre
    //  na mesma distância lateral previsível.
    // ─────────────────────────────────────────────────────────────

    [Header("Percentual do alcance exigido por dificuldade")]
    [Tooltip("Fácil: % do alcance máximo para chegar na próxima pedra")]
    [Range(0.3f, 0.7f)] public float percentualFacil = 0.60f;

    [Tooltip("Médio: % do alcance máximo")]
    [Range(0.5f, 0.9f)] public float percentualMedio = 0.80f;

    [Tooltip("Difícil: % do alcance máximo")]
    [Range(0.7f, 1.0f)] public float percentualDificil = 0.95f;

    [Header("Posição horizontal fixa das pedras (metros do centro)")]
    [Tooltip("Distância do centro para o lado direito/esquerdo. Fixa, sem aleatoriedade.")]
    public float offsetHorizontal = 0.30f;

    // ─────────────────────────────────────────────────────────────
    //  GERAR DEGRAUS
    // ─────────────────────────────────────────────────────────────

    public void GerarDegraus()
    {
        // Limpa pedras antigas
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (posicaoComeco == null || posicaoFinal == null)
        {
            Debug.LogError("[Degrais] posicaoComeco ou posicaoFinal não estão setados!");
            return;
        }

        // ── 1. Ler alcance calibrado do GameSettings ──
        float alcanceCM = GameSettings.Instance.alcanceMaximoCM;

        if (alcanceCM < 10f)
        {
            Debug.LogWarning("[Degrais] alcanceMaximoCM muito baixo ou zero. " +
                             "Calibração foi feita? Usando fallback de 60cm.");
            alcanceCM = 60f;
        }

        // ── 2. Calcular distância vertical com base na dificuldade ──
        float diff = GameSettings.Instance.difficulty;
        float percentual = diff < 0.33f ? percentualFacil
                          : diff < 0.66f ? percentualMedio
                          : percentualDificil;

        // Converte CM → metros para a cena Unity
        float distanciaVertical = (alcanceCM * percentual) / 100f;

        string nomeDiff = diff < 0.33f ? "Fácil" : diff < 0.66f ? "Médio" : "Difícil";
        Debug.Log($"[Degrais] Gerando degraus: " +
                  $"AlcanceMax={alcanceCM:F1}cm  " +
                  $"Dificuldade={nomeDiff} ({percentual * 100f:F0}%)  " +
                  $"→ DistânciaVertical={distanciaVertical * 100f:F1}cm por pedra");

        // ── 3. Spawnar pedras com posição FIXA alternada ──
        float alturaFinal = posicaoFinal.position.y;
        float centroX = posicaoComeco.position.x;
        float z = posicaoComeco.position.z;
        float y = posicaoComeco.position.y;
        int index = 0;

        while (y < alturaFinal)
        {
            y += distanciaVertical;
            if (y > alturaFinal) break;

            // Alternância fixa: par = direita, ímpar = esquerda
            float x = (index % 2 == 0)
                ? centroX + offsetHorizontal
                : centroX - offsetHorizontal;

            Instantiate(pedra,
                        new Vector3(x, y, z),
                        Quaternion.identity,
                        transform);
            index++;
        }

        Debug.Log($"[Degrais] {index} pedras geradas entre " +
                  $"y={posicaoComeco.position.y:F2} e y={alturaFinal:F2}");
    }
}