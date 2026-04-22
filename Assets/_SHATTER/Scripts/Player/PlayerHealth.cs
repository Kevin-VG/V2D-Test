using System.Collections;
using UnityEngine;
using Shatter.Core;

namespace Shatter.Player
{
    /// <summary>
    /// Sistema de vida basado en "Fragmentos". IFrames, knockback, respawn por checkpoint.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int fragmentosMaximos = 4;
        [SerializeField] private float tiempoInvulnerabilidad = 1f;
        [SerializeField] private float fuerzaRetroceso = 8f;
        [SerializeField] private float reduccionDano = 0f; // 0..1

        private int fragmentosActuales;
        private bool esInvulnerable;
        private Rigidbody2D rb;
        private SpriteRenderer[] renderizadores;
        private Vector3 posicionInicialSpawn;

        public int Actuales => fragmentosActuales;
        public int Maximo => fragmentosMaximos;
        public bool EsInvulnerable => esInvulnerable;

        public void EstablecerReduccionDano(float r) => reduccionDano = Mathf.Clamp01(r);
        public void AgregarFragmentosMaximos(int delta) { fragmentosMaximos = Mathf.Max(1, fragmentosMaximos + delta); fragmentosActuales = Mathf.Min(fragmentosActuales, fragmentosMaximos); }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            renderizadores = GetComponentsInChildren<SpriteRenderer>();
            fragmentosActuales = fragmentosMaximos;
            posicionInicialSpawn = transform.position;
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AlMorirJugador += ManejarMuerte;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AlMorirJugador -= ManejarMuerte;
        }

        public void RecibirDano(int cantidad, Vector2 posicionFuente)
        {
            if (esInvulnerable || cantidad <= 0) return;

            int reducido = Mathf.Max(1, Mathf.RoundToInt(cantidad * (1f - reduccionDano)));
            fragmentosActuales -= reducido;
            GameEvents.LanzarGolpeJugador(reducido);
            GameEvents.LanzarAgitacionCamara(0.2f, 0.12f);

            if (fragmentosActuales <= 0)
            {
                fragmentosActuales = 0;
                Morir();
                return;
            }

            // Retroceso
            Vector2 dir = ((Vector2)transform.position - posicionFuente).normalized;
            if (dir.sqrMagnitude < 0.001f) dir = Vector2.up;
            rb.linearVelocity = new Vector2(dir.x * fuerzaRetroceso, fuerzaRetroceso * 0.6f);

            StartCoroutine(DestelloInvulnerabilidad());
        }

        private IEnumerator DestelloInvulnerabilidad()
        {
            esInvulnerable = true;
            float t = 0f;
            while (t < tiempoInvulnerabilidad)
            {
                EstablecerAlfaRenderizadores(0.3f);
                yield return new WaitForSeconds(0.08f);
                EstablecerAlfaRenderizadores(1f);
                yield return new WaitForSeconds(0.08f);
                t += 0.16f;
            }
            EstablecerAlfaRenderizadores(1f);
            esInvulnerable = false;
        }

        private void EstablecerAlfaRenderizadores(float a)
        {
            if (renderizadores == null) return;
            foreach (var r in renderizadores)
            {
                if (r == null) continue;
                var c = r.color; c.a = a; r.color = c;
            }
        }

        private void Morir()
        {
            GameEvents.LanzarMuerteJugador();
            if (GameManager.Instance != null)
                GameManager.Instance.NotificarMuerteJugador();
            ManejarMuerte();
        }

        private void ManejarMuerte()
        {
            Vector3 respawn = GameManager.Instance != null
                ? GameManager.Instance.ObtenerPosicionRespawn(posicionInicialSpawn)
                : posicionInicialSpawn;
            transform.position = respawn;
            rb.linearVelocity = Vector2.zero;
            fragmentosActuales = fragmentosMaximos;
            GameEvents.LanzarReaparicionJugador();
        }

        public void Curar(int cantidad)
        {
            fragmentosActuales = Mathf.Min(fragmentosMaximos, fragmentosActuales + cantidad);
        }
    }
}
