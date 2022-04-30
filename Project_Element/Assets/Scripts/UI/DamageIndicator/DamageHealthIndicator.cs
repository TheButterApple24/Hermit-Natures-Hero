/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              DamageHealthNumbers
Description:        Component that handles all damage and health UI numbers
Date Created:       02/02/2022
Author:             Maxime Dubois
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    02/02/2022
        - [Max] Started DamageHealthIndicator
    08/02/2022
        - [Max] Added Comments
    15/02/2022
        - [Max] Fixed Health Potion related bug
    18/02/2022
        - [Max] Refactored Player Damage Indicator
    17/03/2022
        - [Max] Modified Enemy Damage Numbers to appear on UI
    19/03/2022
        - [Max] Refactored class
        - [Max] Fixed Enemy Numbers

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageHealthIndicator : MonoBehaviour
{
    [SerializeField] Animator m_HealthAnimator;
    [SerializeField] Text m_Text;

    GameObject m_PlayerTarget;
    Vector3 m_Pos;
    float m_ActivationRange = 15.0f;

    Camera m_Camera;
    Transform m_EnemyTransform;


    Color m_NonCritColour;
    Color m_CritColour;
    Color m_HealthColourHealing;
    Color m_HealthColourDamage;

    public void PlayerInit(float value)
    {
        // Set Player Target
        m_PlayerTarget = PlayerManager.Instance.Player.gameObject;

        // Set Colours
        m_HealthColourHealing = new Color(0.0f, 0.90f, 0.25f, 1.0f);
        m_HealthColourDamage = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        // Destroy object if Menu is open
        if (m_PlayerTarget.GetComponent<Player>().m_IsMenuOpen)
        {
            Destroy(gameObject);
            return;
        }

        // Set Text settings
        if (m_Text != null)
        {
            m_Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            m_Text.alignment = (TextAnchor)TextAlignment.Center;
            m_Text.fontSize = 36;

            // Set appropriate Text color
            if (value < 0)
            {
                // Change text color
                m_Text.color = m_HealthColourDamage;

                // Set Text Component to Player Healing Numbers
                m_Text.text = "-" + Mathf.Abs(value).ToString();
            }
            else if (value > 0)
            {
                // Change text color
                m_Text.color = m_HealthColourHealing;

                // Set Text Component to Player Healing Numbers
                m_Text.text = "+" + Mathf.Abs(value).ToString();
            }
        }

        // Handle Animation
        StartCoroutine(HandleOnScreenNumbers(m_HealthAnimator, "IsPlayerHealthVisible"));
    }

    public void EnemyInit(float value, bool isCrit, Transform location, Enemy enemy = null)
    {
        // Set Player Target
        m_PlayerTarget = PlayerManager.Instance.Player.gameObject;

        // Don't show damage/health numbers if far enough to the enemy
        if (enemy != null)
        {
            float distance = Vector3.Distance(m_PlayerTarget.transform.position, enemy.transform.position);

            if (distance > m_ActivationRange)
            {
                return;
            }
        }

        // Set Enemy Transform
        m_EnemyTransform = location;

        // Set Colours
        m_NonCritColour = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        m_CritColour = new Color(0.84f, 0.74f, 0.0f, 1.0f);
        m_HealthColourHealing = new Color(0.0f, 0.90f, 0.25f, 1.0f);

        // Destroy object if Menu is open
        if (m_PlayerTarget.GetComponent<Player>().m_IsMenuOpen)
        {
            Destroy(gameObject);
            return;
        }

        // Set Text settings
        if (m_Text != null)
        {
            m_Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            m_Text.alignment = (TextAnchor)TextAlignment.Center;
            m_Text.fontSize = 42;

            if (value < 0)
            {
                // Set Text color
                if (isCrit)
                {
                    m_Text.color = m_CritColour;
                }
                else
                {
                    m_Text.color = m_NonCritColour;
                }
            }
            else
            {
                m_Text.color = m_HealthColourHealing;
            }


            // Assign Text value
            m_Text.text = Mathf.Abs(value).ToString();
        }

        // Get main Camera
        m_Camera = GameObject.Find("Camera").GetComponent<Camera>();

        // Assign main Camera to TrackingUI
        GetComponentInChildren<TrackingUI>().m_Camera = m_Camera;

        // Set Text location to Enemy location in Screen Point
        if (m_Camera != null)
        {
            m_Pos = m_Camera.WorldToScreenPoint(m_EnemyTransform.position);
            m_Text.gameObject.transform.position = m_Pos;
        }

        // Handle Animation Timer
        StartCoroutine(HandleTimer());
    }

    IEnumerator HandleOnScreenNumbers(Animator anim, string parameterName)
    {
        // Enable appropriate UI numbers
        anim.SetBool(parameterName, true);

        // Wait 1 sec
        yield return new WaitForSeconds(0.5f);

        // Disable appropriate UI numbers
        anim.SetBool(parameterName, false);

        // Destroy this gameObject
        Destroy(gameObject);
    }

    IEnumerator HandleTimer()
    {
        // Wait 1 sec
        yield return new WaitForSeconds(0.5f);

        // Destroy this gameObject
        Destroy(gameObject);
    }
}
