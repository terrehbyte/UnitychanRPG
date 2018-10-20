using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(CustomTile))]
public class CustomTileEditor : Editor
{
    private struct Layer
    {
        public string name;
        public int index;

        public Layer(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
    };

    private List<Layer> layers = new List<Layer>();
    private List<string> layerNames = new List<string>();

    private SerializedProperty spriteProperty;  // todo: render this
    private SerializedProperty colorProperty;
    private SerializedProperty colliderProperty;
    private SerializedProperty layerProperty;

    public void OnEnable()
    {
        colorProperty = serializedObject.FindProperty("m_Color");
        colliderProperty = serializedObject.FindProperty("m_ColliderType");
        spriteProperty = serializedObject.FindProperty("m_Sprite");
        layerProperty = serializedObject.FindProperty("layerId");
    }

    public override void OnInspectorGUI()
    {
        var tile = (CustomTile)target;

        EditorGUILayout.PropertyField(spriteProperty);
        EditorGUILayout.PropertyField(colorProperty);
        EditorGUILayout.PropertyField(colliderProperty);
        for(int i = 0; i < 32; ++i)
        {
            string layerName = LayerMask.LayerToName(i);
            if(layerName.Length > 0)
            {
                layers.Add(new Layer(layerName, i));
                layerNames.Add(layerName);
            }
        }
        
        int currentIndex = layers.Find(
            delegate(Layer layer)
            {
                return layer.index == tile.layerId;
            }
            ).index;

        layerProperty.intValue = EditorGUILayout.Popup(currentIndex, layerNames.ToArray());

        serializedObject.ApplyModifiedProperties();
    }
}