﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script is only here to stock a KeeperInstance to process it elsewhere
/// </summary>
public class UIKeeperInstance : MonoBehaviour, IPointerClickHandler
{
    public KeeperInstance keeperInstance;

    public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;

        if (tap == 2)
        {
            int freeSlot = -1;
            SelectBattleCharactersPanelHandler sbcPanelHandler = GetComponentInParent<SelectBattleCharactersPanelHandler>();
            if (transform.parent.parent == sbcPanelHandler.transform.GetChild((int)SelectBattleCharactersScreenChildren.CharactersSelected))
            {
                freeSlot = sbcPanelHandler.FindFreeSlotInCharactersOnTile();
                if (freeSlot != -1)
                {
                    transform.SetParent(sbcPanelHandler.transform.GetChild((int)SelectBattleCharactersScreenChildren.CharactersOnTile).GetChild(freeSlot));
                }
            }
            else
            {
                freeSlot = sbcPanelHandler.FindFreeSlotInSelection();
                if (freeSlot != -1)
                {
                    transform.SetParent(sbcPanelHandler.transform.GetChild((int)SelectBattleCharactersScreenChildren.CharactersSelected).GetChild(freeSlot));
                }
            }

        }
    }
}
