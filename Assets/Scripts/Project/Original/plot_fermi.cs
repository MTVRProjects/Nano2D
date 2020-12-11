////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// plot fermi surface
/// xyz_list[3*i+0] = (x1,y1,z1)  the 1st vertex of triangle-i
/// xyz_list[3*i+1] = (x2,y2,z2)  the 2nd vertex of triangle-i
/// xyz_list[3*i+2] = (x3,y3,z3)  the 3rd vertex of triangle-i
/// </summary> 

public class plot_fermi : PlotBase
{

    // update mesh
    protected override IEnumerator UpdateMesh()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;  // fix mesh 65535 vertices limit
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material.color = Color.white;
        //meshRenderer.material.shader = Shader.Find("shaders/Standard/Diffuse Bump");  // double surface

        yield return null;
    }

}