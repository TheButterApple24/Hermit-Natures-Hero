/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              ObjectPool
Description:        Stores gameobjects to be used for the PoolManager
Date Created:       19/01/22
Author:             Gerard Hogue
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/22
        - [Gerard] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObjectPool
{
    public Type PoolType;
    public string ObjectName = "";
    public float lastUsed = 0.0f;
    List<GameObject> m_PoolGameObjects = new List<GameObject>();

    public void ClearObjectPool()
    {
        m_PoolGameObjects.Clear();
    }

    public int ObjectCount
    {
        get { return m_PoolGameObjects.Count; }
    }

    public GameObject GetObject(GameObject prefab)
    {
        GameObject retrievedObject = null;

        var disabledObjects = m_PoolGameObjects.Where(x => x.activeSelf == false);

        //Debug.Log(prefab);

        if (disabledObjects.Any())
        {
            retrievedObject = disabledObjects.First();
        }
        else
        {
            retrievedObject = CreateNewObject(prefab);
        }

        retrievedObject.gameObject.SetActive(true);

        lastUsed = Time.timeSinceLevelLoad;

        return retrievedObject;
    }
    public void RemoveDisabledObjects()
    {

        var disabledPoolObjects = m_PoolGameObjects.Where(poolObject => poolObject.gameObject.activeSelf == false).ToList();

        m_PoolGameObjects = m_PoolGameObjects.Where(poolObject => poolObject.gameObject.activeSelf == true).ToList();


        if (disabledPoolObjects != null)
        {
            foreach (GameObject poolObject in disabledPoolObjects)
            {
                GameObject.Destroy(poolObject);
            }
        }
        lastUsed = Time.timeSinceLevelLoad;
    }

    public GameObject CreateNewObject(GameObject prefab)
    {
        GameObject returnObject = PoolManager.ObjectCreator(prefab);
        m_PoolGameObjects.Add(returnObject);

        return returnObject;
    }
}
