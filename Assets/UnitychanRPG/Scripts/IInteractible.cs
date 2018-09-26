using UnityEngine;

public interface IInteractible
{
    bool CanInteract(System.Object caller);
    void TryInteract(System.Object caller, bool forceSuccess = false);
}