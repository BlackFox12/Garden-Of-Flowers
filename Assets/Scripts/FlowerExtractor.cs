using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowerExtractor : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap flowerTilemap; // Tilemap som innehåller blommorna
    public TileBase flowerTile;  // Den specifika Tile som representerar en blomma

    [Header("Output")]
    public List<Vector3> flowerPositions; // Lista med blompositioner


    private void Start()
    {
        // Skapa listan och fyll den med blompositioner
        flowerPositions = ExtractFlowerPositions();
    }

    private List<Vector3> ExtractFlowerPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        // Loopa igenom alla celler i Tilemapens gränser
        BoundsInt bounds = flowerTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                // Kontrollera om cellen innehåller en blomma
                if (flowerTilemap.GetTile(cellPosition) == flowerTile)
                {
                    // Lägg till världens position för blomman i listan
                    positions.Add(flowerTilemap.CellToWorld(cellPosition) + flowerTilemap.tileAnchor);
                }
            }
        }

        Debug.Log($"Hittade {positions.Count} blommor i tilemapen.");
        return positions;
    }
}
