using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveVelocity : MonoBehaviour
{
    public Vector3 velocity;
    public float cooldown = 1.0f;

    private float timeLeft = 0.0f;


    private void Update()
    {
        timeLeft -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timeLeft < 0 && other.CompareTag("Player"))
        {
            timeLeft = cooldown;
            other.GetComponent<PlayerController>().playerVelocity += other.transform.TransformDirection(velocity);
        }
    }
}
