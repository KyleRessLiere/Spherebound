using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    protected GridManager grid;
    protected Vector2Int coord;

    [Tooltip("Initial grid coordinate")]
    public Vector2Int startCoord;

    // Expose current coord
    public Vector2Int CurrentCoord => coord;

    void Awake()
    {
        grid = UnityEngine.Object.FindFirstObjectByType<GridManager>();
    }

    void Start()
    {
        coord = startCoord;
        SnapToGrid();
        grid.GetTile(coord).isOccupied = true;
    }

    /// <summary>Subclasses implement their own movement logic here.</summary>
    public abstract void TakeTurn();

    /// <summary>Attempt to move onto a target tile; returns true if moved.</summary>
    protected bool TryMoveTo(Vector2Int target)
    {
        if (!grid.IsValidCoord(target)) return false;
        if (grid.GetTile(target).isOccupied) return false;

        grid.GetTile(coord).isOccupied = false;
        coord = target;
        grid.GetTile(coord).isOccupied = true;
        SnapToGrid();
        return true;
    }

    protected void SnapToGrid()
    {
        transform.position = grid.CoordToWorld(coord.x, coord.y);
    }

    // IGridEntity impl
    public void SetCoord(Vector2Int newCoord) => coord = newCoord;
}
