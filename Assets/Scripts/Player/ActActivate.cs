using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActActivate : Action
{
    public float speed = 5f;
    public Vector3 direction;

    private MeshRenderer meshRenderer;
    private Collider boxCollider;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
    }
    private void OnDisable()
    {

    }

    public override void OnReset()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
