using System;
using UnityEngine;
using UnityEngine.Events;

public class EditMode : MonoBehaviour
{
    public enum State { Play, EditPlayer, EditEnemy, EditMap }
    public State currentState;

    [SerializeField] GameMap map;
    int cachedPaintColor;

    [HideInInspector] public UnityEvent onEnterPlay;
    [HideInInspector] public UnityEvent onChangeState;

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
                PerformMovement(drawer);
                break;
            case State.EditPlayer:
                ChangePlayerLocation(drawer);
                break;
            case State.EditEnemy:
                CreateEnemy(drawer);
                break;
            case State.EditMap:
                DrawOnMap(drawer);
                break;
        }
    }

    public void ChangeState(int stateID)
    {
        if (currentState == (State)stateID) return;
        if (currentState == State.Play) { OnExitPlay(); }
        currentState = (State)stateID; 
        if (currentState == State.Play) { OnEnterPlay(); }
        onChangeState.Invoke();
    }


    void PerformMovement(TileDrawer drawer)
    {
        map.PerformPlayerAction(map.ConvertCoordinate(drawer.transform.localPosition));
    }

    void ChangePlayerLocation(TileDrawer drawer)
    {
        map.SetPlayerPosition(map.ConvertCoordinate(drawer.transform.localPosition));
    }

    void CreateEnemy(TileDrawer drawer)
    {
        map.AddOrDeleteEnemy(map.ConvertCoordinate(drawer.transform.localPosition));
    }

    void DrawOnMap(TileDrawer drawer)
    {
        if (!map.TileVacancyCheck(map.ConvertCoordinate(drawer.transform.localPosition))) return;

        if (cachedPaintColor == -1)
        {
            cachedPaintColor = drawer.GetNextTileState();
        }
        drawer.assignedTile.tileState = (Tile.State)cachedPaintColor;
        drawer.ChangeMaterial(cachedPaintColor);
    }

    void OnEnterPlay()
    {
        map.ResetSearch();
        onEnterPlay.Invoke();
    }
    void OnExitPlay()
    {
        map.ClearPath();
    }

    public void UpdateMoveRange(int newMoveRange)
    {
        map.moveRange = newMoveRange;
    }
    public void UpdateAttackRange(int newAtkRange)
    {
        map.attackRange = newAtkRange;
    }
    public void UpdateMap(Vector2Int size, bool obstacles)
    {
        map.GenerateNewMap(size, obstacles);
    }
}
