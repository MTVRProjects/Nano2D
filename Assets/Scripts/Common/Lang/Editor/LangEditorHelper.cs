using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using HMLFramwork.Helpers;
using HMLFramwork;
using System.Text;

public class LangEditorHelper : MonoBehaviour
{
    static string langCSV_path = Application.streamingAssetsPath + "/Configs/Lang/Language.csv";
    [MenuItem("HML/Lang/创建多语言CSV文件")]
    static void createCSVFile()
    {
        FileHelper.Save("ID,Describe,CH,EN", langCSV_path);
    }

    [MenuItem("HML/Lang/将所有Text写入CSV中")]
    static void getAllTextAndToLangCSV()
    {
        List<List<string>> csv_contents = new List<List<string>>();
        Text[] texts = FindObjectsOfType<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            Text text_temp = texts[i];

            List<string> text_csv_temp = new List<string>();
            List<Transform> parent_level = text_temp.transform.getParentLevel();
            parent_level.Reverse();
            string ID = "";
            string describe = "";
            for (int j = 0; j < parent_level.Count; j++)
            {
                ID += parent_level[j].name + "_";
                describe += parent_level[j].name + "/";
            }
            ID = ID.Remove(ID.Length - 1);
            describe = describe.Remove(describe.Length - 1);

            text_csv_temp.Add(ID);
            text_csv_temp.Add(describe);
            text_csv_temp.Add("");
            text_csv_temp.Add(text_temp.text);

            csv_contents.Add(text_csv_temp);
        }

        string csv_str = CSVHelper.toCSV<string>(csv_contents);
        Debug.Log(csv_str);
        FileHelper.Save(csv_str, langCSV_path, false, true, Encoding.UTF8);
    }

    [MenuItem("HML/Lang/将所有Text写入CSV中(同时标准化对象名称)")]
    static void getAllTextAndToLangCSV_and_Rename()
    {
        List<List<string>> csv_contents = new List<List<string>>();
        Text[] texts = FindObjectsOfType<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            Text text_temp = texts[i];

            List<string> text_csv_temp = new List<string>();
            List<Transform> parent_level = text_temp.transform.getParentLevel();
            parent_level.Reverse();
            string ID = "";
            string describe = "";
            for (int j = 0; j < parent_level.Count; j++)
            {
                ID += parent_level[j].name + "_";
                describe += parent_level[j].name + "/";
            }
            ID = ID.Remove(ID.Length - 1);
            describe = describe.Remove(describe.Length - 1);

            text_temp.name = ID;

            text_csv_temp.Add(ID);
            text_csv_temp.Add(describe);
            text_csv_temp.Add("");
            text_csv_temp.Add(text_temp.text);

            csv_contents.Add(text_csv_temp);
        }

        string csv_str = CSVHelper.toCSV<string>(csv_contents);
        FileHelper.Save(csv_str, langCSV_path, false, true, Encoding.UTF8);
    }
}
