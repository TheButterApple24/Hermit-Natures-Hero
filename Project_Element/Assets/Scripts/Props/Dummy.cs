using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : BreakableObject
{
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        m_HealthComp.MaxHealth = int.MaxValue;
    }

    // Update is called once per frame
    public override void Break()
    {
        m_HealthComp.MaxHealth = int.MaxValue;
    }
}
