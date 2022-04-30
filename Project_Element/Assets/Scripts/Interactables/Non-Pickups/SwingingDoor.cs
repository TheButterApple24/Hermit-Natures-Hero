using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingDoor : InteractableBase
{
    [SerializeField] private float m_SwingingAnimTimer = 0.0f;
    [SerializeField] private Vector3 m_DoorRotation;
    [SerializeField] private bool m_IsOpen = false;

    [Header("Sound")]
    [SerializeField] private AudioClip m_OpenSFX;
    [SerializeField] private AudioClip m_CloseSFX;
    [SerializeField] private AudioSource m_AudioSource;

    public override void Activate()
    {
        m_IsActivated = true;

        if (!m_IsOpen)
        {
            m_AudioSource.PlayOneShot(m_OpenSFX);
        }
        else
        {
            m_AudioSource.PlayOneShot(m_CloseSFX);
        }
    }

    public override void Update()
    {
        base.Update();

        if (m_IsActivated)
        {
            if (m_IsOpen)
            {
                m_SwingingAnimTimer +=  Time.deltaTime;
                gameObject.transform.localRotation = Quaternion.Euler(Vector3.Lerp(m_DoorRotation, Vector3.zero, m_SwingingAnimTimer));

                if (m_SwingingAnimTimer >= 1.0f)
                {
                    m_IsOpen = false;
                    m_IsActivated = false;
                    m_SwingingAnimTimer = 0.0f;
                }
            }
            else if (!m_IsOpen)
            {
                m_SwingingAnimTimer += Time.deltaTime;
                gameObject.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, m_DoorRotation, m_SwingingAnimTimer));

                if (m_SwingingAnimTimer >= 1.0f)
                {
                    m_IsOpen = true;
                    m_IsActivated = false;
                    m_SwingingAnimTimer = 0.0f;
                }
            }
        }
        
    }
}
