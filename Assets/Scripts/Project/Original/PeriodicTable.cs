////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using TMPro;
using HMLFramwork.Singleton;

/// <summary>
/// 元素周期表
/// </summary>
public class PeriodicTable : MonoSingleton<PeriodicTable>
{
    // parameters
    public bool isPersistentData = false;
    string fname;
    public string fname_persistent = "/database04_MaterialsCloud/PeriodicTable.json";

    public int[] ElementState;   //ElementState[Z-1] = -1, 0, 1  => disabled, not selected, selected
    public GameObject Element_prefab;
    float spacing_ratio = 0.1f;  //L(spacing)/L(button)
    float delta = 0.002f;        //PeriodicTable/RectTransform/Scale        
    int n1 = 10;                 //number of rows     
    int n2 = 18;                 //number of cols
    int n_element = 118;         //number of elements

    // awake
    void Awake()
    {
        // fname
        if (isPersistentData)
        {
            fname = DataPathHelper.Datas_path + fname_persistent;
        }

        //string fname = "D:/Scratch/VR_projects/VR_3dband/proj_database_v1.1/Matlab/PeriodicTable.json";        
        string text = File.ReadAllText(fname);
        JSONObject dict = (JSONObject)JSON.Parse(text);
        JSONArray ij_list = (JSONArray)dict["ij_list"];
        JSONArray symbol_list = (JSONArray)dict["symbol_list"];
        JSONArray Z_list = (JSONArray)dict["Z_list"];
        JSONArray tf_list = (JSONArray)dict["tf_list"];
        float ElementSize = Element_prefab.GetComponent<RectTransform>().rect.width * delta;
        float spacing = ElementSize * spacing_ratio;
        ElementState = new int[n_element];
        for (int i=0; i<ij_list.Count; i++)
        {
            int i1 = ij_list[i][0] - 1;
            int i2 = ij_list[i][1] - 1;
            string ButtonText = symbol_list[i];
            int Z = Z_list[i];
            int int_tf = tf_list[i];
            bool tf = Convert.ToBoolean(int_tf);
            GameObject button = Instantiate(Element_prefab) as GameObject;
            button.SetActive(true);
            button.transform.GetComponentInChildren<TextMeshProUGUI>().text = ButtonText;
            button.name = ButtonText;
            float y = -(i1 - n1 / 2 + 0.5f) * (ElementSize + spacing) / delta;
            float x = (i2 - n2 / 2 + 0.5f) * (ElementSize + spacing) / delta;
            button.transform.position = new Vector3(x, y, 0);
            button.GetComponent<Element>().isEnabled = tf;
            button.GetComponent<Element>().Z = Z;    
            button.transform.SetParent(Element_prefab.transform.parent, false);
            if (tf)
            {
                ElementState[Z - 1] = 0;
            }
            else
            {
                ElementState[Z - 1] = -1;
            }
        }
    }
}
