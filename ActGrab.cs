using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActGrab : Action
{
    private InteractionObject attachedObject;
    private void OnEnable()
    {
        attachedObject.enabled = false;
    }
    private void OnDisable()
    {

    }
}
