﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestSystem;
using Behaviour;
using System;

public class KillMonsterObjective : IQuestObjective
{
    InitEvent onInit;
    CompleteEvent onComplete;
    private string title;
    private string description;
    private bool isComplete;

    // We must give an ID to every Quest Objective class (static because it belongs to the class)
    // So we can know what to call when loading the quest from JSON
    private static int id = 0;

    public string monsterID;
    bool monsterKilled = false;

    public KillMonsterObjective(string _title, string desc, string _monsterID, bool complete = false)
    {
        title = _title;
        description = desc;
        monsterID = _monsterID;
        isComplete = complete;
        monsterKilled = false;
    }

    public string Title
    {
        get
        {
            return title;
        }
    }

    public string Description
    {
        get
        {
            return description;
        }
    }

    public bool IsComplete
    {
        get
        {
            return isComplete;
        }
    }

    public int ID
    {
        get
        {
            return id;
        }
    }

    InitEvent IQuestObjective.OnInit
    {
        get
        {
            return onInit;
        }

        set
        {
            onInit = value;
        }
    }

    public CompleteEvent OnComplete
    {
        get
        {
            return onComplete;
        }

        set
        {
            onComplete = value;
        }
    }

    public void CheckProgress()
    {
        if (monsterKilled)
        {
            isComplete = true;
        }
        else
        {
            isComplete = false;
        }
    }

    public void CheckProgressWithEvent()
    {
        if (monsterKilled)
        {
            isComplete = true;
            if (onComplete != null)
                onComplete();
        }
        else
        {
            isComplete = false;
        }
    }

    public void UpdateProgress()
    {
        
    }

    public void UpdateProgress(Monster m)
    {
        if(m.getPawnInstance.Data.PawnId == monsterID)
        {
            EventManager.OnMonsterDie -= UpdateProgress;
            monsterKilled = true;
            CheckProgressWithEvent();
        }
    }

    public void Init()
    {
        EventManager.OnMonsterDie += UpdateProgress;
        Debug.Log(isComplete);
        Debug.Log(monsterKilled);
        Debug.Log(monsterID);
        if (onInit != null)
        {
            onInit();
        }
    }

    public IQuestObjective GetCopy()
    {
        return new KillMonsterObjective(title, description, monsterID, isComplete);
    }
}
