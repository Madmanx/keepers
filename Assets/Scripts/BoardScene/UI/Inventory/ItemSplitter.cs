﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class ItemSplitter : MonoBehaviour {

    string regexPattern = "[^0-9]";
    public InputField inputFieldText;
    // inventaire from
    // inventaire to
    public InventoryOwner inventoryFrom;
    public InventoryOwner inventoryTo;
    int quantityToSend;
    public Transform originSlot;
    public Transform targetSlot;
    public ItemContainer selectedItem;
    public GameObject uiItem;

    void OnEnable()
    {
        inputFieldText.text = selectedItem.Quantity.ToString();
        uiItem.transform.position = Input.mousePosition;
        uiItem.GetComponent<DragHandler>().enabled = false;
    }

    void OnDisable()
    {
        UpdateInventories();
        inventoryFrom = null;
        inventoryTo = null;
        originSlot = null;
        targetSlot = null;
        quantityToSend = 0;
        inputFieldText.text = "0";
    }

    public void LeftArrow()
    {
        int parsedValue;
        if (inputFieldText.text != "0" && Int32.TryParse(inputFieldText.text, out parsedValue))
            inputFieldText.text = (parsedValue - 1).ToString();
    }

    public void RightArrow()
    {
        int parsedValue;
        if (inputFieldText.text != selectedItem.Quantity.ToString() && Int32.TryParse(inputFieldText.text, out parsedValue))
            inputFieldText.text = (parsedValue + 1).ToString();
    }

    public void InputFieldCapper()
    {
        if (inputFieldText.text.Length > 2)
            inputFieldText.text = inputFieldText.text.Substring(0, 2);

        Regex rgx = new Regex(regexPattern);
        inputFieldText.text = rgx.Replace(inputFieldText.text, "");
        if (inputFieldText.text.Length > 0 && Int32.Parse(inputFieldText.text) > selectedItem.Quantity)
            inputFieldText.text = selectedItem.Quantity.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Split();
    }

    public void Split()
    {
        if (inventoryFrom.transform.parent.gameObject.activeSelf && inventoryTo.transform.parent.gameObject.activeSelf)
        {
            quantityToSend = Int32.Parse(inputFieldText.text);

            int quantityLeft = 0;
            if (targetSlot.GetComponent<Slot>().hasAlreadyAnItem)
            {
                quantityLeft = InventoryManager.MergeStackables(targetSlot.GetComponent<Slot>().currentItem.GetComponent<ItemInstance>().ItemContainer, selectedItem, quantityToSend);
            }
            else
            {
                if (quantityToSend >= 0)
                {
                    if (InventoryManager.AddItemToInventory(inventoryTo.Owner.GetComponent<Behaviour.Inventory>().Items, new ItemContainer(selectedItem.Item, quantityToSend)))
                    {
                        selectedItem.Quantity -= quantityToSend;
                        quantityLeft = selectedItem.Quantity;
                    }
                }
            }

            if (quantityLeft <= 0)
            {
                InventoryManager.RemoveItem(inventoryFrom.Owner.GetComponent<Behaviour.Inventory>().Items, selectedItem);
            }

            if (inventoryFrom.Owner.GetComponent<LootInstance>() != null)
            {
                bool isEmpty = true;
                for (int i = 0; i < inventoryFrom.Owner.GetComponent<Behaviour.Inventory>().Items.Length; i++)
                {
                    if (inventoryFrom.Owner.GetComponent<Behaviour.Inventory>().Items[i] != null)
                    {
                        isEmpty = false;
                        break;
                    }
                }
                if (isEmpty)
                {
                    if (inventoryFrom.Owner.gameObject.GetComponentInChildren<Canvas>() != null)
                    {
                        inventoryFrom.Owner.gameObject.GetComponentInChildren<Canvas>().transform.SetParent(null);
                    }
                    Destroy(inventoryFrom.Owner.gameObject);
                    Destroy(inventoryFrom.GetComponentInParent<DragHandlerInventoryPanel>().gameObject);
                }
            }
        }
        gameObject.SetActive(false);
    }

    public void Swap()
    {
        InventoryManager.SwapItemBeetweenInventories(inventoryFrom.Owner.GetComponent<Behaviour.Inventory>().Items, originSlot.GetSiblingIndex(), inventoryTo.Owner.GetComponent<Behaviour.Inventory>().Items, targetSlot.GetSiblingIndex());
        gameObject.SetActive(false);
    }

    void UpdateInventories()
    {
        Destroy(uiItem);
        inventoryFrom.Owner.GetComponent<Behaviour.Inventory>().UpdateInventories();
        inventoryTo.Owner.GetComponent<Behaviour.Inventory>().UpdateInventories();
    }
}
