﻿using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BattleHandler {
    private enum AttackType { Physical, Magical }
    // Is the prisoner on the tile where the battle is processed
    public static bool isPrisonerOnTile = false;
    private static Text battleLogger;

    // Debug parameters
    private static bool isDebugModeActive = false;

    /// <summary>
    /// Autoselect keepers if there are not enough for a selection
    /// </summary>
    /// <param name="tile"></param>
    public static void StartBattleProcess(Tile tile)
    {
        // Auto selection
        if (TileManager.Instance.KeepersOnTile[tile].Count <= 1)
        {
            List<KeeperInstance> keepersForBattle = TileManager.Instance.KeepersOnTile[tile];
            if (TileManager.Instance.PrisonerTile == tile)
            {
                isPrisonerOnTile = true;
            }

            LaunchBattle(tile, keepersForBattle);
        }
        // Manual selection
        else
        {
            GameManager.Instance.OpenSelectBattleCharactersScreen(tile);
        }
    }


    /// <summary>
    /// Battle entry point, called to start the battle.
    /// </summary>
    /// <param name="tile">Tile where the battle happens</param>
    /// <param name="selectedKeepersForBattle">Keepers selected for the battle</param>
    public static void LaunchBattle(Tile tile, List<KeeperInstance> selectedKeepersForBattle)
    {
        battleLogger = GameManager.Instance.BattleResultScreen.GetChild((int)BattleResultScreenChildren.Logger).GetComponentInChildren<Text>();
        bool isVictorious = ResolveBattle(selectedKeepersForBattle, tile);
        if (isVictorious)
        {
            HandleBattleVictory(selectedKeepersForBattle, tile);
        }
        else
        {
            HandleBattleDefeat(selectedKeepersForBattle);
        }

        PrintResultsScreen(isVictorious);
        PostBattleCommonProcess(selectedKeepersForBattle, tile);
    }

    /*
     * Auto resolve battle. Will later be replaced by EngageBattle. Returns true if the battle is won, else false. 
     */
    private static bool ResolveBattle(List<KeeperInstance> keepers, Tile tile)
    {
        List<MonsterInstance> monsters = new List<MonsterInstance>();
        monsters.AddRange(TileManager.Instance.MonstersOnTile[tile]);

        // General melee!
        int totalDamageTaken = 0;

        while (monsters.Count > 0 && totalDamageTaken < 50)
        {
            int totalTurns = keepers.Count + monsters.Count;
            int turnCounter = 0;
            while (turnCounter < totalTurns)
            {
                if ((turnCounter + 1) % 2 == 1
                    && (turnCounter + 1) <= (keepers.Count * 2) - 1)
                {
                    // Keeper turn
                    KeeperInstance attacker = keepers[turnCounter / 2];
                    MonsterInstance target;
                    AttackType attackType = attacker.Keeper.GetEffectiveStrength() > attacker.Keeper.GetEffectiveIntelligence() ? AttackType.Physical : AttackType.Magical;

                    target = GetTargetForAttack(monsters, attackType);
                    // Inflict damage to target
                    KeeperDamageCalculation(attacker, target, attackType);

                    // Remove monster from list if dead
                    if (target.CurrentHp <= 0)
                    {
                        target.CurrentHp = 0;
                        BattleLog(target.Monster.CharacterName + " died.\n");
                        monsters.Remove(target);
                    }

                    if (monsters.Count == 0)
                        break;
                }
                else
                {
                    // Monster turn
                    MonsterInstance attacker = monsters[turnCounter <= keepers.Count * 2 ? turnCounter / 2 : turnCounter - keepers.Count];
                    bool isPrisonerTargeted = false;
                    AttackType attackType = attacker.Monster.GetEffectiveStrength() > attacker.Monster.GetEffectiveIntelligence() ? AttackType.Physical : AttackType.Magical;

                    if (isPrisonerOnTile)
                    {
                        float determineTarget = Random.Range(0, 100);
                        if (determineTarget < ((100.0f / (keepers.Count + 2)) * 2))
                        {
                            isPrisonerTargeted = true;
                        }
                    }

                    if (isPrisonerTargeted)
                    {
                        totalDamageTaken += MonsterDamageCalculation(attacker, null, attackType, true);
                    }
                    else
                    {
                        KeeperInstance target = GetTargetForAttack(keepers);
                        totalDamageTaken += MonsterDamageCalculation(attacker, target, attackType);

                        if (target.CurrentHp <= 0)
                        {
                            target.CurrentHp = 0;
                            BattleLog(target.Keeper.CharacterName + " died.\n");
                            keepers.Remove(target);
                        }
                    }

                    if (totalDamageTaken >= 50)
                    {
                        BattleLog("Over 50hp lost. Battle ends.\n");
                        break;
                    }
                    else if (GameManager.Instance.PrisonerInstance.CurrentHp <= 0)
                    {
                        BattleLog("Prisoner died. Battle ends.\n");
                        break;
                    }
                }

                turnCounter++;
            }
        }

        // Battle result
        if (monsters.Count == 0)
        {
            BattleLog("All monsters defeated.\n");
            BattleLog("Battle won! Yippi!\n");
            return true;
        }
        else
        {
            BattleLog("Battle lost :'(\n");
            return false;
        }
    }

    private static MonsterInstance GetTargetForAttack(List<MonsterInstance> monsters, AttackType attackType)
    {
        List<MonsterInstance> subMonstersList = new List<MonsterInstance>();
        foreach (MonsterInstance mi in monsters)
        {
            if (mi.CurrentHp == 0)
            {
                continue;
            }

            if (attackType == AttackType.Physical)
            {
                if (mi.Monster.GetEffectiveSpirit() > mi.Monster.GetEffectiveDefense())
                    subMonstersList.Add(mi);
            }
            else
            {
                if (mi.Monster.GetEffectiveSpirit() < mi.Monster.GetEffectiveDefense())
                    subMonstersList.Add(mi);
            }
        }

        if (subMonstersList.Count == 0)
        {
            subMonstersList.AddRange(monsters);
        }

        MonsterInstance target = null;
        int tmpHp = 100;

        foreach (MonsterInstance mi in subMonstersList)
        {
            if (mi.CurrentHp <= tmpHp)
            {
                target = mi;
                tmpHp = mi.CurrentHp;
            }
        }

        return target;
    }

    private static KeeperInstance GetTargetForAttack(List<KeeperInstance> keepers)
    {
        return keepers[Random.Range(0, keepers.Count)];
    }

    private static void KeeperDamageCalculation(KeeperInstance attacker, MonsterInstance targetMonster, AttackType attackType)
    {
        int damage = 0;
        if (attackType == AttackType.Physical)
        {
            damage = Mathf.RoundToInt(Mathf.Pow(attacker.Keeper.GetEffectiveStrength(), 2) / targetMonster.Monster.GetEffectiveDefense());
        }
        else
        {
            damage = Mathf.RoundToInt(Mathf.Pow(attacker.Keeper.GetEffectiveIntelligence(), 2) / targetMonster.Monster.GetEffectiveSpirit());
        }

        damage = Mathf.RoundToInt(damage * Random.Range(0.75f, 1.25f));

        targetMonster.CurrentHp -= damage;
        BattleLog(attacker.Keeper.CharacterName + " deals " + damage + " damage to " + targetMonster.Monster.CharacterName + ".\n");
        BattleLog(targetMonster.Monster.CharacterName + " has " + targetMonster.CurrentHp + " left.\n");
    }

    private static int MonsterDamageCalculation(MonsterInstance attacker, KeeperInstance targetKeeper, AttackType attackType, bool prisonerTargeted = false)
    {
        int damage = 0;
        if (attackType == AttackType.Physical)
        {
            if (!prisonerTargeted)
                damage = Mathf.RoundToInt(attacker.Monster.GetEffectiveStrength() / targetKeeper.Keeper.GetEffectiveDefense());
            else
                damage = Mathf.RoundToInt(attacker.Monster.GetEffectiveStrength() / GameManager.Instance.PrisonerInstance.Prisoner.GetEffectiveDefense());
        }
        else
        {
            if (!prisonerTargeted)
                damage = Mathf.RoundToInt(attacker.Monster.GetEffectiveIntelligence() / targetKeeper.Keeper.GetEffectiveSpirit());
            else
                damage = Mathf.RoundToInt(attacker.Monster.GetEffectiveIntelligence() / GameManager.Instance.PrisonerInstance.Prisoner.GetEffectiveSpirit());
        }

        damage = Mathf.RoundToInt(damage * Random.Range(0.75f, 1.25f));

        if (prisonerTargeted)
        {
            GameManager.Instance.PrisonerInstance.CurrentHp -= damage;
            BattleLog(attacker.Monster.CharacterName + " deals " + damage + " damage to prisoner.\n");
            BattleLog("Prisoner has " + GameManager.Instance.PrisonerInstance.CurrentHp + " left.\n");
        }
        else
        {
            targetKeeper.CurrentHp -= damage;
            BattleLog(attacker.Monster.CharacterName + " deals " + damage + " damage to " + targetKeeper.Keeper.CharacterName + ".\n");
            BattleLog(targetKeeper.Keeper.CharacterName + " has " + targetKeeper.CurrentHp + " left.\n");

        }

        return damage;
    }

    /*
     * Process everything that needs to be processed after a victory
     */
    private static void HandleBattleVictory(List<KeeperInstance> keepers, Tile tile)
    {
        foreach (KeeperInstance ki in keepers)
        {
            ki.CurrentMentalHealth += 10;
            ki.CurrentHunger += 5;
            BattleLog(ki.Keeper.CharacterName + " won 10 mental health and lost 5 hunger due to victory.\n");

            if (isPrisonerOnTile)
            {
                GameManager.Instance.PrisonerInstance.CurrentMentalHealth += 10;
                GameManager.Instance.PrisonerInstance.CurrentHunger += 5;
                BattleLog("Prisoner won 10 mental health and lost 5 hunger due to victory.\n");
            }
        }
    }

    /*
     * Process everything that needs to be processed after a defeat
     */
    private static void HandleBattleDefeat(List<KeeperInstance> keepers)
    {
        foreach (KeeperInstance ki in keepers)
        {
            ki.CurrentMentalHealth -= 10;
            ki.CurrentHunger += 5;

            ki.CurrentHp -= 10;
            BattleLog(ki.Keeper.CharacterName + " lost 10 mental health, 5 hunger, 10HP due to defeat.\n");

            if (isPrisonerOnTile)
            {
                GameManager.Instance.PrisonerInstance.CurrentMentalHealth -= 10;
                GameManager.Instance.PrisonerInstance.CurrentHunger += 5;
                GameManager.Instance.PrisonerInstance.CurrentHp -= 10;
                BattleLog("Prisoner lost 10 mental health, 5 hunger, 10HP due to defeat.\n");
            }
        }

        foreach (KeeperInstance ki in GameManager.Instance.ListOfSelectedKeepers)
        {
            ki.transform.position = ki.transform.position - ki.transform.forward * 0.5f;
        }
    }

    private static void PostBattleCommonProcess(List<KeeperInstance> keepers, Tile tile)
    {
        List<ItemContainer> loot = ComputeTotalLoot(tile);
        TileManager.Instance.RemoveDefeatedMonsters(tile);

        if (loot != null && loot.Count > 0)
        {
            IngameScreens.Instance.goInventoryLoot.GetComponentInParent<Inventory>().List_inventaire = loot;
            IngameScreens.Instance.UpdateLootInterface();
        }
    }

    private static void PrintResultsScreen(bool isVictorious)
    {
        GameManager.Instance.BattleResultScreen.gameObject.SetActive(true);
        Transform header = GameManager.Instance.BattleResultScreen.GetChild((int)BattleResultScreenChildren.Header);

        header.GetComponentInChildren<Text>().text = isVictorious ? "Victory!" : "Defeat";

        IngameScreens.Instance.CreateLootInterface();

        // Freeze time until close button is pressed
        GameManager.Instance.ClearListKeeperSelected();
        Time.timeScale = 0.0f;
    }

    private static List<ItemContainer> ComputeTotalLoot(Tile tile)
    {
        List<ItemContainer> lootList = new List<ItemContainer>();
        foreach (MonsterInstance mi in TileManager.Instance.MonstersOnTile[tile])
        {
            if (mi.CurrentHp == 0)
                lootList.AddRange(mi.GetComponent<Loot>().ComputeLoot());
        }

        return lootList;
    }

    private static void BattleLog(string log)
    {
        string tmp = battleLogger.text;

        if (tmp.Length > 1500 && !isDebugModeActive)
        {
            int indexOfFirstEndline = tmp.IndexOf("\n") + 1;
            tmp = tmp.Substring(indexOfFirstEndline);
        }
        tmp += log;
        battleLogger.text = tmp;
    }
}
