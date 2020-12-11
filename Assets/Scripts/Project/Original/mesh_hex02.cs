////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HMLFramwork.Singleton;

public class mesh_hex02 : SingleInstance<mesh_hex02>
{
    public List<float[,]> BZ_to_FEM(List<float[,]> data_BZ, int n1, int n2, Vector3 v1, Vector3 v2)
    {
        if (n1 != n2)
        {
            throw new System.ArgumentException("n1 != n2", "n1, n2");
        }
        int n_band = data_BZ.Count;
        int n_FEM = 3 * n1 * n2;  //each of triangle A and triangle B has n1*n2 small triangles 
        List<float[,]> data_FEM = new List<float[,]>();
        var tuple = make_FEM_hex02(n1);
        int[] FEM_index_listA = tuple.Item1;
        int[] FEM_index_listB = tuple.Item2;
        for (int nn = 0; nn < n_band; nn++)
        {
            float[,] data_BZ_n = data_BZ[nn];
            float[,] data_FEM_nA = new float[n_FEM, 3];
            float[,] data_FEM_nB = new float[n_FEM, 3];
            for (int i = 0; i < n_FEM; i++)
            {
                data_FEM_nA[i, 0] = data_BZ_n[FEM_index_listA[i], 0];
                data_FEM_nA[i, 1] = data_BZ_n[FEM_index_listA[i], 1];
                data_FEM_nA[i, 2] = data_BZ_n[FEM_index_listA[i], 2];
                data_FEM_nB[i, 0] = data_BZ_n[FEM_index_listB[i], 0];
                data_FEM_nB[i, 1] = data_BZ_n[FEM_index_listB[i], 1];
                data_FEM_nB[i, 2] = data_BZ_n[FEM_index_listB[i], 2];
            }
            float[,] data_FEM_n_ext = extendBZ(data_FEM_nA, data_FEM_nB, v1, v2);
            data_FEM.Add(data_FEM_n_ext);
        }
        return data_FEM;
    }


    float[,] extendBZ(float[,] data_FEM_A, float[,] data_FEM_B, Vector3 v1, Vector3 v2)
    {
        int n_BZ = 3;
        int n_FEM = data_FEM_A.GetLength(0);
        float[,] data_FEM_new = new float[2 * n_BZ * n_FEM, 3];
        int[][] displacement_list = new int[][] {
            new int[] { 0, 0 },
            new int[] { 0, 0 },
            new int[] { -1, 0 },
            new int[] { 0, -1 },
            new int[] { -1, -1 },
            new int[] { -1, -1 }};
        int Mod(int a, int b) { return (a % b + b) % b; }
        for (int i = 0; i < displacement_list.Length; i++)
        {
            int i1 = displacement_list[i][0];
            int i2 = displacement_list[i][1];
            Vector3 delta = i1 * v1 + i2 * v2;
            if (Mod(i, 2) == 0)
            {
                for (int k = 0; k < n_FEM; k++)
                {
                    data_FEM_new[i * n_FEM + k, 0] = data_FEM_A[k, 0] + delta.x;
                    data_FEM_new[i * n_FEM + k, 1] = data_FEM_A[k, 1] + delta.y;
                    data_FEM_new[i * n_FEM + k, 2] = data_FEM_A[k, 2];
                }
            }
            else
            {
                for (int k = 0; k < n_FEM; k++)
                {
                    data_FEM_new[i * n_FEM + k, 0] = data_FEM_B[k, 0] + delta.x;
                    data_FEM_new[i * n_FEM + k, 1] = data_FEM_B[k, 1] + delta.y;
                    data_FEM_new[i * n_FEM + k, 2] = data_FEM_B[k, 2];
                }
            }
        }
        return data_FEM_new;
    }


    //comment: generate triangular meshes for upper and lower hexagonal unitcell
    //example: n = 2
    // 7 - 8 - 9
    //  \ / \ / \
    //   4 - 5 - 6
    //    \ / \ / \
    //     1 - 2 - 3
    //index_listA: [1,2,5, 2,3,6, 5,6,2, 5,6,9]
    //index_listB: [9,8,5, 9,7,4, 5,4,8, 5,4,1]
    Tuple<int[], int[]> make_FEM_hex02(int n)
    {
        int[] index_list = new int[3 * n * n];
        int[] index_listA = new int[3 * n * n];
        int[] index_listB = new int[3 * n * n];
        int counter = -1;
        for (int ii = 1; ii <= n; ii++)
        {
            int ind1 = 1 + (ii - 1) * (n + 2);
            int ind2 = 1 + ii * (n + 2);
            for (int jj = 1; jj <= n - ii + 1; jj++)
            {
                index_list[counter + 3 * (jj - 1) + 1] = ind1 + (jj - 1);
                index_list[counter + 3 * (jj - 1) + 2] = ind1 + (jj - 1) + 1;
                index_list[counter + 3 * (jj - 1) + 3] = ind2 + (jj - 1);
            }
            counter = counter + 3 * (n - ii + 1);
            for (int jj = 1; jj <= n - ii; jj++)
            {
                index_list[counter + 3 * (jj - 1) + 1] = ind2 + (jj - 1);
                index_list[counter + 3 * (jj - 1) + 2] = ind2 + (jj - 1) + 1;
                index_list[counter + 3 * (jj - 1) + 3] = ind1 + (jj - 1) + 1;
            }
            counter = counter + 3 * (n - ii);
        }
        for (int i = 0; i < 3 * n * n; i++)
        {
            index_listA[i] = index_list[i] - 1;
            index_listB[i] = 1 + (n + 1) * (n + 1) - index_list[i] - 1;
        }
        return new Tuple<int[], int[]>(index_listA, index_listB);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //function[index_listA, index_listB] = make_FEM_hex02(n)
    //%comment: generate triangular meshes for upper and lower hexagonal unitcell
    //%example: n = 2
    //%7 - 8 - 9
    //% \ / \ / \
    //%  4 - 5 - 6
    //%   \ / \ / \
    //%    1 - 2 - 3
    //%index_listA: [1,2,5, 2,3,6, 5,6,2, 5,6,9]
    //%index_listB: [9,8,5, 9,7,4, 5,4,8, 5,4,1]
    //index_list = zeros(1, 3*n^2);
    //counter = 0;
    //for ii = 1:n
    //    ind1 = 1 + (ii - 1) * (n + 2);
    //    ind2 = 1 + ii* (n+2);
    //    indices1 = ind1 : ind1 + n-ii + 1;
    //    indices2 = ind2 : ind2 + n-ii;
    //    for jj = 1:n-ii+1
    //        index_list(counter+3*(jj-1)+1) = indices1(jj);
    //        index_list(counter+3*(jj-1)+2) = indices1(jj+1);
    //        index_list(counter+3*(jj-1)+3) = indices2(jj);
    //    end %jj
    //    counter = counter + 3 * (n - ii + 1);
    //    for jj = 1:n-ii
    //        index_list(counter+3*(jj-1)+1) = indices2(jj);
    //        index_list(counter+3*(jj-1)+2) = indices2(jj+1);
    //        index_list(counter+3*(jj-1)+3) = indices1(jj+1);
    //    end %jj
    //    counter = counter + 3 * (n - ii);
    //end %ii
    //index_listA = index_list;
    //index_listB = 1 + (n+1)^2 - index_list;
    //end %make_FEM_hex02
}
