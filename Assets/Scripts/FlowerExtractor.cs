using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowerExtractor : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap flowerTilemap; 
    public TileBase flowerTile; 

    [Header("Output")]
    public List<Vector3> flowerPositions;


    private void Start()
    {
        flowerPositions = ExtractFlowerPositions();
    }

    private List<Vector3> ExtractFlowerPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        BoundsInt bounds = flowerTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (flowerTilemap.GetTile(cellPosition) == flowerTile)
                {
                    positions.Add(flowerTilemap.CellToWorld(cellPosition) + flowerTilemap.tileAnchor);
                }
            }
        }

        return positions;
    }
}
