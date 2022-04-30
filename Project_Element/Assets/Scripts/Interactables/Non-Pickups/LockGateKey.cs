using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGateKey : InteractableBase
{
    public LockGate m_LockGate;

    public override void Activate()
    {
        if (m_LockGate != null)
        {
            m_LockGate.GateLock.SetActive(false);
            m_LockGate.IsLocked = false;
            gameObject.SetActive(false);
        }
    }
}
