using UnityEngine;

namespace Shatter.CameraSystem
{
    /// <summary>
    /// Hace que un GameObject de fondo (ej: montañas, nubes, cielo) siga a la cámara con efecto Parallax.
    /// Soporta repetición infinita horizontal (tiling) para fondos continuos usando SpriteRenderer.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform camara;

        [Header("Efecto Parallax")]
        [Tooltip("Factor de movimiento horizontal. 0 = estático en el mundo. 1 = se mueve 1:1 con la cámara (cielo infinito). 0.8 = montañas lejanas.")]
        [Range(0f, 1f)] [SerializeField] private float factorParallaxX = 0.5f;
        [Tooltip("Factor de movimiento vertical. 0 = estático. 1 = se mueve 1:1 con la cámara.")]
        [Range(0f, 1f)] [SerializeField] private float factorParallaxY = 0.3f;

        [Header("Repetición Infinita (Tiling)")]
        [Tooltip("¿El fondo debe repetirse infinitamente al avanzar horizontalmente? (Requiere SpriteRenderer)")]
        [SerializeField] private bool repetirInfinitoX = false;

        private float longitudSpriteX;
        private float posicionInicialX;
        private float posicionInicialY;

        private void Start()
        {
            // Si no se asigna la cámara, buscamos automáticamente la Main Camera
            if (camara == null)
            {
                var mainCam = Camera.main;
                if (mainCam != null)
                {
                    camara = mainCam.transform;
                }
            }

            posicionInicialX = transform.position.x;
            posicionInicialY = transform.position.y;

            // Obtener el ancho del sprite para la repetición infinita horizontal
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                longitudSpriteX = spriteRenderer.bounds.size.x;
            }
            else
            {
                repetirInfinitoX = false; // Desactivar si no hay SpriteRenderer
            }
        }

        private void LateUpdate()
        {
            if (camara == null) return;

            // 1. Calcular la distancia que la cámara se ha movido respecto al inicio, multiplicada por el factor
            float distanciaX = camara.position.x * factorParallaxX;
            float distanciaY = camara.position.y * factorParallaxY;

            // 2. Mover el fondo a la nueva posición con el desplazamiento de desfase
            transform.position = new Vector3(posicionInicialX + distanciaX, posicionInicialY + distanciaY, transform.position.z);

            // 3. Lógica de repetición infinita (Tiling) horizontal
            if (repetirInfinitoX)
            {
                // Calcula el movimiento relativo de la cámara respecto al fondo
                float temp = camara.position.x * (1 - factorParallaxX);

                if (temp > posicionInicialX + longitudSpriteX)
                {
                    posicionInicialX += longitudSpriteX;
                }
                else if (temp < posicionInicialX - longitudSpriteX)
                {
                    posicionInicialX -= longitudSpriteX;
                }
            }
        }
    }
}
