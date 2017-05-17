﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeqAshleyLowHunger : Sequence {
    private GameObject pawnMrResetti;
    public AnimationClip jumpAnimationClip;
    public GameObject shortcutPanels;
    public List<GameObject> feedbacks;

    public class ShowAshleyHunger : Step
    {
        string str;
        GameObject feedback;
        public ShowAshleyHunger(string _str)
        {
            stepFunction = Message_fct;
            str = _str;
        }

        public void Message_fct()
        {
            if (!TutoManager.s_instance.GetComponent<SeqAshleyLowHunger>().shortcutPanels.activeInHierarchy)
                GameManager.Instance.Ui.ToggleShortcutPanel();

            SeqAshleyLowHunger seqAshleyLowHunger = TutoManager.s_instance.GetComponent<SeqAshleyLowHunger>();
            if (feedback == null)
            {
                feedback = Instantiate(TutoManager.s_instance.uiPointer, GameManager.Instance.Ui.transform.GetChild(0));
                feedback.GetComponent<FlecheQuiBouge>().PointToPoint = seqAshleyLowHunger.shortcutPanels.transform.GetChild(0).position;
                feedback.GetComponent<FlecheQuiBouge>().distanceOffset = 180.0f;
                feedback.GetComponent<FlecheQuiBouge>().speed = 12.0f;

                feedback.transform.localEulerAngles = new Vector3(0, 0, -135);
            }

            if (!TutoManager.s_instance.GetComponent<SeqAshleyLowHunger>().shortcutPanels.activeInHierarchy)
                GameManager.Instance.Ui.ToggleShortcutPanel();

            TutoManager.s_instance.EcrireMessage(str);
            TutoManager.s_instance.PlayingSequence.CurrentState = SequenceState.Idle;
        }

        public override void Reverse()
        {
            Destroy(feedback);
            alreadyPlayed = false;
        }
    }

    public class ShowFeedSlot : Step
    {
        string str;
        GameObject feedback;
        public ShowFeedSlot(string _str)
        {
            stepFunction = Message_fct;
            str = _str;
        }

        public void Message_fct()
        {
            SeqAshleyLowHunger seqAshleyLowHunger = TutoManager.s_instance.GetComponent<SeqAshleyLowHunger>();
            if (feedback == null)
            {
                feedback = Instantiate(TutoManager.s_instance.uiPointer, GameManager.Instance.Ui.transform.GetChild(0));
                feedback.GetComponent<FlecheQuiBouge>().PointToPoint = GameManager.Instance.PrisonerInstance.GetComponent<Behaviour.Inventory>().SelectedInventoryPanel.transform.position;
                feedback.GetComponent<FlecheQuiBouge>().distanceOffset = 180.0f;
                feedback.GetComponent<FlecheQuiBouge>().speed = 12.0f;

                feedback.transform.localEulerAngles = new Vector3(0, 0, -45);
            }

            TutoManager.s_instance.EcrireMessage(str);
            TutoManager.s_instance.PlayingSequence.CurrentState = SequenceState.Idle;
        }

        public override void Reverse()
        {
            Destroy(feedback);
            alreadyPlayed = false;
        }
    }

    public override void Init()
    {
        base.Init();
        pawnMrResetti = TutoManager.s_instance.SpawnMmeResetti(new Vector3(0.0f, 0.15f, -0.7f) + GameManager.Instance.ActiveTile.transform.position);

        Etapes = new List<Step>();
        Etapes.Add(new TutoManager.Spawn(pawnMrResetti, jumpAnimationClip));

        Etapes.Add(new ShowAshleyHunger("Be careful, Ashley is starving!"));

        if (GameManager.Instance.PrisonerInstance.GetComponent<Behaviour.Inventory>().SelectedInventoryPanel.activeSelf)
            Etapes.Add(new TutoManager.Message(pawnMrResetti, "Use the Feed slot here to feed her"));
        else
            Etapes.Add(new TutoManager.Message(pawnMrResetti, "Go on the area where she is and use the Feed slot next to your inventory to feed her"));        
    }

    public override void End()
    {
        base.End();
        if (pawnMrResetti != null)
            TutoManager.UnSpawn(pawnMrResetti);
        if (TutoManager.s_instance.TutoPanelInstance != null)
            Destroy(TutoManager.s_instance.TutoPanelInstance);
        TutoManager.s_instance.GetComponent<SeqAshleyLowHunger>().AlreadyPlayed = true;
        TutoManager.s_instance.PlayingSequence = null;
    }
}
