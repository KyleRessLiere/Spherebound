using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    public GameObject highlightPrefab;

    private readonly List<GameObject> activeHighlights = new();

    void Awake()
    {
        if (grid == null)
        {
            Debug.LogError("TileHighlighter: GridManager not assigned in inspector.");
        }
    }

    public void ShowTiles(List<Vector2Int> coords, GridManager gridOverride = null)
    {
        ClearTiles();

        GridManager useGrid = gridOverride ?? grid;

        foreach (var coord in coords)
        {
            if (!useGrid.IsValidCoord(coord)) continue;

            Tile tile = useGrid.GetTile(coord);
            Vector3 tilePos = tile.worldPos;

            float tileHeight = useGrid.tilePrefab.GetComponent<Renderer>().bounds.size.y;
            Vector3 highlightPos = tilePos + Vector3.up * (tileHeight + 0.01f);

            GameObject go = Instantiate(highlightPrefab, highlightPos, Quaternion.Euler(90, 0, 0));
            go.transform.SetParent(transform);
            MatchTileSize(go, useGrid);

            activeHighlights.Add(go);
        }
    }

    public void ClearTiles()
    {
        foreach (var go in activeHighlights)
        {
            Destroy(go);
        }
        activeHighlights.Clear();
    }

    private void MatchTileSize(GameObject highlight, GridManager gm)
    {
        if (gm.tilePrefab != null)
        {
            Renderer tileRenderer = gm.tilePrefab.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                Vector3 tileSize = tileRenderer.bounds.size;
                highlight.transform.localScale = new Vector3(tileSize.x, 1f, tileSize.z);
            }
        }
    }
}
