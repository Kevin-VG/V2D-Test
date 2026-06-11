using UnityEngine;

namespace Shatter.CameraSystem
{
    /// <summary>
    /// Controlador de cámara 2D personalizado que sigue al jugador usando SmoothDamp.
    /// Incluye look-ahead (anticipación de movimiento), límites de mapa (bounds) y sacudida de pantalla (screen shake).
    /// </summary>
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform objetivo;

        [Header("Ajustes de seguimiento")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
        [Range(0f, 1f)] [SerializeField] private float suavizadoX = 0.15f;
        [Range(0f, 1f)] [SerializeField] private float suavizadoY = 0.25f;

        [Header("Anticipación (Look Ahead)")]
        [SerializeField] private float distanciaAnticipacion = 2f;
        [SerializeField] private float velocidadAnticipacion = 4f;
        [SerializeField] private float umbralMovimientoX = 0.1f;

        [Header("Límites del Mapa (Bounds)")]
        [SerializeField] private bool usarLimites = false;
        [SerializeField] private Vector2 limiteMin = new Vector2(-50f, -10f);
        [SerializeField] private Vector2 limiteMax = new Vector2(50f, 10f);

        [Header("Suavizado Vertical al Saltar")]
        [Tooltip("Si está activo, la cámara será más suave verticalmente cuando el jugador esté en el aire para evitar mareos.")]
        [SerializeField] private bool amortiguarSalto = true;
        [Range(0f, 1f)] [SerializeField] private float suavizadoSaltoY = 0.45f;

        private Vector3 velocidadCamara;
        private float anticipacionActualX;

        // Variables de Screen Shake
        private float duracionSacudida;
        private float magnitudSacudida;
        private Vector3 offsetSacudida;

        private void Start()
        {
            if (objetivo == null)
            {
                // Buscamos automáticamente al jugador usando el tag "Player"
                GameObject jugador = GameObject.FindWithTag("Player");
                if (jugador != null)
                {
                    objetivo = jugador.transform;
                }
            }

            // Posicionamos la cámara instantáneamente al iniciar para evitar transiciones feas
            if (objetivo != null)
            {
                Vector3 posicionInicial = objetivo.position + offset;
                if (usarLimites)
                {
                    posicionInicial.x = Mathf.Clamp(posicionInicial.x, limiteMin.x, limiteMax.x);
                    posicionInicial.y = Mathf.Clamp(posicionInicial.y, limiteMin.y, limiteMax.y);
                }
                transform.position = new Vector3(posicionInicial.x, posicionInicial.y, offset.z);
            }
        }

        private void LateUpdate()
        {
            if (objetivo == null) return;

            // 1. Obtener la velocidad horizontal del Rigidbody2D del objetivo
            float velocidadTargetX = 0f;
            var rb = objetivo.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                velocidadTargetX = rb.linearVelocity.x;
            }

            // 2. Calcular la anticipación horizontal (hacia dónde se mueve el jugador)
            float anticipacionObjetivoX = 0f;
            if (Mathf.Abs(velocidadTargetX) > umbralMovimientoX)
            {
                anticipacionObjetivoX = Mathf.Sign(velocidadTargetX) * distanciaAnticipacion;
            }

            // Transicionar suavemente hacia la anticipación horizontal
            anticipacionActualX = Mathf.MoveTowards(anticipacionActualX, anticipacionObjetivoX, velocidadAnticipacion * Time.deltaTime);

            // 3. Suavizado vertical dinámico si está en el aire saltando/cayendo
            float suavizadoActualY = suavizadoY;
            if (amortiguarSalto && rb != null && Mathf.Abs(rb.linearVelocity.y) > 0.1f)
            {
                suavizadoActualY = suavizadoSaltoY;
            }

            // 4. Posición deseada
            Vector3 posicionDeseada = objetivo.position + offset;
            posicionDeseada.x += anticipacionActualX;

            // 5. Interpolación suave e independiente para cada eje (SmoothDamp)
            float nuevoX = Mathf.SmoothDamp(transform.position.x, posicionDeseada.x, ref velocidadCamara.x, suavizadoX);
            float nuevoY = Mathf.SmoothDamp(transform.position.y, posicionDeseada.y, ref velocidadCamara.y, suavizadoActualY);

            // 6. Aplicar límites de movimiento (Bounds) si están activos
            if (usarLimites)
            {
                nuevoX = Mathf.Clamp(nuevoX, limiteMin.x, limiteMax.x);
                nuevoY = Mathf.Clamp(nuevoY, limiteMin.y, limiteMax.y);
            }

            // 7. Procesar el Screen Shake (Sacudida de pantalla)
            ManejarSacudida();

            // 8. Aplicar la posición final con el desplazamiento Z y el screen shake
            transform.position = new Vector3(nuevoX, nuevoY, offset.z) + offsetSacudida;
        }

        private void ManejarSacudida()
        {
            if (duracionSacudida > 0)
            {
                float shakeX = Random.Range(-1f, 1f) * magnitudSacudida;
                float shakeY = Random.Range(-1f, 1f) * magnitudSacudida;
                offsetSacudida = new Vector3(shakeX, shakeY, 0f);

                duracionSacudida -= Time.deltaTime;
            }
            else
            {
                offsetSacudida = Vector3.zero;
            }
        }

        /// <summary>
        /// Activa una sacudida de pantalla (Screen Shake) para impactos, explosiones, caídas, etc.
        /// </summary>
        /// <param name="duracion">Duración del efecto en segundos.</param>
        /// <param name="magnitud">Intensidad del temblor.</param>
        public void SacudirPantalla(float duracion, float magnitud)
        {
            // Si el jugador ha desactivado la vibración de pantalla en los ajustes, no hacemos nada
            if (PlayerPrefs.GetInt("ScreenShakePreference", 1) == 0)
            {
                offsetSacudida = Vector3.zero;
                return;
            }

            duracionSacudida = duracion;
            magnitudSacudida = magnitud;
        }
    }
}
