using UnityEngine;

namespace Shatter.AI
{
    /// <summary>
    /// Patrulla flotante entre dos puntos, ignora gravedad.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyerEnemy : EnemyBase
    {
        [SerializeField] private float velocidadVuelo = 2f;
        [SerializeField] private Vector2 offsetPatrulla = new Vector2(3f, 0f);
        [SerializeField] private float amplitudFlotacion = 0.3f;
        [SerializeField] private float frecuenciaFlotacion = 2f;

        private Rigidbody2D rb;
        private Vector2 puntoA;
        private Vector2 puntoB;
        private Vector2 objetivo;
        private float faseFlotacion;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            puntoA = transform.position;
            puntoB = puntoA + offsetPatrulla;
            objetivo = puntoB;
        }

        private void FixedUpdate()
        {
            if (!EstaVivo) return;
            faseFlotacion += Time.fixedDeltaTime * frecuenciaFlotacion;
            Vector2 flotacion = new Vector2(0f, Mathf.Sin(faseFlotacion) * amplitudFlotacion);
            Vector2 hacia = (objetivo - (Vector2)transform.position);
            Vector2 dir = hacia.sqrMagnitude > 0.01f ? hacia.normalized : Vector2.zero;
            rb.linearVelocity = dir * velocidadVuelo * multiplicadorVelocidad + flotacion;

            if (Vector2.Distance(transform.position, objetivo) < 0.2f)
                objetivo = (objetivo == puntoA) ? puntoB : puntoA;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Vector3 a = Application.isPlaying ? (Vector3)puntoA : transform.position;
            Vector3 b = Application.isPlaying ? (Vector3)puntoB : transform.position + (Vector3)offsetPatrulla;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawWireSphere(a, 0.2f);
            Gizmos.DrawWireSphere(b, 0.2f);
        }
    }
}
