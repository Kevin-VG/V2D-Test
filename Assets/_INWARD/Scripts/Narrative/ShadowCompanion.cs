using Shatter.Player;
using UnityEngine;

namespace Shatter.Narrative
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShadowCompanion : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 followOffset = new Vector2(-1.2f, 0.05f);
        [SerializeField] private float followSmoothTime = 0.18f;
        [SerializeField] private float walkVelocityThreshold = 0.2f;
        [SerializeField] private bool presentByDefault = true;

        private SpriteRenderer spriteRenderer;
        private SpriteFrameAnimator animator;
        private PlayerController2D playerController;
        private Vector3 velocity;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<SpriteFrameAnimator>();
        }

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                    playerController = player.GetComponent<PlayerController2D>();
                }
            }

            gameObject.SetActive(presentByDefault);
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (playerController == null)
            {
                playerController = target.GetComponent<PlayerController2D>();
            }

            int direction = playerController != null ? playerController.Direccion : 1;
            Vector3 desired = target.position + new Vector3(followOffset.x * direction, followOffset.y, 0f);
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, followSmoothTime);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction < 0;
            }

            if (animator != null)
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
    }
}
