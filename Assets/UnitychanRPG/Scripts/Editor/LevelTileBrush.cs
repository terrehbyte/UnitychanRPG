using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;



[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Level Tile Brush")]
[System.Obsolete("This is not ready for use yet. Please don't use it!")]
public class LevelTileBrush : GridBrush
{
    public enum LevelTileType
    {
        Floor,
        Wall,
        Void
    }

    public TileBase floorTile;
    public TileBase voidTile;

    private const int InternalEditorLayer = 13; 

    private GameObject lastUsedBrushTarget;
    private Tilemap lastUsedTileMap;

    public BoundsInt? moveSelection { get; private set; }
    public TileData[] moveTileData { get; private set; }

    public BoundsInt CalculateTargetBounds(Vector3Int position)
    {
            Vector3Int min = position - pivot;
            return new BoundsInt(min, size);
    }

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        // do not paint on the palette layer
        if (brushTarget.layer == InternalEditorLayer) { return; }

        // do we need to update our cached variables?
        if(lastUsedBrushTarget != brushTarget)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap == null) { return; }

            lastUsedTileMap = tilemap;
            lastUsedBrushTarget = brushTarget;
        }

        var bounds = CalculateTargetBounds(position);

        foreach(Vector3Int subposition in bounds.allPositionsWithin)
        {
            lastUsedTileMap.SetTile(subposition, floorTile);
        }
    }

    public override void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
    {
        // do not paint on the palette layer
        if (brushTarget.layer == InternalEditorLayer) { return; }

        base.MoveStart(gridLayout, brushTarget, position);
        moveSelection = position;
    }

    public override void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
    {
        if (brushTarget == null ||
            brushTarget.layer == 31) { return; }

        var tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap == null) { return; }

        foreach(var subposition in moveSelection.Value.allPositionsWithin)
        {
            tilemap.SetTile(subposition, voidTile);
        }

        base.MoveEnd(gridLayout, brushTarget, position);
        moveSelection = null;
    }
}

/*
[CustomEditor(typeof(LevelTileBrush))]
public class LevelTileBrushEditor : GridBrushEditor
{
    private LevelTileBrush levelTileBrush { get { return brush as LevelTileBrush; } }
    private GameObject lastBrushTarget;

    private const int InternalEditorLayer = 13; 

    public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null ||
            brushTarget.layer == InternalEditorLayer) { return; }

        var tilemap = brushTarget.GetComponent<Tilemap>();
        if (tilemap == null) { return; }

        Vector3Int min = position - brush.pivot;
        Vector3Int max = min + brush.size;
        BoundsInt bounds = new BoundsInt(min, max - min);

        // set move selection to be previewed as void?
        if(levelTileBrush.moveSelection != null)
        {
            foreach(var subposition in levelTileBrush.moveSelection.Value.allPositionsWithin)
            {
                tilemap.SetEditorPreviewTile(subposition, levelTileBrush.voidTile);
            }
        }
        foreach(var subposition in bounds.allPositionsWithin)
        {
            Vector3Int brushPosition = subposition - min;
            GridBrush.BrushCell cell = brush.cells[brush.GetCellIndex(brushPosition)];

            tilemap.SetEditorPreviewTile(subposition, cell.tile);
        }
        
        lastBrushTarget = brushTarget;
    }

    public override void ClearPreview()
    {
        if (lastBrushTarget != null)
        {
            var tilemap = lastBrushTarget.GetComponent<Tilemap>();
            if (tilemap == null)
                return;

            tilemap.ClearAllEditorPreviewTiles();

            lastBrushTarget = null;
        }
        else
        {
            base.ClearPreview();
        }
    }

    public override void OnSelectionInspectorGUI()
    {
        if (GridSelection.target.layer == InternalEditorLayer)
        {
            GridSelection.Clear();
            return;
        }

        base.OnSelectionInspectorGUI();
    }

    public override void OnToolActivated(GridBrushBase.Tool tool)
    {
        if(tool != GridBrushBase.Tool.Paint &&
           tool != GridBrushBase.Tool.Select &&
           tool != GridBrushBase.Tool.Move &&
           tool != GridBrushBase.Tool.Erase)
        {
            Debug.LogWarningFormat("The {0} tool is not supported by {1}.", tool, brush.GetType().Name);
        }
    }
}
*/