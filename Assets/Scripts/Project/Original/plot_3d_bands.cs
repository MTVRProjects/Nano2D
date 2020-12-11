////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using SimpleJSON;
using System.IO;
using UnityEngine.UI;
using HMLFramwork;
using HMLFramwork.Singleton;

/// <summary>
/// DataFile: BandData[#band][#kx][#ky]
///           RecicellVector1[3]
///           RecicellVector2[3]
/// <summary>


public class plot_3d_bands : MonoSingleton<plot_3d_bands>
{
    [Header("指定路径数据存在，则勾选")]
    public bool isPersistentData = true;
    // parameters   
    string DataPath;
    public string DataPath_persistent = "/banddata_20200601/";

    public float E_min = -6f;                                  //energy window
    public float E_max = 6f;                                   //energy window
    public float ScalingFactorZ = 0.5f;

    string DatabaseIndex = "";
    string DatabaseIndex_ = "";
    int index_selected = -1;
    string formula_selected = "";
    string magnetism_selected = "";
    string fname;
    GameObject CanvasBand;

    /// <summary>
    /// 3 D能带生成位置对象，也是3D能带的父节点
    /// </summary>
    GameObject plot_band3d;
    /// <summary>
    /// 3 D能带生成位置对象，也是3D能带的父节点
    /// </summary>
    Transform plot_band3d_trans;//2020.7.20

    GameObject plot_fermi;
    GameObject Ruler;


    GameObject surface_prefab;

    GameObject band3d_panel_01;
    GameObject band3d_panel_02;
    GameObject band3d_panel_03;
    Canvas_database canvas00;
    bool isInitialized = false;

    Vector3 plot_band3d_position;
    Quaternion plot_band3d_rotation;
    Vector3 plot_band3d_scale;
    Vector3 ruler_position;
    Vector3 ruler_scale;
    Quaternion ruler_rotation;

    // Awake
    void Awake()
    {
        // data path
        if (isPersistentData)
        {
            DataPath = DataPathHelper.Datas_path + DataPath_persistent;
        }

        // find
        CanvasBand = GameObject.Find("Canvas_band").gameObject;

        plot_band3d = GameObject.Find("plot_band3d").gameObject;
        plot_band3d_trans = plot_band3d.transform;//2020.7.20

        plot_fermi = GameObject.Find("plot_fermi").gameObject;
        Ruler = GameObject.Find("Ruler").gameObject;
        Ruler.SetActive(false);
        canvas00 = GameObject.Find("Canvas_database").GetComponent<Canvas_database>();


        band3d_panel_01 = GameObject.Find("Canvas_band/panel_band3d/show_3d_bands").gameObject;

        band3d_panel_01_bt.clickEventHandle += create3DBands;

        band3d_panel_02 = GameObject.Find("Canvas_band/panel_band3d/FermiLevel").gameObject;
        band3d_panel_03 = GameObject.Find("Canvas_band/panel_band3d/LockBands").gameObject;

        // surface_prefab
        surface_prefab = new GameObject();
        surface_prefab.name = "surface_prefab";
        surface_prefab.AddComponent<MeshFilter>();
        surface_prefab.AddComponent<MeshRenderer>().material.shader = Shader.Find("shaders/Standard/Diffuse Bump"); ;
        surface_prefab.AddComponent<MeshCollider>();
        surface_prefab.AddComponent<Interactable>();
        surface_prefab.AddComponent<plot_surface>();

        //2020.7.21
        surfacePool = new ObjectPool2(surface_prefab, 10, plot_band3d_trans);

        // position and rotation
        plot_band3d_position = plot_band3d_trans.position;
        plot_band3d_rotation = plot_band3d_trans.rotation;
        plot_band3d_scale = plot_band3d_trans.localScale;
        ruler_position = Ruler.transform.position;
        ruler_rotation = Ruler.transform.rotation;
        ruler_scale = Ruler.transform.localScale;

        // sound
        AudioSource source = band3d_panel_01.requireComponent<AudioSource>();
        AudioClip clip = Resources.Load<AudioClip>("AudioClip/sound_ui_hover");
        source.clip = clip;
    }


    // load data

    //////////////////////////////////////////////////////////////////2020.7.20//////////////////////////////////////////////////////////////////////////
    //注意每次要清空链表
    List<float[,]> data_BZ = new List<float[,]>();
    List<Color> band_color_list = new List<Color>();
    int n_band;
    int n1;
    int n2;
    Vector3 v1;
    Vector3 v2;

