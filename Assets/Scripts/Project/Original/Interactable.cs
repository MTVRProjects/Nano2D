////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using HMLFramwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // parameters
    public bool isGravity = false;
    public bool isKinematic = true;
    public bool isContained = false;
    public int N_grabber = 0;

    // start
    void Start()
    {
        gameObject.tag = "Interactable";
        var rb = gameObject.requireComponent<Rigidbody>();
        rb.useGravity = isGravity;
        rb.isKinematic = isKinematic;
    }
}
