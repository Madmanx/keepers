﻿using UnityEngine;
using UnityEngine.UI;
using Behaviour;

public class DebugControls : MonoBehaviour {

    bool isDebugModeActive = false;
    bool isUnlimitedActionPointsModeActive = false;

    [SerializeField]
    GameObject monsterToPopPrefab;
    [SerializeField]
    [Range(1, 99)] int itemsToPopAmount = 1;
    [SerializeField]
    GameObject debugCanvas;

	void Start () {
        isDebugModeActive = false;
        isUnlimitedActionPointsModeActive = false;
        debugCanvas.SetActive(false);
    }
	
	void Update () {
		if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKeyDown(KeyCode.RightControl))
        {
            isDebugModeActive = !isDebugModeActive;
            debugCanvas.SetActive(isDebugModeActive);
            Debug.Log("Debug mode now active.");
        }

        if (isDebugModeActive)
        {
            // Help window
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                debugCanvas.SetActive(!debugCanvas.activeInHierarchy);
            }

            if (GameManager.Instance.ListOfSelectedKeepers == null || GameManager.Instance.ListOfSelectedKeepers.Count == 0)
            {
                Debug.Log("Select a keeper to use debug tools.");
                return;
            }

            // Kill selected character
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Killed " + GameManager.Instance.GetFirstSelectedKeeper().Data.PawnName + " using debug tools.");
                GameManager.Instance.GetFirstSelectedKeeper().GetComponent<Mortal>().CurrentHp = 0;
            }

            // Decrease food
            if (Input.GetKey(KeyCode.Alpha2))
            {
                GameManager.Instance.GetFirstSelectedKeeper().GetComponent<HungerHandler>().CurrentHunger--;
            }

            // Decrease mental health
            if (Input.GetKey(KeyCode.Alpha3))
            {
                GameManager.Instance.GetFirstSelectedKeeper().GetComponent<MentalHealthHandler>().CurrentMentalHealth--;
            }

            // Decrease HP
            if (Input.GetKey(KeyCode.Alpha4))
            {
                GameManager.Instance.GetFirstSelectedKeeper().GetComponent<Mortal>().CurrentHp--;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                GameManager.Instance.GetFirstSelectedKeeper().GetComponent<Keeper>().ActionPoints--;
            }

            // Unlimited action points
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (isUnlimitedActionPointsModeActive)
                {
                    Debug.Log("Deactivate unlimited action points mode.");
                    foreach (PawnInstance pi in GameManager.Instance.AllKeepersList)
                    {
                        pi.GetComponent<Keeper>().MaxActionPoints = 3;
                        pi.GetComponent<Keeper>().ActionPoints = 3;
                    }
                }
                else
                {
                    Debug.Log("Activate unlimited action points mode.");
                    foreach (PawnInstance pi in GameManager.Instance.AllKeepersList)
                    {
                        pi.GetComponent<Keeper>().MaxActionPoints = 99;
                        pi.GetComponent<Keeper>().ActionPoints = 99;
                    }
                }
                isUnlimitedActionPointsModeActive = !isUnlimitedActionPointsModeActive;
            }

            // Pop a monster
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                GameObject go = Instantiate(monsterToPopPrefab,
                    GameManager.Instance.GetFirstSelectedKeeper().CurrentTile.transform.position,
                    Quaternion.identity, GameManager.Instance.GetFirstSelectedKeeper().CurrentTile.transform);
                TileManager.Instance.AddMonsterOnTile(GameManager.Instance.GetFirstSelectedKeeper().CurrentTile, go.GetComponent<PawnInstance>());
            }

            // Pop an item on the ground
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ItemContainer[] itemToSpawn = new ItemContainer[1];
                Debug.Log(GameManager.Instance.ItemDataBase.ItemsList.Count);
                itemToSpawn[0] = new ItemContainer(GameManager.Instance.ItemDataBase.ItemsList[Random.Range(0, GameManager.Instance.ItemDataBase.ItemsList.Count)], itemsToPopAmount);
                ItemManager.AddItemOnTheGround(GameManager.Instance.GetFirstSelectedKeeper().CurrentTile, GameManager.Instance.GetFirstSelectedKeeper().CurrentTile.transform, itemToSpawn);
            }

        }
    }
}