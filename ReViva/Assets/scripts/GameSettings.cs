using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public float difficulty = 0.5f;

    private void Awake()
    {
        Instance = this;
    }
}