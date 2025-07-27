// PlayerClassBase.cs
using UnityEngine;

public abstract class PlayerClassBase : MonoBehaviour
{
    public abstract void TryAttack(Vector2Int currentCoord, Vector2Int facingDir, GridManager grid);
}
