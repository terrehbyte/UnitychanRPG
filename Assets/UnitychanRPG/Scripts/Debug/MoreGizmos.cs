using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Allow for persistent settings (like Unity) where you can enable/disable gizmos for certain classes
// TODO: Optimize this out for builds where Gizmos can't be seen anyway
// TODO: Print stack of strings
//   - this should accept a function used to determine a value to print to screen
// TODO: support execution during edit-time

// LONG-TODO: Support execution at run-time https://docs.unity3d.com/ScriptReference/GL.html
//            I don't like the idea of modifying script execution order, so this should be
//            added to a custom update loop

// Retained mode wrapper for Unity's Gizmos class
public class MoreGizmos : MonoBehaviour
{
    // TODO: consider exposing GizmoDraw for custom behavior from end-user

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

        public virtual void BeforeDraw()
        {
            Gizmos.color = color;
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
            Gizmos.DrawCube(position, size);
        }
    }
    class GizmoCircle : GizmoDraw
    {
        public Vector3 normal;
        public float radius;
        public int sides;

        public GizmoCircle(Vector3 pos, Vector3 norm, float rad, int sideCount, Color col)
        {
            Debug.Assert(sideCount > 2);

            position = pos;
            normal = norm;
            radius = rad;
            sides = sideCount;
            color = col;
        }

        public override void Draw()
        {
            float step = 2 * Mathf.PI / (float)sides;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);

            for(int i = 0; i < sides; ++i)
            {
                int startIdx = i;
                int endIdx = (i+1) % (sides);

                Vector3 legStart = rot * new Vector3(radius * Mathf.Cos(step * startIdx), radius * Mathf.Sin(step * startIdx), 0.0f);
                Vector3 legEnd = rot * new Vector3(radius * Mathf.Cos(step * endIdx),   radius * Mathf.Sin(step * endIdx), 0.0f);

                Gizmos.DrawLine(position + legStart, position + legEnd);
            }

            return; // remove below when done
        }
    }
    class GizmoSquare : GizmoDraw
    {
        public float degrees;
        public Vector3 normal;
        public Vector2 size;

        public GizmoSquare(Vector3 center, Vector3 norm, Vector2 extents, float deg, Color col)
        {
            position = center;
            normal = norm;
            size = extents;
            color = col;
            degrees = deg;
        }

        public override void Draw()
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, normal);

            Vector2 halfExt = size / 2;

            Quaternion localRot = Quaternion.AngleAxis(degrees, Vector3.forward);

            Vector3[] verts =
            {
                localRot * new Vector3(-halfExt.x,  halfExt.y, 0), // top-left
                localRot * new Vector3( halfExt.x,  halfExt.y, 0), // top-right
                localRot * new Vector3( halfExt.x, -halfExt.y, 0), // bottom-right
                localRot * new Vector3(-halfExt.x, -halfExt.y, 0), // bottom-left
            };

            for(int i = 0; i < verts.Length; ++i)
            {
                int endIdx = (i+1) % (verts.Length);

                Vector3 legStart = (position + rot * verts[i]);
                Vector3 legEnd = (position + rot * verts[endIdx]);

                Gizmos.DrawLine(legStart, legEnd);
            }
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
    public static void DrawCircle(Vector3 center, Vector3 normal, float radius = 1.0f, int sides = 9, Color color = default(Color))
    {
        instance.gizmos.Add(new GizmoCircle( center, normal, radius, sides, color ));
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawSquare(Vector3 center, Vector3 normal, Vector2 size, float degrees = 0.0f, Color color = default(Color))
    {
        instance.gizmos.Add(new GizmoSquare( center, normal, size, degrees, color ));
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
            gizmos[i].BeforeDraw();
            gizmos[i].Draw();
        }

        gizmos.Clear();

        Gizmos.color = originalColor;
    }
}