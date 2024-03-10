using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerTool : MonoBehaviour
{
    public InteractionManager playerInteraction;
    public Action tool;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tool != null)
        {
            playerInteraction.AddAction(tool);
            tool = null;
            Destroy(gameObject);
        }
    }
}
