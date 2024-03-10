using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour
{
    public int subSample;
    public float doNotChooseProba = -1f;
    public GameObject visu;
    public Transform visuParent;
    public ParticleSystem pointCloudParticleSystem;
    private MeshFilter meshFilter;

    [HideInInspector] public Vector3[] points;
    [HideInInspector] public Vector3[] normals;
    public bool ready = false;

    public ParticleSystem.Particle[] particles;

    // Start is called before the first frame update
    void Start()
    {
        ready = false;
        meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = meshFilter.sharedMesh;

        (points, normals) = BigMaths.makePointCloudFromMesh(mesh, subSample, doNotChooseProba);

        transform.TransformPoints(points);
        transform.TransformDirections(normals);

        pointCloudParticleSystem.Emit(points.Length);
        if (particles == null || particles.Length < pointCloudParticleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[pointCloudParticleSystem.main.maxParticles];
        int currentAmount = pointCloudParticleSystem.GetParticles(particles);
        for (int i = 0; i < currentAmount && i < points.Length; i++)
        {
            particles[i].position = points[i];
        }
        pointCloudParticleSystem.SetParticles(particles, currentAmount);


        //if (instantiateVisu)
        //{
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        Vector3 point = points[i];
        //        Vector3 norm = normals[i];
        //        GameObject v = Instantiate(visu, point, visu.transform.rotation, visuParent);
        //        v.transform.LookAt(point + norm);
        //    }
        //}

        ready = true;
    }
}
