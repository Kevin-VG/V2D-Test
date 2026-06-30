using Shatter.Player;
using System.Collections;
using UnityEngine;

namespace Shatter.Narrative
{
    public class Level05NarrativeController : MonoBehaviour
    {
        [System.Serializable]
        public class MemoryEvent
        {
            public string id;
            public string speaker = "Eco Amable";
            [TextArea(2, 4)] public string line;
            public GameObject rewardRoot;
            public GameObject visualRoot;
            public float duration = 5f;
        }

        [SerializeField] private SimpleDialogueUI dialogueUI;
        [SerializeField] private PlayerController2D player;
        [SerializeField] private SpriteFrameAnimator ecoAnimator;
        [SerializeField] private EcoCompanion ecoCompanion;
        [SerializeField] private Level05AudioDirector audioDirector;
        [SerializeField] private SpriteFrameAnimator shadowAnimator;
        [SerializeField] private GameObject shadowCompanion;
        [SerializeField] private bool shadowAcceptedForTesting = true;
        [SerializeField] private MemoryEvent intro;
        [SerializeField] private MemoryEvent[] memories = new MemoryEvent[5];
        [SerializeField] private MemoryEvent finalEvent;
        [SerializeField] private string finalScreenTitle = "INWARD";
        [SerializeField] private string finalScreenBody = "Mateo abre los ojos.\nEsta vez no despierta huyendo.\nGracias por caminar con el hasta que pudo volver a si mismo.";
        [SerializeField] private float finalScreenDelay = 5f;
        [SerializeField] private bool playLevelIntroOnStart = true;
        [SerializeField] private string levelIntroTitle = "Nivel 5";
        [SerializeField] private string levelIntroBody = "El jardin donde Mateo aprende a mirar sus recuerdos.";
        [SerializeField] private float levelIntroFadeIn = 1.1f;
        [SerializeField] private float levelIntroHold = 2.1f;
        [SerializeField] private float levelIntroFadeOut = 1.6f;

        private bool introPlayed;
        private bool finalPlayed;
        private bool[] memoriesPlayed;
        private Coroutine pendingEventRoutine;
        private Coroutine finalSequenceRoutine;

        private void Awake()
        {
            memoriesPlayed = new bool[memories != null ? memories.Length : 0];
        }

        private void Start()
        {
            if (dialogueUI == null)
            {
                dialogueUI = FindAnyObjectByType<SimpleDialogueUI>();
            }

            if (player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    player = playerObject.GetComponent<PlayerController2D>();
                }
            }

            if (ecoCompanion == null && ecoAnimator != null)
            {
                ecoCompanion = ecoAnimator.GetComponent<EcoCompanion>();
            }

            if (audioDirector == null)
            {
                audioDirector = FindAnyObjectByType<Level05AudioDirector>();
            }

            if (shadowCompanion != null)
            {
                shadowCompanion.SetActive(shadowAcceptedForTesting);
            }

            SetRewardsActive(false);

            if (playLevelIntroOnStart && dialogueUI != null)
            {
                dialogueUI.ShowLevelIntro(levelIntroTitle, levelIntroBody, levelIntroFadeIn, levelIntroHold, levelIntroFadeOut, player);
            }
        }

        public void PlayIntro()
        {
            if (introPlayed)
            {
                return;
            }

            introPlayed = true;
            PlayEventWhenReady(intro);
        }

        public void PlayMemory(int index)
        {
            if (memories == null || index < 0 || index >= memories.Length)
            {
                return;
            }

            if (memoriesPlayed[index])
            {
                return;
            }

            memoriesPlayed[index] = true;
            PlayEventWhenReady(memories[index]);
        }

        public void PlayFinal()
        {
            if (finalPlayed)
            {
                return;
            }

            finalPlayed = true;
            if (finalSequenceRoutine != null)
            {
                StopCoroutine(finalSequenceRoutine);
            }

            finalSequenceRoutine = StartCoroutine(PlayFinalSequenceRoutine());
        }

        private void PlayEventWhenReady(MemoryEvent memoryEvent)
        {
            if (pendingEventRoutine != null)
            {
                StopCoroutine(pendingEventRoutine);
            }

            pendingEventRoutine = StartCoroutine(PlayEventWhenReadyRoutine(memoryEvent));
        }

        private IEnumerator PlayEventWhenReadyRoutine(MemoryEvent memoryEvent)
        {
            while (dialogueUI != null && dialogueUI.IsShowing)
            {
                yield return null;
            }

            PlayEvent(memoryEvent);
            pendingEventRoutine = null;
        }

        private IEnumerator PlayFinalSequenceRoutine()
        {
            while (dialogueUI != null && dialogueUI.IsShowing)
            {
                yield return null;
            }

            PlayEvent(finalEvent);

            while (dialogueUI != null && dialogueUI.IsShowing)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.35f);

            ShowFinalScreen();
            finalSequenceRoutine = null;
        }

        private void PlayEvent(MemoryEvent memoryEvent)
        {
            if (memoryEvent == null)
            {
                return;
            }

            if (memoryEvent.visualRoot != null)
            {
                memoryEvent.visualRoot.SetActive(true);
            }

            if (memoryEvent.rewardRoot != null)
            {
                memoryEvent.rewardRoot.SetActive(true);
            }

            if (ecoAnimator != null)
            {
                ecoAnimator.PlayTalk();
                CancelInvoke(nameof(StopEcoTalking));
                Invoke(nameof(StopEcoTalking), Mathf.Max(1f, memoryEvent.duration));
            }

            if (audioDirector != null)
            {
                audioDirector.PlayEcoMoment(memoryEvent.duration);
            }

            if (ecoCompanion != null)
            {
                Vector3 focusPoint = memoryEvent.visualRoot != null
                    ? memoryEvent.visualRoot.transform.position + new Vector3(0f, 1.8f, 0f)
                    : ecoCompanion.transform.position;
                ecoCompanion.MoveToFocusPoint(focusPoint);
            }

            if (memoryEvent.speaker == "Sombra" && shadowAnimator != null)
            {
                shadowAnimator.PlayTalk();
            }

            if (dialogueUI != null)
            {
                dialogueUI.Show(memoryEvent.speaker, memoryEvent.line, memoryEvent.duration, player);
            }
            else
            {
                Debug.Log($"{memoryEvent.speaker}: {memoryEvent.line}");
            }
        }

        private void StopEcoTalking()
        {
            if (ecoAnimator != null)
            {
                ecoAnimator.PlayIdle();
            }

            if (ecoCompanion != null)
            {
                ecoCompanion.ResumeFollowing();
            }
        }

        private void ShowFinalScreen()
        {
            if (audioDirector != null)
            {
                audioDirector.PlayFinalMoment();
            }

            if (dialogueUI != null)
            {
                dialogueUI.ShowFinalScreen(finalScreenTitle, finalScreenBody, player);
            }
        }

        private void SetRewardsActive(bool active)
        {
            if (memories != null)
            {
                foreach (MemoryEvent memoryEvent in memories)
                {
                    if (memoryEvent?.rewardRoot != null)
                    {
                        memoryEvent.rewardRoot.SetActive(active);
                    }
                }
            }

            if (intro?.rewardRoot != null)
            {
                intro.rewardRoot.SetActive(active);
            }
        }
    }
}
