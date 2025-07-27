using System.Collections.Generic;
using UnityEngine;

public class StrikerClass : IPlayer
{
    private readonly PlayerController player;

    public StrikerClass(PlayerController player)
    {
        this.player = player;
    }

    public List<AttackInstance> GetAttackPreview(Vector2Int origin, GridManager grid)
    {
        List<AttackInstance> preview = new();

        Vector2Int forward = Vector2Int.up;
        for (int i = 1; i <= 2; i++)
        {
            Vector2Int coord = origin + forward * i;
            if (grid.IsValidCoord(coord))
            {
                preview.Add(new AttackInstance
                {
                    targetCoord = coord,
                    damage = 5,
                    //statusEffect = null
                });
            }
        }

        return preview;
    }

    public void Attack(Vector2Int origin, GridManager grid)
    {
        var preview = GetAttackPreview(origin, grid);
        foreach (var atk in preview)
        {
            foreach (var enemy in EnemyManager.Instance.GetEnemiesOnTile(atk.targetCoord))
            {
                if (enemy is EnemyBase baseEnemy)
                {
                    baseEnemy.TakeDamage(atk.damage);
                }
            }
        }
    }
}
