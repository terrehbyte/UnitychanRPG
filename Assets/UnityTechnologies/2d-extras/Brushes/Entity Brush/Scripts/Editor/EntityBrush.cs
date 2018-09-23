using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
	// TODO: pick gameobjects from clicking on a cell (likely populates via PaintInspectorGUI)

	[CreateAssetMenu]
	[CustomGridBrush(false, true, false, "Entity Brush")]
	public class EntityBrush : GridBrushBase
	{
		public GameObject[] m_Prefabs;
		public int m_Z;
		public int index;
		public bool shouldDestroyTile = true;

		private GameObject lastBrushTarget;

		public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			GameObject prefab = m_Prefabs[index];
			Debug.Assert(prefab != null, "Prefab selected was null, canceling operation.");

			// Cancel if an object already exists
			Transform existing = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
			if (existing != null)
			{
				Debug.Log("An object already exists, canceling operation.");
				return;
			}

			GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
			Debug.Assert(instance != null, "Failed to instantiate prefab, canceling operation.");

			Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Entities");
			Undo.RegisterCreatedObjectUndo((Object)instance, "Paint Entities");
			instance.transform.SetParent(brushTarget.transform);
			instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3Int(position.x, position.y, m_Z) + new Vector3(.5f, .5f, .5f)));

			if(shouldDestroyTile)
			{
				var tilemap = brushTarget.GetComponentInParent<Tilemap>();
				
				tilemap.SetTile(position, null);
				tilemap.SetTransformMatrix(position, Matrix4x4.identity);
				tilemap.SetColor(position, Color.white);
			}
		}

		public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			// Do not allow editing palettes
			if (brushTarget.layer == 31)
				return;

			Transform erased = GetObjectInCell(grid, brushTarget.transform, new Vector3Int(position.x, position.y, m_Z));
			if (erased != null)
				Undo.DestroyObjectImmediate(erased.gameObject);
		}

		private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
		{
			int childCount = parent.childCount;
			Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
			Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3Int.one));
			Bounds bounds = new Bounds((max + min)*.5f, max - min);

			for (int i = 0; i < childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (bounds.Contains(child.position))
					return child;
			}
			return null;
		}
	}

	[CustomEditor(typeof(EntityBrush))]
	public class EntityBrushEditor : GridBrushEditorBase
	{
		private EntityBrush EntityBrush { get { return target as EntityBrush; } }

		private SerializedProperty m_Prefabs;
		private SerializedObject m_SerializedObject;

		protected void OnEnable()
		{
			m_SerializedObject = new SerializedObject(target);
			m_Prefabs = m_SerializedObject.FindProperty("m_Prefabs");
		}

		public override void OnPaintInspectorGUI()
		{
			m_SerializedObject.UpdateIfRequiredOrScript();
			EntityBrush.m_Z = EditorGUILayout.IntField("Position Z", EntityBrush.m_Z);
			EntityBrush.index = EditorGUILayout.IntSlider("Index", EntityBrush.index, 0, m_Prefabs.arraySize-1);
			EntityBrush.shouldDestroyTile = EditorGUILayout.ToggleLeft("Should Destroy Tile", EntityBrush.shouldDestroyTile);

			EditorGUILayout.PropertyField(m_Prefabs, true);
			m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();

			// TODO: Add a nicer visual picker for prebabs rather than by index
		}
	}
}
