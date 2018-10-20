using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu]
public class CustomTile : Tile
{
    public int layerId;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        base.StartUp(position, tilemap, go);

        if(go != null)
        {
            go.layer = layerId;
        }

        return true;
    }

}