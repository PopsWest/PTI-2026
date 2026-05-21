using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class ForestLayoutTool : MonoBehaviour
{
    [Header("=== OBJETOS ===")]
    [Tooltip("Arraste aqui o grupo/prefab de árvores (pode ser vários objetos diferentes)")]
    public GameObject[] treePrefabs;

    [Header("=== ÁREA DA FLORESTA ===")]
    [Tooltip("Tamanho total da área onde as árvores vão aparecer")]
    public float forestRadius = 50f;

    [Header("=== ESPAÇAMENTO ===")]
    [Tooltip("Distância mínima entre cada árvore")]
    public float minDistanceBetweenTrees = 3f;
    [Tooltip("Variação aleatória extra de distância (0 = grade uniforme, >0 = mais orgânico)")]
    public float randomOffset = 1.5f;

    [Header("=== CLAREIRA CENTRAL ===")]
    [Tooltip("Raio da área circular SEM árvores no centro")]
    public float clearingRadius = 10f;
    [Tooltip("Deslocar o centro da clareira da origem do objeto")]
    public Vector2 clearingOffset = Vector2.zero;

    [Header("=== CORREÇÃO DE ROTAÇÃO ===")]
    [Tooltip("Corrige a rotação base dos prefabs. Se nascerem deitados, tente X = 90 ou X = -90")]
    public Vector3 rotationCorrection = Vector3.zero;

    [Header("=== VARIAÇÃO VISUAL ===")]
    [Tooltip("Rotacionar árvores aleatoriamente no eixo Y")]
    public bool randomRotation = true;
    [Tooltip("Variar o tamanho das árvores")]
    public bool randomScale = true;
    [Min(0.1f)] public float scaleMin = 0.8f;
    [Min(0.1f)] public float scaleMax = 1.3f;

    [Header("=== TERRENO ===")]
    [Tooltip("Ativar para as árvores colarem no chão (Terrain ou Collider)")]
    public bool snapToGround = true;
    public float raycastHeight = 50f;
    public LayerMask groundLayer = ~0;

    [Header("=== SEED ===")]
    [Tooltip("Mude para gerar um layout diferente")]
    public int randomSeed = 42;

    // Referência interna para os filhos gerados
    private const string CONTAINER_NAME = "_ForestGenerated";
}


