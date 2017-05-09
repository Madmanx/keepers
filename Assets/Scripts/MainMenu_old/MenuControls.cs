﻿using Behaviour;
using UnityEngine;
using UnityEngine.UI;

// Temp
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour {
    public int levelSelected = -1;
    public Image cardLevelSelectedImg;

    // TODO handle this better
    [SerializeField]
    Sprite[] levelImg;

    // Update is called once per frame
    void Update () {
        if (!CinematiqueManager.Instance.isPlaying)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // On Click on a personnage
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("KeeperInstance"))
                    {
                        PawnInstance k = hit.transform.gameObject.GetComponent<PawnInstance>();
                        if (k != null)
                        {
                            if (GameManager.Instance.AllKeepersList.Contains(k))
                            {
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.deselectSound, 0.25f);
                                k.GetComponent<Keeper>().IsSelectedInMenu = false;
                                GameManager.Instance.AllKeepersList.Remove(k);
                            }
                            else
                            {
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.selectSound, 0.25f);
                                k.GetComponent<Keeper>().IsSelectedInMenu = true;
                                GameManager.Instance.AllKeepersList.Add(k);
                            }
                            GetComponent<MenuUI>().UpdateUI();
                        }
                    }
                    else if (hit.transform.GetComponent<CardLevel>() != null)
                    {
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.paperSelectSound, 0.5f);
                        if (levelSelected == hit.transform.GetComponent<CardLevel>().levelIndex)
                        {
                            levelSelected = -1;
                            cardLevelSelectedImg.enabled = false;
                        }
                        else
                        {
                            levelSelected = hit.transform.GetComponent<CardLevel>().levelIndex;
                            cardLevelSelectedImg.sprite = levelImg[levelSelected-1];
                            cardLevelSelectedImg.enabled = true;
                        }
                    }
                }
            }

            // Deselect all
            if (Input.GetKeyDown(KeyCode.A))
            {
                foreach (PawnInstance ki in GameManager.Instance.AllKeepersList)
                {
                    ki.GetComponent<Keeper>().IsSelectedInMenu = false;       
                }
                GameManager.Instance.AllKeepersList.Clear();
                GetComponent<MenuUI>().UpdateUI();
            }
        }

        // Skip Cinematique
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.Instance.AllKeepersList.Count == 0)
            {
                Keeper[] keeperInstances = FindObjectsOfType<Keeper>();
                for (int i = 0; i < keeperInstances.Length; i++)
                {
                    GameManager.Instance.AllKeepersList.Add(keeperInstances[i].getPawnInstance);
                }
            }

            // Prevents doing shit with load scene
            if (levelSelected == -1)
                levelSelected = 1;

            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioClip toPlay;
            switch(levelSelected)
            {
                case 1:
                    toPlay = AudioManager.Instance.Scene1Clip;
                    break;
                case 2:
                    toPlay = AudioManager.Instance.Scene2Clip;
                    break;
                default:
                    toPlay = AudioManager.Instance.menuMusic;
                    break;
            }
            AudioManager.Instance.Fade(toPlay);
        }
        SceneManager.LoadScene(levelSelected);
    }
}