using UnityEngine;

public interface IAimable
{
    void AimUp(float delta);
    void AimRight(float delta);
    void Aim(Vector2 delta);
    void Set(Vector2 newPosition);

    Vector2 screenSpaceTarget {get;}
    Vector3 worldSpaceTarget {get;}
}