[CustomEditor(typeof(ForestLayoutTool))]
public class ForestLayoutToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ForestLayoutTool tool = (ForestLayoutTool)target;

        EditorGUILayout.Space(10);

        GUI.backgroundColor = new Color(0.4f, 0.85f, 0.4f);
        if (GUILayout.Button("🌲  GERAR FLORESTA", GUILayout.Height(40)))
        {
            GenerateForest(tool);
        }

        GUI.backgroundColor = new Color(0.85f, 0.4f, 0.4f);
        if (GUILayout.Button("🗑️  LIMPAR FLORESTA", GUILayout.Height(30)))
        {
            ClearForest(tool);
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "• Funciona fora do Play Mode\n" +
            "• Mude o Seed para layouts diferentes\n" +
            "• A clareira é a área circular SEM árvores\n" +
            "• Use Snap to Ground para terrenos irregulares\n" +
            "• Árvores deitadas? Use Rotation Correction X = 90 ou -90",
            MessageType.Info
        );
    }

    void GenerateForest(ForestLayoutTool tool)
    {
        if (tool.treePrefabs == null || tool.treePrefabs.Length == 0)
        {
            EditorUtility.DisplayDialog("Erro", "Adicione pelo menos um prefab em 'Tree Prefabs'!", "OK");
            return;
        }

        // Limpa geração anterior
        ClearForest(tool);

        // Cria container filho
        GameObject container = new GameObject(GetContainerName());
        container.transform.SetParent(tool.transform);
        container.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(container, "Gerar Floresta");

        Random.InitState(tool.randomSeed);

        List<Vector2> placedPositions = new List<Vector2>();
        Vector3 origin = tool.transform.position;
        Vector2 clearCenter = new Vector2(origin.x + tool.clearingOffset.x, origin.z + tool.clearingOffset.y);

        // Gera pontos em grid com offset aleatório (Poisson-like simples)
        float step = tool.minDistanceBetweenTrees;
        int attempts = 0;
        int maxAttempts = 50000;

        float xStart = origin.x - tool.forestRadius;
        float xEnd = origin.x + tool.forestRadius;
        float zStart = origin.z - tool.forestRadius;
        float zEnd = origin.z + tool.forestRadius;

        for (float x = xStart; x <= xEnd; x += step)
        {
            for (float z = zStart; z <= zEnd; z += step)
            {
                if (attempts++ > maxAttempts) break;

                // Offset orgânico
                float ox = Random.Range(-tool.randomOffset, tool.randomOffset);
                float oz = Random.Range(-tool.randomOffset, tool.randomOffset);
                float px = x + ox;
                float pz = z + oz;

                Vector2 pos2D = new Vector2(px, pz);
                Vector2 originXZ = new Vector2(origin.x, origin.z);

                // Verifica se está dentro da área da floresta
                if (Vector2.Distance(pos2D, originXZ) > tool.forestRadius)
                    continue;

                // Verifica clareira
                if (Vector2.Distance(pos2D, clearCenter) < tool.clearingRadius)
                    continue;

                // Verifica distância mínima dos já colocados
                bool tooClose = false;
                foreach (var placed in placedPositions)
                {
                    if (Vector2.Distance(pos2D, placed) < tool.minDistanceBetweenTrees)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;

                // Determina Y
                float py = origin.y;
                if (tool.snapToGround)
                {
                    Ray ray = new Ray(new Vector3(px, origin.y + tool.raycastHeight, pz), Vector3.down);
                    if (Physics.Raycast(ray, out RaycastHit hit, tool.raycastHeight * 2f, tool.groundLayer))
                        py = hit.point.y;
                }

                // Escolhe prefab aleatório
                GameObject prefab = tool.treePrefabs[Random.Range(0, tool.treePrefabs.Length)];
                if (prefab == null) continue;

                // Instancia
                Vector3 spawnPos = new Vector3(px, py, pz);
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
                if (instance == null)
                    instance = Instantiate(prefab, container.transform);

                instance.transform.position = spawnPos;

                // Rotação — aplica correção de base + rotação aleatória no Y
                float rotY = tool.randomRotation ? Random.Range(0f, 360f) : 0f;
                Quaternion correction = Quaternion.Euler(tool.rotationCorrection);
                Quaternion randomY = Quaternion.Euler(0f, rotY, 0f);
                instance.transform.rotation = randomY * correction;

                // Escala
                if (tool.randomScale)
                {
                    float s = Random.Range(tool.scaleMin, tool.scaleMax);
                    instance.transform.localScale = Vector3.one * s;
                }

                placedPositions.Add(pos2D);
            }
        }

        Debug.Log($"[ForestLayoutTool] ✅ {placedPositions.Count} árvores geradas! Container: '{GetContainerName()}'");
        EditorUtility.SetDirty(tool);
    }

    void ClearForest(ForestLayoutTool tool)
    {
        string containerName = GetContainerName();
        Transform existing = tool.transform.Find(containerName);
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
            Debug.Log("[ForestLayoutTool] 🗑️ Floresta anterior removida.");
        }
    }

    string GetContainerName() => "_ForestGenerated";

    // Desenha gizmos de preview na SceneView
    void OnSceneGUI()
    {
        ForestLayoutTool tool = (ForestLayoutTool)target;
        Vector3 origin = tool.transform.position;

        // Área da floresta
        Handles.color = new Color(0.2f, 0.8f, 0.2f, 0.4f);
        Handles.DrawWireDisc(origin, Vector3.up, tool.forestRadius);

        // Clareira
        Vector3 clearCenter = new Vector3(
            origin.x + tool.clearingOffset.x,
            origin.y,
            origin.z + tool.clearingOffset.y
        );
        Handles.color = new Color(1f, 0.8f, 0.1f, 0.6f);
        Handles.DrawWireDisc(clearCenter, Vector3.up, tool.clearingRadius);

        // Labels
        Handles.color = Color.white;
        Handles.Label(origin + Vector3.right * tool.forestRadius, " Floresta");
        Handles.Label(clearCenter + Vector3.right * tool.clearingRadius, " Clareira");
    }
}
#endif