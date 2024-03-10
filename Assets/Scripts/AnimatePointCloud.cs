using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePointCloud : MonoBehaviour
{
    public float loopTime = 5f;
    public float randomAmount = 0f;
    public float loopTimeOverride = -1f;
    public bool useLooptimeOverride = false;
    private PointCloud pointCloud;
    private MeshFilter meshFilter;

    private Vector3[] startPoints;
    private Vector3[] startNormals;
    private Vector3[] endPoints;
    private Vector3[] endNormals;

    private float timeLeft;

    private bool initialized;
    private Mesh originalMesh;

    private void Start()
    {
        pointCloud = GetComponent<PointCloud>();
        meshFilter = GetComponent<MeshFilter>();

        originalMesh = meshFilter.mesh;

        initialized = false;
    }

    private void initNext()
    {
        startPoints = endPoints;
        startNormals = endNormals;

        (endPoints, endNormals) = BigMaths.makePointCloudFromMesh(originalMesh, pointCloud.subSample, pointCloud.doNotChooseProba);

        transform.TransformPoints(endPoints);
        transform.TransformDirections(endNormals);

        for (int i = 0; i < endPoints.Length; i++)
        {
            endPoints[i] += new Vector3((Random.value - .5f) * randomAmount, (Random.value - .5f) * randomAmount, (Random.value - .5f) * randomAmount);
        }

        if (useLooptimeOverride)
        {
            loopTimeOverride = -1f;
            useLooptimeOverride = false;
        }

        if (loopTimeOverride > 0)
        {
            timeLeft = loopTimeOverride;
            useLooptimeOverride = true;
        }
        else
        {
            timeLeft = loopTime;
        }
    }

    public void Initialize()
    {
        initialized = false;
        timeLeft = 0;
    }

    void Update()
    {
        if (!initialized)
        {
            if (pointCloud.ready)
            {
                endPoints = new Vector3[pointCloud.points.Length];
                endNormals = new Vector3[pointCloud.normals.Length];
                pointCloud.points.CopyTo(endPoints, 0);
                pointCloud.normals.CopyTo(endNormals, 0);
                initNext();
                initialized = true;
            }
        }
        else
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft > 0)
            {
                ParticleSystem.Particle[] particles = pointCloud.particles;
                if (particles == null || particles.Length < pointCloud.pointCloudParticleSystem.main.maxParticles)
                    particles = new ParticleSystem.Particle[pointCloud.pointCloudParticleSystem.main.maxParticles];
                int particleAmount = pointCloud.pointCloudParticleSystem.GetParticles(particles);
                int currentAmount = Mathf.Min(particleAmount, Mathf.Min(startPoints.Length, endPoints.Length));
                float loopTimeActive = (useLooptimeOverride) ? loopTimeOverride : loopTime;
                float timeNorm = BigMaths.easeInOutSine(timeLeft / loopTimeActive);
                for (int i = 0; i < currentAmount; i++)
                {
                    Vector3 newPoint =  timeNorm * startPoints[i] + (1f - timeNorm) * endPoints[i];
                    pointCloud.points[i] = newPoint;
                    pointCloud.normals[i] = timeNorm * startNormals[i] + (1f - timeNorm) * endNormals[i];
                    particles[i].position = newPoint;
                }
                pointCloud.pointCloudParticleSystem.SetParticles(particles, currentAmount);
            }
            else
            {
                initNext();
            }
        }

    }
}
