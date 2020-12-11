using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HMLFramwork;

/// <summary>
/// algorithm:: 4 trigger states
/// algorithm:: _________        _________
/// algorithm::          |______|
/// algorithm:: idle, catch (GetDown), hold (Get), release (GetUp)
/// algorithm:: item_touch: collided objects with the hand
/// algorithm:: item_touch_pointer: collided objectes with the pointer
/// algorithm:: workmode = "idle", "move", "scale"
/// </summary>

public class Hand : MonoBehaviour
{
    // parameters
    string targetTag = "Interactable";
    GameObject item_touch;
    GameObject item_touch_all;
    [HideInInspector] public GameObject item_touch_pointer;
    OVRInput.Controller controller;
    Hand otherController;
    bool isCatch = false;
    string workmode = "idle";

    Vector3 q0_, p1_, p2_, p0_, p12_, qp0_;
    Vector3 sca_;
    Quaternion Rq_, Rp_, Rqp_;
    Quaternion rot_;

    Transform trans;
    // Start
    void Start()
    {
        trans = transform;
        // add component SphereCollider
        var sc = gameObject.requireComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 0.1f;
        // add component Rigidbody
        var r = gameObject.requireComponent<Rigidbody>();
        r.useGravity = false;
        r.isKinematic = true;
        // set controller
        if (name == "LeftHandAnchor")
        {
            controller = OVRInput.Controller.LTouch;
            otherController = GameObject.Find("RightHandAnchor").GetComponent<Hand>();
        }
        else if (name == "RightHandAnchor")
        {
            controller = OVRInput.Controller.RTouch;
            otherController = GameObject.Find("LeftHandAnchor").GetComponent<Hand>();
        }
        else
        {
            throw new System.ArgumentException("wrong gameObject", "gameObject.name");
        }
    }

