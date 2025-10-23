using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected Outline outline;

    protected virtual void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public virtual void OnHoverEnter()
    {
        if (outline != null)
            outline.EnableOutline();
    }

    public virtual void OnHoverExit()
    {
        if (outline != null)
            outline.DisableOutline();
    }

    public abstract void Interact();
}