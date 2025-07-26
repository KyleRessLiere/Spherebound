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

    public Vector2Int CurrentCoord => coord;

    void Awake()
    {
        grid = UnityEngine.Object.FindFirstObjectByType<GridManager>();
        currentHP = maxHP;
    }

    void Start()
    {
        coord = startCoord;
        SnapToGrid();
        grid.GetTile(coord).isOccupied = true;

        CreateHealthBar();
    }

    void LateUpdate()
    {
        if (healthBar != null)
            healthBar.FaceCamera();
    }

    protected void CreateHealthBar()
    {
        GameObject barGO = new GameObject("EnemyHealthBar");
        barGO.transform.SetParent(transform);
        barGO.transform.localPosition = new Vector3(0, 2f, 0);

        healthBar = barGO.AddComponent<EnemyHealthBar>();
        healthBar.Init(currentHP, maxHP);
    }

    protected void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.UpdateBar(currentHP, maxHP);
    }

    public void TakeDamage(int dmg)
    {
        currentHP = Mathf.Max(0, currentHP - dmg);
        UpdateHealthBar();

        if (currentHP == 0)
            Die();
    }

    protected bool TryMoveTo(Vector2Int target)
    {
        if (!grid.IsValidCoord(target)) return false;
        if (grid.GetTile(target).isOccupied) return false;

        grid.GetTile(coord).isOccupied = false;
        coord = target;
        grid.GetTile(coord).isOccupied = true;
        SnapToGrid();

        TakeDamage(1); // Deal 1 damage on move
        return true;
    }

    protected void SnapToGrid()
    {
        Vector3 basePos = grid.CoordToWorld(coord.x, coord.y);

        float yOffset = 0.5f;
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            yOffset = rend.bounds.size.y * 0.5f;

        transform.position = basePos + Vector3.up * yOffset;
    }

    public void SetCoord(Vector2Int newCoord)
    {
        coord = newCoord;
    }

    protected virtual void Die()
    {
        grid.GetTile(coord).isOccupied = false;

        var manager = FindObjectOfType<EnemyManager>();
        if (manager != null)
            manager.Unregister(this);

        Destroy(gameObject);
    }

    public abstract void TakeTurn();
}
