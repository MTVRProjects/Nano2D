using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using System.Linq;
using System;
using HMLFramwork;
using HMLFramwork.Singleton;

public class Canvas_phonon : MonoSingleton<Canvas_phonon>
{
    // Parameters
    [Header("指定路径数据存在，则勾选")]
    public bool isPersistentData = true;
    string DataPath;
    public string DataPath_persistent = "/database04_MaterialsCloud/phonons_data/";

    public string DatabaseIndex = "";
    public int I_k_selected = -1;
    public int I_b_selected = -1;
    [HideInInspector]public GameObject dot_selected = null;
    public Vector2 center = new Vector2(0, 2.2f);
    public Vector2 length = new Vector2(2.8f, 1.4f);
    public LineRenderer line;
    public Vector3[][] line_position_list;

    float alfa = 0.1f;    //plot margin ratio
    float gamma = 0.02f;  //y limit ratio
    float z0 = -0.001f;   //line offset
    float eps = 0.01f;     //line resolution

    GameObject cursor;
    GameObject[] dot_list;

    RawImage m_image;
    string DatabaseIndex_ = "";
    GameObject dot_template;
    GameObject ButtonPanel;
    Canvas_database canvas00;
    Vector3 Canvas_phonon_position;
    Quaternion Canvas_phonon_rotation;

    Vector3 dashboard_x_axis;
    Vector3 dashboard_y_axis;
    Vector3 dashboard_z_axis;
    Vector3 dashboard_center;
    float dashboard_Lx;
    float dashboard_Ly;
    float dashboard_Lz;

    AudioSource source;
    // Start
    void Start()
    {
        // data path
        if (isPersistentData)
        {
            DataPath = DataPathHelper.Datas_path + DataPath_persistent;
        }

        // Find
        m_image = transform.Find("dashboard").GetComponent<RawImage>();
        canvas00 = GameObject.Find("Canvas_database").GetComponent<Canvas_database>();
        dot_template = transform.Find("dot_template").gameObject;
        ButtonPanel = transform.Find("ButtonPanel").gameObject;

        // Image
        RectTransform rt = m_image.GetComponent<RectTransform>();
        rt.transform.localPosition = center;
        rt.transform.localScale = length;

        // line
        line = transform.Find("dashboard").GetComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.material = Resources.Load<Material>("Materials/color_blue");
        line.positionCount = 0;

        // sound
        source = gameObject.requireComponent<AudioSource>();
        AudioClip clip = Resources.Load<AudioClip>("AudioClip/sound_ui_hover");
        source.clip = clip;

        // Canvas_phonon position and rotation
        Canvas_phonon_position = transform.position;
        Canvas_phonon_rotation = transform.rotation;

        // cursor
        cursor = GameObject.Find("XRplayer/OVRCameraRig/TrackingSpace/RightHandAnchor/Pointer/dot").gameObject;
        // dashboard
        dashboard_x_axis = Canvas_phonon_rotation * Vector3.right;
        dashboard_y_axis = Canvas_phonon_rotation * Vector3.up;
        dashboard_z_axis = Canvas_phonon_rotation * Vector3.forward;
        dashboard_center = Canvas_phonon_position + new Vector3(center.x, center.y, 0);
        dashboard_Lx = length.x * (1 - 2 * alfa);
        dashboard_Ly = length.y * (1 - 2 * alfa);
        dashboard_Lz = 0.1f;


        dot_template_pool = new ObjectPool2(dot_template, 30, ButtonPanel.transform);

        EventCenter.Ins.Add("Canvas_phonon", "PlayAudio", playAudio);

    }


    // Update
    void Update()
    {
        int index_selected = canvas00.index_selected;
        if (index_selected < 0) return;
        DatabaseIndex = index_selected.ToString("D4");
        if (DatabaseIndex == DatabaseIndex_)
        {
            if (ButtonPanel.activeSelf) find_nearest_dot();
        }
        else
        {
            DatabaseIndex_ = DatabaseIndex;
            initialization();
        }
    }


    ObjectPool2 dot_template_pool;

