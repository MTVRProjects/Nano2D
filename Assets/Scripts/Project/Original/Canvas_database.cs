using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using System;
using TMPro;
using HMLFramwork;
using HMLFramwork.Singleton;

public class Canvas_database : MonoSingleton<Canvas_database>
{
    // parameters
    [Header("指定路径数据存在，则勾选")]
    public bool isPersistentData = true;
    string fname;
    public string fname_persistent = "/database04_MaterialsCloud/database_info.json";

    public int index_selected = -1;
    public string formula_selected = "";
    public string magnetism_selected = "";

    List<int> index_selected_list;
    string[] formula_list;
    string[] magnetism_list;
    //string[] db_id_list;
    int[] index_list;
    int[][] elements_list;
    GameObject content;
    TextMeshProUGUI material_index;
    //scroll_bar
    float scroll_rate = 0.1f;
    Scrollbar scroll_bar;

    Transform cursor;
    Vector3 dashboard_x_axis;
    Vector3 dashboard_y_axis;
    Vector3 dashboard_z_axis;
    Vector3 dashboard_position;
    Quaternion dashboard_rotation;
    float dashboard_Lx;
    float dashboard_Ly;
    float dashboard_Lz;

    AudioSource source;
    // start
    void Start()
    {
        // fname
        if (isPersistentData)
        {
            fname = DataPathHelper.Datas_path + fname_persistent;
        }

        // load database_info.json
        string text = File.ReadAllText(fname);
        //string text = new WWW(fname).text;
        JSONObject dict = (JSONObject)JSON.Parse(text);
        JSONArray formula_array = (JSONArray)dict["formula_list"];
        JSONArray magnetism_array = (JSONArray)dict["magnetism_list"];
        //JSONArray db_id_array = (JSONArray)dict["db_id_list"];
        JSONArray elements_array = (JSONArray)dict["elements_list"];

        // convert JSONArray to list
        int N = formula_array.Count;
        formula_list = new string[N];
        magnetism_list = new string[N];
        index_list = new int[N];
        elements_list = new int[N][];
        for (int i = 0; i < N; i++)
        {
            formula_list[i] = formula_array[i];
            magnetism_list[i] = magnetism_array[i];
            index_list[i] = i + 1;
            int M = elements_array[i].Count;
            if (M == 0)
            {
                elements_list[i] = new int[] { elements_array[i] };
            }
            else
            {
                elements_list[i] = new int[M];
                for (int k = 0; k < M; k++)
                {
                    elements_list[i][k] = elements_array[i][k];
                }
            }
        }

        // find
        content = GameObject.Find("Canvas_database/ButtonList/Viewport/Content");
        material_index = GameObject.Find("Canvas_database/material_index").GetComponent<TextMeshProUGUI>();
        scroll_bar = GameObject.Find("Canvas_database/ButtonList/Scrollbar").GetComponent<Scrollbar>();
        cursor = GameObject.Find("XRplayer/OVRCameraRig/TrackingSpace/RightHandAnchor/Pointer/dot").transform;

        // initialize ButtonList
        initialize_content(index_list, formula_list, magnetism_list);

        // sound
        source = gameObject.requireComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>("AudioClip/sound_ui_hover");

        // dashboard
        dashboard_position = transform.Find("background").position;
        dashboard_rotation = transform.Find("background").rotation;
        dashboard_Lx = transform.Find("background").GetComponent<RectTransform>().sizeDelta.x;
        dashboard_Ly = transform.Find("background").GetComponent<RectTransform>().sizeDelta.y;
        dashboard_Lz = 0.1f;
        dashboard_x_axis = dashboard_rotation * new Vector3(1, 0, 0);
        dashboard_y_axis = dashboard_rotation * new Vector3(0, 1, 0);
        dashboard_z_axis = dashboard_rotation * new Vector3(0, 0, 1);


        EventCenter.Ins.Add("Canvas_database", "PlayAudio", playAudio);
    }

