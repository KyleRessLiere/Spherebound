using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    List<AttackInstance> GetAttackPreview(Vector2Int origin, GridManager grid);
    void Attack(Vector2Int origin, GridManager grid);
}
