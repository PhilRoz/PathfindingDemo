using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Tilemaps;

public class GameMap : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(25, 25);

    [Range(0, 0.25f)] public float randomObstacleChance = 0.1f;
    [Range(0, 0.25f)] public float randomCoverChance = 0.05f;

    public int moveRange = 6;
    public int attackRange = 2;

    [Space(20)]
    public GameObject tilePrefab;

    Tile[,] tiles;


    void Start()
    {
        GenerateNewMap();
    }


    void GenerateNewMap(bool randomizeObstacles = true)
    {
        tiles = new Tile[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                tiles[x, y] = new Tile();
                tiles[x, y].tileDrawer = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0), this.transform).GetComponent<TileDrawer>();
                tiles[x, y].tileDrawer.assignedTile = tiles[x, y];

                if (randomizeObstacles)
                {
                    int obstacleType = GetRandomTileStateID();
                    tiles[x, y].tileState = (Tile.State)obstacleType;
                    tiles[x, y].tileDrawer.ChangeMaterial(obstacleType);
                }
            }
        }
    }

    int GetRandomTileStateID()
    {
        float randomNumber = Random.Range(0, 1.0f);
        if (randomNumber < randomObstacleChance) { return 1; }
        if (randomNumber < randomObstacleChance + randomCoverChance) { return 2; }
        return 0;
    }
}
