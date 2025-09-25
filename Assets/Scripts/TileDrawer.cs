using System.Collections.Generic;
using UnityEngine;

public class TileDrawer : MonoBehaviour
{
    public Tile assignedTile;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private List<Material> mats;
    public void ChangeMaterial(int materialID)
    {
        mesh.material = mats[materialID];
    }
}
