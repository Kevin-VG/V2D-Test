using UnityEngine;
using UnityEngine.EventSystems;

namespace Shatter.UI
{
    /// <summary>
    /// Permite arrastrar botones del HUD móvil durante la personalización
    /// y guardar su posición en PlayerPrefs.
    /// </summary>
    public class DragHUDElement : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public static bool ModoEdicionHUD = false;

        private RectTransform rectTransform;
        private Canvas canvas;
        private MobileActionButton actionButton;
        private Vector2 posicionOriginal;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            actionButton = GetComponent<MobileActionButton>();
            
            if (rectTransform != null)
            {
                posicionOriginal = rectTransform.anchoredPosition;
            }
        }

        private void Start()
        {
            CargarPosicion();
        }

        public void CargarPosicion()
        {
            if (rectTransform == null) return;

            string keyX = gameObject.name + "_PosX";
            string keyY = gameObject.name + "_PosY";

            if (PlayerPrefs.HasKey(keyX) && PlayerPrefs.HasKey(keyY))
            {
                rectTransform.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(keyX), PlayerPrefs.GetFloat(keyY));
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!ModoEdicionHUD || rectTransform == null) return;

            if (canvas != null)
            {
                // Mueve el botón escalando según la escala del canvas
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
            else
            {
                rectTransform.anchoredPosition += eventData.delta;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!ModoEdicionHUD) return;

            GuardarPosicion();
        }

        public void GuardarPosicion()
        {
            if (rectTransform == null) return;

            string keyX = gameObject.name + "_PosX";
            string keyY = gameObject.name + "_PosY";
            PlayerPrefs.SetFloat(keyX, rectTransform.anchoredPosition.x);
            PlayerPrefs.SetFloat(keyY, rectTransform.anchoredPosition.y);
            PlayerPrefs.Save();
        }

        public void RestablecerOriginal()
        {
            if (rectTransform == null) return;

            rectTransform.anchoredPosition = posicionOriginal;
            string keyX = gameObject.name + "_PosX";
            string keyY = gameObject.name + "_PosY";
            PlayerPrefs.DeleteKey(keyX);
            PlayerPrefs.DeleteKey(keyY);
            PlayerPrefs.Save();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Bloqueamos eventos de MobileActionButton si estamos en modo edición
            if (ModoEdicionHUD)
            {
                eventData.Use(); // Consume el evento para que no pase al botón de juego
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (ModoEdicionHUD)
            {
                eventData.Use();
            }
        }
    }
}
