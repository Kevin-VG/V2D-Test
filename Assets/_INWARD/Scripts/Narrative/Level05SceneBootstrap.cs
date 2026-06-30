using Shatter.Player;
using UnityEngine;

namespace Shatter.Narrative
{
    [DefaultExecutionOrder(-10000)]
    public class Level05SceneBootstrap : MonoBehaviour
    {
        [SerializeField] private bool forceTestingSpawn = true;
        [SerializeField] private Vector3 testingSpawnPosition = new Vector3(0.5f, 1.25f, 0f);
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.5f, -10f);
        [SerializeField] private Transform player;

        private void Awake()
        {
            if (!forceTestingSpawn)
            {
                return;
            }

            if (player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    player = playerObject.transform;
                }
            }

            if (player == null)
            {
                return;
            }

            player.position = testingSpawnPosition;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            PlayerController2D controller = player.GetComponent<PlayerController2D>();
            if (controller != null)
            {
                controller.ReanudarMovimiento();
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.position = testingSpawnPosition + cameraOffset;
            }
        }
    }
}
