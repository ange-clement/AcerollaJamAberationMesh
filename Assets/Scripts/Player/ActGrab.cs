using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActGrab : Action
{
    public float moveAmount = .3f;
    public Vector3 posOffset;
    private MakeMesh attachedObject;

    private MeshRenderer meshRenderer;
    private Collider boxCollider;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        //meshRenderer.enabled = false;
        //boxCollider.enabled = false;

        RaycastHit hit;
        if (Physics.Raycast(startPos.position, startPos.forward, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag("InteractionObject"))
            {
                //if (attachedObject != null)
                //{
                //    attachedObject.enabled = false;
                //}
                attachedObject = hit.collider.GetComponent<MakeMesh>();
                attachedObject.Grabbed = true;
                //attachedObject.enabled = false;
            }
            else
            {
                attachedObject = null;
            }
        }
        else
        {
            attachedObject = null;
        }
    }
    private void OnDisable()
    {
        if (attachedObject != null)
        {
            attachedObject.EndMoveTo();
            attachedObject.Grabbed = false;
        }
    }

    public override void OnReset()
    {
        
    }

    private void Update()
    {
        if (attachedObject != null)
        {
            Vector3 targetPos = startPos.position + startPos.TransformDirection(posOffset);
            Vector3 pos = attachedObject.transform.position + moveAmount * (startPos.position - attachedObject.transform.position);
            attachedObject.MoveTo(targetPos, moveAmount);
        }
    }
}
