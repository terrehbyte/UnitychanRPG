using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCollisionRouter : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    protected Tilemap tilemap;

    [SerializeField]
    protected int maxContactPoints = 32;
    protected ContactPoint2D[] lastCollisionContacts;

    // Offset applied to contact point to determine position of collided tile
    [SerializeField]
    protected float contactResolutionOffset = 0.01f;

    TileCollisionRouter()
    {
        lastCollisionContacts = new ContactPoint2D[maxContactPoints];
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        RouteCollision(ContactTiming.Start, collision);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        RouteCollision(ContactTiming.Stay, collision);
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        RouteCollision(ContactTiming.Exit, collision);
    }

    protected virtual void RouteCollision(ContactTiming timing, Collision2D collision)
    {
        // What happens if the array is too small?

        int contacts = collision.contactCount;
        collision.GetContacts(lastCollisionContacts);

        foreach(var contact in lastCollisionContacts)
        {
            Vector2 roughTilePosition = new Vector2(contact.point.x - 0.01f * contact.normal.x,
                                                    contact.point.y - 0.01f * contact.normal.y);

            //tilemap.WorldToCell(roughTilePosition);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        RouteTrigger(ContactTiming.Start, other);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        RouteTrigger(ContactTiming.Stay, other);
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        RouteTrigger(ContactTiming.Exit, other);
    }

    protected virtual void RouteTrigger(ContactTiming timing, Collider2D other)
    {
        // Is this actually gonna get used...?
        other.GetComponent<ITileCollidable>().OnTrigger(timing, other);
    }

    void OnValidate()
    {
        //tilemap = tilemap == null ? GetComponent<Tilemap>() : tilemap;
    }
}
