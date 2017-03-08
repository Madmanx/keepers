﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTrigger : MonoBehaviour {
    
    KeeperInstance ki;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("KeeperInstance"))
        {

            ki = other.gameObject.GetComponent<KeeperInstance>();
            InteractionImplementer InteractionImplementer = new InteractionImplementer();
            Direction eTrigger = Direction.None;

            string strTag = tag;

            switch (strTag)
            {
                case "NorthTrigger":
                    eTrigger = Direction.North;
                    break;
                case "NorthEastTrigger":
                    eTrigger = Direction.North_East;
                    break;
                case "SouthEastTrigger":
                    eTrigger = Direction.South_East;
                    break;
                case "SouthTrigger":
                    eTrigger = Direction.South;
                    break;
                case "SouthWestTrigger":
                    eTrigger = Direction.South_West;
                    break;
                case "NorthWestTrigger":
                    eTrigger = Direction.North_West;
                    break;
                default:
                    eTrigger = Direction.None;
                    break;
            }

            if (eTrigger != Direction.None && GetComponentInParent<Tile>().Neighbors[(int)eTrigger] != null )
            {
                if ( ki.ActionPoints > 0)
                {
                    IngameUI ui = GameObject.Find("IngameUI").GetComponent<IngameUI>();
                    if (GetComponentInParent<Tile>().Neighbors[(int)eTrigger].State == TileState.Discovered)
                    {
                        InteractionImplementer.Add(new Interaction(Move), "Move", GameManager.Instance.Ui.spriteMove, true, (int)eTrigger);
                        ui.UpdateActionPanelUIQ(InteractionImplementer);
                    }

                    if (GetComponentInParent<Tile>().Neighbors[(int)eTrigger].State == TileState.Greyed)
                    {
                        InteractionImplementer.Add(new Interaction(Explore), "Explore", GameManager.Instance.Ui.spriteExplore, true, (int)eTrigger);
                        ui.UpdateActionPanelUIQ(InteractionImplementer);
                    }
                }
                else
                {
                    GameManager.Instance.Ui.ZeroActionTextAnimation();
                }
            }
        }
   
    }


    void Move(int _i)
    {
        TileManager.Instance.MoveKeeper(ki, TileManager.Instance.GetTileFromKeeper[ki], (Direction)_i);

        GameManager.Instance.SelectedKeeperNeedUpdate = true;
        GameManager.Instance.ShortcutPanel_NeedUpdate = true;
    }

    void Explore(int _i)
    {
        //Check if the prisoner is following
        PrisonerInstance prisoner = null;
        if (ki.Keeper.GoListCharacterFollowing.Count > 0 && ki.Keeper.GoListCharacterFollowing[0].GetComponent<PrisonerInstance>())
        {
            prisoner = ki.Keeper.GoListCharacterFollowing[0].GetComponent<PrisonerInstance>();
        }

        // Move to explored tile
        TileManager.Instance.MoveKeeper(ki, TileManager.Instance.GetTileFromKeeper[ki], (Direction)_i);
        // Tell the tile it has been discovered (and watch it panic)
        Tile exploredTile = TileManager.Instance.GetTileFromKeeper[ki];
        exploredTile.State = TileState.Discovered;
        foreach (Tile t in exploredTile.Neighbors)
        {
            if (t != null && t.State == TileState.Hidden)
            {
                t.State = TileState.Greyed;
            }

        }

        // Apply exploration costs
        ki.CurrentHunger -= 5;
        //TODO: Apply this only when the discovered tile is unfriendly
        ki.CurrentMentalHealth -= 5;
        // If the player is exploring with the prisoner following, apply costs to him too
        if (prisoner != null)
        {
            prisoner.CurrentHunger -= 5;
            //TODO: Apply this only when the discovered tile is unfriendly
            prisoner.CurrentMentalHealth -= 5;
        }

        // Apply bad effects if monsters are discovered
        if (TileManager.Instance.MonstersOnTile.ContainsKey(exploredTile)
            && TileManager.Instance.MonstersOnTile[exploredTile] != null
            && TileManager.Instance.MonstersOnTile[exploredTile].Count > 0)
        {
            ki.CurrentHp -= 5;
            ki.CurrentMentalHealth -= 5;
            if (prisoner != null)
            {
                prisoner.CurrentHp -= 5;
                prisoner.CurrentMentalHealth -= 5;
            }
        }
        GameManager.Instance.SelectedKeeperNeedUpdate = true;
        GameManager.Instance.ShortcutPanel_NeedUpdate = true;
    }
}