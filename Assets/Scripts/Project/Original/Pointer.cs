////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using HMLFramwork.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class Pointer : MonoSingleton<Pointer>
{
    // parameters
    public float MaxLength = 5.0f;
    public GameObject SphereCursor;
    Transform sphereCursor_trans;

    public EventSystem eventSystem;
    public StandaloneInputModule inputModule;
    LineRenderer Laser;

    bool isPointerOn = true;


    // awake
    void Awake()
    {
        trans = transform;

        Laser = GetComponent<LineRenderer>();
        Laser.enabled = isPointerOn;
        SphereCursor.SetActive(isPointerOn);
        sphereCursor_trans = SphereCursor.transform;

        eventSystem_comp = eventSystem.GetComponent<EventSystem>();
    }

    Hand _rightHandAnchor_hand;
    Hand rightHandAnchor_hand
    {
        get
        {
            if (_rightHandAnchor_hand == null)
                _rightHandAnchor_hand = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").GetComponent<Hand>();
            return _rightHandAnchor_hand;
        }
    }

    bool _isOpenHandRay = true;
    public bool isOpenHandRay
    {
        get { return _isOpenHandRay; }
        set
        {
            _isOpenHandRay = value;
            if (Laser)
            {
                SphereCursor.SetActive(_isOpenHandRay);
                eventSystem_comp.enabled = _isOpenHandRay;
                Laser.enabled = _isOpenHandRay;
            }
        }
    }


    RaycastHit hit;
    Ray ray;
    Vector3 endPosition;
    GameObject hitObject;

    Transform trans;

    EventSystem eventSystem_comp;
   
    // update
    void Update()
    {
        if (!_isOpenHandRay)
        {
            return;
        }
        // check on/off
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            isPointerOn = !isPointerOn;
            Laser.enabled = isPointerOn;
            SphereCursor.SetActive(isPointerOn);
            eventSystem_comp.enabled = true;
        }
        if (!isPointerOn)
        {
            //2020.7.21
            //GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").GetComponent<Hand>().item_touch_pointer = null;

            rightHandAnchor_hand.item_touch_pointer = null;
            eventSystem_comp.enabled = false;
            return;
        }

        // get canvas distance
        float targetLength = MaxLength;
        float canvasDistance = GetCanvasDistance();
        if (canvasDistance > 0)
        {
            targetLength = canvasDistance;
        }

        // cast ray
        
        ray = new Ray(trans.position, trans.forward);
        Physics.Raycast(ray, out hit, targetLength);

        if (hit.collider != null)
        {
            endPosition = hit.point;
            hitObject = hit.transform.gameObject;

            if (!hit.transform.CompareTag("UI"))
            {
                eventSystem_comp.enabled = false;
            }
        }
        else
        {
            endPosition = trans.position + (trans.forward * targetLength);
            hitObject = null;
            eventSystem_comp.enabled = true;
        }
        sphereCursor_trans.position = endPosition;
        Laser.SetPosition(0, trans.position);
        Laser.SetPosition(1, endPosition);
        //2020.7.21
        rightHandAnchor_hand.item_touch_pointer = hitObject;
    }


    // GetCanvasDistance
    float GetCanvasDistance()
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = inputModule.inputOverride.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);
        RaycastResult closestResult = FindFirstRaycast(results);
        float distance = closestResult.distance;
        distance = Mathf.Clamp(distance, 0.0f, MaxLength);
        return distance;
    }


    //FindFirstRaycast
    RaycastResult FindFirstRaycast(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (!result.gameObject) continue;
            return result;
        }
        return new RaycastResult();
    }

}


