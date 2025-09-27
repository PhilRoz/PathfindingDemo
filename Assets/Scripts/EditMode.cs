using UnityEngine;

public class EditMode : MonoBehaviour
{
    public enum State { Play, EditPlayer, EditEnemy, EditMap }
    public State currentState;

    [SerializeField] GameMap map;

    int cachedPaintColor;

    void Start()
    {
        currentState = State.Play;
        cachedPaintColor = -1;
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (currentState == State.EditMap)
            {
                cachedPaintColor = -1;
            }
        }
    }

    public void Click(TileDrawer drawer)
    {
        switch (currentState)
        {
            case State.Play:
                PerformMovement();
                break;
            case State.EditPlayer:
                ChangePlayerLocation();
                break;
            case State.EditEnemy:
                CreateEnemy();
                break;
            case State.EditMap:
                DrawOnMap(drawer);
                break;
        }
    }



    void PerformMovement()
    {

    }

    void ChangePlayerLocation()
    {

    }

    void CreateEnemy()
    {

    }

    void DrawOnMap(TileDrawer drawer)
    {
        Debug.Log(cachedPaintColor);
        if (cachedPaintColor == -1)
        {
            cachedPaintColor = drawer.GetNextTileState();
        }
        drawer.assignedTile.tileState = (Tile.State)cachedPaintColor;
        drawer.ChangeMaterial(cachedPaintColor);
    }

    public void OnEnterPlay()
    {
        //reset search cache
    }
    public void OnExitPlay()
    {
        //reset pathfinding
    }

}
