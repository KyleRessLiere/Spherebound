using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnInfo
    {
        [Tooltip("Drag your enemy prefab here (must implement IEnemy)")]
        public GameObject prefab;
        [Tooltip("Grid coordinate where this enemy will start")]
        public Vector2Int startCoord;
    }

    [Header("Enemy Spawns")]
    [Tooltip("Size = the number of enemies you want to spawn")]
    public SpawnInfo[] spawns;

    private readonly List<IEnemy> enemies = new List<IEnemy>();

    void Start()
    {
        foreach (var s in spawns)
        {
            if (s.prefab == null)
            {
                Debug.LogWarning("EnemyManager: prefab is null in spawns array");
                continue;
            }

            var go = Instantiate(s.prefab);
            var e = go.GetComponent<IEnemy>();
            if (e == null)
            {
                Debug.LogError($"Prefab {s.prefab.name} has no IEnemy component!");
                Destroy(go);
                continue;
            }

            // If it’s an EnemyBase, give it its starting coord
            if (e is EnemyBase eb)
                eb.startCoord = s.startCoord;

            enemies.Add(e);
        }
    }

    /// <summary>
    /// Call this when you want every enemy to take its turn.
    /// </summary>
    public void TakeAllTurns()
    {
        foreach (var e in enemies)
            e.TakeTurn();
    }
}
