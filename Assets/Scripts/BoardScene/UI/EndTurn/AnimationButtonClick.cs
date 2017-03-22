﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimationButtonClick : MonoBehaviour, IPointerEnterHandler {

    private Light directionalLight;
    private float temps;

    public void Start()
    {
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();

        AnimationClip ac = null;
        for (int i = 0; i < GetComponent<Animator>().runtimeAnimatorController.animationClips.Length; i++)
        {
            if (GetComponent<Animator>().runtimeAnimatorController.animationClips[i].name == "NewTurn")
            {
                ac = GetComponent<Animator>().runtimeAnimatorController.animationClips[i];
                break;
            }
      
        }
        temps = ac.length;
    }


    public void ChangeLight()
    {
        StartCoroutine(GodsWork());
    }

    private IEnumerator GodsWork()
    {
        directionalLight.intensity = Mathf.Clamp(directionalLight.intensity, 0, 1);
        for (float f = temps/2; f >= 0; f -= Time.deltaTime)
        {
            //valeur = 0
            directionalLight.intensity -= Time.deltaTime *2;


            yield return null;
        }

        for (float f = temps / 2; f >= 0; f -= Time.deltaTime)
        {
            // valeur = 1
            directionalLight.intensity += Time.deltaTime*2;
            yield return null;
        }
    }

    // Update is called once per frame
    public void HandleAnimation() {
        GameManager.Instance.Ui.TurnPanel.transform.GetChild(0).GetComponentInChildren<Text>().text = "Jour " + ++GameManager.Instance.NbTurn;

        GameManager.Instance.Ui.isTurnEnding = false;
        EventManager.EndTurnEvent();
        GetComponent<Animator>().enabled = false;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < GameManager.Instance.AllKeepersList.Count; i++)
        {
            if (GameManager.Instance.AllKeepersList[i].ActionPoints > 0)
            {
                GameManager.Instance.Ui.goShortcutKeepersPanel.SetActive(true);
                // Actions
                GameManager.Instance.Ui.goShortcutKeepersPanel.transform.GetChild(i + 1).GetChild(4).GetComponent<Text>().color = Color.green;
                GameManager.Instance.Ui.goShortcutKeepersPanel.transform.GetChild(i + 1).GetChild(4).transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                StartCoroutine(TextAnimationNormalState(i));
                return;
            }
        }
    }

    private IEnumerator TextAnimationNormalState(int _i)
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.Ui.goShortcutKeepersPanel.transform.GetChild(_i + 1).GetChild(4).GetComponent<Text>().color = Color.white;
        GameManager.Instance.Ui.goShortcutKeepersPanel.transform.GetChild(_i + 1).GetChild(4).transform.localScale = Vector3.one;
        yield return null;
    }
}