    float[] EigenvalueX;
    float[,] EigenvalueY;
    int N_k, N_b;
    float x_min, x_max, y_min, y_max;
    //initialization
    void initialization()
    {
        //2020.7.24

        dot_template_pool.PushAll();
        // import image
        string fname = "phonons_png/phonons" + DatabaseIndex;
        bool tf = importImage(fname);
        I_k_selected = -1;
        I_b_selected = -1;
        if (!tf)
        {
            //foreach (Transform childObj in ButtonPanel.transform)
            //{
            //    Destroy(childObj.gameObject);
            //}
            return;
        }

        // Button Panel
        ButtonPanel.SetActive(false);

        Loom.RunAsync(() =>
        {
            ////////////////////////////////////////////////////////
            // import data
            string fname2 = DataPath + "phonons" + DatabaseIndex + ".json";
            string text = File.ReadAllText(fname2);

            JSONObject dict = (JSONObject)JSON.Parse(text);
            JSONArray EigenvalueX_ = (JSONArray)dict["EigenvalueX"];
            JSONArray EigenvalueY_ = (JSONArray)dict["EigenvalueY"];

            EigenvalueX = JSONArray_to_Array_1d(EigenvalueX_);
            EigenvalueY = JSONArray_to_Array_2d(EigenvalueY_);

            // calc xlim and ylim
            x_min = EigenvalueX.Cast<float>().Min();
            x_max = EigenvalueX.Cast<float>().Max();
            float y1 = EigenvalueY.Cast<float>().Min();
            float y2 = EigenvalueY.Cast<float>().Max();
            y_min = y1 - (y2 - y1) * gamma;
            y_max = y2 + (y2 - y1) * gamma;

            N_k = EigenvalueY.GetLength(0);
            N_b = EigenvalueY.GetLength(1);
            float delta_y = (y_max - y_min) * eps;


            // line position list
            line_position_list = new Vector3[N_b][];
            for (int j = 0; j < N_b; j++)
            {
                Vector3[] position_list = new Vector3[N_k];
                for (int i = 0; i < N_k; i++)
                {
                    float x = EigenvalueX[i];
                    float y = EigenvalueY[i, j];
                    Vector3 position = calc_position(x, y, alfa, x_min, x_max, y_min, y_max, center.x, center.y, length.x, length.y);
                    position.z = z0;
                    position = Canvas_phonon_rotation * position + Canvas_phonon_position;
                    position_list[i] = position;
                }
                line_position_list[j] = position_list;
            }

            ////////////////////////////////////////////////////////
            Loom.QueueOnMainThread(() =>
            {
                ///////////////////////////////////////////////////////
                dot_list = new GameObject[N_k * N_b];
                for (int i = 0; i < N_k; i++)
                {
                    for (int j = 0; j < N_b; j++)
                    {
                        // get x and y
                        float x = EigenvalueX[i];
                        float y = EigenvalueY[i, j];
                        //calc overlapping band indices
                        var I_b_list = new List<int>();

                        for (int j_ = 0; j_ < N_b; j_++)
                        {
                            float y_ = EigenvalueY[i, j_];
                            if (Mathf.Abs(y - y_) < delta_y)
                            {
                                I_b_list.Add(j_);
                            }
                        }
                        //calc dot position

                        Vector3 dot_position = line_position_list[j][i];
                        dot_position -= transform.forward * 0.05f;
                        //create dot
                        GameObject dot = dot_template_pool.Pop();
                        dot.transform.position = dot_position;
                        dot_template dot_Template_comp = dot.requireComponent<dot_template>();

                        dot_Template_comp.I_k = i;
                        dot_Template_comp.I_b = j;
                        dot_Template_comp.I_b_list = I_b_list;
                        //dot.transform.SetParent(ButtonPanel.transform, false);
                        dot_list[i * N_b + j] = dot;

                        ////////////////////////////////////////////////////////

                    }
                }

                ////////////////////////////////////////////////////////
            });
        });


    }


