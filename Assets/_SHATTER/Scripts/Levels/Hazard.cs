using UnityEngine;
using Shatter.Player;
using Shatter.Systems;
using Shatter.ScriptableObjects;

namespace Shatter.Levels
{
    /// <summary>
    /// Trigger que aplica dano y/o debuff al player.
    /// Usado para pinchos, zonas oscuras, acido.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private int dano = 1;
        [SerializeField] private DebuffSO debuffAlTocar;
        [SerializeField] private bool matarInstantaneamente;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D otro) { IntentarAplicar(otro); }
        private void OnTriggerStay2D(Collider2D otro) { IntentarAplicar(otro); }

        private void IntentarAplicar(Collider2D otro)
        {
            if (!otro.CompareTag("Player")) return;
            var salud = otro.GetComponent<PlayerHealth>();
            if (salud == null || salud.EsInvulnerable) return;

            int danio = matarInstantaneamente ? 9999 : dano;
            salud.RecibirDano(danio, transform.position);

            if (debuffAlTocar != null)
            {
                var debuff = otro.GetComponent<DebuffManager>();
                if (debuff != null) debuff.Aplicar(debuffAlTocar);
            }
        }
    }
}
