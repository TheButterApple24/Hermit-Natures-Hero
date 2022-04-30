using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGate : InteractableBase
{
    GameObject m_LeftHinge;
    GameObject m_RightHinge;
    GameObject m_GateLock;

    bool m_IsLocked = true;
    bool m_HasBeenUsed = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        m_LeftHinge = transform.GetChild(1).gameObject;
        m_RightHinge = transform.GetChild(2).gameObject;
        m_GateLock = transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Activate()
    {
        if (m_IsLocked == false && m_HasBeenUsed == false)
        {
            m_HasBeenUsed = true;

            Vector3 rot = m_RightHinge.transform.rotation.eulerAngles;
            rot -= m_RightHinge.transform.right * Time.deltaTime;

            m_RightHinge.transform.Rotate(-rot);

            Vector3 leftRot = m_LeftHinge.transform.rotation.eulerAngles;
            leftRot -= m_LeftHinge.transform.right * Time.deltaTime;

            m_LeftHinge.transform.Rotate(leftRot);
        }
    }

    public GameObject GateLock
    {
        get { return m_GateLock; }
        set { m_GateLock = value; }
    }

    public bool IsLocked
    {
        get { return m_IsLocked; }
        set { m_IsLocked = value; }
    }
}
