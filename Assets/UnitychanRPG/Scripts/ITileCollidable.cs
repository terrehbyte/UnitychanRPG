using UnityEngine;

public enum ContactTiming
{
    Start,
    Stay,
    Exit
}

public interface ITileCollidable
{
    void OnTrigger(ContactTiming timing, Collider2D other);
    void OnCollision(ContactTiming timing , Collision2D collision);
}