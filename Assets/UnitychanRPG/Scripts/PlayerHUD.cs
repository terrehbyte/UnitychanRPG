using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Create interface for getting interaction icon
// - Accessible
// - Inaccessible
// - Other states? (or vary it based on object? i.e. locked door vs unlocked door)

public class PlayerHUD : MonoBehaviour
{
    [SerializeField]
    private Image interactIcon;

    [SerializeField]
    private PlayerController2D controller;

    public void SetInteractTarget(Transform target)
    {
        interactIcon.gameObject.SetActive(target != null);

        if(target == null) { return; }

        var sprite = target.GetComponent<SpriteRenderer>();
        Vector3 offset = sprite.bounds.center + Vector3.up * sprite.bounds.extents.y;
        interactIcon.transform.position = controller.cam.WorldToScreenPoint(offset);
    }
}