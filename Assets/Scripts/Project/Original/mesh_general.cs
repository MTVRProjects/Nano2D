////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using HMLFramwork.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mesh_general : SingleInstance<mesh_general>
{
    public List<float[,]> BZ_to_FEM(List<float[,]> data_BZ, int n1, int n2, Vector3 v1, Vector3 v2)
    {
        int n_band = data_BZ.Count;
        int n_FEM = 6 * n1 * n2;  //each parallelogram is composed of 2 triangles which have 6 vertices 
        List<float[,]> data_FEM = new List<float[,]>();
        int[] FEM_index_list = make_FEM_general(n1, n2);
        for (int nn = 0; nn < n_band; nn++)
        {
            float[,] data_BZ_n = data_BZ[nn];
            float[,] data_FEM_n = new float[n_FEM, 3];
            for (int i = 0; i < n_FEM; i++)
            {
                data_FEM_n[i, 0] = data_BZ_n[FEM_index_list[i], 0];
                data_FEM_n[i, 1] = data_BZ_n[FEM_index_list[i], 1];
                data_FEM_n[i, 2] = data_BZ_n[FEM_index_list[i], 2];
            }
            float[,] data_FEM_n_ext = extendBZ(data_FEM_n, v1, v2);
            data_FEM.Add(data_FEM_n_ext);
        }
        return data_FEM;
    }


    float[,] extendBZ(float[,] data_FEM, Vector3 v1, Vector3 v2)
    {
        int n_BZ = 4;
        int n_FEM = data_FEM.GetLength(0);
        float[,] data_FEM_new = new float[n_BZ * n_FEM, 3];
        int[][] displacement_list = new int[][] {
            new int[] { 0, 0 },
            new int[] { 0, -1 },
            new int[] { -1, 0 },
            new int[] { -1, -1 } };
        for (int i = 0; i < n_BZ; i++)
        {
            int i1 = displacement_list[i][0];
            int i2 = displacement_list[i][1];
            Vector3 delta = i1 * v1 + i2 * v2;
            for (int k = 0; k < n_FEM; k++)
            {
                data_FEM_new[i * n_FEM + k, 0] = data_FEM[k, 0] + delta.x;
                data_FEM_new[i * n_FEM + k, 1] = data_FEM[k, 1] + delta.y;
                data_FEM_new[i * n_FEM + k, 2] = data_FEM[k, 2];
            }
        }
        return data_FEM_new;
    }


    //comment: generate triangular mesh for uniform grid
    //example: n1 = 2, n2 = 1
    //         grid: 4-5-6
    //               |\|\|
    //               1-2-3
    //         index: [1,4,2, 2,4,5, 2,5,3, 3,5,6]
    int[] make_FEM_general(int n1, int n2)
    {
        int[] index_list;
        index_list = new int[6 * n1 * n2];
        int vert = 0;
        int tris = 0;
        for (int i2 = 0; i2 < n2; i2++)
        {
            for (int i1 = 0; i1 < n1; i1++)
            {
                index_list[tris + 0] = vert + 0;          //triangle - 1
                index_list[tris + 1] = vert + n1 + 1;     //triangle - 1
                index_list[tris + 2] = vert + 1;          //triangle - 1
                index_list[tris + 3] = vert + 1;          //triangle - 2
                index_list[tris + 4] = vert + n1 + 1;     //triangle - 2
                index_list[tris + 5] = vert + n1 + 2;     //triangle - 2
                vert = vert + 1;
                tris = tris + 6;
            }
            vert = vert + 1;
        }
        return index_list;
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////

//displacement_list[0] = new int[] { 0, 0 };
//displacement_list[1] = new int[] { 0, -1 };
//displacement_list[2] = new int[] { -1, 0 };
//displacement_list[3] = new int[] { -1, -1 };
//float[,] data_FEM_n_ext = new float[n_FEM_ext, 3];
//int n_BZ = 4;
//int n_BZ = 1;

//int n_FEM_ext = n_BZ * n_FEM;

//Debug.Log("i = " + i + "  " + FEM_index_list[i]);
//Debug.Log(FEM_index_list[i] + "  " + data_BZ_n.GetLength(0) + "  " + data_BZ_n.GetLength(1) + "  " + i + "  " + FEM_index_list.GetLength(0));
//data_FEM = data_BZ;

//FEM_index_list = make_FEM_general(2, 3);
//for (int i=0; i<FEM_index_list.GetLength(0); i++)
//{
//    Debug.Log("index = " + FEM_index_list[i]);
//}