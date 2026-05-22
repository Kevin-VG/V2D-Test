using UnityEngine;
using Shatter.Core;
using Shatter.Player;
using Shatter.Systems;
using Shatter.ScriptableObjects;

namespace Shatter.AI
{
    /// <summary>
    /// Clase abstracta para enemigos. Maneja dano al tocar al player y debuff asociado.
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Estadisticas")]
        [SerializeField] protected int vidaMaxima = 3;
        [SerializeField] protected int danoContacto = 1;
        [SerializeField] protected DebuffSO debuffAlContacto;

        protected int vidaActual;
        protected float multiplicadorVelocidad = 1f;
        protected float temporizadorRalentizacion;

        public bool EstaVivo => vidaActual > 0;

        protected virtual void Awake()
        {
            vidaActual = vidaMaxima;
        }

        protected virtual void Update()
        {
            if (temporizadorRalentizacion > 0f)
            {
                temporizadorRalentizacion -= Time.deltaTime;
                if (temporizadorRalentizacion <= 0f) multiplicadorVelocidad = 1f;
            }
        }

        public void AplicarRalentizacion(float multiplicador, float duracion)
        {
            multiplicadorVelocidad = Mathf.Clamp(multiplicador, 0.1f, 1f);
            temporizadorRalentizacion = Mathf.Max(temporizadorRalentizacion, duracion);
        }

        public virtual void RecibirDano(int cantidad)
        {
            if (!EstaVivo) return;
            vidaActual -= cantidad;
            if (vidaActual <= 0) Morir();
        }

        protected virtual void Morir()
        {
            GameEvents.LanzarEnemigoDerotado(gameObject);
            gameObject.SetActive(false);
        }

        protected void OnCollisionEnter2D(Collision2D col) { IntentarGolpearJugador(col.collider); }
        protected void OnCollisionStay2D(Collision2D col) { IntentarGolpearJugador(col.collider); }
        protected void OnTriggerEnter2D(Collider2D col) { IntentarGolpearJugador(col); }
        protected void OnTriggerStay2D(Collider2D col) { IntentarGolpearJugador(col); }

        private void IntentarGolpearJugador(Collider2D col)
        {
            if (col == null || !col.CompareTag("Player")) return;
            var salud = col.GetComponent<PlayerHealth>();
            if (salud == null || salud.EsInvulnerable) return;

            // Jump-stomp: si el player cae sobre el enemigo, recibe dano el enemigo
            var controlador = col.GetComponent<PlayerController2D>();
            if (controlador != null && controlador.VelocidadY < -1f && col.transform.position.y > transform.position.y + 0.3f)
            {
                RecibirDano(1);
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 12f);
                return;
            }

            salud.RecibirDano(danoContacto, transform.position);
            if (debuffAlContacto != null)
            {
                var debuff = col.GetComponent<DebuffManager>();
                if (debuff != null) debuff.Aplicar(debuffAlContacto);
            }
        }
    }
}
