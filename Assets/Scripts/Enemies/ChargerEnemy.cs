using UnityEngine;

public class ChargerEnemy : EnemyBase
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
        modelYOffset = 1.1f;
        modelOffsetXZ = new Vector2(-0.2f, -0.7f);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void TakeTurn()
    {
        print("Spike Turn ");
        //for (int i = 0; i < dirs.Length; i++)
        //{
        //    var d = dirs[Random.Range(0, dirs.Length)];
        //    if (TryMoveTo(coord + d))
        //        return;
        //}
    }
}
