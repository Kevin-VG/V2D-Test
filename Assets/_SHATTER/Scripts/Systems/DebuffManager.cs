using UnityEngine;
using Shatter.Core;
using Shatter.Player;
using Shatter.ScriptableObjects;

namespace Shatter.Systems
{
    /// <summary>
    /// Aplica debuffs al player. Solo 1 simultaneo en el prototipo.
    /// </summary>
    public class DebuffManager : MonoBehaviour
    {
        private DebuffSO activo;
        private float restante;

        private PlayerController2D controlador;

        public DebuffSO Activo => activo;
        public float Restante => restante;
        public float RestanteNormalizado => activo != null && activo.duracion > 0f ? restante / activo.duracion : 0f;

        private void Awake()
        {
            controlador = GetComponent<PlayerController2D>();
        }

        public void Aplicar(DebuffSO so)
        {
            if (so == null || controlador == null) return;
            // Si el escudo esta activo, consume el escudo y no aplica debuff.
            var power = GetComponent<PowerUpManager>();
            if (power != null && power.ConsumirEscudo()) return;

            if (activo != null) Limpiar();

            activo = so;
            restante = so.duracion;
            AplicarEfecto(so);
            GameEvents.LanzarDebuffAplicado(so.nombreDebuff, so.duracion);
            if (AudioManager.Instance != null && so.sonidoAplicacion != null)
                AudioManager.Instance.ReproducirEfecto(so.sonidoAplicacion);
        }

        private void Update()
        {
            if (activo == null) return;
            restante -= Time.deltaTime;
            if (restante <= 0f) Limpiar();
        }

        private void AplicarEfecto(DebuffSO so)
        {
            switch (so.tipo)
            {
                case TipoDebuff.VisionTunel:
                    controlador.AplicarMultiplicadorVelocidad(so.multiplicadorVelocidad);
                    break;
                case TipoDebuff.PasosDePlomo:
                    controlador.AplicarMultiplicadorVelocidad(so.multiplicadorVelocidad);
                    break;
                case TipoDebuff.ControlesInvertidos:
                    controlador.EstablecerControlesInvertidos(true);
                    break;
            }
        }

        private void Limpiar()
        {
            if (activo == null) return;
            string nombre = activo.nombreDebuff;
            switch (activo.tipo)
            {
                case TipoDebuff.VisionTunel:
                case TipoDebuff.PasosDePlomo:
                    controlador.AplicarMultiplicadorVelocidad(1f);
                    break;
                case TipoDebuff.ControlesInvertidos:
                    controlador.EstablecerControlesInvertidos(false);
                    break;
            }
            activo = null;
            restante = 0f;
            GameEvents.LanzarDebuffExpirado(nombre);
        }
    }
}
