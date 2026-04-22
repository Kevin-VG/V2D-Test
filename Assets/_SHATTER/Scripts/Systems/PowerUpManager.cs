using System.Collections.Generic;
using UnityEngine;
using Shatter.Core;
using Shatter.Player;
using Shatter.ScriptableObjects;

namespace Shatter.Systems
{
    /// <summary>
    /// Gestiona el power-up activo en el player. Solo 1 simultaneo.
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        private PowerUpSO activo;
        private float restante;
        private bool escudoDisponible;

        private PlayerController2D controlador;
        private float multiplicadorVelocidadBase = 1f;
        private int saltosExtraBase;

        public PowerUpSO Activo => activo;
        public float Restante => restante;
        public float RestanteNormalizado => activo != null && activo.duracion > 0f ? restante / activo.duracion : 0f;
        public bool TieneEscudo => escudoDisponible;

        private void Awake()
        {
            controlador = GetComponent<PlayerController2D>();
            if (controlador != null) saltosExtraBase = controlador.ObtenerSaltosExtra();
        }

        public void Activar(PowerUpSO so)
        {
            if (so == null) return;
            if (activo != null) Desactivar();

            activo = so;
            restante = so.duracion;
            AplicarEfecto(so);
            GameEvents.LanzarPowerUpActivado(so.nombrePowerUp, so.duracion);
            if (AudioManager.Instance != null && so.sonidoActivacion != null)
                AudioManager.Instance.ReproducirEfecto(so.sonidoActivacion);
        }

        private void Update()
        {
            if (activo == null) return;
            // El escudo no se descuenta por tiempo — dura hasta recibir golpe.
            if (activo.tipo == TipoPowerUp.EscudoRespiracion) return;

            restante -= Time.deltaTime;
            if (restante <= 0f) Desactivar();
        }

        private void AplicarEfecto(PowerUpSO so)
        {
            switch (so.tipo)
            {
                case TipoPowerUp.LatidoCalmado:
                    AplicarRalentizacionEnemigos(so.radio, 0.4f, so.duracion);
                    break;
                case TipoPowerUp.DestelloGuia:
                    ResaltarColeccionables(so.radio, so.duracion);
                    break;
                case TipoPowerUp.EscudoRespiracion:
                    escudoDisponible = true;
                    break;
                case TipoPowerUp.MemoriaClara:
                    if (controlador != null) controlador.AplicarMultiplicadorVelocidad(multiplicadorVelocidadBase * (1f + so.magnitud));
                    break;
                case TipoPowerUp.FocoClaridad:
                    if (controlador != null) controlador.EstablecerSaltosExtra(saltosExtraBase + 1);
                    break;
            }
        }

        private void Desactivar()
        {
            if (activo == null) return;
            string nombre = activo.nombrePowerUp;
            switch (activo.tipo)
            {
                case TipoPowerUp.MemoriaClara:
                    if (controlador != null) controlador.AplicarMultiplicadorVelocidad(multiplicadorVelocidadBase);
                    break;
                case TipoPowerUp.FocoClaridad:
                    if (controlador != null) controlador.EstablecerSaltosExtra(saltosExtraBase);
                    break;
                case TipoPowerUp.EscudoRespiracion:
                    escudoDisponible = false;
                    break;
            }
            activo = null;
            restante = 0f;
            GameEvents.LanzarPowerUpExpirado(nombre);
        }

        public bool ConsumirEscudo()
        {
            if (!escudoDisponible) return false;
            escudoDisponible = false;
            if (activo != null && activo.tipo == TipoPowerUp.EscudoRespiracion) Desactivar();
            return true;
        }

        private void AplicarRalentizacionEnemigos(float radio, float multiplicador, float duracion)
        {
            var impactos = Physics2D.OverlapCircleAll(transform.position, radio);
            foreach (var h in impactos)
            {
                var e = h.GetComponentInParent<AI.EnemyBase>();
                if (e != null) e.AplicarRalentizacion(multiplicador, duracion);
            }
        }

        private void ResaltarColeccionables(float radio, float duracion)
        {
            var impactos = Physics2D.OverlapCircleAll(transform.position, radio);
            foreach (var h in impactos)
            {
                var c = h.GetComponentInParent<Collectible>();
                if (c != null) c.Resaltar(duracion);
            }
        }
    }
}
