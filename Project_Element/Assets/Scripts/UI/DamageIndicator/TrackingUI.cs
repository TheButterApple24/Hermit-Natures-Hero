/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              TrackingUI
Description:        Component that tracks GameObject in Screen Space
Date Created:       19/03/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/03/2022
        - [Max] Started TrackingUI
    19/03/2022
        - [Max] Added Breakable objects

 ===================================================*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingUI : MonoBehaviour
{
    public GameObject m_TargetObject { get; set; }
    public Camera m_Camera { get; set; }

    float m_Offset = 0.0f;
    float m_Speed = 0.3f;

    private void FixedUpdate()
    {
        m_Offset += Time.deltaTime * m_Speed;
    }

    void LateUpdate()
    {
        if (m_TargetObject != null && m_Camera != null)
        {
            Vector3 pos = new Vector3();

            if (m_TargetObject.tag == "Enemy")
            {
                Enemy enemy = m_TargetObject.GetComponent<Enemy>();

                pos = m_Camera.WorldToScreenPoint(enemy.DmgNumbersLocation.position + new Vector3(0.0f, m_Offset, 0.0f));

                if (pos.z > 0)
                {
                    transform.position = m_Camera.WorldToScreenPoint(enemy.DmgNumbersLocation.position + new Vector3(0.0f, m_Offset, 0.0f));
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }

            if (m_TargetObject.tag == "Breakable")
            {
                BreakableObject obj = m_TargetObject.GetComponent<BreakableObject>();

                pos = m_Camera.WorldToScreenPoint(obj.DmgNumbersLocation.position + new Vector3(0.0f, m_Offset, 0.0f));

                if (pos.z > 0)
                {
                    transform.position = m_Camera.WorldToScreenPoint(obj.DmgNumbersLocation.position + new Vector3(0.0f, m_Offset, 0.0f));
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }
        }
    }
}
