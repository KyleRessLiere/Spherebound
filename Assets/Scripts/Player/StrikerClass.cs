using UnityEngine;

public class StrikerClass : IPlayerClass
{
    private readonly PlayerController player;

    public StrikerClass(PlayerController player)
    {
        this.player = player;
    }

    public void Attack(Vector2Int origin, Vector2Int direction)
    {
        GridManager grid = Object.FindObjectOfType<GridManager>();
        EnemyManager enemyManager = Object.FindObjectOfType<EnemyManager>();

        for (int i = 1; i <= 2; i++)
        {
            Vector2Int target = origin + direction * i;
            if (!grid.IsValidCoord(target)) continue;

            Tile tile = grid.GetTile(target);
            Debug.Log($"[Striker] Attacking tile {target}");

            foreach (var enemy in enemyManager.GetEnemiesOnTile(target))
            {
                Debug.Log($"[Striker] Hit enemy at {enemy.CurrentCoord}");
                enemy.TakeDamage(1);
            }
        }
    }
}
