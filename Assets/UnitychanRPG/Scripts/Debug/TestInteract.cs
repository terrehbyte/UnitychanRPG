using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteract : MonoBehaviour, IInteractible
{
    public bool CanInteract(object caller)
    {
        return true;
    }

    public void TryInteract(object caller, bool forceSuccess = false)
    {
        if (!CanInteract(caller)) { return; }
        
        Debug.Log((caller as UnityEngine.Object)?.name + " interacted with " + gameObject.name, this);
    }
}
