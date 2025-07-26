using UnityEngine;

public class SpikeCubeEnemy : EnemyBase
{
    static readonly Vector2Int[] dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public override void TakeTurn()
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            var d = dirs[Random.Range(0, dirs.Length)];
            if (TryMoveTo(coord + d))
                return;
        }
        // no valid move, do nothing
    }
}
