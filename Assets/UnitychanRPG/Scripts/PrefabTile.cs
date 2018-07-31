using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class PrefabTile : TileBase
{
    public Sprite editorSprite;
    public GameObject associatedPrefab;

    public GameObject instance;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = editorSprite;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        // TODO: Interact with GridInformation to trigger setup stuff
        
        // do we need to instantiate our object?
        if(instance == null)
        {
            var transform = tilemap.GetComponent<Transform>();
            if(transform)
            {
                Debug.Log(transform.name);
            }
            else
            {
                Debug.Log(":(");
            }
            //
        }
    }
}