    /// <summary>
    /// 读取Json数据
    /// </summary>
    void readJsonData()
    {
        data_BZ.Clear();
        band_color_list.Clear();
        // load data
        JSONObject dict = (JSONObject)JSON.Parse(File.ReadAllText(fname));
        JSONArray data_band = (JSONArray)dict["BandData"];                      //BandData[#band][#kx][#ky]
        JSONArray RecicellVector1 = (JSONArray)dict["RecicellVector1"];         //RecicellVector1[3]
        JSONArray RecicellVector2 = (JSONArray)dict["RecicellVector2"];         //RecicellVector2[3]

        // data_band to data_BZ

        n_band = data_band.Count;
        n1 = data_band[0].Count;
        n2 = data_band[0][0].Count;
        v1 = new Vector3(RecicellVector1[0], RecicellVector1[1], RecicellVector1[2]);
        v2 = new Vector3(RecicellVector2[0], RecicellVector2[1], RecicellVector2[2]);
        int Mod(int a, int b) { return (a % b + b) % b; }
        for (int nn = 0; nn < n_band; nn++)
        {
            // convert data format
            float[,] data_BZ_n = new float[(n1 + 1) * (n2 + 1), 3];
            for (int i2 = 0; i2 < n2 + 1; i2++)
            {
                for (int i1 = 0; i1 < n1 + 1; i1++)
                {
                    int ii = i1 + i2 * (n1 + 1);
                    Vector3 K_ii = (float)i1 / n1 * v1 + (float)i2 / n2 * v2;
                    float E_ii = data_band[nn][Mod(i1, n1)][Mod(i2, n2)];
                    data_BZ_n[ii, 0] = K_ii.x;
                    data_BZ_n[ii, 1] = K_ii.y;
                    data_BZ_n[ii, 2] = E_ii;
                }
            }
            // set spin color
            Color band_color = Color.blue;
            if (magnetism_selected != "NM")
            {
                if (nn < n_band / 2)
                {
                    band_color = Color.red;   //spin-up
                }
                else
                {
                    band_color = Color.blue;  //spin-dn
                }
            }
            // energy window screen
            float E_mean = calc_band_mean(data_band, nn);
            if (E_mean > E_min & E_mean < E_max)
            {
                data_BZ.Add(data_BZ_n);
                band_color_list.Add(band_color);
            }
        }
        //if (data_BZ.Count == 0) return;
        if (data_BZ.Count == 0) return;
    }


    List<float[,]> data_FEM;
    Vector3[] xyz_list2;
    int n_point;
    string BZ_mode;
    void processingData()
    {
        if (data_BZ.Count > 0)
        {
            // data_BZ to data_FEM
            // data_BZ:  List[#band]<float[#k,#xyz]>
            // data_FEM: List[#band]<float[#FEM,#xyz]>
            //           each band surface is composed of #FEM triangles
            //           each FEM triangle is defined by 3 vertices
            float err1 = 1e-2f;  //  |v1|/|v2|
            float err2 = 1e-2f;  //  dot(v1/|v1|, v2/|v2|)
            float ratio = v1.magnitude / v2.magnitude;
            float cos12 = Vector3.Dot(v1 / v1.magnitude, v2 / v2.magnitude);

            if (Math.Abs(ratio - 1) < err1 && Math.Abs(cos12 - 0.5) < err2)
            {
                data_FEM = mesh_hex01.Ins.BZ_to_FEM(data_BZ, n1, n2, v1, v2);
                BZ_mode = "hex01";
            }
            else if (Math.Abs(ratio - 1) < err1 && Math.Abs(cos12 + 0.5) < err2)
            {
                data_FEM = mesh_hex02.Ins.BZ_to_FEM(data_BZ, n1, n2, v1, v2);
                BZ_mode = "hex02";
            }
            else
            {
                data_FEM = mesh_general.Ins.BZ_to_FEM(data_BZ, n1, n2, v1, v2);
                BZ_mode = "general";
            }
            //print("BZ_mode = " + BZ_mode);        


            // plot Fermi surface        
            n_point = data_FEM[0].GetLength(0);
            xyz_list2 = new Vector3[n_point];
            for (int i = 0; i < n_point; i++)
            {
                xyz_list2[i] = new Vector3(data_FEM[0][i, 0], data_FEM[0][i, 1], 0);
            }
        }

    }

    /// <summary>
    /// surface对象池
    /// </summary>
    ObjectPool2 surfacePool;
    IEnumerator createObjForDatas()
    {

        plot_fermi scp2 = plot_fermi.GetComponent<plot_fermi>();

      
        scp2.Reset();
        scp2.Plot(xyz_list2);

        //所有出池的对象入池
        surfacePool.PushAll();

        Vector3 v3_temp = Vector3.zero;
        GameObject surface_temp;

        for (int k = 0; k < data_FEM.Count; k++)
        {
            // prepare xyz_list
            var xyz_list = new Vector3[n_point];
            for (int i = 0; i < n_point; i++)
            {
                v3_temp.x = data_FEM[k][i, 0];
                v3_temp.y = data_FEM[k][i, 1];
                v3_temp.z = data_FEM[k][i, 2] * ScalingFactorZ;
                xyz_list[i] = v3_temp;
            }

            surface_temp = surfacePool.Pop();
            surface_temp.name = "band";
            plot_surface scp = surface_temp.requireComponent<plot_surface>();

            scp.Reset();
            scp.Plot(xyz_list, band_color_list[k]);

            yield return null;
        }
        Pointer.Ins.isOpenHandRay = true;

    }


