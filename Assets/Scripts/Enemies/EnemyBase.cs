using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    [Header("Grid Setup")]
    public Vector2Int startCoord;

    [Header("Health")]
    public int maxHP = 3;

    [Header("Model Positioning")]
    public Transform modelTransform;
    public float modelYOffset = 0f;
    public Vector2 modelOffsetXZ = Vector2.zero; // ⬅️ New: offset X/Z across tiles

    protected GridManager grid;
    protected Vector2Int coord;
    protected int currentHP;
    protected EnemyHealthBar healthBar;

    public Vector2Int CurrentCoord => coord;

    public virtual void Awake()
    {
        grid = UnityEngine.Object.FindFirstObjectByType<GridManager>();
        currentHP = maxHP;

        if (modelTransform == null)
        {
            var rend = GetComponentInChildren<Renderer>();
            if (rend != null)
                modelTransform = rend.transform;
        }
    }

    public virtual void Start()
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
        barGO.transform.SetParent(transform, false);

        Vector3 worldPos = transform.position + Vector3.up * 2f;

        var rend = GetComponentInChildren<Renderer>();
        if (rend != null && Camera.main != null)
        {
            Vector3 top = rend.bounds.max;
            Vector3 camUp = Camera.main.transform.up;
            worldPos = top + camUp.normalized * 0.5f;
        }

        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        barGO.transform.localPosition = localPos;

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

        TakeDamage(1);
        return true;
    }

    protected void SnapToGrid()
    {
        Vector3 basePos = grid.CoordToWorld(coord.x, coord.y);
        basePos += new Vector3(modelOffsetXZ.x, modelYOffset, modelOffsetXZ.y);
        transform.position = basePos;
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
