using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestActivator : MonoBehaviour
{
    [SerializeField] private Quest m_Quest;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (m_Quest != null)
            {
                m_Quest.OnAccepted();
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning(gameObject.name + " does not have a quest to activate");
            }
        }
    }
}
