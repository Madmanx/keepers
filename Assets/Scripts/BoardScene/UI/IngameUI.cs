﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class IngameUI : MonoBehaviour
{
    // KeeperPanel
    [Header("Character Panel")]
    //public GameObject goSelectedKeeperPanel;
    // Inventory
    public GameObject goInventory;
    public GameObject goEquipement;
    public GameObject goStats;
    public GameObject keeper_inventory_prefab;
    public GameObject panel_keepers_inventory;
    public GameObject slotPrefab;
    public GameObject itemUI;

    // Turn Panel
    [Header("Turn Panel")]
    public GameObject TurnPanel;
    public GameObject TurnButton;
    public float buttonRotationSpeed = 1.0f;

    // ActionPanel
    [Header("Action Panel")]
    public GameObject goActionPanelQ;
    public GameObject baseActionImage;

 
    // ShortcutPanel
    [Header("ShortcutPanel Panel")]
    public GameObject baseKeeperShortcutPanel;
    public GameObject goShortcutKeepersPanel;

    // Quentin
    //public List<GameObject> listGoActionPanelButton;

    [HideInInspector]
    public bool isTurnEnding = false;

    public void Start()
    {
        CreateShortcutPanel();
        CreateKeepersInventoryPanels();
    }

    public void Update()
    {
        if (GameManager.Instance != null )
        {
            if (GameManager.Instance.SelectedKeeperNeedUpdate)
            {
                UpdateSelectedKeeperPanel();
            }
            if (GameManager.Instance.ShortcutPanel_NeedUpdate)
            {
                UpdateShortcutPanel();
            }
        }
    }

    // TODO : optimise
    /*
    public void UpdateActionPanelUIOptimize(InteractionImplementer ic)
    {
        if (GameManager.Instance == null) { return; }
        if (goActionPanelQ == null) { return; }

        goActionPanelQ.GetComponent<RectTransform>().position = (Input.mousePosition) + new Vector3(30.0f, 0.0f);

        int i;

        for (i = 0; i < ic.listActionContainers.Count; ++i)
        {
            int n = i;

            if (i < listGoActionPanelButton.Count && listGoActionPanelButton[i] != null)
            {
                listGoActionPanelButton[i].name = ic.listActionContainers[i].strName;

                Button btn = listGoActionPanelButton[i].GetComponent<Button>();

                btn.onClick.RemoveAllListeners();  

                btn.onClick.AddListener(() => { ic.listActionContainers[n].action(); });

                btn.GetComponentInChildren<Text>().text = ic.listActionContainers[i].strName;
            }
            else
            {
                GameObject goAction = Instantiate(baseActionImage, goActionPanelQ.transform);

                goAction.name = ic.listActionContainers[i].strName;

                Button btn = goAction.GetComponent<Button>();

                btn.onClick.AddListener(() => { ic.listActionContainers[n].action(); });

                btn.GetComponentInChildren<Text>().text = ic.listActionContainers[i].strName;

                listGoActionPanelButton.Add(goAction);
            }
        }

        for(int k = listGoActionPanelButton.Count-1; k >= i; k--)
        {
            GameObject goTemp = listGoActionPanelButton[k];
            listGoActionPanelButton.Remove(goTemp);
            Destroy(goTemp);
        }

    }*/

    #region Action
    public void UpdateActionPanelUIQ(InteractionImplementer ic)
    {
        if (GameManager.Instance == null) { return; }
        if (goActionPanelQ == null) { return; }

        //Clear
        ClearActionPanel();

        goActionPanelQ.GetComponent<RectTransform>().position = (Input.mousePosition) + new Vector3(30.0f, 0.0f);

        // Actions
        for (int i = 0; i < ic.listActionContainers.Count; i++)
        {
            bool bIsForbiden = ic.listActionContainers[i].strName == "Escort" && !GameManager.Instance.ListOfSelectedKeepers[0].isEscortAvailable;
            bIsForbiden = bIsForbiden || ic.listActionContainers[i].strName == "Unescort" && GameManager.Instance.ListOfSelectedKeepers[0].isEscortAvailable;
            if (!bIsForbiden)
            {
                GameObject goAction = Instantiate(baseActionImage, goActionPanelQ.transform);
                goAction.name = ic.listActionContainers[i].strName;

                goAction.GetComponent<RectTransform>().position = (Input.mousePosition);

                Button btn = goAction.GetComponent<Button>();

                int n = i;
                int iParam = ic.listActionContainers[n].iParam;
                btn.onClick.AddListener(() => {
                    ic.listActionContainers[n].action(iParam);
                    GameObject.Find("IngameUI").GetComponent<IngameUI>().ClearActionPanel();
                });

                btn.GetComponentInChildren<Text>().text = ic.listActionContainers[i].strName;
            }
        }   
    }

    public void ClearActionPanel()
    {
        if (goActionPanelQ.GetComponentsInChildren<Image>().Length > 0)
        {
            foreach (Image ActionPanel in goActionPanelQ.GetComponentsInChildren<Image>())
            {
                Destroy(ActionPanel.gameObject);
            }
        }
    }
    #endregion

    #region Turn
    // TODO: @Rémi bouton à corriger (on ne doit pas pouvoir cliquer 2x de suite)
    public void EndTurn()
    {
        if (!isTurnEnding)
        {
            AnimateButtonOnClick();
        }
    }
    
    
    private void AnimateButtonOnClick()
    {
        // Activation de l'animation au moment du click
        Animator anim_button = TurnButton.GetComponent<Animator>();
  
        anim_button.speed = buttonRotationSpeed;
        anim_button.enabled = true;
        isTurnEnding = false;
    }
    #endregion

    #region ShortcutPanel
    public void ToogleShortcutPanel()
    {
        goShortcutKeepersPanel.SetActive(!goShortcutKeepersPanel.activeSelf);
    }

    public void CreateShortcutPanel()
    {
        if (GameManager.Instance == null) { return; }
        if (goShortcutKeepersPanel == null) { return; }

        int nbCaracters = GameManager.Instance.AllKeepersList.Count;

        for (int i = 0; i < nbCaracters; i++)
        {
            KeeperInstance currentSelectedCharacter = GameManager.Instance.AllKeepersList[i];

            Sprite associatedSprite = currentSelectedCharacter.Keeper.AssociatedSprite;

            if (associatedSprite != null)
            {
                GameObject goKeeper = Instantiate(baseKeeperShortcutPanel, goShortcutKeepersPanel.transform);

                goKeeper.name = "Panel_Shortcut_" + currentSelectedCharacter.Keeper.CharacterName;
                goKeeper.transform.GetChild(0).GetComponent<Image>().sprite = associatedSprite;
                goKeeper.transform.localScale = Vector3.one;

                UpdateShortcutPanel();
            }
        }
    }

    public void UpdateShortcutPanel()
    {
        if (GameManager.Instance == null) { return; }
        if (goShortcutKeepersPanel == null) { return; }

        // nb Character + Ashley
        for (int i = 0; i < goShortcutKeepersPanel.transform.childCount; i++)
        {
            if (i == 0)
            {
                //Prisoner prisonner = GameManager.Instance.Prisonner ;
                //// Update HP
                //goShortcutKeepersPanel.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)prisonner.CurrentHp / (float)prisonner.MaxHp;
                //// Update Hunger
                //goShortcutKeepersPanel.transform.GetChild(i).GetChild(1).GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)prisonner.ActualHunger / (float)prisonner.MaxHunger;
            }
            else
            {
                KeeperInstance currentCharacter = GameManager.Instance.AllKeepersList[i-1];

                if (currentCharacter != null)
                {
                    int f = 1;
                    // Update HP
                    goShortcutKeepersPanel.transform.GetChild(i).GetChild(f++).GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)currentCharacter.CurrentHp / (float)currentCharacter.Keeper.MaxHp;
                    // Update Hunger
                    goShortcutKeepersPanel.transform.GetChild(i).GetChild(f++).GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)currentCharacter.CurrentHunger / (float)currentCharacter.Keeper.MaxHunger;
                    // Update MentalHealth
                    goShortcutKeepersPanel.transform.GetChild(i).GetChild(f++).GetChild(0).gameObject.GetComponent<Image>().fillAmount = (float)currentCharacter.CurrentMentalHealth / (float)currentCharacter.Keeper.MaxMentalHealth;

                    // Update Action Points
                    goShortcutKeepersPanel.transform.GetChild(i).GetChild(f).gameObject.GetComponentInChildren<Text>().text = currentCharacter.ActionPoints.ToString();
                }
            }
        }

        GameManager.Instance.ShortcutPanel_NeedUpdate = false;
    }


    #endregion

    #region SelectedKeeper
    public void UpdateSelectedKeeperPanel()
    {
        if (GameManager.Instance == null) { return; }
        if (goInventory == null) { return; }

        KeeperInstance currentSelectedKeeper = GameManager.Instance.ListOfSelectedKeepers[0];

        Sprite associatedSprite = currentSelectedKeeper.Keeper.AssociatedSprite;

        foreach (ItemInstance holder in goInventory.transform.GetComponentsInChildren<ItemInstance>())
        {
            DestroyImmediate(holder.gameObject);
        }
        if (associatedSprite != null)
        {
            goInventory.name = "Panel_Inventory" + currentSelectedKeeper.Keeper.CharacterName;
            goStats.GetComponentInChildren<Image>().sprite = associatedSprite;

            if (currentSelectedKeeper.Inventory != null)
            {
                Item[] inventory = currentSelectedKeeper.Inventory;

                for (int i = 0; i < inventory.Length; i++)
                {
                    if(inventory[i] != null)
                    {
                        GameObject currentSlot = goInventory.transform.GetChild(i).gameObject;
                        GameObject go = Instantiate(itemUI);
                        go.transform.SetParent(currentSlot.transform);
                        go.GetComponent<ItemInstance>().item = inventory[i];
                        go.name = inventory[i].ToString();


                        go.GetComponent<Image>().sprite = inventory[i].sprite;
                        go.transform.localScale = Vector3.one;

                        go.transform.position = currentSlot.transform.position;
                        go.transform.SetAsFirstSibling();

                        if (go.GetComponent<ItemInstance>().item.GetType() == typeof(Consummable))
                        {
                            go.transform.GetComponentInChildren<Text>().text = ((Consummable)inventory[i]).quantite.ToString();
                        }
                    }
                }
            }
        }

        GameManager.Instance.SelectedKeeperNeedUpdate = false;
    }
    #endregion

    #region Keepers_Inventory
    public void CreateKeepersInventoryPanels()
    {
        foreach (KeeperInstance ki in GameManager.Instance.AllKeepersList)
        {
            GameObject goInventoryKeeper = Instantiate(keeper_inventory_prefab, panel_keepers_inventory.transform);
            goInventoryKeeper.transform.localPosition = Vector3.zero;
            goInventoryKeeper.transform.GetChild(1).GetComponent<Image>().sprite = ki.Keeper.AssociatedSprite;
            goInventoryKeeper.name = "Inventory_" + ki.Keeper.CharacterName;
            goInventoryKeeper.SetActive(false);

            int maxSlots = ki.Keeper.MaxInventorySlots;
            for (int i = 0; i < maxSlots; i++)
            {
                //Create Slots
                GameObject currentgoSlotPanel = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                currentgoSlotPanel.transform.SetParent(goInventoryKeeper.transform.GetChild(0).transform);

                currentgoSlotPanel.transform.localPosition = Vector3.zero;
                currentgoSlotPanel.transform.localScale = Vector3.one;
                currentgoSlotPanel.name = "Slot" + i;
            }
        }
    }


    internal void ShowInventoryPanels()
    {
        panel_keepers_inventory.transform.GetChild(GameManager.Instance.GoTarget.transform.GetSiblingIndex()).gameObject.SetActive(true);
    }

    internal void HideInventoryPanels()
    {
        for (int i = 0; i <GameManager.Instance.AllKeepersList.Count; i++)
        {
            panel_keepers_inventory.transform.GetChild(i).gameObject.SetActive(false);
        }
  
    }

    internal void UpdateKeeperInventoryPanel()
    {
        if (GameManager.Instance == null) { return; }
        if (panel_keepers_inventory == null) { return; }

        foreach( KeeperInstance ki in GameManager.Instance.AllKeepersList)
        {
            foreach (ItemInstance holder in panel_keepers_inventory.transform.GetChild(ki.gameObject.transform.GetSiblingIndex()).transform.GetChild(0).transform.GetComponentsInChildren<ItemInstance>())
            {
                DestroyImmediate(holder.gameObject);
            }
            if (ki.Inventory != null)
            {
                Item[] inventory = ki.Inventory;

                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] != null)
                    {

                        GameObject currentSlot = panel_keepers_inventory.transform.GetChild(ki.gameObject.transform.GetSiblingIndex()).transform.GetChild(0).transform.GetChild(i).gameObject;
                        GameObject go = Instantiate(itemUI);
                        go.transform.SetParent(currentSlot.transform);
                        go.GetComponent<ItemInstance>().item = inventory[i];
                        go.name = inventory[i].ToString();


                        go.GetComponent<Image>().sprite = inventory[i].sprite;
                        go.transform.localScale = Vector3.one;

                        go.transform.position = currentSlot.transform.position;
                        go.transform.SetAsFirstSibling();

                        if (go.GetComponent<ItemInstance>().item.GetType() == typeof(Consummable))
                        {
                            go.transform.GetComponentInChildren<Text>().text = ((Consummable)inventory[i]).quantite.ToString();
                        }
                    }
                }

            }
        }
        
        
    }
    #endregion
}
