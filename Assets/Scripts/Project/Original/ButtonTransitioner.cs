using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class ButtonTransitioner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    // parameters
    public Color m_NormalColor = Color.white;
    public Color m_HoverColor = Color.grey;
    public Color m_DownColor = Color.red;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public bool isSelect;

    Image m_Image = null;

    public event Action clickEventHandle = () => { };

    // awake
    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }


    // hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Image.color = m_HoverColor;        
        // haptics
        StartCoroutine( OVR_haptics(0.6f, 0.2f, 0.1f, controller));
    }


    // leave
    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = m_NormalColor;
    }


    // select
    public void OnPointerDown(PointerEventData eventData)
    {
        m_Image.color = m_DownColor;
        isSelect = true;

        clickEventHandle();

        // sound
        AudioSource sound = gameObject.GetComponent<AudioSource>();
        sound.Play();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isSelect = false;
    }

    // other
    //public void OnPointerUp(PointerEventData eventData) { }
    public void OnPointerClick(PointerEventData eventData) { }


    // haptics
    IEnumerator OVR_haptics(float frequency, float amplitude, float duration, OVRInput.Controller controller)
    {
        // frequency: (0,1)
        // amplitude: (0,1)
        // duration: (0,2) (second)
        OVRInput.SetControllerVibration(frequency, amplitude, controller);//手柄振动
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controller);
    }


}

