using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotBase : MonoBehaviour
{

    protected Color color = Color.blue;
    protected Vector3[] xyz_list;

    // private variables
    protected Mesh mesh;
    protected Vector3[] vertices;
    protected int[] triangles;

    protected MeshFilter meshFilter;
    protected MeshCollider meshCollider;
    protected MeshRenderer meshRenderer;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public virtual void Plot(Vector3[] _xyz_list, Color? _color = null)
    {
        if (_xyz_list != null && _xyz_list.Length > 0)
        {
            //设置数据
            xyz_list = _xyz_list;
            if (_color != null) color = (Color)_color;
            //应用数据
            plot_data();
        }
    }

    protected virtual void plot_data()
    {
        Loom.RunAsync(() =>
        {
            CreateShape();
            Loom.QueueOnMainThread(() =>
            {
                StartCoroutine(UpdateMesh());
            });
        });
    }

    // update mesh
    protected virtual IEnumerator UpdateMesh()
    {
        yield return null;
    }
    protected virtual void CreateShape()
    {
        // transform from normal xyz to unity xyz
        int N = xyz_list.GetLength(0);
        vertices = new Vector3[N];
        triangles = new int[N];
        for (int i = 0; i < N; i++)
        {
            var xyz = xyz_list[i];
            vertices[i] = new Vector3(xyz.x, xyz.z, xyz.y);
            triangles[i] = i;
        }
    }

    public virtual void Reset()
    {
        if (mesh != null)
        {
            Destroy(mesh);
        }
        xyz_list = null;
        vertices = null;
        triangles = null;

    }
}
