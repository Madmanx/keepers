﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour {

    public enum ScaleState
    {
        unscale = -1,
        idle = 0,
        scale = 1,
    }

    public enum ChatMode
    {
        mute = -1,
        pickme,
        picked,
    }

    RectTransform trBox;
    Text txt;

    public Transform trTarget;    
    public List<string>[] tabEmotes;
    public bool bEnable = false;

    public bool bIsShown = false;
    public ScaleState state = ScaleState.idle;
    ChatMode mode = ChatMode.mute;
    public float fTimer = 0;
    public float fLerp = 0;

    // Parametres
    public float fShowLength = 3;
    public float fDelayMin = 1, fDelayMax = 2;
    public float fScaleSpeed = 10;

	// Use this for initialization

    public void Start()
    {
        trBox = GetComponent<RectTransform>();
        txt = GetComponentInChildren<Text>();
        
        trBox.transform.localScale = Vector3.zero;

        tabEmotes = new List<string>[2];

        tabEmotes[0] = new List<string>();
        tabEmotes[0].Add("Hey!");
        tabEmotes[0].Add("Pick me!");

        tabEmotes[1] = new List<string>();
        tabEmotes[1].Add("Let's go!");
        tabEmotes[1].Add("Yeah!");

        fTimer = Random.Range(fDelayMin, fDelayMax);
    }

	// Update is called once per frame
	void Update () {
        if (bEnable)
        {
            if (!bIsShown)
            {
                if(mode != ChatMode.mute)
                {
                    if (fTimer <= 0)
                    {
                        txt.text = tabEmotes[(int)mode][Random.Range(0, tabEmotes[(int)mode].Count)];
                        TriggerScale();
                    }
                    else
                    {
                        fTimer -= Time.unscaledDeltaTime;
                    }
                }
            }
            else
            {
                if(state == ScaleState.idle)
                {
                    fTimer -= Time.unscaledDeltaTime;

                    if(fTimer <= 0)
                    {
                        TriggerUnscale();
                    }
                }
            }
        }

        if (bIsShown)
        {
            UpdatePosition();
        }

        if(state != ScaleState.idle)
        {
            UpdateScale();
        }
	}

    void UpdatePosition()
    {
        trBox.position = Camera.main.WorldToScreenPoint(trTarget.position); /*Camera.main.WorldToScreenPoint(trTarget.position)*/;
    }

    void UpdateScale()
    {
        fLerp += Time.deltaTime * (int)state * fScaleSpeed;

        trBox.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Clamp(fLerp,0, 1));

        if(state == ScaleState.scale && fLerp >= 1)
        {
            state = ScaleState.idle;
            fTimer = fShowLength;
        }

        if (state == ScaleState.unscale && fLerp <= 0)
        {
            state = ScaleState.idle;
            fTimer = Random.Range(fDelayMin, fDelayMax);
            bIsShown = false;
        }


    }

    public void TriggerScale()
    {
        fLerp = 0;
        bIsShown = true;
        state = ScaleState.scale;
    }

    public void TriggerUnscale()
    {
        fLerp = 1;
        state = ScaleState.unscale;
    }

    public void SetMode(ChatMode _mode)
    {
        mode = _mode;
        switch (_mode)
        {
            case ChatMode.mute:
                if (bIsShown)
                {
                    TriggerUnscale();
                }
                break;
            case ChatMode.pickme:
                break;
            case ChatMode.picked:
                break;
            default:
                break;
        }
    }

    public void SetEnable(bool status)
    {
        bEnable = status;
        if(status == false)
        {
            if (bIsShown)
            {
                TriggerUnscale();
            }
        }
        else
        {
            fTimer = Random.Range(fDelayMin, fDelayMax);
        }

    }

    public void Say(string message)
    {
        txt.text = message;
        TriggerScale();
    }
}