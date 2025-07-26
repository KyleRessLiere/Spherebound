using UnityEngine;

public class RandomMoverEnemy : EnemyBase
{
    static readonly Vector2Int[] dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    void Reset()
    {
        modelYOffset = 1.0f; // Set default value for RandomMover if needed
    }

    public override void TakeTurn()
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            var d = dirs[Random.Range(0, dirs.Length)];
            if (TryMoveTo(coord + d))
                return;
        }
    }
}
