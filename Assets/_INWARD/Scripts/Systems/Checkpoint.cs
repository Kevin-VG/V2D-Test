using UnityEngine;
using Shatter.Core;

namespace Shatter.Systems
{
    /// <summary>
    /// Trigger que registra la posicion actual como respawn en el GameManager.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color colorInactivo = new Color(0.4f, 0.4f, 0.4f);
        [SerializeField] private Color colorActivo = new Color(1f, 0.85f, 0.2f);

        private bool activado;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
            if (visual == null) visual = GetComponentInChildren<SpriteRenderer>();
            if (visual != null) visual.color = colorInactivo;
        }

        private void OnTriggerEnter2D(Collider2D otro)
        {
            if (activado) return;
            if (!otro.CompareTag("Player")) return;

            activado = true;
            if (GameManager.Instance != null)
                GameManager.Instance.EstablecerCheckpoint(transform.position);
            if (visual != null) visual.color = colorActivo;
        }
    }
}
