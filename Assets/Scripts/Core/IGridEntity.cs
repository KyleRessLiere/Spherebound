using UnityEngine;

/// <summary>
/// Anything that lives on the grid and has a coord.
/// </summary>
public interface IGridEntity
{
    /// <summary>Where on the grid this entity currently is.</summary>
    Vector2Int CurrentCoord { get; }

    /// <summary>Called by MovementModule or managers to update its coord.</summary>
    void SetCoord(Vector2Int newCoord);
}
