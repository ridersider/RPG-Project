using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    public Tilemap groundMap;
    public Tilemap waterMap;
    public Tilemap stoneMap;

    public Tile groundTile;
    public Tile waterTile;
    public Tile stoneTile;

    public int width = 200;
    public int height = 200;
    public float scale = 0.1f;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float n = Mathf.PerlinNoise(x * scale, y * scale);

                if (n < 0.4f)
                {
                    waterMap.SetTile(new Vector3Int(x, y, 0), waterTile);
                }
                else if (n < 0.7f)
                {
                    groundMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
                else
                {
                    stoneMap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                }
            }
        }
    }
}
