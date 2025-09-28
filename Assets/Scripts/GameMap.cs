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
    Dictionary<Vector2Int, Character> enemies;
    List<Vector2Int> paintedTiles;
    new Transform transform;

    void Start()
    {
        enemies = new Dictionary<Vector2Int, Character>();
        GenerateNewMap(mapSize);
        ResetSearch();
        paintedTiles = new List<Vector2Int>();
        transform = GetComponent<Transform>();
    }

    public Vector2Int ConvertCoordinate(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
    }
    public Vector3 ConvertCoordinate(Vector2Int position)
    {
        return new Vector3(position.x, 0, position.y);
    }

    public void FindPath(Vector3 destination)
    {
        Vector2Int vector2 = ConvertCoordinate(destination);
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
        ClearPath();
        foreach (var vector in path)
        {
            tiles[vector.x, vector.y].tileDrawer.ChangeMaterial(3);
            paintedTiles.Add(vector);
        }
    }

    public void ClearPath()
    {
        foreach (var vector in paintedTiles)
        {
            tiles[vector.x, vector.y].tileDrawer.ChangeMaterial((int)tiles[vector.x, vector.y].tileState);
        }
        paintedTiles.Clear();
    }

    public void ResetSearch()
    {
        pathfinding.ResetSearch(tiles, player.currentPosition, true);
    }


    public void GenerateNewMap(Vector2Int size, bool randomizeObstacles = true)
    {
        if (mapSize != size || tiles == null)
        {
            if (tiles != null)
            {
                foreach (Tile tile in tiles)
                {
                    Destroy(tile.tileDrawer.gameObject);
                }
            }
            mapSize = size;
            tiles = new Tile[size.x, size.y];
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    tiles[x, y] = new Tile();
                    tiles[x, y].tileDrawer = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0), transform).GetComponent<TileDrawer>();
                    tiles[x, y].tileDrawer.assignedTile = tiles[x, y];
                }
            }
        }

        foreach (Tile tile in tiles)
        {
            int obstacleType = randomizeObstacles ? GetRandomTileStateID() : 0;
            tile.tileState = (Tile.State)obstacleType;
            tile.tileDrawer.ChangeMaterial(obstacleType);
        }
        
        
        SetPlayerPosition(Vector2Int.zero);

        foreach (KeyValuePair<Vector2Int,Character> kvp in enemies)
        {
            Destroy(kvp.Value.gameObject);
        }
        enemies.Clear();
        AddOrDeleteEnemy(GetRandomVacantPosition(10));

    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public void SetPlayerPosition(Vector2Int position)
    {
        if (player == null)
        {
            player = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity, this.transform).GetComponent<Character>();
            player.ChangeColor(Color.blue);
        }
        player.currentPosition = position;
        player.transform.localPosition = ConvertCoordinate(position);
        tiles[position.x, position.y].tileState = Tile.State.Traversable;
        tiles[position.x, position.y].tileDrawer.ChangeMaterial(0);
    }
    
    public void AddOrDeleteEnemy(Vector2Int position)
    {
        if (enemies.ContainsKey(position))
        {
            var enemy = enemies.GetValueOrDefault(position);
            enemies.Remove(position);
            Destroy(enemy.gameObject);
        }
        else
        {
            var enemy = Instantiate(characterPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity, this.transform).GetComponent<Character>();
            enemies.Add(position, enemy);
            enemy.currentPosition = position;
            enemy.ChangeColor(Color.red);
            tiles[position.x, position.y].tileState = Tile.State.Traversable;
            tiles[position.x, position.y].tileDrawer.ChangeMaterial(0);
        }
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
