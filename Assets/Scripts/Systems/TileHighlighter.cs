using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    [SerializeField] private GridManager grid;

    [Header("Highlight Materials")]
    public Material redMaterial;
    public Material blueMaterial;

    private Dictionary<Vector2Int, Material> originalMaterials = new();
    private List<Vector2Int> highlightedCoords = new();

    void Awake()
    {
        if (grid == null)
        {
            grid = FindAnyObjectByType<GridManager>();
            if (grid == null)
                Debug.LogError("TileHighlighter: GridManager not assigned and not found.");
        }
    }

    public void ShowTiles(List<Vector2Int> coords, string type = "blue", GridManager gridOverride = null)
    {
        ClearTiles();

        GridManager useGrid = gridOverride ?? grid;
        Material highlightMat = type == "red" ? redMaterial : blueMaterial;

        foreach (var coord in coords)
        {
            if (!useGrid.IsValidCoord(coord)) continue;

            Tile tile = useGrid.GetTile(coord);
            if (tile.tileObject == null) continue;

            Renderer renderer = tile.tileObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[coord] = renderer.material;
                renderer.material = highlightMat;
                highlightedCoords.Add(coord);
            }
        }
    }

    public void ClearTiles()
    {
        foreach (var coord in highlightedCoords)
        {
            if (!grid.IsValidCoord(coord)) continue;

            Tile tile = grid.GetTile(coord);
            if (tile.tileObject == null) continue;

            Renderer renderer = tile.tileObject.GetComponent<Renderer>();
            if (renderer != null && originalMaterials.ContainsKey(coord))
            {
                renderer.material = originalMaterials[coord];
            }
        }

        originalMaterials.Clear();
        highlightedCoords.Clear();
    }
}
