using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using HMLFramwork;

/// <summary>
/// 元素周期表中的元素对象
/// </summary>
public class Element : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    public int Z;
    public Color m_NormalColor = new Color(155, 230, 255);
    public Color m_DisabledColor = new Color(200, 200, 200);
    public Color m_SelectColor = new Color(250, 100, 0);
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    public bool isEnabled = false;
    public bool isSelected = false;

    Image m_Image;
    RectTransform m_Trans;

    Sprite unEnableSprite;
    // Start
    private void Start()
    {
        unEnableSprite = Resources.Load<Sprite>("UIRes/database_unenable");
        // Image and Trans
        m_Image = GetComponent<Image>();
        m_Trans = GetComponent<RectTransform>();
        if (!isEnabled)
        {
            m_Image.sprite = unEnableSprite;
            m_Image.color = m_DisabledColor;

        }
        else
        {
            m_Image.color = m_NormalColor;
        }
        // find GameObject

    }


    // hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isEnabled) return;
        m_Trans.localScale = Vector3.one * 1.2f;

        //haptics
        StartCoroutine(OVR_haptics(0.6f, 0.2f, 0.1f, controller));
    }

    // leave
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEnabled) return;
        m_Trans.localScale = new Vector3(1, 1, 1);
    }

    // select
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isEnabled) return;
        isSelected = !isSelected;
        if (isSelected)
        {
            m_Image.color = m_SelectColor;
        }
        else
        {
            m_Image.color = m_NormalColor;
        }
        // update canvas
        update_canvas();
        // sound
        EventCenter.Ins.Excute("Canvas_database", "PlayAudio");

    }


    // other

    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }


    // haptics
    IEnumerator OVR_haptics(float frequency, float amplitude, float duration, OVRInput.Controller controller)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controller);
    }


  
    void update_canvas()
    {
        int state = PeriodicTable.Ins.ElementState[Z - 1];
        state = 1 - state;
        PeriodicTable.Ins.ElementState[Z - 1] = state;
        Canvas_database.Ins.UpdateCanvas();
    }

}
