using UnityEngine;
using Shatter.Core;

namespace Shatter.CameraSystem
{
    /// <summary>
    /// Smooth follow 2D con look-ahead horizontal, deadzone vertical y screen shake.
    /// </summary>
    public class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform objetivo;
        [SerializeField] private Vector3 desplazamiento = new Vector3(0f, 1.2f, -10f);
        [SerializeField] private float tiempoSuavizado = 0.15f;
        [SerializeField] private float distanciaAnticipacion = 2f;
        [SerializeField] private float suavizadoAnticipacion = 0.3f;

        [Header("Limites opcionales")]
        [SerializeField] private bool usarLimites;
        [SerializeField] private Vector2 limitesMinimos;
        [SerializeField] private Vector2 limitesMaximos;

        private Vector3 velocidadActual;
        private float anticipacionActual;
        private float velocidadAnticipacion;

        private float intensidadTemblor;
        private float temporizadorTemblor;

        public void EstablecerObjetivo(Transform t) { objetivo = t; }

        private void OnEnable() { GameEvents.AlAgitarCamara += ManejarTemblor; }
        private void OnDisable() { GameEvents.AlAgitarCamara -= ManejarTemblor; }

        private void ManejarTemblor(float intensidad, float duracion)
        {
            if (intensidad > intensidadTemblor) intensidadTemblor = intensidad;
            if (duracion > temporizadorTemblor) temporizadorTemblor = duracion;
        }

        private void LateUpdate()
        {
            if (objetivo == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) objetivo = p.transform;
                else return;
            }

            float velXObjetivo = 0f;
            var rb = objetivo.GetComponent<Rigidbody2D>();
            if (rb != null) velXObjetivo = rb.linearVelocity.x;

            float anticipacionDeseada = Mathf.Clamp(velXObjetivo / 10f, -1f, 1f) * distanciaAnticipacion;
            anticipacionActual = Mathf.SmoothDamp(anticipacionActual, anticipacionDeseada, ref velocidadAnticipacion, suavizadoAnticipacion);

            Vector3 deseada = objetivo.position + desplazamiento + new Vector3(anticipacionActual, 0f, 0f);
            Vector3 suavizada = Vector3.SmoothDamp(transform.position, deseada, ref velocidadActual, tiempoSuavizado);

            if (usarLimites)
            {
                suavizada.x = Mathf.Clamp(suavizada.x, limitesMinimos.x, limitesMaximos.x);
                suavizada.y = Mathf.Clamp(suavizada.y, limitesMinimos.y, limitesMaximos.y);
            }

            if (temporizadorTemblor > 0f)
            {
                temporizadorTemblor -= Time.deltaTime;
                Vector3 ruido = new Vector3(
                    (Mathf.PerlinNoise(Time.time * 30f, 0f) - 0.5f) * 2f,
                    (Mathf.PerlinNoise(0f, Time.time * 30f) - 0.5f) * 2f,
                    0f) * intensidadTemblor;
                suavizada += ruido;
                if (temporizadorTemblor <= 0f) intensidadTemblor = 0f;
            }

            transform.position = suavizada;
        }
    }
}
