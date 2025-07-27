using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    public GameObject highlightPrefab;

    private readonly List<GameObject> activeHighlights = new();
    private GridManager grid;

    void Awake()
    {
        grid = FindObjectOfType<GridManager>();
        if (grid == null)
        {
            Debug.LogError("TileHighlighter: GridManager not found.");
        }
    }

    public void ShowTiles(List<Vector2Int> coords, GridManager grid)
    {
        ClearTiles();

        foreach (var coord in coords)
        {
            if (!grid.IsValidCoord(coord)) continue;

            Tile tile = grid.GetTile(coord);
            Vector3 tilePos = tile.worldPos;

            float tileHeight = grid.tilePrefab.GetComponent<Renderer>().bounds.size.y;
            Vector3 highlightPos = tilePos + Vector3.up * (tileHeight + 0.01f);

            GameObject go = Instantiate(highlightPrefab, highlightPos, Quaternion.Euler(90, 0, 0));
            go.transform.SetParent(transform);
            MatchTileSize(go);

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

    private void MatchTileSize(GameObject highlight)
    {
        if (grid.tilePrefab != null)
        {
            Renderer tileRenderer = grid.tilePrefab.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                Vector3 tileSize = tileRenderer.bounds.size;
                highlight.transform.localScale = new Vector3(tileSize.x, 1f, tileSize.z);
            }
        }
    }
}
