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

            if (e is EnemyBase eb)
                eb.startCoord = s.startCoord;

            enemies.Add(e);
        }
    }

    /// <summary>Called by enemies when they die.</summary>
    public void Unregister(IEnemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void TakeAllTurns()
    {
        // Loop over a snapshot copy of the list
        var snapshot = new List<IEnemy>(enemies);

        foreach (var e in snapshot)
        {
            if (e is MonoBehaviour mb && mb != null)
                e.TakeTurn();
        }
    }

}