    material_item last_choose_item = null;
    public void SetLastItemNormalState(material_item item)
    {
        if (last_choose_item != null)
        {
            last_choose_item.SetNormalState();
        }
        last_choose_item = item;
    }
    /// <summary>
    /// 刷新UI显示
    /// </summary>
    public void UpdateCanvas()
    {
        int[] ElementState = PeriodicTable.Ins.ElementState;
        //找到被选择的元素
        index_selected_list = find_index_selected(elements_list, ElementState);
        update_content(index_selected_list);
        index_selected = -1;
        formula_selected = "";
        magnetism_selected = "";
    }
    //TextMeshProUGUI 
    // update
    void Update()
    {
        // change material with X/Y button

        if (Input.GetKeyDown(KeyCode.PageUp) || OVRInput.GetDown(OVRInput.RawButton.X))
        {

            int M = 0;
            if (index_selected_list != null)
            {
                M = index_selected_list.Count;
                index_selected = Mod_plus(index_selected - 1, M);
                formula_selected = formula_list[index_selected - 1];
                magnetism_selected = magnetism_list[index_selected - 1];
            }

           
            //db_id_selected = db_id_list[index_selected-1];
        }
        else if (Input.GetKeyDown(KeyCode.PageDown) || OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            int M = 0;
            if (index_selected_list != null)
            {
                M = index_selected_list.Count;
                index_selected = Mod_plus(index_selected + 1, M);
                formula_selected = formula_list[index_selected - 1];
                magnetism_selected = magnetism_list[index_selected - 1];
            }

          
            //db_id_selected = db_id_list[index_selected-1];
        }

        // display material_index
        material_index.text = formula_selected;

        // control scrollbar with joystick
        Vector3 deltaR = cursor.position - dashboard_position;
        float x_cursor = Vector3.Dot(dashboard_x_axis, deltaR);
        float y_cursor = Vector3.Dot(dashboard_y_axis, deltaR);
        float z_cursor = Vector3.Dot(dashboard_z_axis, deltaR);
        if (Mathf.Abs(x_cursor) < dashboard_Lx / 2 && Mathf.Abs(y_cursor) < dashboard_Ly / 2 && Mathf.Abs(z_cursor) < dashboard_Lz / 2)
        {
            float ds = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y * scroll_rate * Time.deltaTime;
            scroll_bar.value += ds;
        }
    }

    int Mod_plus(int a, int b)
    {
        return ((a - 1) % b + b) % b + 1;
    }
    // initialize content
    void initialize_content(int[] index_list, string[] formula_list, string[] magnetism_list)
    {
        GameObject buttonTemplate = content.transform.Find("ButtonTemplate").gameObject;
        for (int i = 0; i < index_list.Length; i++)
        {
            GameObject button = GameObject.Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);
            button.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = formula_list[i];

            var mat_item = button.GetComponent<material_item>();
            mat_item.index = index_list[i];
            mat_item.formula = formula_list[i];
            mat_item.magnetism = magnetism_list[i];
            //button.GetComponent<material_item>().db_id = db_id_list[i];
            button.transform.SetParent(buttonTemplate.transform.parent, false);
        }
    }


    // update content
    public void update_content(List<int> index_selected_list)
    {
        foreach (Transform buttonTrans in content.transform)
        {
            int index = buttonTrans.GetComponent<material_item>().index;
            if (index_selected_list.Contains(index))
            {
                buttonTrans.gameObject.SetActive(true);
                buttonTrans.GetComponent<Image>().color = buttonTrans.GetComponent<material_item>().m_NormalColor;
            }
            else
            {
                buttonTrans.gameObject.SetActive(false);
            }
        }
    }


    // find_index_selected
    List<int> find_index_selected(int[][] elements_list, int[] ElementState)
    {
        // Z_selected_list
        List<int> Z_selected_list = new List<int>();
        for (int i = 0; i < ElementState.Length; i++)
        {
            int Z = i + 1;
            int state = ElementState[i];
            if (state > 0) Z_selected_list.Add(Z);
        }
        // index_selected_list
        List<int> index_selected_list = new List<int>();
        if (Z_selected_list.Count == 0)
        {
            for (int j = 0; j < elements_list.GetLength(0); j++)
            {
                int index = j + 1;
                index_selected_list.Add(index);
            }
        }
        else
        {
            for (int j = 0; j < elements_list.GetLength(0); j++)
            {
                int index = j + 1;
                int[] elements = elements_list[j];
                bool tf = true;
                foreach (int Z in Z_selected_list)
                {
                    if (Array.IndexOf(elements, Z) < 0)
                    {
                        tf = false;
                        break;
                    }
                }
                if (tf) index_selected_list.Add(index);
            }
        }
        return index_selected_list;
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
        EventCenter.Ins.Remove("Canvas_database", "PlayAudio");
    }
}

