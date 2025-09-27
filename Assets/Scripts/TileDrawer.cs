using System.Collections.Generic;
using UnityEngine;

public class TileDrawer : MonoBehaviour
{
    public Tile assignedTile;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private List<Material> mats;

    const int possibleStateCount = 3;
    public void ChangeMaterial(int materialID)
    {
        mesh.material = mats[materialID];
    }

    public int GetNextTileState()
    {
        return ((int)assignedTile.tileState + 1) % possibleStateCount;
    }
}
