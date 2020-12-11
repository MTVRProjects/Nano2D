////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using HMLFramwork;

public class material_item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // parameters
    [HideInInspector] public int index = -1;
    [HideInInspector] public string formula;
    [HideInInspector] public string magnetism;
    //public string db_id;
    public Color m_NormalColor;
    public Color m_SelectColor;
    Canvas_database canvas;
    GameObject Content;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    public float pointEnterScale = 1.05f;

    Image m_Image;
    RectTransform m_Trans;

    AudioSource sound;
    // start

    bool isAccessible = true;
    void Start()
    {
        GameObject Canvas_database = GameObject.Find("Canvas_database");
        if (Canvas_database)
        {
            sound = Canvas_database.requireComponent<AudioSource>();
            canvas = Canvas_database.requireComponent<Canvas_database>();
        }
        // Image and Trans
        m_Image = GetComponent<Image>();
        m_Trans = GetComponent<RectTransform>();
        m_Image.color = m_NormalColor;
        // find
        Content = GameObject.Find("Canvas_database/ButtonList/Viewport/Content").gameObject;

        //ZMZM_FULL全功能版本，ZMZM_FREE免费版本
#if ZMZM_FULL
        isAccessible = true;
#elif ZMZM_FREE
        isAccessible = AccessibleMgr.Ins.getAccessible(index);
#endif

        if (isAccessible) m_Image.sprite = AccessibleMgr.Ins.accessible_sprite;
        else m_Image.sprite = AccessibleMgr.Ins.unAccessible_sprite;

    }


    public void SetNormalState()
    {
        m_Image.color = m_NormalColor;
    }
    // hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Trans.localScale = Vector3.one * pointEnterScale;
        // haptics
        StartCoroutine(OVR_haptics(0.6f, 0.2f, 0.1f, controller));
    }

    // leave
    public void OnPointerExit(PointerEventData eventData)
    {
        m_Trans.localScale = Vector3.one;
    }

    // select
    public void OnPointerDown(PointerEventData eventData)
    {
        //update state
        int index_selected = canvas.index_selected;
        if (index == index_selected)
        {
            //上一次被选中的Item恢复正常状态
            Canvas_database.Ins.SetLastItemNormalState(this);
            m_Image.color = m_SelectColor;
        }
        else
        {
            m_Image.color = m_NormalColor;

        }

        // sound
        sound.Play();
        if (isAccessible)
        {
            // update canvas
            canvas.index_selected = index;
            canvas.formula_selected = formula;
            canvas.magnetism_selected = magnetism;

            plot_3d_bands.Ins.checkBandDataExit();
            Canvas_band.Ins.MaterialItemBnEvent();
            //canvas.db_id_selected = db_id;
        }
        else
        {
            AccessibleMgr.Ins.OpenUnAccessibleMsg();
        }

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



}
