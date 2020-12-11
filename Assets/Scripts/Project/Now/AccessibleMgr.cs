using HMLFramwork;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AccessibleMgr : HMLFramwork.Singleton.SingleInstance<AccessibleMgr>
{
    private string accessible_file_path = "/Configuration/AccessibleElement.cf";
    List<string> accessible_Info = null;
    public bool getAccessible(int index)
    {
        bool is_accessible = true;
        if (accessible_Info == null)
        {
            string path = UnityEngine.Application.streamingAssetsPath + accessible_file_path;
            accessible_Info = File.ReadAllText(path).Split(',').toList();
        }
        is_accessible = accessible_Info.Contains(index.ToString());
        return is_accessible;
    }

    Sprite _accessible_sprite;
    public Sprite accessible_sprite
    {
        get
        {
            if (_accessible_sprite == null)
            {
                _accessible_sprite = Resources.Load<Sprite>("UIRes/material_item_free");
            }
            return _accessible_sprite;
        }
    }
    Sprite _unAccessible_sprite;
    public Sprite unAccessible_sprite
    {
        get
        {
            if (_unAccessible_sprite == null)
            {
                _unAccessible_sprite = Resources.Load<Sprite>("UIRes/material_item_lock");
            }
            return _unAccessible_sprite;
        }
    }

    public void OpenUnAccessibleMsg()
    {
        UnAccessibleMsgMgr.Ins.Open(true);
        TimerEvent.Add(10f, () => { UnAccessibleMsgMgr.Ins.Open(false); });
    }

}
