using UnityEngine;

public class SpikeCubeEnemy : EnemyBase
{
    static readonly Vector2Int[] dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Reset()
    {
        modelYOffset = 0.0f; // Default lift to make it sit correctly on tile
    }

    public override void TakeTurn()
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            var d = dirs[Random.Range(0, dirs.Length)];
            if (TryMoveTo(coord + d))
                return;
        }
        // No valid move, do nothing
    }
}