    // find the nearest dot
    void find_nearest_dot()
    {
        // get cursor position
        Vector3 cursor_position = cursor.transform.position;

        // check cursor range
        Vector3 deltaR = cursor_position - dashboard_center;
        float x_cursor = Vector3.Dot(dashboard_x_axis, deltaR);
        float y_cursor = Vector3.Dot(dashboard_y_axis, deltaR);
        float z_cursor = Vector3.Dot(dashboard_z_axis, deltaR);

        if (!(Mathf.Abs(x_cursor) < dashboard_Lx / 2 && Mathf.Abs(y_cursor) < dashboard_Ly / 2 && Mathf.Abs(z_cursor) < dashboard_Lz / 2))
        {
            if (dot_selected)
            {
                dot_selected.GetComponent<dot_template>().isHovered = false;
            }
            return;
        }

        // find the nearest dot
        float d_min = 1000f;
        GameObject dot_min = null;
        foreach (GameObject dot in dot_list)
        {
            Vector3 dot_position = dot.transform.position;
            dot.GetComponent<dot_template>().isHovered = false;
            float d = (cursor_position - dot_position).magnitude;
            if (d < d_min)
            {
                d_min = d;
                dot_min = dot;
            }
        }
        dot_template dot_para = dot_min.GetComponent<dot_template>();
        dot_para.isHovered = true;
        int I_b = dot_para.I_b;
        List<int> I_b_list = dot_para.I_b_list;

        // plot line
        int I_b_hover;
        int I_b_selected_old = I_b_selected;

        if (I_b_list.Contains(I_b_selected_old))
        {
            I_b_hover = I_b_selected_old;
        }
        else
        {
            I_b_hover = I_b;
        }
        plot_line(line, line_position_list, I_b_hover);
    }


    // plot line
    void plot_line(LineRenderer line, Vector3[][] line_position_list, int I_b)
    {
        if (I_b == -1)
        {
            line.positionCount = 0;
            return;
        }
        Vector3[] position_list = line_position_list[I_b];
        line.positionCount = position_list.Length;
        line.SetPositions(position_list);
    }


    // import image
    bool importImage(string fname)
    {
        Texture2D tex2d = Resources.Load<Texture2D>(fname);
        if (!tex2d)
        {
            m_image.texture = null;
            return false;
        }
        m_image.texture = tex2d;
        RectTransform rt = m_image.GetComponent<RectTransform>();
        rt.transform.localPosition = center;
        rt.transform.localScale = length;
        return true;
    }


    // calc_position
    Vector3 calc_position(float x, float y, float alfa, float x_min, float x_max, float y_min, float y_max, float x0_, float y0_, float Lx, float Ly)
    {
        float x_min_ = x0_ - (0.5f - alfa) * Lx;
        float x_ = x_min_ + Lx * (1 - 2 * alfa) * (x - x_min) / (x_max - x_min);
        float y_min_ = y0_ - (0.5f - alfa) * Ly;
        float y_ = y_min_ + Ly * (1 - 2 * alfa) * (y - y_min) / (y_max - y_min);
        Vector3 xyz_ = new Vector3(x_, y_, 0);
        return xyz_;
    }


    //JSONArray to Array 1d
    float[] JSONArray_to_Array_1d(JSONArray xx)
    {
        int n = xx.Count;
        float[] x = new float[n];
        for (int i = 0; i < n; i++)
        {
            x[i] = xx[i];
        }
        return x;
    }


    //JSONArray to Array 2d
    float[,] JSONArray_to_Array_2d(JSONArray xx)
    {
        int n1 = xx.Count;
        int n2 = xx[0].Count;
        float[,] x = new float[n1, n2];
        for (int i1 = 0; i1 < n1; i1++)
        {
            for (int i2 = 0; i2 < n2; i2++)
            {
                x[i1, i2] = xx[i1][i2];
            }
        }
        return x;
    }


    void playAudio()
    {
        if (source != null)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
            source.Play();
        }
    }

    void OnDestroy()
    {
        EventCenter.Ins.Remove("Canvas_phonon", "PlayAudio");
    }
}