    // Update
    void Update()
    {
        // find item_touch_all (hand or pointer)
        if (!isCatch)
        {
            if (item_touch)
            {
                item_touch_all = item_touch;
            }
            else if (item_touch_pointer && item_touch_pointer.CompareTag(targetTag))
            {
                bool isContained = item_touch_pointer.GetComponent<Interactable>().isContained;
                if (!isContained)
                {
                    item_touch_all = item_touch_pointer;
                }
                else
                {
                    item_touch_all = item_touch_pointer.transform.parent.gameObject;
                }
            }
            else
            {
                item_touch_all = null;
            }
        }

        // catch
        if (!isCatch && item_touch_all && (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller) || OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller)))
        {
            isCatch = true;
            var interactable = item_touch_all.GetComponent<Interactable>();
            if (interactable != null)
            {
                item_touch_all.GetComponent<Interactable>().N_grabber++;
            }
           
            return;
        }

        // release
        if (isCatch && (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller) || OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller)))
        {
            isCatch = false;
            var interactable = item_touch_all.GetComponent<Interactable>();
            if (interactable != null)
            {
                item_touch_all.GetComponent<Interactable>().N_grabber--;
            }
            drop(item_touch_all);
            return;
        }

        // move or scale
        if (isCatch)
        {
            var interactable = item_touch_all.GetComponent<Interactable>();
            if (interactable != null)
            {
                int N_grabber = item_touch_all.GetComponent<Interactable>().N_grabber;
                if (N_grabber == 1)
                {
                    move(item_touch_all);
                }
                else
                {
                    if (controller == OVRInput.Controller.RTouch)
                    {
                        scale(otherController, item_touch_all);
                    }
                }
                return;
            }
        }
    }


    // move
    Vector3 q0, p0;
    Quaternion Rq, Rp;
    Transform item_trans;
    void move(GameObject item)
    {
        item_trans = item.transform;
        if (workmode != "move")
        {
            p0_ = trans.position;
            Rp_ = trans.rotation;
            q0_ = item_trans.position;
            Rq_ = item_trans.rotation;
            qp0_ = Quaternion.Inverse(Rp_) * (q0_ - p0_);
            Rqp_ = Quaternion.Inverse(Rp_) * Rq_;
            workmode = "move";
            return;
        }

        p0 = trans.position;
        Rp = trans.rotation;
        q0 = p0 + Rp * qp0_;
        Rq = Rp * Rqp_;
        item_trans.position = q0;
        item_trans.rotation = Rq;

    }


    // scale
    void scale(Hand otherController, GameObject item)
    {
        Vector3 q0, p1, p2, p0, p12;
        Vector3 sca;
        Quaternion rot;
        float lambda;
        Vector3 axis;
        Quaternion M_rot;
        float angle;

        if (workmode != "scale")
        {
            q0_ = item.transform.position;
            sca_ = item.transform.localScale;
            rot_ = item.transform.rotation;
            p1_ = transform.position;
            p2_ = otherController.transform.position;
            p0_ = (p1_ + p2_) / 2;
            p12_ = p2_ - p1_;
            otherController.workmode = "scale";
            workmode = "scale";
            return;
        }

        p1 = transform.position;
        p2 = otherController.transform.position;
        p0 = (p1 + p2) / 2;
        p12 = p2 - p1;

        (axis, angle) = calc_rot(p12_, p12);
        angle = angle * 180 / (float)Math.PI;
        if (float.IsNaN(angle)) return;
        M_rot = Quaternion.AngleAxis(angle, axis);

        lambda = p12.magnitude / p12_.magnitude;
        sca = lambda * sca_;
        q0 = p0 + lambda * (M_rot * (q0_ - p0_));
        rot = M_rot * rot_;

        item.transform.position = q0;
        item.transform.localScale = sca;
        item.transform.rotation = rot;

    }


    // calculate rotation transform for scale
    (Vector3 axis, float angle) calc_rot(Vector3 xyz1, Vector3 xyz2)
    {
        Vector3 axis;
        float angle;
        xyz1 = Vector3.Normalize(xyz1);
        xyz2 = Vector3.Normalize(xyz2);
        axis = Vector3.Normalize(Vector3.Cross(xyz1, xyz2));
        angle = Mathf.Acos(Vector3.Dot(xyz1, xyz2));
        return (axis, angle);
    }


    // drop
    void drop(GameObject item)
    {
        workmode = "idle";
    }


    // touch check
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(targetTag)) return;
        if (isCatch) return;
        bool isContained = other.gameObject.GetComponent<Interactable>().isContained;
        if (!isContained)
        { item_touch = other.gameObject; }
        else
        { item_touch = other.gameObject.transform.parent.gameObject; }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag(targetTag)) return;
        if (isCatch) return;
        bool isContained = other.gameObject.GetComponent<Interactable>().isContained;
        if (!isContained)
        { item_touch = other.gameObject; }
        else
        { item_touch = other.transform.parent.gameObject; }
    }

    void OnTriggerExit(Collider other)
    {
        if (isCatch) return;
        item_touch = null;
    }

}



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////debug <<<
//print("hand = " + gameObject.name + "  isCatch = " + isCatch + "  item_touch_all = " + item_touch_all);
////debug >>>
///
//debug <<<
//item_touch = null;
//debug >>>
////debug <<<
//print("item_touch_pointer = " + item_touch_pointer);
////debug >>>

////debug <<<
//if (item_touch_pointer)
//{
//    print("item_touch_pointer " + item_touch_pointer.name);
//}        
////debug >>>



////debug <<<
//print("================================================================================================== OnTriggerEnter");
//if (other) print("================================================================================================== OnTriggerEnter: " + other.gameObject.name);
////debug >>>

////debug <<<
//print("---------------------------- OnTriggerEnter: " + other.gameObject.name);
////debug >>>


//debug <<<
//if (gameObject.name == "RightHandAnchor") print("work mode = " + workmode + "  isCatch = " + isCatch + "  item_touch = " + item_touch);
//debug >>>



//// catch
//if (triggerState == "catch")
//{
//}

//// hold
//if (triggerState == "hold")
//{
//    move(item_touch);
//}

//// release
//if (triggerState == "release")
//{
//    drop(item_touch);
//}


//item_touch = other.gameObject.transform.parent.gameObject;
//debug <<<
//print("---------------------------------------------------------- item_touch = " + item_touch.name);
//debug >>>

//// awake
//private void Awake()
//{
//    m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
//}

