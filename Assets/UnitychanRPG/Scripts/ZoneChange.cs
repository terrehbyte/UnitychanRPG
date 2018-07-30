using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneChange : MonoBehaviour, ITileCollidable
{
    public void OnCollision(ContactTiming timing, Collision2D collision)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrigger(ContactTiming timing, Collider2D other)
    {
        return;
    }
}
