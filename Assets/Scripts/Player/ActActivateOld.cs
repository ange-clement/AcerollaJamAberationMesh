using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActActivateOld : Action
{
    //private SortedSet<InteractionObject> attachedObject;

    //private MeshRenderer meshRenderer;
    //private Collider boxCollider;

    //private void Awake()
    //{
    //    attachedObject = new SortedSet<InteractionObject>();

    //    meshRenderer = GetComponent<MeshRenderer>();
    //    boxCollider = GetComponent<Collider>();
    //}

    //private void OnEnable()
    //{
    //    meshRenderer.enabled = true;
    //    boxCollider.enabled = true;
    //    OnReset();
    //    transform.position = startPos.position;
    //    transform.rotation = startPos.rotation;
    //}
    //private void OnDisable()
    //{
    //    meshRenderer.enabled = true;
    //    boxCollider.enabled = false;
    //}
    //public override void OnReset()
    //{
    //    foreach (InteractionObject o in attachedObject)
    //    {
    //        o.enabled = false;
    //    }
    //    attachedObject.Clear();
    //}

    //public void OnTriggerEnter(Collider other)
    //{
    //    if (enabled && other.CompareTag("InteractionObject"))
    //    {
    //        InteractionObject obj = other.GetComponent<InteractionObject>();
    //        if (obj != null)
    //        {
    //            attachedObject.Add(obj);
    //            obj.Activate(gameObject);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("InteractionObject tag but no InteractionObject on " + other.name);
    //        }
    //    }
    //}
    //public void OnTriggerExit(Collider other)
    //{
    //    if (enabled && other.CompareTag("InteractionObject"))
    //    {
    //        InteractionObject obj = other.GetComponent<InteractionObject>();
    //        if (obj != null)
    //        {
    //            attachedObject.Remove(obj);
    //            obj.enabled = false;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("InteractionObject tag but no InteractionObject on " + other.name);
    //        }
    //    }
    //}
    public override void OnReset()
    {
        throw new System.NotImplementedException();
    }
}
