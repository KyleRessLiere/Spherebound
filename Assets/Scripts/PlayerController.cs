using UnityEngine;
using UnityEngine.InputSystem;  // if you’re using the new Input System

public class PlayerController : MonoBehaviour, IGridEntity
{
    [Header("Action Points")]
    [Tooltip("How many moves the player gets each turn")]
    public int maxActionsPerTurn = 2;

    [HideInInspector]
    public int actionsPerTurn;     // remaining this turn

    // IGridEntity
    public Vector2Int CurrentCoord { get; private set; }

    GridManager grid;
    MovementModule mover;

    void Start()
    {
        // 1) Find your grid
        grid = UnityEngine.Object.FindFirstObjectByType<GridManager>();

        // 2) Initialize actions
        actionsPerTurn = maxActionsPerTurn;

        // 3) Decide your start coord (center of player side)
        CurrentCoord = new Vector2Int(grid.width / 2, 1);

        // 4) Compute the vertical offset so the sphere rests atop the cube
        float tileHalfH = grid.tilePrefab.transform.localScale.y * 0.5f;
        float sphereRadius = transform.localScale.y * 0.5f;
        float yOffset = tileHalfH + sphereRadius;

        // 5) Snap to starting tile (world position + yOffset)
        Vector3 worldPos = grid.CoordToWorld(CurrentCoord.x, CurrentCoord.y);
        transform.position = worldPos + Vector3.up * yOffset;

        // 6) Mark that tile occupied
        grid.GetTile(CurrentCoord).isOccupied = true;

        // 7) Create the movement module with the yOffset
        mover = new MovementModule(this, this, grid, yOffset);
    }

    void Update()
    {
        // Using the new Input System:
        if (Keyboard.current.wKey.wasPressedThisFrame) AttemptMove(Vector2Int.up);
        if (Keyboard.current.sKey.wasPressedThisFrame) AttemptMove(Vector2Int.down);
        if (Keyboard.current.aKey.wasPressedThisFrame) AttemptMove(Vector2Int.left);
        if (Keyboard.current.dKey.wasPressedThisFrame) AttemptMove(Vector2Int.right);
    }

    void AttemptMove(Vector2Int dir)
    {
        if (actionsPerTurn <= 0) return;

        Vector2Int target = CurrentCoord + dir;
        if (!grid.IsValidCoord(target)) return;
        if (grid.GetTile(target).isOccupied) return;

        // Free old tile immediately
        grid.GetTile(CurrentCoord).isOccupied = false;

        // Tell mover to animate & update coord
        mover.TryMove(target);

        // Update logical coord and reserve new tile
        CurrentCoord = target;
        grid.GetTile(CurrentCoord).isOccupied = true;

        actionsPerTurn--;
    }

    // IGridEntity
    public void SetCoord(Vector2Int newCoord)
    {
        CurrentCoord = newCoord;
    }
}
