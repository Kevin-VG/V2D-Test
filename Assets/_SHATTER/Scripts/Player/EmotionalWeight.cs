using UnityEngine;
using System;

namespace Shatter.Player
{
    /// <summary>
    /// Gestiona el Peso Emocional del personaje.
    /// A mayor peso, menor es la velocidad de movimiento y mayor la inercia.
    /// El peso puede aumentar al recoger ciertos objetos o por eventos de la trama.
    /// </summary>
    public class EmotionalWeight : MonoBehaviour
    {
        [Header("Configuracion de Peso")]
        [SerializeField] private float pesoBase = 0f;
        [SerializeField] private float pesoMaximo = 100f;
        
        [Header("Efectos en Gameplay")]
        [SerializeField] private float reduccionVelocidadMax = 0.5f; // Al 100% de peso, se mueve a la mitad
        [SerializeField] private float aumentoGravedadMax = 2f;    // Al 100% de peso, la gravedad es el doble

        private float pesoActual;

        public event Action<float, float> OnWeightChanged; // (actual, maximo)

        public float PesoActual => pesoActual;
        public float PorcentajePeso => pesoActual / pesoMaximo;

        private void Awake()
        {
            pesoActual = pesoBase;
        }

        public void ModificarPeso(float cantidad)
        {
            pesoActual = Mathf.Clamp(pesoActual + cantidad, 0f, pesoMaximo);
            ActualizarEfectos();
            OnWeightChanged?.Invoke(pesoActual, pesoMaximo);
        }

        private void ActualizarEfectos()
        {
            // Calculamos el multiplicador de velocidad basado en el peso
            // 0 peso -> multiplicador 1.0
            // Max peso -> multiplicador (1.0 - reduccionVelocidadMax)
            float t = PorcentajePeso;
            float multiplicadorVel = Mathf.Lerp(1f, 1f - reduccionVelocidadMax, t);
            
            // Enviamos el multiplicador al controlador
            var controller = GetComponent<PlayerController2D>();
            if (controller != null)
            {
                controller.AplicarMultiplicadorVelocidad(multiplicadorVel);
            }

            // Podriamos ajustar la gravedad del Rigidbody aqui tambien si fuera necesario
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // rb.gravityScale = ... (aunque el controlador ya maneja escalas de gravedad propias)
            }
        }

        public void ResetearPeso()
        {
            pesoActual = pesoBase;
            ActualizarEfectos();
            OnWeightChanged?.Invoke(pesoActual, pesoMaximo);
        }
    }
}