//// isActive
//public bool isActive(string targetName)
//{
//    string touchName;
//    if (!item_touch)
//    {
//        touchName = null;
//    }
//    else
//    {
//        touchName = item_touch.name;
//    }
//    return isTrigger && (targetName == touchName);
//}


//if (m_GrabAction.GetStateDown(m_Pose.inputSource)) isTrigger = true;
//if (m_GrabAction.GetStateUp(m_Pose.inputSource)) isTrigger = false;


//public SteamVR_Action_Boolean m_GrabAction = null;
//private SteamVR_Behaviour_Pose m_Pose = null;


//// scale
//public void scale(Hand otherController, Interactable item)
//{
//    Vector3 q0, p1, p2, p0, p12;
//    Vector3 sca;
//    Quaternion rot;
//    float lambda;
//    Vector3 axis;
//    Quaternion M_rot;
//    float angle;

//    if (workmode != "scale")
//    {
//        q0_ = item.transform.position;
//        sca_ = item.transform.localScale;
//        rot_ = item.transform.rotation;
//        p1_ = transform.position;
//        p2_ = otherController.transform.position;
//        p0_ = (p1_ + p2_) / 2;
//        p12_ = p2_ - p1_;
//        otherController.workmode = "scale";
//        workmode = "scale";
//        return;
//    }

//    p1 = transform.position;
//    p2 = otherController.transform.position;
//    p0 = (p1 + p2) / 2;
//    p12 = p2 - p1;

//    (axis, angle) = calc_rot(p12_, p12);
//    angle = angle * 180 / (float)Math.PI;
//    if (float.IsNaN(angle)) return;
//    M_rot = Quaternion.AngleAxis(angle, axis);

//    lambda = p12.magnitude / p12_.magnitude;
//    sca = lambda * sca_;
//    q0 = p0 + lambda * (M_rot * (q0_ - p0_));
//    rot = M_rot * rot_;

//    item.transform.position = q0;
//    item.transform.localScale = sca;
//    item.transform.rotation = rot;

//    item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//    item.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
//}


//private (Vector3 axis, float angle) calc_rot(Vector3 xyz1, Vector3 xyz2)
//{
//    Vector3 axis;
//    float angle;
//    xyz1 = Vector3.Normalize(xyz1);
//    xyz2 = Vector3.Normalize(xyz2);
//    axis = Vector3.Normalize(Vector3.Cross(xyz1, xyz2));
//    angle = Mathf.Acos(Vector3.Dot(xyz1, xyz2));
//    return (axis, angle);
//}


//// move
//public void move(Interactable item)
//{
//    //if (!item) return;
//    if (!isInitialized)
//        {
//            Container = item.transform.parent.gameObject;
//            item.transform.position = Container.transform.position + item.transform.position - transform.position;
//            Container.GetComponent<FixedJoint>().connectedBody = item.GetComponent<Rigidbody>();
//            isInitialized = true;
//        }
//    Container.transform.position = transform.position;
//    m_Joint.connectedBody = Container.GetComponent<Rigidbody>();
//}


//// scale
//public void scale(Hand otherController)
//{
//    Vector3 p12_old, dp_old;
//    Vector3 p12_new, dp_new;
//    float lambda;        
//    Vector3 axis;
//    float angle;
//    float angle_min = 0.01f; //degree

//    // get positions
//    if (!item) return;
//    p1_new = transform.position;
//    p2_new = otherController.transform.position;
//    q0_new = item.transform.position;             

//    // initialize
//    if (!isStrechMode)
//    {
//        Container = item.transform.parent.gameObject;
//        Container.GetComponent<FixedJoint>().connectedBody = null;
//        isInitialized = false;
//        m_Joint.connectedBody = null;
//        item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//        item.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
//        p1_old = p1_new;
//        p2_old = p2_new;
//        q0_old = q0_new;
//        isStrechMode = true;
//        return;
//    }

//    p12_new = (p1_new + p2_new) / 2;
//    dp_new = p2_new - p1_new;
//    p12_old = (p1_old + p2_old) / 2;
//    dp_old = p2_old - p1_old;
//    lambda = dp_new.magnitude / dp_old.magnitude;
//    (axis, angle) = calc_rot(dp_old, dp_new);
//    angle = angle * 180 / (float)Math.PI;
//    if (float.IsNaN(angle) || Math.Abs(angle) < angle_min) return;

