using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Shatter.Player;

namespace Shatter.UI
{
    /// <summary>
    /// Joystick virtual para UI de dispositivos móviles.
    /// Controla el movimiento horizontal del PlayerController2D.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Elementos UI")]
        [Tooltip("El fondo del joystick (círculo exterior)")]
        [SerializeField] private RectTransform fondoJoystick;
        [Tooltip("El pomo o manija del joystick (círculo interior)")]
        [SerializeField] private RectTransform manijaJoystick;

        [Header("Configuración")]
        [Tooltip("El radio máximo de movimiento de la manija")]
        [SerializeField] private float radioMovimiento = 100f;

        private Vector2 vectorEntrada = Vector2.zero;
        private PlayerController2D playerController;

        private void Start()
        {
            // Si no se asignaron en el Inspector, intentamos obtenerlos del componente y sus hijos
            if (fondoJoystick == null)
                fondoJoystick = GetComponent<RectTransform>();
            
            if (manijaJoystick == null && transform.childCount > 0)
                manijaJoystick = transform.GetChild(0).GetComponent<RectTransform>();

            // Buscar automáticamente al jugador en la escena
            EncontrarJugador();
        }

        private void EncontrarJugador()
        {
            // En versiones nuevas de Unity, FindFirstObjectByType es la forma recomendada en vez de FindObjectOfType
            playerController = FindFirstObjectByType<PlayerController2D>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 posicionLocal;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                fondoJoystick, 
                eventData.position, 
                eventData.pressEventCamera, 
                out posicionLocal))
            {
                // Limitar la posición de la manija dentro del radio máximo configurado
                posicionLocal = Vector2.ClampMagnitude(posicionLocal, radioMovimiento);
                
                // Mover la manija visualmente
                if (manijaJoystick != null)
                {
                    manijaJoystick.anchoredPosition = posicionLocal;
                }

                // Calcular el vector de entrada normalizado (valores entre -1 y 1)
                vectorEntrada = posicionLocal / radioMovimiento;

                // Enviar la entrada de movimiento horizontal al jugador
                EnviarInputAlJugador(vectorEntrada.x);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Activar el arrastre inmediatamente al tocar la zona del joystick
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Resetear la posición de la manija al centro al soltar el dedo
            vectorEntrada = Vector2.zero;
            if (manijaJoystick != null)
            {
                manijaJoystick.anchoredPosition = Vector2.zero;
            }

            // Detener el movimiento horizontal del jugador enviando 0
            EnviarInputAlJugador(0f);
        }

        private void EnviarInputAlJugador(float valorX)
        {
            if (playerController == null)
            {
                EncontrarJugador();
            }

            if (playerController != null)
            {
                playerController.EntradaJoystickHorizontal = valorX;
            }
        }
    }
}
