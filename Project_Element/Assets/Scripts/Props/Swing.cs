using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField] private Vector3 m_TargetRotation;
    private Vector3 m_StartingRotation;

    public void Start()
    {
        m_StartingRotation = gameObject.transform.rotation.eulerAngles;
    }

    public void Update()
    {
        gameObject.transform.localRotation = Quaternion.Euler(m_TargetRotation * Mathf.Sin(Time.time));
    }
}
