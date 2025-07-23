// PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour, IGridEntity
{
    public Vector2Int CurrentCoord { get; private set; }

    GridManager grid;
    MovementModule mover;
    PlayerStats stats;

    void Start()
    {
        grid = FindObjectOfType<GridManager>();
        stats = GetComponent<PlayerStats>();
        stats.ResetActions();

        // position setup (unchanged)…
        CurrentCoord = new Vector2Int(grid.width / 2, 1);
        float tileHalfH = grid.tilePrefab.transform.localScale.y * .5f;
        float sphereRadius = transform.localScale.y * .5f;
        float yOffset = tileHalfH + sphereRadius;
        Vector3 worldPos = grid.CoordToWorld(CurrentCoord.x, CurrentCoord.y);
        transform.position = worldPos + Vector3.up * yOffset;
        grid.GetTile(CurrentCoord).isOccupied = true;
        mover = new MovementModule(this, this, grid, yOffset);
    }

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame) AttemptMove(Vector2Int.up);
        if (Keyboard.current.sKey.wasPressedThisFrame) AttemptMove(Vector2Int.down);
        if (Keyboard.current.aKey.wasPressedThisFrame) AttemptMove(Vector2Int.left);
        if (Keyboard.current.dKey.wasPressedThisFrame) AttemptMove(Vector2Int.right);
    }

    void AttemptMove(Vector2Int dir)
    {
        var target = CurrentCoord + dir;
        if (!grid.IsValidCoord(target) || grid.GetTile(target).isOccupied)
            return;

        if (!stats.TryUseAction())
            return;

        // move...
        grid.GetTile(CurrentCoord).isOccupied = false;
        mover.TryMove(target);
        CurrentCoord = target;
        grid.GetTile(CurrentCoord).isOccupied = true;

        // **TEST**: knock off 1 HP each move
        stats.ChangeHealth(-1);
    }

    public void SetCoord(Vector2Int newCoord) => CurrentCoord = newCoord;
}
