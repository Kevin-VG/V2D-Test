using UnityEngine;
using System;

namespace Shatter.Player
{
    /// <summary>
    /// Gestiona el recurso de Enfoque (Focus) / Chispas.
    /// Se consume al realizar acciones avanzadas (doble salto, dash, wall jump).
    /// Se regenera al estar en el suelo o mediante coleccionables.
    /// </summary>
    public class FocusSystem : MonoBehaviour
    {
        [Header("Configuracion de Enfoque")]
        [SerializeField] private float enfoqueMaximo = 100f;
        [SerializeField] private float regeneracionPorSegundo = 25f;
        [SerializeField] private float retrasoRegeneracion = 0.5f;

        [Header("Costes de Accion")]
        [SerializeField] private float costeDobleSalto = 30f;
        [SerializeField] private float costeDash = 40f;
        [SerializeField] private float costeWallJump = 20f;

        private float enfoqueActual;
        private float temporizadorRetraso;

        public event Action<float, float> OnFocusChanged; // (actual, maximo)

        public float EnfoqueActual => enfoqueActual;
        public float EnfoqueMaximo => enfoqueMaximo;
        public float PorcentajeEnfoque => enfoqueActual / enfoqueMaximo;

        private void Awake()
        {
            enfoqueActual = enfoqueMaximo;
        }

        private void Update()
        {
            ManejarRegeneracion();
        }

        private void ManejarRegeneracion()
        {
            if (temporizadorRetraso > 0)
            {
                temporizadorRetraso -= Time.deltaTime;
                return;
            }

            if (enfoqueActual < enfoqueMaximo)
            {
                enfoqueActual = Mathf.Min(enfoqueActual + regeneracionPorSegundo * Time.deltaTime, enfoqueMaximo);
                OnFocusChanged?.Invoke(enfoqueActual, enfoqueMaximo);
            }
        }

        public bool ConsumirEnfoque(float cantidad)
        {
            if (enfoqueActual >= cantidad)
            {
                enfoqueActual -= cantidad;
                temporizadorRetraso = retrasoRegeneracion;
                OnFocusChanged?.Invoke(enfoqueActual, enfoqueMaximo);
                return true;
            }
            return false;
        }

        public bool PuedeRealizarDobleSalto() => enfoqueActual >= costeDobleSalto;
        public bool PuedeRealizarDash() => enfoqueActual >= costeDash;
        public bool PuedeRealizarWallJump() => enfoqueActual >= costeWallJump;

        public void ConsumirDobleSalto() => ConsumirEnfoque(costeDobleSalto);
        public void ConsumirDash() => ConsumirEnfoque(costeDash);
        public void ConsumirWallJump() => ConsumirEnfoque(costeWallJump);

        public void RecargarCompletamente()
        {
            enfoqueActual = enfoqueMaximo;
            OnFocusChanged?.Invoke(enfoqueActual, enfoqueMaximo);
        }

        public void AñadirEnfoque(float cantidad)
        {
            enfoqueActual = Mathf.Min(enfoqueActual + cantidad, enfoqueMaximo);
            OnFocusChanged?.Invoke(enfoqueActual, enfoqueMaximo);
        }
    }
}
