using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericInteractible : MonoBehaviour, IInteractible
{
    public UnityEventObject OnInteract;
    
    [SerializeField]
    private bool _interactible;
    public bool interactible { get { return _interactible; } set { _interactible = value; }}

    public bool CanInteract(object caller)
    {
        return interactible;
    }

    public void TryInteract(object caller, bool forceSuccess = false)
    {
        bool success = CanInteract(caller) || forceSuccess;

        if(!success) { return; }

        OnInteract.Invoke(caller);
    }
}