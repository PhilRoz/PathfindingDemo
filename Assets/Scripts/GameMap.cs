using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public Vector2Int mapSize = new Vector2Int(25, 25);

    [Range(0, 0.25f)] public float randomObstacleChance = 0.1f;
    [Range(0, 0.25f)] public float randomCoverChance = 0.05f;

    public int moveRange = 6;
    public int attackRange = 2;

    [Space(20)]
    public GameObject tilePrefab;
    public GameObject characterPrefab;
    public BFSPathfinding pathfinding;

    Tile[,] tiles;
    Character player;
    Character enemy;
    List<Vector2Int> paintedTiles;


    void Start()
    {
        GenerateNewMap();
        paintedTiles = new List<Vector2Int>();
        pathfinding.ResetSearch(tiles, player.currentPosition, true);
    }


    public void FindPath(Vector3 destination)
    {
        Vector2Int vector2 = new Vector2Int(Mathf.RoundToInt(destination.x), Mathf.RoundToInt(destination.z));
        FindPath(vector2);
    }
    public void FindPath(Vector2Int destination)
    {
        var path = pathfinding.FindPath(destination, maxStepsPerCall: 1000000);
        if (path != null && path.Count > 0)
        {
            PaintPath(path);
        }
    }
    public void PaintPath(List<Vector2Int> path)
    {
        foreach (var vector in paintedTiles)
        {
            tiles[vector.x, vector.y].tileDrawer.ChangeMaterial((int)tiles[vector.x, vector.y].tileState);
        }
        paintedTiles.Clear();
        foreach (var vector in path)
        {
            tiles[vector.x, vector.y].tileDrawer.ChangeMaterial(3);
            paintedTiles.Add(vector);
        }
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

        player = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity, this.transform).GetComponent<Character>();
        player.currentPosition = Vector2Int.zero;
        player.ChangeColor(Color.blue);
        tiles[0, 0].tileState = Tile.State.Traversable;
        tiles[0, 0].tileDrawer.ChangeMaterial(0);

        Vector2Int enemyPosition = GetRandomVacantPosition(10);
        enemy = Instantiate(characterPrefab, new Vector3(enemyPosition.x, 0, enemyPosition.y), Quaternion.identity, this.transform).GetComponent<Character>();
        enemy.currentPosition = enemyPosition;
        enemy.ChangeColor(Color.red);
        tiles[enemyPosition.x, enemyPosition.y].tileState = Tile.State.Traversable;
        tiles[enemyPosition.x, enemyPosition.y].tileDrawer.ChangeMaterial(0);
    }


    Vector2Int GetRandomVacantPosition(int radiusFromPlayer = -1)
    {
        int x = player.currentPosition.x;
        int y = player.currentPosition.y;
        if (radiusFromPlayer > 0)
        {
            x = Random.Range(Mathf.Max(0, -radiusFromPlayer + x), Mathf.Min(mapSize.x, radiusFromPlayer + x + 1));
            y = Random.Range(Mathf.Max(0, -radiusFromPlayer + y), Mathf.Min(mapSize.y, radiusFromPlayer + y + 1));
        }
        else
        {
            x = Random.Range(0, mapSize.x);
            y = Random.Range(0, mapSize.y);
        }
        return (new Vector2Int(x, y));
    }

    int GetRandomTileStateID()
    {
        float randomNumber = Random.Range(0, 1.0f);
        if (randomNumber < randomObstacleChance) { return 1; }
        if (randomNumber < randomObstacleChance + randomCoverChance) { return 2; }
        return 0;
    }
}
