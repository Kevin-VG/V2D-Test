using UnityEngine;

namespace Shatter.AI
{
    /// <summary>
    /// Patrulla horizontal, gira al llegar a un borde o pared.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class WalkerEnemy : EnemyBase
    {
        [SerializeField] private float velocidadPatrulla = 2.5f;
        [SerializeField] private float distanciaChequeoBorde = 0.6f;
        [SerializeField] private float distanciaChequeoPared = 0.35f;
        [SerializeField] private LayerMask capaSuelo;

        private Rigidbody2D rb;
        private int direccion = 1;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            if (!EstaVivo) return;
            Patrullar();
        }

        private void Patrullar()
        {
            Vector2 vel = rb.linearVelocity;
            vel.x = velocidadPatrulla * direccion * multiplicadorVelocidad;
            rb.linearVelocity = vel;

            Vector2 origen = (Vector2)transform.position + new Vector2(direccion * 0.4f, -0.4f);
            bool borde = Physics2D.Raycast(origen, Vector2.down, distanciaChequeoBorde, capaSuelo);

            Vector2 origenPared = (Vector2)transform.position + new Vector2(direccion * 0.4f, 0f);
            bool pared = Physics2D.Raycast(origenPared, Vector2.right * direccion, distanciaChequeoPared, capaSuelo);

            if (!borde || pared) direccion = -direccion;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + new Vector3(0.4f, -0.4f), transform.position + new Vector3(0.4f, -0.4f - distanciaChequeoBorde));
            Gizmos.DrawLine(transform.position + new Vector3(-0.4f, -0.4f), transform.position + new Vector3(-0.4f, -0.4f - distanciaChequeoBorde));
        }
    }
}