    void load_data()
    {

        //子线程中计算
        Loom.RunAsync(
            () =>
            {
                readJsonData();
                if (data_BZ.Count > 0)
                {
                    processingData();

                    //子线程计算完毕后进入主线程生成对象
                    Loom.QueueOnMainThread(
                   () =>
                   {
                       Pointer.Ins.isOpenHandRay = false;

                       StartCoroutine(createObjForDatas());
                   });
                }

            });
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Update

    ButtonTransitioner _band3d_panel_01_bt;
    ButtonTransitioner band3d_panel_01_bt
    {
        get
        {
            if (_band3d_panel_01_bt == null)
            {
                _band3d_panel_01_bt = band3d_panel_01.GetComponent<ButtonTransitioner>();
            }
            return _band3d_panel_01_bt;
        }
    }

    Toggle _band3d_panel_02_tg;
    Toggle band3d_panel_02_tg
    {
        get
        {
            if (_band3d_panel_02_tg == null)
            {
                _band3d_panel_02_tg = band3d_panel_02.GetComponent<Toggle>();
            }
            return _band3d_panel_02_tg;
        }
    }

    Toggle _band3d_panel_03_tg;
    Toggle band3d_panel_03_tg
    {
        get
        {
            if (_band3d_panel_03_tg == null)
            {
                _band3d_panel_03_tg = band3d_panel_03.GetComponent<Toggle>();
            }
            return _band3d_panel_03_tg;
        }
    }

    public void checkBandDataExit()
    {
        // check if updated
        index_selected = canvas00.index_selected;
        formula_selected = canvas00.formula_selected;
        magnetism_selected = canvas00.magnetism_selected;
        if (index_selected < 0) return;
        DatabaseIndex = index_selected.ToString("D4");
        if (DatabaseIndex != DatabaseIndex_)
        {
            //disable band3d and fermi
            plot_band3d.SetActive(false);
            plot_fermi.SetActive(false);
            Ruler.SetActive(false);
            isInitialized = false;
            DatabaseIndex_ = DatabaseIndex;
            //disable band3d panel if data not available
            fname = DataPath + "band" + DatabaseIndex + ".json";
            bool isExist = File.Exists(fname);
            band3d_panel_01.SetActive(isExist);
            band3d_panel_02.SetActive(isExist);
            band3d_panel_03.SetActive(isExist);
        }
    }

    /// <summary>
    /// 绑定在show_3d_bands组件《ButtonTransitioner》的clickEventHandle上
    /// band3d_panel_01
    /// </summary>
    void create3DBands()
    {
        // load or reset
        bool tf0 = band3d_panel_01_bt.isSelect;
        if (tf0)
        {
            if (!isInitialized)  // load data
            {
                isInitialized = true;
                plot_band3d.SetActive(true);
                plot_fermi.SetActive(true);
                Ruler.SetActive(true);

                //所有出池的对象入池
                surfacePool.PushAll();

                load_data();
            }
            else // reset
            {
                plot_band3d_trans.position = plot_band3d_position;
                plot_band3d_trans.rotation = plot_band3d_rotation;
                plot_band3d_trans.localScale = plot_band3d_scale;

                for (int i = 0; i < plot_band3d_trans.childCount; i++)
                {
                    Transform child_temp = plot_band3d_trans.GetChild(i);
                    child_temp.position = plot_band3d_position;
                    child_temp.rotation = plot_band3d_rotation;
                    child_temp.localScale = Vector3.one;
                }


                Ruler.transform.position = ruler_position;
                Ruler.transform.rotation = ruler_rotation;
                Ruler.transform.localScale = ruler_scale;
            }
        }

        // Fermi level on/off
        bool tf1 = band3d_panel_02_tg.isOn;
        // lock bands
        bool tf2 = band3d_panel_03_tg.isOn;
        if (isInitialized)
        {
            plot_fermi.SetActive(tf1);
            /* 2020.7.20
            foreach (Transform band in plot_band3d_trans)
            {
                band.GetComponent<Interactable>().isContained = tf2;
            }
            */
            for (int i = 0; i < plot_band3d_trans.childCount; i++)
            {
                plot_band3d_trans.GetChild(i).GetComponent<Interactable>().isContained = tf2;
            }


        }
    }

    // calc_band_mean

    float calc_band_mean(JSONArray data_band, int nn)
    {
        float E_mean = 0;
        int n1 = data_band[0].Count;
        int n2 = data_band[0][0].Count;
        for (int i2 = 0; i2 < n2; i2++)
        {
            for (int i1 = 0; i1 < n1; i1++)
            {
                E_mean = E_mean + data_band[nn][i1][i2];
            }
        }
        E_mean = E_mean / (n1 * n2);
        return E_mean;
    }

}
