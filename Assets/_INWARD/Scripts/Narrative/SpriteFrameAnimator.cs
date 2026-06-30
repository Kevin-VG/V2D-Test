using UnityEngine;

namespace Shatter.Narrative
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFrameAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] idleFrames;
        [SerializeField] private Sprite[] walkFrames;
        [SerializeField] private Sprite[] talkFrames;
        [SerializeField] private float framesPerSecond = 8f;
        [SerializeField] private bool playOnAwake = true;

        private SpriteRenderer spriteRenderer;
        private Sprite[] currentFrames;
        private int frameIndex;
        private float timer;
        private string currentState;

        public string CurrentState => currentState;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (playOnAwake)
            {
                PlayIdle();
            }
        }

        private void Update()
        {
            if (currentFrames == null || currentFrames.Length == 0 || framesPerSecond <= 0f)
            {
                return;
            }

            timer += Time.deltaTime;
            float frameDuration = 1f / framesPerSecond;
            while (timer >= frameDuration)
            {
                timer -= frameDuration;
                frameIndex = (frameIndex + 1) % currentFrames.Length;
                spriteRenderer.sprite = currentFrames[frameIndex];
            }
        }

        public void PlayIdle()
        {
            SetState("Idle", idleFrames);
        }

        public void PlayWalk()
        {
            SetState("Walk", walkFrames != null && walkFrames.Length > 0 ? walkFrames : idleFrames);
        }

        public void PlayTalk()
        {
            SetState("Talk", talkFrames != null && talkFrames.Length > 0 ? talkFrames : idleFrames);
        }

        private void SetState(string state, Sprite[] frames)
        {
            if (currentState == state && currentFrames == frames)
            {
                return;
            }

            currentState = state;
            currentFrames = frames;
            frameIndex = 0;
            timer = 0f;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (currentFrames != null && currentFrames.Length > 0)
            {
                spriteRenderer.sprite = currentFrames[0];
            }
        }
    }
}
