using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    [Header("Grid Setup")]
    [Tooltip("Initial grid coordinate")]
    public Vector2Int startCoord;

    [Header("Health")]
    [Tooltip("Maximum hit points")]
    public int maxHP = 3;

    protected GridManager grid;
    protected Vector2Int coord;
    protected int currentHP;
    protected EnemyHealthBar healthBar;

    // Expose current coord
    public Vector2Int CurrentCoord => coord;

    void Awake()
    {
        // find the board
        grid = UnityEngine.Object.FindFirstObjectByType<GridManager>();

        // grab our health bar (must be a child in the prefab)
        healthBar = GetComponentInChildren<EnemyHealthBar>();

        // init HP
        currentHP = maxHP;
    }

    void Start()
    {
        // place on the grid
        coord = startCoord;
        SnapToGrid();
        grid.GetTile(coord).isOccupied = true;

        // initialize bar to full
        if (healthBar != null)
            healthBar.SetFraction(1f);
    }

    /// <summary>Subclasses implement their own movement logic here.</summary>
    public abstract void TakeTurn();

    /// <summary>Call to deal damage; updates bar and handles death.</summary>
    public void TakeDamage(int dmg)
    {
        currentHP = Mathf.Max(0, currentHP - dmg);

        if (healthBar != null)
            healthBar.SetFraction((float)currentHP / maxHP);

        if (currentHP == 0)
            Die();
    }

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

    /// <summary>Cleanup logic when HP hits zero.</summary>
    protected virtual void Die()
    {
        // free the tile
        grid.GetTile(coord).isOccupied = false;

        // TODO: play VFX / SFX here

        // destroy this enemy
        Destroy(gameObject);
    }
}
