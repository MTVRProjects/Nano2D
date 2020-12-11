using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;

public class ScreenDebugTool : MonoBehaviour
{
    Transform DebugPanel { get { return GameObject.Find("DebugPanel").transform; } }
    GameObject _clearButton;
    GameObject m_ClearButton { get { if (_clearButton == null) _clearButton = DebugPanel.Find("ClearBN").gameObject; return _clearButton; } }

    GameObject _closeButton;
    GameObject m_CloseButton { get { if (_closeButton == null) _closeButton = DebugPanel.Find("CloseBN").gameObject; return _closeButton; } }

    GameObject _scrollButton;
    GameObject m_ScrollButton { get { if (_scrollButton == null) _scrollButton = DebugPanel.Find("ScrollBN").gameObject; return _scrollButton; } }

    Text _logPanel;
    Text m_LogPanel { get { if (_logPanel == null) _logPanel = DebugPanel.Find("LogText").GetComponent<Text>(); return _logPanel; } }
    Image _logBackground;
    Image m_LogBackground { get { if (_logBackground == null) _logBackground = DebugPanel.parent.Find("BG").GetComponent<Image>(); return _logBackground; } }

    Text _scrollButtonText;
    Text m_ScrollButtonText { get { if (_scrollButtonText == null) _scrollButtonText = m_ScrollButton.transform.Find("Text").GetComponent<Text>(); return _scrollButtonText; } }

    private StringBuilder logSB = new StringBuilder();

    void Awake()
    {
#if UNITY_EDITOR
        DebugPRO.isForbiddenLocal = false;
#else
        DebugPRO.isForbiddenLocal = true;
#endif
        DebugPRO.RedirectOutPut += Debug_RedirectOutPut;

        Close();
        InitBnOnClick();
        DebugPRO.RegistExceptionHandler();
    }

    void InitBnOnClick()
    {
        m_CloseButton.GetComponent<Button>().onClick.AddListener(Close);
        m_ScrollButton.GetComponent<Button>().onClick.AddListener(SetScrollBuntton);
        m_ClearButton.GetComponent<Button>().onClick.AddListener(Clear);
    }

    private void Debug_RedirectOutPut(string msg, DebugPRO.DebugLevel type)
    {
        switch (type)
        {
            case DebugPRO.DebugLevel.INFO:
                AddLine("INFO—>" + msg, "lime");
                break;
            case DebugPRO.DebugLevel.WARNING:
                AddLine("Warning—>" + msg, "yellow");
                break;
            case DebugPRO.DebugLevel.ERROR:
                AddLine("Error—>" + msg, "red");
                break;
            case DebugPRO.DebugLevel.EXCEPTION:
                AddLine("Exception—>" + msg, "black");
                break;
        }
    }

    private void AddLine(string msg, string color)
    {
        string head = "[" + DateTime.Now.ToString("HH:mm:ss") + "] ";
        if (!m_LogPanel.gameObject.activeSelf)
        {
            m_LogPanel.gameObject.SetActive(true);
            m_CloseButton.SetActive(true);
            m_ScrollButton.SetActive(true);
            m_ClearButton.SetActive(true);
        }

        if (logSB.Length == 0)
        {
            logSB.Append(head + "<color=" + color + ">" + msg + "</color>");
        }
        else
        {
            logSB.AppendLine();
            logSB.Append(head + "<color=" + color + ">" + msg + "</color>");
        }
        m_LogPanel.text = logSB.ToString();
    }

    void Clear()
    {
        logSB = new StringBuilder();
        m_LogPanel.text = "已清空...";
    }

    private void Close()
    {
        if (m_LogPanel.gameObject.activeSelf)
        {
            m_LogPanel.gameObject.SetActive(false);
            m_CloseButton.SetActive(false);
            m_ScrollButton.SetActive(false);
            m_ClearButton.SetActive(false);
        }
    }

    public void SetScrollBuntton()
    {
        m_LogBackground.raycastTarget = !m_LogBackground.raycastTarget;
        m_ScrollButtonText.text = m_LogBackground.raycastTarget ? "屏蔽滑动" : "启用滑动";
    }
}
