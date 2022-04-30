using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableActivator : MonoBehaviour
{
    [SerializeField] private InteractableBase m_Interactable;
    [SerializeField] private bool m_DestroyOnTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (m_Interactable != null)
            {
                m_Interactable.Activate();

                if (m_DestroyOnTrigger)
                    Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning(gameObject.name + " does not have an interactable to activate");
            }
        }
    }
}
