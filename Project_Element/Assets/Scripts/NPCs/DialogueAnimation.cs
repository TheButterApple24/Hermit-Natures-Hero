using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimation : MonoBehaviour
{
    public float ChangeFrequencyInSeconds = 1f;
    public Animator DialogueAnimator;
    int m_TalkIndex = 0;
    float m_CurrenTime = 0f;

    private void Update()
    {
        m_CurrenTime += ChangeFrequencyInSeconds * Time.deltaTime;

        if (m_CurrenTime >= ChangeFrequencyInSeconds)
        {
            m_TalkIndex = Random.Range(0, 5);
            DialogueAnimator.SetInteger("TalkIndex", m_TalkIndex);
            m_CurrenTime = 0;
        }
    }
}