//    item.transform.position = p12_new + lambda * (q0_old - p12_old);
//    item.transform.RotateAround(p12_new, axis, angle);
//    item.transform.localScale *= lambda;
//    p1_old = p1_new;
//    p2_old = p2_new;
//    q0_old = q0_new;
//}

//debug <<<
//Debug.Log("    lambda = " + lambda + "  axis = " + axis.x + " " + axis.y + " " + axis.z + "     angle = " + angle); //p12_new.x + "  q0_old = " + q0_old.x + "  p12_old = " + p12_old.x);
//debug >>>

//private float d12_old;
//private float d12_new;
//    if (!isStrechMode)
//    {
//        //this.drop();
//        otherController.drop();

//        if (isInitialized)
//        {
//            Container = item.transform.parent.gameObject;
//            Container.GetComponent<FixedJoint>().connectedBody = null;
//            isInitialized = false;
//        }

//        //item = gameObject.GetComponent<Interactable>();  //???????
//        //Debug.Log("                              item = " + item.tag);


//        m_Joint.connectedBody = null;


//        item12 = item;
//        item12.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//        item12.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);


//        d12_old = (transform.position - otherController.transform.position).magnitude;
//        isStrechMode = true;
//        return;
//    }
//    d12_new = (transform.position - otherController.transform.position).magnitude;
//    Container = item12.transform.parent.gameObject;
//    //Container.transform.position = transform.position;
//    //            m_Joint.connectedBody = null;        
//    Container.transform.localScale *= d12_new / d12_old;
//    d12_old = d12_new;
//}



//// scale
//public void scale(Hand otherController)
//{
//    if (!isStrechMode)
//    {
//        //this.drop();
//        otherController.drop();

//        if (isInitialized)
//        {
//            Container = item.transform.parent.gameObject;
//            Container.GetComponent<FixedJoint>().connectedBody = null;
//            isInitialized = false;
//        }

//        //item = gameObject.GetComponent<Interactable>();  //???????
//        //Debug.Log("                              item = " + item.tag);


//        m_Joint.connectedBody = null;


//        item12 = item;
//        item12.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//        item12.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);


//        d12_old = (transform.position - otherController.transform.position).magnitude;
//        isStrechMode = true;
//        return;
//    }
//    d12_new = (transform.position - otherController.transform.position).magnitude;
//    Container = item12.transform.parent.gameObject;
//    //Container.transform.position = transform.position;
//    //            m_Joint.connectedBody = null;        
//    Container.transform.localScale *= d12_new / d12_old;
//    d12_old = d12_new;
//}



//private FixedJoint m_Joint2 = null;

//// case-1
//if (isTrigger && !isTrigger2)
//{            
//    Container.transform.position = transform.position;
//    m_Joint.connectedBody = Container.GetComponent<Rigidbody>();
//}

//// case-2
//if (!isTrigger && isTrigger2)
//{
//    // do nothing
//}

//// case-3
//if (isTrigger && isTrigger2)
//{
//    print(gameObject.CompareTag("HandL"));
//    if (gameObject.CompareTag("HandL"))
//    {

//        item.transform.position = Container.transform.position + item.transform.position - transform.position;
//        m_Joint.connectedBody = null;
//        d12_touch = (position_touch - otherController.GetComponent<Hand>().position_touch).sqrMagnitude;
//        d12 = (transform.position - otherController.transform.position).sqrMagnitude;
//        Container.transform.position = transform.position;
//        //Container.transform.localScale *= d12 / d12_touch;
//    }
//}

//// case-4
//if (!isTrigger && !isTrigger2)
//{
//    m_Joint.connectedBody = null;
//}

//// freeze
//item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//item.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);



//isMaster = (this.tag == "HandL");
//Debug.Log("tag = " + this.tag + "  isMaster = " + isMaster);






