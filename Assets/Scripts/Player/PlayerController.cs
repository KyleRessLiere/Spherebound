using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private const float ContextMenuOffsetRight = 1.2f;
    private const float ContextMenuOffsetUp = 0.4f;

    private bool isPreviewingAttack = false;
    private TileHighlighter highlighter;

    private List<AttackInstance> currentPreview = new();

    void Start()
    {
        grid = FindObjectOfType<GridManager>();
        stats = GetComponent<PlayerStats>();
        stats.ResetActions();

        playerClass = new StrikerClass(this); // Replaceable with other classes

        CurrentCoord = new Vector2Int(grid.width / 2, 1);
        float tileHalfH = grid.tilePrefab.transform.localScale.y * 0.5f;
        float sphereRadius = transform.localScale.y * 0.5f;
        float yOffset = tileHalfH + sphereRadius;
        Vector3 worldPos = grid.CoordToWorld(CurrentCoord.x, CurrentCoord.y);
        transform.position = worldPos + Vector3.up * yOffset;

        grid.GetTile(CurrentCoord).isOccupied = true;
        mover = new MovementModule(this, this, grid, yOffset);

        contextMenu = transform.Find("PlayerContextMenu")?.gameObject;
        attackButton = contextMenu?.GetComponentInChildren<Button>();

        if (contextMenu != null)
        {
            contextMenu.SetActive(false);
            contextMenu.transform.localScale = Vector3.one * 0.01f;

            if (attackButton != null)
            {
                attackButton.onClick.AddListener(OnAttackButtonClicked);
            }
            else
            {
                Debug.LogWarning("⚠️ Attack Button not found in PlayerContextMenu");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerContextMenu not found under Player");
        }

        highlighter = FindObjectOfType<TileHighlighter>();
        if (highlighter == null)
        {
            Debug.LogWarning("⚠️ TileHighlighter not found in scene.");
        }
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame) AttemptMove(Vector2Int.up);
        if (Keyboard.current.sKey.wasPressedThisFrame) AttemptMove(Vector2Int.down);
        if (Keyboard.current.aKey.wasPressedThisFrame) AttemptMove(Vector2Int.left);
        if (Keyboard.current.dKey.wasPressedThisFrame) AttemptMove(Vector2Int.right);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject && contextMenu != null)
                {
                    contextMenu.SetActive(!contextMenu.activeSelf);
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
            List<Vector2Int> affectedCoords = new();
            foreach (var atk in currentPreview)
                affectedCoords.Add(atk.targetCoord);

            highlighter?.ShowTiles(affectedCoords, grid);
            isPreviewingAttack = true;
        }
        else
        {
            if (stats.TryUseAction())
            {
                foreach (var atk in currentPreview)
                {
                    var enemies = EnemyManager.Instance.GetEnemiesOnTile(atk.targetCoord);
                    foreach (var enemy in enemies)
                    {
                        if (enemy is EnemyBase eBase)
                            eBase.TakeDamage(atk.damage);
                    }
                }
            }

            CancelAttackPreview();
            contextMenu?.SetActive(false);
        }
    }

    void CancelAttackPreview()
    {
        highlighter?.ClearTiles();
        currentPreview.Clear();
        isPreviewingAttack = false;
    }

    public void SetCoord(Vector2Int newCoord) => CurrentCoord = newCoord;
}
