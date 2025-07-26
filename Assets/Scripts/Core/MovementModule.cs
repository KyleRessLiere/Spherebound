using System.Collections;
using UnityEngine;

public class MovementModule
{
    readonly GridManager grid;
    readonly IGridEntity entity;
    readonly MonoBehaviour runner;
    readonly float yOffset;

    public MovementModule(IGridEntity entity, MonoBehaviour runner, GridManager grid, float yOffset)
    {
        this.entity = entity;
        this.runner = runner;
        this.grid = grid;
        this.yOffset = yOffset;
    }

    public bool CanMove(Vector2Int target)
    {
        if (!grid.IsValidCoord(target)) return false;
        if (grid.GetTile(target).isOccupied) return false;
        return true;
    }

    public void TryMove(Vector2Int target)
    {
        if (!CanMove(target)) return;
        runner.StartCoroutine(MoveRoutine(target));
    }

    protected virtual IEnumerator MoveRoutine(Vector2Int target)
    {
        var fromTile = grid.GetTile(entity.CurrentCoord);
        var toTile = grid.GetTile(target);

        fromTile.isOccupied = false;
        toTile.isOccupied = true;

        entity.SetCoord(target);

        Vector3 startPos = fromTile.worldPos + Vector3.up * yOffset;
        Vector3 endPos = toTile.worldPos + Vector3.up * yOffset;
        float duration = 0.2f;
        float t = 0f;

        while (t < duration)
        {
            runner.transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        runner.transform.position = endPos;
    }
}
