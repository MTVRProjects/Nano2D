using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using HMLFramwork;

public class dot_template : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // parameters
    public int I_k = -1;
    public int I_b = -1;
    public List<int> I_b_list;  //overlapping band indices
    public bool isSelected = false;
    public bool isHovered = false;

    Image m_Image;
    RectTransform m_Trans;
    Sprite dot_red;
    Sprite dot_blue;
    //Color color_show = new Color(255, 255, 255, 255);
    Color color_hide = new Color(255, 255, 255, 0);

    OVRInput.Controller controller = OVRInput.Controller.RTouch;

    // Start
    void Start()
    {
        // Image and Trans
        m_Image = GetComponent<Image>();
        m_Trans = GetComponent<RectTransform>();
        m_Image.color = color_hide;
        // find GameObject
        dot_red = Resources.Load<Sprite>("Materials/dot_red");
        dot_blue = Resources.Load<Sprite>("Materials/dot_blue");
        // canvas
    }

   
    // Update
    void Update()
    {
        if (isHovered)
        {
            // press trigger
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                isSelected = true;
                // deselect old dot
                GameObject dot_deselected = Canvas_phonon.Ins.dot_selected;
                if (dot_deselected && dot_deselected != gameObject)
                {
                    dot_deselected.GetComponent<dot_template>().isSelected = false;
                }
                // I_b_selected
                int I_b_selected;
                int I_b_selected_old = Canvas_phonon.Ins.I_b_selected;
                if (I_b_list.Contains(I_b_selected_old))
                {
                    I_b_selected = I_b_selected_old;
                }
                else
                {
                    I_b_selected = I_b;
                }
                Canvas_phonon.Ins.I_k_selected = I_k;
                Canvas_phonon.Ins.I_b_selected = I_b_selected;
                Canvas_phonon.Ins.dot_selected = gameObject;
                // sound
                EventCenter.Ins.Excute("Canvas_phonon", "PlayAudio");
            }

            // dot color
            if (isSelected)
            {
                m_Image.color = Color.white;
                m_Image.sprite = dot_red;
            }
            else
            {
                m_Image.color = Color.white;
                m_Image.sprite = dot_blue;
            }
        }
        else
        {
            // dot color
            if (isSelected)
            {
                m_Image.color = Color.white;
                m_Image.sprite = dot_red;
            }
            else
            {
                m_Image.color = color_hide;
                m_Image.sprite = dot_blue;
            }
        }
    }


    // hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        // haptics
        StartCoroutine(OVR_haptics(0.6f, 0.2f, 0.1f, controller));
    }

    //// other    
    public void OnPointerExit(PointerEventData eventData) { }
    public void OnPointerDown(PointerEventData eventData) { }
    public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }


    // haptics
    IEnumerator OVR_haptics(float frequency, float amplitude, float duration, OVRInput.Controller controller)
    {
        // frequency: (0,1)
        // amplitude: (0,1)
        // duration: (0,2) (second)
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
