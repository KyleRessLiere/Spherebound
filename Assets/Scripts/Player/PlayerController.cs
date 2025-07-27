using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour, IGridEntity
{
    public Vector2Int CurrentCoord { get; private set; }

    GridManager grid;
    MovementModule mover;
    PlayerStats stats;

    public IPlayerClass playerClass;
    Vector2Int lastMoveDir = Vector2Int.up;

    [Header("Context Menu UI")]
    private GameObject contextMenu;         // PlayerContextMenu child
    private Button attackButton;            // UI Button component

    void Start()
    {
        grid = FindObjectOfType<GridManager>();
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

        // 🔍 Find context menu and attack button
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
            Vector3 offset = transform.right * 0.7f + Vector3.up * 0.4f;
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
        lastMoveDir = dir;

        stats.ChangeHealth(-1); // TEMP test damage
    }

    public void OnAttackButtonClicked()
    {
        if (stats.TryUseAction())
        {
            playerClass.Attack(CurrentCoord, lastMoveDir);
        }

        if (contextMenu != null)
            contextMenu.SetActive(false);
    }

    public void SetCoord(Vector2Int newCoord) => CurrentCoord = newCoord;
}
