using UnityEngine;

namespace Shatter.Narrative
{
    [RequireComponent(typeof(Collider2D))]
    public class NarrativeTrigger2D : MonoBehaviour
    {
        public enum TriggerKind
        {
            Intro,
            Memory,
            Final
        }

        [SerializeField] private Level05NarrativeController controller;
        [SerializeField] private TriggerKind kind;
        [SerializeField] private int memoryIndex;
        [SerializeField] private bool onlyOnce = true;

        private bool triggered;

        private void Awake()
        {
            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void Start()
        {
            if (controller == null)
            {
                controller = FindAnyObjectByType<Level05NarrativeController>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (onlyOnce && triggered)
            {
                return;
            }

            triggered = true;
            switch (kind)
            {
                case TriggerKind.Intro:
                    controller?.PlayIntro();
                    break;
                case TriggerKind.Memory:
                    controller?.PlayMemory(memoryIndex);
                    break;
                case TriggerKind.Final:
                    controller?.PlayFinal();
                    break;
            }
        }
    }
}
