using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnInfo
    {
        public GameObject prefab;
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
                Debug.LogWarning("EnemyManager: prefab is null");
                continue;
            }

            GameObject go = Instantiate(s.prefab);
            IEnemy e = go.GetComponent<IEnemy>();

            if (e == null)
            {
                Debug.LogError($"Prefab {s.prefab.name} has no IEnemy!");
                Destroy(go);
                continue;
            }

            if (e is EnemyBase eb)
                eb.startCoord = s.startCoord;

            enemies.Add(e);
        }
    }

    public void Unregister(IEnemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void TakeAllTurns()
    {
        var snapshot = new List<IEnemy>(enemies);
        foreach (var e in snapshot)
        {
            if (e is MonoBehaviour mb && mb != null)
                e.TakeTurn();
        }
    }

    public IEnumerable<IEnemy> GetEnemiesOnTile(Vector2Int coord)
    {
        foreach (var e in enemies)
            if (e.CurrentCoord == coord)
                yield return e;
    }
}