//    // up to dn
//    if (m_GrabAction.GetStateDown(m_Pose.inputSource))
//    {
//        isTrigger = true;
//        if (!isTrigger2)
//        {
//            Container = item.transform.parent.gameObject;
//            item.transform.position = Container.transform.position + item.transform.position - transform.position;
//            m_Joint2 = Container.GetComponent<FixedJoint>();
//            m_Joint2.connectedBody = item.GetComponent<Rigidbody>();
//        }
//        else
//        {
//            d12_touch = (transform.position - otherController.transform.position).sqrMagnitude;
//            Container = item.transform.parent.gameObject;
//            scale_touch = Container.transform.localScale;
//        }
//    }

//    // dn to up
//    if (m_GrabAction.GetStateUp(m_Pose.inputSource))
//    {
//        isTrigger = false;
//        Container = item.transform.parent.gameObject;
//        m_Joint2 = Container.GetComponent<FixedJoint>();
//        m_Joint2.connectedBody = null;
//    }

//    // holding
//    if (isTrigger)
//    {
//        if (!isTrigger2)
//        {
//            Container.transform.position = transform.position;
//            m_Joint.connectedBody = Container.GetComponent<Rigidbody>();
//        }
//        else
//        {
//            Container.transform.position = transform.position;
//            m_Joint.connectedBody = null;
//            d12 = (transform.position - otherController.transform.position).sqrMagnitude;
//            Container.transform.localScale = scale_touch * d12 / d12_touch;
//        }
//    }
//    else
//    {
//        m_Joint.connectedBody = null;
//        item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//        item.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
//    }
//}

////debug <<<
//Debug.Log("old hand" + this.tag);
////if (this.tag == "HandL") return;
//Debug.Log("new hand" + this.tag);
//            //debug >>>

//debug <<<
//Debug.Log("    [1] "  + this.tag + "    d12_touch = " + d12_touch);
//debug >>>           

//debug <<<
//Debug.Log("    [2] " + this.tag + "  d12 = " + d12 + "  d12_touch = " + d12_touch + "  isInitialized = " + isInitialized + "  isInitialized2 = " + isInitialized2);
//debug >>>

//// update
//private void Update()
//{
//    // check
//    if (!item) return;
//    if (m_GrabAction.GetStateDown(m_Pose.inputSource)) isTrigger = true;
//    if (m_GrabAction.GetStateUp(m_Pose.inputSource)) isTrigger = false;
//    //isTrigger2 = otherController.GetComponent<Hand>().isTrigger;
//    //isInitialized2 = otherController.GetComponent<Hand>().isInitialized2;

//    // case-1
//    if (isTrigger && !isTrigger2)
//    {
//        if (!isInitialized)
//        {
//            Container = item.transform.parent.gameObject;
//            item.transform.position = Container.transform.position + item.transform.position - transform.position;
//            m_Joint2 = Container.GetComponent<FixedJoint>();
//            m_Joint2.connectedBody = item.GetComponent<Rigidbody>();
//            scale_touch = Container.transform.localScale;
//            d12_touch = (transform.position - otherController.transform.position).sqrMagnitude;
//            isInitialized = true;
//        }
//        Container.transform.position = transform.position;
//        m_Joint.connectedBody = Container.GetComponent<Rigidbody>();
//    }

//    // case-2
//    if (isTrigger && isTrigger2)
//    {
//        if (!isInitialized && !isInitialized2)
//        {
//            Container = item.transform.parent.gameObject;
//            item.transform.position = Container.transform.position + item.transform.position - transform.position;
//            m_Joint2 = Container.GetComponent<FixedJoint>();
//            m_Joint2.connectedBody = item.GetComponent<Rigidbody>();
//            scale_touch = Container.transform.localScale;
//            d12_touch = (transform.position - otherController.transform.position).sqrMagnitude;
//            isInitialized = true;
//        }
//        if (isInitialized)
//        {
//            Container.transform.position = transform.position;
//            m_Joint.connectedBody = null;
//            d12 = (transform.position - otherController.transform.position).sqrMagnitude;
//            Container.transform.localScale = scale_touch * d12 / d12_touch;
//        }

//    }

//    // case-3
//    if (!isTrigger)
//    {
//        if (isInitialized)
//        {
//            Container = item.transform.parent.gameObject;
//            m_Joint2 = Container.GetComponent<FixedJoint>();
//            m_Joint2.connectedBody = null;
//            isInitialized = false;
//        }
//        m_Joint.connectedBody = null;
//        item.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
//        item.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
//    }

//}
