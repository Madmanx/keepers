﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    public class QuestDeck
    {
        int id;
        string deckName;
        string mainQuest;
        List<string> sideQuests;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public List<string> SideQuests
        {
            get
            {
                return sideQuests;
            }

            set
            {
                sideQuests = value;
            }
        }

        public string DeckName
        {
            get
            {
                return deckName;
            }

            set
            {
                deckName = value;
            }
        }

        public string MainQuest
        {
            get
            {
                return mainQuest;
            }

            set
            {
                mainQuest = value;
            }
        }
    }
}