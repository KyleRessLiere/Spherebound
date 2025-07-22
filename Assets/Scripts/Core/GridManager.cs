using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;
    public float spacing = 0.2f;

    [Header("Split Gap (player vs enemy)")]
    public int splitRow = 3;
    public float halfGap = 1f;

    [Header("Visual Tile Prefab")]
    public GameObject tilePrefab;  // drag your thin‐cube prefab here

    private Tile[,] grid;

    void Awake()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        grid = new Tile[width, height];

        float step = tileSize + spacing;
        float halfW = (width - 1) * 0.5f;
        float halfH = (height - 1) * 0.5f;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                // center around origin:
                float worldX = (x - halfW) * step;
                float worldZ = (y - halfH) * step;
                if (y >= splitRow) worldZ += halfGap;

                Vector3 pos = new Vector3(worldX, 0f, worldZ);
                grid[x, y] = new Tile(new Vector2Int(x, y), pos);

                if (tilePrefab != null)
                {
                    var go = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                    go.name = $"Tile_{x}_{y}";
                }
            }
    }

    public Vector3 CoordToWorld(int x, int y)
        => grid[x, y].worldPos;

    public bool IsValidCoord(Vector2Int c)
        => c.x >= 0 && c.x < width && c.y >= 0 && c.y < height;

    public Tile GetTile(Vector2Int c)
        => IsValidCoord(c) ? grid[c.x, c.y] : null;
}

public class Tile
{
    public Vector2Int coord;
    public Vector3 worldPos;
    public bool isOccupied;

    public Tile(Vector2Int coord, Vector3 pos)
    {
        this.coord = coord;
        this.worldPos = pos;
        this.isOccupied = false;
    }
}
