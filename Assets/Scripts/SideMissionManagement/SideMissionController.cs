using System;
using System.Collections.Generic;
using Events;
using PixelCrushers.DialogueSystem;
using SideMissionManagement.EventImplementations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SideMissionManagement
{
    public class SideMissionController : MonoBehaviour
    {
        public WaveController WaveController;

        public SideMissionData MissionData;

        [BoxGroup("Ongoing Info")]
        public int CurrentWaveId;

        [BoxGroup("Ongoing Info")]
        public int SequenceIndex;

        [BoxGroup("Position Info")]
        public List<Collider> SpawnAreas = new List<Collider>();

        //TODO: may be needed
        [BoxGroup("Position Info")]
        public List<Vector3> SpecialSpawnPoints = new List<Vector3>();

        // TODO: aynen kanka
        public static Transform DoorTransform;

        private void Awake()
        {
            DoorTransform = GameObject.FindWithTag("Door").transform;
            GEM.AddListener<WaveCompletedEvent>(OnWaveCompleted);
        }

        [Button]
        public void OnSideMissionStarted()
        {
            CurrentWaveId = 0;
            SequenceIndex = 0;

            SetSequence(MissionData.Sequence[SequenceIndex].Type);
        }

        public void SetSequence(SideMissionSequenceItemType itemType)
        {
            switch (itemType)
            {
                case SideMissionSequenceItemType.Wave:
                    OnStartWave();
                    break;
                case SideMissionSequenceItemType.Dialogue:
                    StartDialogueSequence();
                    break;
                case SideMissionSequenceItemType.Event:
                    break;
                case SideMissionSequenceItemType.Boss:
                    break;
                case SideMissionSequenceItemType.Wait:
                    break;
            }
        }

        public void UpdateSequence()
        {
            SequenceIndex++;

            if (SequenceIndex >= MissionData.Sequence.Count)
            {
                OnSideMissionCompleted();
                return;
            }

            SetSequence(MissionData.Sequence[SequenceIndex].Type);
        }

        public void OnSideMissionCompleted()
        {
        }

        public void OnSideMissionFailed()
        {
            
        }

        #region Wave

        public void OnStartWave()
        {
            WaveController.Initialize(MissionData.Waves[CurrentWaveId], SpawnAreas);
        }

        public void OnWaveCompleted(WaveCompletedEvent evt)
        {
            CurrentWaveId++;

            UpdateSequence();
        }

        #endregion

        #region Dialogue

        public void StartDialogueSequence()
        {
            var dialogueManager = DialogueManager.instance;
            dialogueManager.StartConversation("SideMissionDuring");

            var events = dialogueManager.GetComponent<DialogueSystemEvents>();
            
            UnityAction<Transform> onDialogueEnd = transform1 => OnDialogueCompleted();

            events.conversationEvents.onConversationEnd.AddListener(onDialogueEnd);
        }

        public void OnDialogueCompleted()
        {
            UpdateSequence();
        }

        #endregion

        #region Event

        #endregion

        #region Boss

        #endregion

        #region Wait

        #endregion
    }
}