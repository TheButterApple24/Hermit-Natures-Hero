using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : InteractableBase
{
    [SerializeField] private float m_HealAmount = 5;
    [SerializeField] private bool m_IsMaterialRandomized = true;
    [SerializeField] private List<Material> m_MaterialsToRandomize;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        m_RemoveFromListOnTrigger = true;

        if (m_IsMaterialRandomized && m_MaterialsToRandomize.Count > 0)
        {
            GetComponent<MeshRenderer>().material = m_MaterialsToRandomize[Random.Range(0, m_MaterialsToRandomize.Count)];
        }
    }
    public override void Start()
    {
        base.Start();

        m_RemoveFromListOnTrigger = true;

        if (m_IsMaterialRandomized && m_MaterialsToRandomize.Count > 0)
        {
            GetComponent<MeshRenderer>().material = m_MaterialsToRandomize[Random.Range(0, m_MaterialsToRandomize.Count)];
        }
    }

    public override void Activate()
    {
        PlayerManager.Instance.Player.m_HealthComp.ModifyHealth(m_HealAmount, false);
        Destroy(gameObject);
    }
}
