using Shatter.Player;
using UnityEngine;

namespace Shatter.Narrative
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EcoCompanion : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 followOffset = new Vector2(2.2f, 1.35f);
        [SerializeField] private float followSmoothTime = 0.28f;
        [SerializeField] private float focusSmoothTime = 0.12f;
        [SerializeField] private float verticalFloatAmplitude = 0.16f;
        [SerializeField] private float verticalFloatSpeed = 1.8f;
        [SerializeField] private float walkVelocityThreshold = 0.12f;

        private PlayerController2D playerController;
        private SpriteFrameAnimator animator;
        private SpriteRenderer spriteRenderer;
        private Vector3 velocity;
        private Vector3 anchorPosition;
        private bool isAnchored;

        private void Awake()
        {
            animator = GetComponent<SpriteFrameAnimator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            FindTargetIfNeeded();
        }

        private void LateUpdate()
        {
            FindTargetIfNeeded();

            Vector3 desired = isAnchored ? anchorPosition : GetFollowPosition();
            desired.y += Mathf.Sin(Time.time * verticalFloatSpeed) * verticalFloatAmplitude;
            float smoothTime = isAnchored ? focusSmoothTime : followSmoothTime;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

            if (target != null && spriteRenderer != null)
            {
                spriteRenderer.flipX = transform.position.x > target.position.x;
            }

            if (animator != null && !isAnchored)
            {
                float speed = playerController != null ? Mathf.Abs(playerController.VelocidadX) : Mathf.Abs(velocity.x);
                if (speed > walkVelocityThreshold)
                {
                    animator.PlayWalk();
                }
                else
                {
                    animator.PlayIdle();
                }
            }
        }

        public void MoveToFocusPoint(Vector3 position)
        {
            anchorPosition = position;
            isAnchored = true;
            velocity = Vector3.zero;
            if (Vector3.Distance(transform.position, anchorPosition) > 7f)
            {
                transform.position = anchorPosition + new Vector3(-1.2f, 0.35f, 0f);
            }

            if (animator != null)
            {
                animator.PlayTalk();
            }
        }

        public void ResumeFollowing()
        {
            isAnchored = false;
            if (animator != null)
            {
                animator.PlayIdle();
            }
        }

        private Vector3 GetFollowPosition()
        {
            if (target == null)
            {
                return transform.position;
            }

            int direction = playerController != null ? playerController.Direccion : 1;
            return target.position + new Vector3(followOffset.x * direction, followOffset.y, 0f);
        }

        private void FindTargetIfNeeded()
        {
            if (target != null)
            {
                if (playerController == null)
                {
                    playerController = target.GetComponent<PlayerController2D>();
                }

                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

            target = player.transform;
            playerController = player.GetComponent<PlayerController2D>();
            transform.position = GetFollowPosition();
        }
    }
}
