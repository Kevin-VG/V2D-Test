using UnityEngine;

namespace Shatter.CameraSystem
{
    /// <summary>
    /// Capa de parallax. Se mueve proporcional al desplazamiento de la camara.
    /// factorParallax = 0 -> estatica (fondo infinito). 1 -> sigue 1:1 con la camara.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [Range(0f, 1.5f)] public float factorParallaxX = 0.5f;
        [Range(0f, 1.5f)] public float factorParallaxY = 0.1f;
        public bool desplazamientoInfinitoX;

        private Transform camara;
        private Vector3 ultimaPosicionCamara;
        private float anchuraSprite;

        private void Start()
        {
            camara = Camera.main != null ? Camera.main.transform : null;
            if (camara != null) ultimaPosicionCamara = camara.position;

            if (desplazamientoInfinitoX)
            {
                var sr = GetComponentInChildren<SpriteRenderer>();
                if (sr != null) anchuraSprite = sr.bounds.size.x;
            }
        }

        private void LateUpdate()
        {
            if (camara == null) { camara = Camera.main != null ? Camera.main.transform : null; if (camara == null) return; ultimaPosicionCamara = camara.position; }

            Vector3 delta = camara.position - ultimaPosicionCamara;
            transform.position += new Vector3(delta.x * factorParallaxX, delta.y * factorParallaxY, 0f);
            ultimaPosicionCamara = camara.position;

            if (desplazamientoInfinitoX && anchuraSprite > 0f)
            {
                float dx = camara.position.x - transform.position.x;
                if (Mathf.Abs(dx) >= anchuraSprite)
                {
                    float offset = (dx % anchuraSprite);
                    transform.position = new Vector3(camara.position.x - offset, transform.position.y, transform.position.z);
                }
            }
        }
    }
}
