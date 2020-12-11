////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class plot_surface : PlotBase
{

    // update mesh
    protected override IEnumerator UpdateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        mesh.Clear();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;  
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.material.color = color;

        yield return null;

    }

}

