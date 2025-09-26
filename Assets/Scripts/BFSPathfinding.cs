using System;
using System.Collections.Generic;
using UnityEngine;

public class BFSPathfinding : MonoBehaviour
{
    private bool[,] visited;
    private int[,] parentX, parentY;
    private int[] queueX, queueY;
    private int queueStart, queueEnd;
    private Vector2Int startPosition;
    private Tile[,] savedTiles;
    private bool isCoverAnObstacle;
    private int width, height;
    private static readonly int[] DirectionX = { 0, 0, -1, 1 };
    private static readonly int[] DirectionY = { -1, 1, 0, 0 };

    public void ResetSearch(Tile[,] tiles, Vector2Int start, bool treatCoverAsObstacle = false)
    {
        width = tiles.GetLength(0);
        height = tiles.GetLength(1);

        if (visited == null || visited.GetLength(0) != width || visited.GetLength(1) != height)
        {
            visited = new bool[width, height];
            parentX = new int[width, height];
            parentY = new int[width, height];
            queueX = new int[width * height];
            queueY = new int[width * height];
        }

        Array.Clear(visited, 0, visited.Length);
        Array.Clear(parentX, 0, parentX.Length);
        Array.Clear(parentY, 0, parentY.Length);

        startPosition = start;
        this.savedTiles = tiles;
        this.isCoverAnObstacle = treatCoverAsObstacle;

        queueStart = 0;
        queueEnd = 0;

        if (!IsValidAndPassable(start.x, start.y)) return;

        queueX[queueEnd] = start.x;
        queueY[queueEnd] = start.y;
        queueEnd++;
        visited[start.x, start.y] = true;
    }

    public List<Vector2Int> FindPath(Vector2Int end, int maxStepsPerCall = int.MaxValue)
    {
        if (parentX == null || savedTiles == null || queueX == null)
        {
            Debug.LogError("You must initialize the search data by calling ResetSearch() first!");
            return null;
        }
        int endX = end.x, endY = end.y;

        if (endX < 0 || endX >= width || endY < 0 || endY >= height) return null;

        if (visited[endX, endY])
            return BuildPath(endX, endY);

        int steps = 0;
        while (queueStart < queueEnd && steps < maxStepsPerCall)
        {
            int currentX = queueX[queueStart];
            int currentY = queueY[queueStart];
            queueStart++;
            steps++;

            for (int dir = 0; dir < 4; dir++)
            {
                int neighborX = currentX + DirectionX[dir];
                int neighborY = currentY + DirectionY[dir];

                if (TryAddNeighbor(neighborX, neighborY, currentX, currentY))
                {
                    if (neighborX == endX && neighborY == endY)
                        return BuildPath(endX, endY);
                }
            }
        }

        if (visited[endX, endY])
            return BuildPath(endX, endY);

        return null;
    }

    private bool IsValidAndPassable(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        ref Tile tile = ref savedTiles[x, y];
        return tile.tileState != Tile.State.Obstacle &&
               (!isCoverAnObstacle || tile.tileState != Tile.State.Cover);
    }

    private bool TryAddNeighbor(int neighborX, int neighborY, int currentX, int currentY)
    {
        if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height)
            return false;
        if (visited[neighborX, neighborY])
            return false;

        ref Tile tile = ref savedTiles[neighborX, neighborY];
        if (tile.tileState == Tile.State.Obstacle ||
            (isCoverAnObstacle && tile.tileState == Tile.State.Cover))
            return false;

        queueX[queueEnd] = neighborX;
        queueY[queueEnd] = neighborY;
        queueEnd++;
        visited[neighborX, neighborY] = true;
        parentX[neighborX, neighborY] = currentX;
        parentY[neighborX, neighborY] = currentY;

        return true;
    }

    private List<Vector2Int> BuildPath(int endX, int endY)
    {
        var path = new List<Vector2Int>();
        int currentX = endX, currentY = endY;

        while (currentX != startPosition.x || currentY != startPosition.y)
        {
            path.Add(new Vector2Int(currentX, currentY));
            int nextX = parentX[currentX, currentY];
            int nextY = parentY[currentX, currentY];
            currentX = nextX;
            currentY = nextY;
        }

        path.Add(startPosition);
        path.Reverse();
        return path;
    }
}