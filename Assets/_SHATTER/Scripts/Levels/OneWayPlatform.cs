using System.Collections;
using UnityEngine;

namespace Shatter.Levels
{
    /// <summary>
    /// Plataforma one-way: permite saltar desde abajo, bloquea desde arriba.
    /// Drop-through con S+Espacio. Usa PlatformEffector2D de Unity.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(PlatformEffector2D))]
    public class OneWayPlatform : MonoBehaviour
    {
        [SerializeField] private float duracionCaida = 0.35f;
        private Collider2D col;
        private PlatformEffector2D efector;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.usedByEffector = true;

            efector = GetComponent<PlatformEffector2D>();
            efector.surfaceArc = 170f;       // angulo de la superficie solida (arriba)
            efector.useOneWay = true;
            efector.rotationalOffset = 0f;   // superficie apunta hacia arriba
        }

        public void DesactivarColisionTemporal(Collider2D colJugador)
        {
            if (col == null || colJugador == null) return;
            StartCoroutine(Ignorar(colJugador));
        }

        private IEnumerator Ignorar(Collider2D colJugador)
        {
            Physics2D.IgnoreCollision(col, colJugador, true);
            yield return new WaitForSeconds(duracionCaida);
            if (col != null && colJugador != null)
                Physics2D.IgnoreCollision(col, colJugador, false);
        }
    }
}
