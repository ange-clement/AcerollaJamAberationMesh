using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;

public class MakeMesh : MonoBehaviour
{
    public float cooldown = 0f;
    public int subSample = 1;
    public int knn = 3;
    private PointCloud pointCloud;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    class TransformObj : IComparable
    {
        public Transform obj;

        public TransformObj(Transform t)
        {
            obj = t;
        }

        public int CompareTo(object other)
        {
            // If other is not a valid object reference, or a different object, this instance is greater.
            if (other == null || other is not TransformObj) return 1;

            return obj.GetInstanceID().CompareTo(((TransformObj)other).obj.GetInstanceID());
        }
    }

    private SortedSet<TransformObj> interactionAreas;

    private AnimatePointCloud pointsAnimation = null;

    private float cooldownLeft = 0f;

    private bool grabbed;
    public bool Grabbed
    {
        get => grabbed; set
        {
            grabbed = value;
            if (value)
            {
                Vector3[] vertices = new Vector3[0];
                int[] triangles = new int[0];
                SetMesh(vertices, triangles);
            }
        }
    }

    void Awake()
    {
        pointCloud = GetComponent<PointCloud>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        enabled = false;

        TryGetComponent(out pointsAnimation);

        interactionAreas = new SortedSet<TransformObj>();
    }

    public void AddArea(Transform interationArea)
    {
        interactionAreas.Add(new TransformObj(interationArea));
        enabled = true;
    }

    public void RemoveArea(Transform interationArea)
    {
        interactionAreas.Remove(new TransformObj(interationArea));
        if (interactionAreas.Count == 0)
        {
            enabled = false;
        }
    }

    public void MoveTo(Vector3 position, float amount)
    {
        transform.position = amount * position + (1f - amount) * transform.position;
        if (pointsAnimation != null)
        {
            pointsAnimation.enabled = false;
        }
        ParticleSystem.Particle[] particles = pointCloud.particles;
        if (particles == null || particles.Length < pointCloud.pointCloudParticleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[pointCloud.pointCloudParticleSystem.main.maxParticles];
        int particleAmount = pointCloud.pointCloudParticleSystem.GetParticles(particles);
        for (int i = 0; i < particleAmount; i++)
        {
            Vector3 newPoint = amount * position + (1f - amount) * pointCloud.points[i];
            pointCloud.points[i] = newPoint;
            particles[i].position = newPoint;
        }
        pointCloud.pointCloudParticleSystem.SetParticles(particles, particleAmount);
    }
    public void EndMoveTo()
    {
        if (pointsAnimation == null)
        {
            pointsAnimation = gameObject.AddComponent<AnimatePointCloud>();
        }
        pointsAnimation.Initialize();
        pointsAnimation.loopTimeOverride = .5f;
        pointsAnimation.enabled = true;
    }

    private void OnEnable()
    {
        Vector3[] vertices = new Vector3[0];
        int[] triangles = new int[0];
        SetMesh(vertices, triangles);
        meshRenderer.enabled = true;
        meshCollider.enabled = true;
    }

    private void OnDisable()
    {
        meshRenderer.enabled = false;
        meshCollider.enabled = false;
    }

    public void SetMesh(Vector3[] vertices, int[] triangles)
    {
        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        meshFilter.sharedMesh = m;
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    private void Update()
    {
        cooldownLeft -= Time.deltaTime;
        if (cooldownLeft < 0 && pointCloud.ready && !Grabbed)
        {
            cooldownLeft = cooldown;

            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();
            foreach (TransformObj area in interactionAreas)
            {
                Vector3 startPos = area.obj.TransformPoint(new Vector3(-.5f, -.5f, -.5f));
                Vector3 right = area.obj.TransformVector(Vector3.right) / subSample;
                Vector3 up = area.obj.TransformVector(Vector3.up) / subSample;
                Vector3 forward = area.obj.TransformVector(Vector3.forward) / subSample;

                (List<Vector3> curVertices, List<int> curTriangles) = BigMaths.dualContour(startPos, right, up, forward, subSample, knn, pointCloud.points, pointCloud.normals);

                verticesList.AddRange(curVertices);
                trianglesList.AddRange(curTriangles);
            }

            Vector3[] vertices = verticesList.ToArray();
            int[] triangles = trianglesList.ToArray();

            transform.InverseTransformPoints(vertices);

            SetMesh(vertices, triangles);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractionArea"))
        {
            AddArea(other.transform);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InteractionArea"))
        {
            RemoveArea(other.transform);
        }
    }
}
