using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using System.Linq;
using System;
using HMLFramwork.Singleton;

public class Canvas_band : MonoSingleton<Canvas_band>
{
    // Parameters
    string DatabaseIndex = "";
    public Vector2 center = new Vector2(0, 2.2f);
    public Vector2 length = new Vector2(2.8f, 1.4f);

    RawImage m_image;
    string DatabaseIndex_ = "";
    GameObject dot_template;
    GameObject ButtonPanel;
    Canvas_database canvas00;


    // Start
    void Start()
    {
        // Find
        m_image = transform.Find("dashboard").GetComponent<RawImage>();
        canvas00 = GameObject.Find("Canvas_database").GetComponent<Canvas_database>();
        // Image
        RectTransform rt = m_image.GetComponent<RectTransform>();
        rt.transform.localPosition = center;
        rt.transform.localScale = length;
    }

    public void MaterialItemBnEvent()
    {
        int index_selected = canvas00.index_selected;
        if (index_selected < 0) return;
        DatabaseIndex = index_selected.ToString("D4");
        if (DatabaseIndex == DatabaseIndex_) return;
        DatabaseIndex_ = DatabaseIndex;

        // import image
        string fname = "bands_png/bands" + DatabaseIndex;
        bool tf = importImage(fname);
    }


    // Update
    //void Update()
    //{
    //    // check if updated
    //    int index_selected = canvas00.index_selected;
    //    if (index_selected < 0) return;
    //    DatabaseIndex = index_selected.ToString("D4");
    //    if (DatabaseIndex == DatabaseIndex_) return;
    //    DatabaseIndex_ = DatabaseIndex;

    //    // import image
    //    string fname = "bands_png/bands" + DatabaseIndex;
    //    bool tf = importImage(fname);
    //}


    // import image
    bool importImage(string fname)
    {
        //2020.7.24
        //Resources.UnloadUnusedAssets();

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

}
