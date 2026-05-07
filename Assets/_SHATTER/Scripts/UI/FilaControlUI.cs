using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Shatter.UI
{
    /// <summary>
    /// Componente que va en el Prefab de la fila de controles para poder editar todo visualmente desde Unity.
    /// </summary>
    public class FilaControlUI : MonoBehaviour
    {
        [Header("Componentes de la Fila")]
        [Tooltip("El texto de la acción (ej: 'Saltar') usando Text normal de Unity")]
        public Text textoAccion;
        [Tooltip("El texto de la acción usando TextMeshPro (opcional, si prefieres usar TMP)")]
        public TMP_Text textoAccionTMP;

        [Tooltip("La imagen donde se mostrará la tecla")]
        public Image imagenTecla;

        [Tooltip("El botón de interacción sobre el cual el jugador hace clic para reasignar")]
        public Button botonRemapeo;

        [Tooltip("El texto de respaldo encima del botón de piedra vacío usando Text normal")]
        public Text textoFallback;
        [Tooltip("El texto de respaldo usando TextMeshPro (opcional)")]
        public TMP_Text textoFallbackTMP;

        /// <summary>
        /// Configura los elementos visuales de la fila con los datos de la acción.
        /// </summary>
        public void ConfigurarFila(string accion, Sprite icono, string nombreTecla, Sprite iconoPorDefecto)
        {
            // 1. Configurar textos de la acción
            if (textoAccion != null) textoAccion.text = accion;
            if (textoAccionTMP != null) textoAccionTMP.text = accion;

            // 2. Configurar la imagen de la tecla con escalado dinámico de aspecto
            if (imagenTecla != null)
            {
                Sprite spriteAsignado = (icono != null) ? icono : iconoPorDefecto;
                imagenTecla.sprite = spriteAsignado;
                imagenTecla.preserveAspect = true;

                if (spriteAsignado != null)
                {
                    RectTransform rtKey = imagenTecla.rectTransform;
                    
                    // Calculamos el alto idóneo (tomamos el alto actual o un default de 45 si está en 0)
                    float height = rtKey.rect.height;
                    if (height <= 0)
                    {
                        RectTransform parentRt = rtKey.parent as RectTransform;
                        height = (parentRt != null) ? parentRt.rect.height * 0.8f : 45f;
                    }

                    // Calculamos la proporción (ancho / alto) del sprite
                    float aspect = (float)spriteAsignado.rect.width / spriteAsignado.rect.height;
                    float newWidth = height * aspect;

                    // Únicamente cambiamos el tamaño del ancho para respetar la relación de aspecto.
                    // Respetamos al 100% la posición, anclaje y pivote que tú definas en el inspector de Unity!
                    rtKey.sizeDelta = new Vector2(newWidth, height);
                }
            }

            // 3. Configurar texto de respaldo (solo se activa si no hay un icono personalizado asignado)
            bool usaFallback = (icono == null || icono == iconoPorDefecto);

            if (textoFallback != null)
            {
                textoFallback.gameObject.SetActive(usaFallback);
                if (usaFallback) textoFallback.text = nombreTecla;
            }

            if (textoFallbackTMP != null)
            {
                textoFallbackTMP.gameObject.SetActive(usaFallback);
                if (usaFallback) textoFallbackTMP.text = nombreTecla;
            }
        }
    }
}
