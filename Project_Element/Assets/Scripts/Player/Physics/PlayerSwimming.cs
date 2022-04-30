/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              PlayerSwimming
Description:        Handles how the player will swim when in water
Date Created:       19/01/2022
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    19/01/2022
        - [Jeffrey] Created base implementation

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

public class PlayerSwimming : MonoBehaviour
{
    public ThirdPersonController m_Controller;
    public float m_DepthBeforeSubmerged = 1.0f;
    public float m_DisplacementAmount = 3.0f;
    public float m_CameraHeight;

    public float m_WaterHeight;

    public Transform m_SpringArm;

    public float m_OldCameraY;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Controller.m_MovementState == MovementState.Swimming)
        {
            if (m_Controller.m_Inputs == Vector3.zero)
            {
                m_Controller.m_Player.m_StaminaComp.DrainStamina(2.0f * Time.deltaTime);
            }
            else
            {
                m_Controller.m_Player.m_StaminaComp.DrainStamina(5.0f * Time.deltaTime);
            }

            Vector3 newPos = m_SpringArm.position;

            newPos.y = m_CameraHeight;

            m_SpringArm.position = newPos;


            if (m_Controller.m_Player.m_StaminaComp.m_CurrentStamina <= 0)
            {
                m_Controller.m_Player.OnDeath();

            }
        }

    }

    private void FixedUpdate()
    {
        if (m_Controller.gameObject.transform.position.y < m_WaterHeight && m_Controller.m_MovementState == MovementState.Swimming)
        {
            float displacementMultiplier = Mathf.Clamp01((m_WaterHeight - m_Controller.gameObject.transform.position.y) / m_DepthBeforeSubmerged) * m_DisplacementAmount;
            m_Controller.m_RigidBody.AddForce(new Vector3(0.0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0.0f), ForceMode.Acceleration);
        }
    }
}
