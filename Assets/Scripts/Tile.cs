using UnityEngine;

public class Tile
{
    public enum State
    {
        Traversable, Obstacle, Cover
    }
    public State tileState;
    public TileDrawer tileDrawer;
}
