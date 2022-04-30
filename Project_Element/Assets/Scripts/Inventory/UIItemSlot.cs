/*===================================================
 
Copyright 2021 Digital Donut Games. All Rights Reserved

Class:              UIItemSlot
Description:        Handles ItemSlot data and UIItem slots
Date Created:       26/11/2021
Author:             Jeffrey MacNab
Verified By:        Jeffrey MacNab [13/04/2022]

Changelog:

    26/11/2021
        - [Jeffrey] Created base class

 ===================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    [HideInInspector] public bool m_IsLinked = false;
    public ItemSlot m_ItemSlot;
    public Image m_SlotIcon;
    public GameObject m_Highlight;
    public GameObject EquippedIcon;
    public Text m_SlotAmount;
    [HideInInspector] public bool m_IsBeingHovered = false;
    [HideInInspector] public bool m_IsArmor = false;

    private void Awake()
    {

    }

    public void Update()
    {

        if (m_IsBeingHovered)
        {
            m_Highlight.SetActive(true);
        }
        else
        {
            m_Highlight.SetActive(false);
        }
    }


    public bool HasItem
    {
        get
        {
            if (m_ItemSlot == null)
            {
                return false;
            }
            else
            {
                return m_ItemSlot.HasItem;
            }
        }
    }

    public void Link(ItemSlot itemSlot)
    {
        m_ItemSlot = itemSlot;
        m_IsLinked = true;
        itemSlot.LinkUISlot(this);
        UpdateSlot();
    }

    public void UnLink()
    {
        m_ItemSlot.UnLinkUISlot();
        m_ItemSlot = null;
        m_IsLinked = false;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (m_ItemSlot != null && m_ItemSlot.HasItem)
        {
            m_SlotIcon.sprite = PlayerManager.Instance.Player.m_Inventory.InventoryDatabase[m_ItemSlot.m_Stack.ItemID].ItemIcon;
            m_SlotAmount.text = m_ItemSlot.m_Stack.ItemAmount.ToString();
            m_SlotIcon.enabled = true;
            m_SlotAmount.enabled = true;
        }
        else
        {
            Clear();
        }
    }

    public void Clear()
    {
        m_SlotIcon.sprite = null;
        m_SlotAmount.text = "";
        m_SlotIcon.enabled = false;
        m_SlotAmount.enabled = false;
    }

    private void OnDestroy()
    {
        if (m_IsLinked)
        {
            if (m_ItemSlot != null)
            {
                m_ItemSlot.UnLinkUISlot();
            }
        }
    }
}

public class ItemSlot
{
    public ItemStack m_Stack = null;
    UIItemSlot m_UIItemSlot = null;

    public ItemSlot(UIItemSlot uiItemSlot)
    {
        m_Stack = null;
        m_UIItemSlot = uiItemSlot;
        m_UIItemSlot.Link(this);

    }

    public void LinkUISlot(UIItemSlot uiSlot)
    {
        m_UIItemSlot = uiSlot;
    }

    public void UnLinkUISlot()
    {
        m_UIItemSlot = null;
    }

    public void EmptySlot()
    {
        m_Stack = null;
        if (m_UIItemSlot != null)
        {
            m_UIItemSlot.UpdateSlot();
        }
    }

    public void InsertStack(ItemStack stack)
    {
        m_Stack = stack;
        m_UIItemSlot.UpdateSlot();
    }

    public bool HasItem
    {
        get
        {
            if (m_Stack != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
