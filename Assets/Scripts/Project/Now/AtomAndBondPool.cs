using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMLFramwork.Singleton;
using HMLFramwork;

public class AtomAndBondPool : SingleInstance<AtomAndBondPool>
{
    bool isInit = false;
    ObjectPool2 atomPool;
    ObjectPool2 bondPool;
    public void Init()
    {
        if (!isInit)
        {
            atomPool = new ObjectPool2(Resources.Load<GameObject>("Prefabs/Sphere"), 20, new GameObject("Atom_Root").transform);
            bondPool = new ObjectPool2(Resources.Load<GameObject>("Prefabs/Cylinder"), 20, new GameObject("Bond_Root").transform);

            isInit = true;
        }

    }

    public GameObject getAtom()
    {
        return atomPool.Pop();
    }

    public void RecycleAllAtom()
    {
        atomPool.PushAll();
    }

    public GameObject getBond()
    {
        return bondPool.Pop();
    }

    public void RecycleAllBond()
    {
        bondPool.PushAll();
    }

    public void RecycleAll()
    {
        atomPool.PushAll();
        bondPool.PushAll();
    }
}
