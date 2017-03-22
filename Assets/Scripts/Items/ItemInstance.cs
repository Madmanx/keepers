﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInstance : MonoBehaviour, IHavestable
{
    private InteractionImplementer interactionImplementer;

    [SerializeField]
    private ItemContainer itemContainer = null;

    public GlowObjectCmd GlowCmd;

    [SerializeField]
    int quantity = 1;

    [SerializeField]
    private bool isInScene = false;

    [SerializeField]
    private string idItem;

    void Awake()
    {
        if (isInScene)
        {
            Init(idItem, quantity);
        }
        interactionImplementer = new InteractionImplementer();
        interactionImplementer.Add(new Interaction(Harvest), "Harvest", GameManager.Instance.Ui.spriteHarvest);
    }


    public void Init(string _IdItem, int _iNb)
    {
        idItem = _IdItem;
        itemContainer = new ItemContainer(GameManager.Instance.Database.getItemById(_IdItem), quantity);
        //Vector3 v3Pos = transform.localPosition;
        //Quaternion quat = transform.localRotation;
        if (itemContainer.Item.IngameVisual != null)
        {
            //Destroy(transform.GetChild(0).gameObject);
            GameObject go = Instantiate(itemContainer.Item.IngameVisual, transform);
            go.transform.localPosition = go.transform.GetChild(0).localPosition = Vector3.zero;//transform.parent.localPosition; //Vector3.one;
            go.transform.localRotation = Quaternion.identity;//transform.parent.localRotation; //Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.Log("Pas de Visuel Ingame pour l'item :\"" + itemContainer.Item.ItemName +"\"");
        }


    }

    public ItemContainer ItemContainer
    {
        get
        {
            return itemContainer;
        }

        set
        {
            itemContainer = value;
        }
    }

    public InteractionImplementer InteractionImplementer
    {
        get
        {
            return interactionImplementer;
        }

        set
        {
            interactionImplementer = value;
        }
    }

    public void Harvest(int _i = 0)
    {
        if (GameManager.Instance.ListOfSelectedKeepers[0].ActionPoints > 0)
        {
            int nbSlot = GameManager.Instance.ListOfSelectedKeepers[0].Keeper.nbSlot;
            bool isNoLeftOver = InventoryManager.AddItemToInventory(GameManager.Instance.ListOfSelectedKeepers[0].GetComponent<Inventory>().List_inventaire, nbSlot, itemContainer);
            if (isNoLeftOver)
            {

                Destroy(this);
                GlowController.UnregisterObject(GlowCmd);
                if (this.transform.childCount > 0)
                {
                    DestroyImmediate(this.transform.GetChild(0).gameObject);
                }

            }
            GameManager.Instance.ListOfSelectedKeepers[0].ActionPoints -= 1;
            GameManager.Instance.Ui.UpdateSelectedKeeperPanel();
            GameManager.Instance.Ui.UpdateInventoryPanel(GameManager.Instance.ListOfSelectedKeepers[0].gameObject);
        }
        else
        {
            GameManager.Instance.Ui.ZeroActionTextAnimation();
        }
    }
    public void OnMouseOver()
    {
        GameManager.Instance.Ui.UiIconFeedBack.TriggerFeedback(itemContainer.Item.InventorySprite);
    }
    public void OnMouseExit()
    {
        GameManager.Instance.Ui.UiIconFeedBack.DisableFeedback();
    }
       

}