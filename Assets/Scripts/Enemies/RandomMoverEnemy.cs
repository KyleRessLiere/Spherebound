using UnityEngine;

public class RandomMoverEnemy : EnemyBase
{
    static readonly Vector2Int[] dirs = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public override void Awake()
    {
        base.Awake();
        modelYOffset = 0.5f;
        modelOffsetXZ = Vector2.zero;
    }

    public override void Start()
    {
        base.Start();
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
