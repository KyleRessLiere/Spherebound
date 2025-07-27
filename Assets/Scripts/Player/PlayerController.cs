using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour, IGridEntity
{
    public Vector2Int CurrentCoord { get; private set; }

    GridManager grid;
    MovementModule mover;
    PlayerStats stats;

    public IPlayer playerClass;

    [Header("Context Menu UI")]
    private GameObject contextMenu;
    private Button attackButton;
    private Button moveButton;

    private const float ContextMenuOffsetRight = 1.2f;
    private const float ContextMenuOffsetUp = 0.4f;

    private bool isPreviewingAttack = false;
    private TileHighlighter highlighter;
    private List<AttackInstance> currentPreview = new();

    private bool isPreviewingMove = false;
    private List<Vector2Int> currentMoveOptions = new();

    void Start()
    {
        grid = FindAnyObjectByType<GridManager>();
        stats = GetComponent<PlayerStats>();
        stats.ResetActions();

        playerClass = new StrikerClass(this);

        CurrentCoord = new Vector2Int(grid.width / 2, 1);
        float tileHalfH = grid.tilePrefab.transform.localScale.y * 0.5f;
        float sphereRadius = transform.localScale.y * 0.5f;
        float yOffset = tileHalfH + sphereRadius;
        Vector3 worldPos = grid.CoordToWorld(CurrentCoord.x, CurrentCoord.y);
        transform.position = worldPos + Vector3.up * yOffset;

        grid.GetTile(CurrentCoord).isOccupied = true;
        mover = new MovementModule(this, this, grid, yOffset);

        contextMenu = transform.Find("PlayerContextMenu")?.gameObject;

        if (contextMenu != null)
        {
            contextMenu.SetActive(false);
            contextMenu.transform.localScale = Vector3.one * 0.01f;

            var buttons = contextMenu.GetComponentsInChildren<Button>(true);
            attackButton = buttons.FirstOrDefault(b => b.name == "AttackButton");
            moveButton = buttons.FirstOrDefault(b => b.name == "MoveButton");

            if (attackButton != null)
                attackButton.onClick.AddListener(OnAttackButtonClicked);
            else
                Debug.LogWarning("⚠️ Attack Button not found in PlayerContextMenu");

            if (moveButton != null)
                moveButton.onClick.AddListener(OnMoveButtonClicked);
            else
                Debug.LogWarning("⚠️ Move Button not found in PlayerContextMenu");
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerContextMenu not found under Player");
        }

        highlighter = FindAnyObjectByType<TileHighlighter>();
        if (highlighter == null)
            Debug.LogWarning("⚠️ TileHighlighter not found in scene.");
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelAttackPreview();
            CancelMovePreview();
            contextMenu?.SetActive(false);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (isPreviewingAttack)
                {
                    Vector2Int? clickedCoord = GetClickedTileCoord(hit.point);
                    if (clickedCoord.HasValue && currentPreview.Exists(a => a.targetCoord == clickedCoord.Value))
                    {
                        ExecuteAttack();
                    }
                    CancelAttackPreview();
                    contextMenu?.SetActive(false);
                    return;
                }

                if (isPreviewingMove)
                {
                    Vector2Int? moveCoord = GetClickedMoveCoord(hit.point);
                    if (moveCoord.HasValue && currentMoveOptions.Contains(moveCoord.Value))
                    {
                        TryMoveTo(moveCoord.Value);
                    }
                    CancelMovePreview();
                    contextMenu?.SetActive(false);
                    return;
                }

                if (hit.collider != null && hit.collider.gameObject == gameObject && contextMenu != null)
                {
                    contextMenu.SetActive(!contextMenu.activeSelf);
                    return;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (contextMenu != null && contextMenu.activeSelf)
        {
            Vector3 offset = transform.right * ContextMenuOffsetRight + Vector3.up * ContextMenuOffsetUp;
            contextMenu.transform.position = transform.position + offset;

            if (Camera.main != null)
            {
                contextMenu.transform.rotation = Quaternion.LookRotation(
                    contextMenu.transform.position - Camera.main.transform.position
                );
            }
        }
    }

    void TryMoveTo(Vector2Int target)
    {
        if (!stats.TryUseAction())
            return;

        grid.GetTile(CurrentCoord).isOccupied = false;
        mover.TryMove(target);
        CurrentCoord = target;
        grid.GetTile(CurrentCoord).isOccupied = true;
        stats.ChangeHealth(-1); // TEMP

        CancelMovePreview();
    }

    void AttemptMove(Vector2Int dir)
    {
        var target = CurrentCoord + dir;
        if (!grid.IsValidCoord(target) || grid.GetTile(target).isOccupied)
            return;

        if (!stats.TryUseAction())
            return;

        grid.GetTile(CurrentCoord).isOccupied = false;
        mover.TryMove(target);
        CurrentCoord = target;
        grid.GetTile(CurrentCoord).isOccupied = true;

        stats.ChangeHealth(-1); // TEMP

        if (isPreviewingAttack)
            CancelAttackPreview();
    }

    public void OnAttackButtonClicked()
    {
        if (!isPreviewingAttack)
        {
            currentPreview = playerClass.GetAttackPreview(CurrentCoord, grid);
            List<Vector2Int> coords = new();
            foreach (var atk in currentPreview)
                coords.Add(atk.targetCoord);

            highlighter?.ShowTiles(coords, grid);
            isPreviewingAttack = true;
        }
        else
        {
            ExecuteAttack();
            CancelAttackPreview();
            contextMenu?.SetActive(false);
        }
    }

    public void OnMoveButtonClicked()
    {
        if (!isPreviewingMove)
        {
            currentMoveOptions = GetValidAdjacentTiles(CurrentCoord);
            highlighter?.ShowTiles(currentMoveOptions, grid);
            isPreviewingMove = true;
        }
        else
        {
            CancelMovePreview();
            contextMenu?.SetActive(false);
        }
    }

    List<Vector2Int> GetValidAdjacentTiles(Vector2Int origin)
    {
        var dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var valid = new List<Vector2Int>();

        foreach (var dir in dirs)
        {
            var next = origin + dir;
            if (grid.IsValidCoord(next) && !grid.GetTile(next).isOccupied)
                valid.Add(next);
        }

        return valid;
    }

    void ExecuteAttack()
    {
        if (!stats.TryUseAction())
            return;

        foreach (var atk in currentPreview)
        {
            foreach (var enemy in new List<IEnemy>(EnemyManager.Instance.GetEnemiesOnTile(atk.targetCoord)))
            {
                if (enemy is EnemyBase eBase)
                    eBase.TakeDamage(atk.damage);
            }
        }
    }

    void CancelAttackPreview()
    {
        if (!isPreviewingAttack) return;
        highlighter?.ClearTiles();
        currentPreview.Clear();
        isPreviewingAttack = false;
    }

    void CancelMovePreview()
    {
        if (!isPreviewingMove) return;
        highlighter?.ClearTiles();
        currentMoveOptions.Clear();
        isPreviewingMove = false;
    }

    Vector2Int? GetClickedTileCoord(Vector3 point)
    {
        float minDist = float.MaxValue;
        Vector2Int? closest = null;

        foreach (var atk in currentPreview)
        {
            Vector3 tilePos = grid.CoordToWorld(atk.targetCoord.x, atk.targetCoord.y);
            float dist = Vector3.Distance(tilePos, point);
            if (dist < minDist && dist < 1.0f)
            {
                minDist = dist;
                closest = atk.targetCoord;
            }
        }

        return closest;
    }

    Vector2Int? GetClickedMoveCoord(Vector3 point)
    {
        float minDist = float.MaxValue;
        Vector2Int? closest = null;

        foreach (var coord in currentMoveOptions)
        {
            Vector3 tilePos = grid.CoordToWorld(coord.x, coord.y);
            float dist = Vector3.Distance(tilePos, point);
            if (dist < minDist && dist < 1.0f)
            {
                minDist = dist;
                closest = coord;
            }
        }

        return closest;
    }

    public void SetCoord(Vector2Int newCoord) => CurrentCoord = newCoord;
}
