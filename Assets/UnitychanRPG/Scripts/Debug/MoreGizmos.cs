using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Allow for persistent settings (like Unity) where you can enable/disable gizmos for certain classes
// TODO: Optimize this out for builds where Gizmos can't be seen anyway
// TODO: Print stack of strings
//   - this should accept a function used to determine a value to print to screen

// Retained mode wrapper for Unity's Gizmos class
public class MoreGizmos : MonoBehaviour
{
    abstract class GizmoDraw
    {
        public Vector3 position;
        private Color _color;
        public Color color
        {
            get
            {
                return _color == Color.clear ? Color.magenta : _color;
            }
            set
            {
                _color = value;
            }
        }

        public abstract void Draw();
    }
    class GizmoSphere : GizmoDraw
    {
        public GizmoSphere(Vector3 pos, float rad, Color col)
        {
            position = pos;
            radius = rad;
            color = col;
        }

        public float radius;

        public override void Draw()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, radius);
        }
    }
    class GizmoCube : GizmoDraw
    {
        public GizmoCube(Vector3 pos, Vector3 siz, Color col)
        {
            position = pos;
            size = siz;
            color = col;
        }

        public Vector3 size;
        public override void Draw()
        {
            Gizmos.color = color;
            Gizmos.DrawCube(position, size);
        }
    }

    private List<GizmoDraw> gizmos = new List<GizmoDraw>();

    private static MoreGizmos _instance;
    private static MoreGizmos instance
    {
        get
        {
            // abort if an instance already exists
            if(_instance != null) { return _instance; }

            // create a hidden GameObject that will recieve gizmo calls
            GameObject hidden = new GameObject("MoreGizmosHandler");

            // BUG: apparently hiding -> native Unity error. Unity issue?
            //hidden.hideFlags = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
            GameObject hiddenChild = new GameObject("Implementer");
            hiddenChild.transform.parent = hidden.transform;
            _instance = hiddenChild.AddComponent<MoreGizmos>();

            //Debug.Log("MoreGizmos initialized.");
            return _instance;
        }
        set
        {
            // clean-up needed?
            if(_instance != null && _instance != value)
            {
                //Debug.Log("Cleaning up extra instance of MoreGizmos.");
                GameObject.Destroy(_instance.gameObject);
            }

            _instance = value;
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawSphere(Vector3 center, float radius, Color color = default(Color))
    {
        instance.gizmos.Add(new GizmoSphere( center, radius, color));
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawCube(Vector3 center, Vector3 size, Color color = default(Color))
    {
        instance.gizmos.Add(new GizmoCube( center, size, color ));
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Awake()
    {
        _instance = (_instance == null ? this : _instance);
        if(_instance != this) { Destroy(this.gameObject); return; }
    }

    private void OnDrawGizmos()
    {
        Color originalColor = Gizmos.color;

        for(int i = 0; i < gizmos.Count; ++i)
        {
            gizmos[i].Draw();
        }

        gizmos.Clear();

        Gizmos.color = originalColor;
    }
}