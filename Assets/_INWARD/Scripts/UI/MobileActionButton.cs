using UnityEngine;
using UnityEngine.EventSystems;
using Shatter.Player;

namespace Shatter.UI
{
    /// <summary>
    /// Script auxiliar para botones táctiles en pantalla móvil.
    /// Maneja eventos de presionado y liberación (Pointer Down / Pointer Up)
    /// y los comunica al PlayerController2D.
    /// </summary>
    public class MobileActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum TipoAccion
        {
            Saltar,
            Agacharse,
            Dash,
            Interactuar,
            MoverIzquierda,
            MoverDerecha,
            Pausa,
            AbrirInventario,
            ReiniciarNivel
        }

        [Header("Configuración de Acción")]
        [Tooltip("La acción que realiza este botón de UI")]
        [SerializeField] private TipoAccion accion;

        private PlayerController2D playerController;

        private void Start()
        {
            EncontrarJugador();
        }

        private void EncontrarJugador()
        {
            playerController = FindFirstObjectByType<PlayerController2D>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (playerController == null)
            {
                EncontrarJugador();
            }

            if (playerController != null)
            {
                switch (accion)
                {
                    case TipoAccion.Saltar:
                        playerController.IniciarSaltoVirtual();
                        break;
                    case TipoAccion.Agacharse:
                        playerController.IniciarAbajoVirtual();
                        break;
                    case TipoAccion.Dash:
                        playerController.IniciarDashVirtual();
                        break;
                    case TipoAccion.Interactuar:
                        var interactionSystem = FindFirstObjectByType<InteractionSystem>();
                        if (interactionSystem != null)
                        {
                            interactionSystem.TriggerVirtualInteraction();
                        }
                        break;
                    case TipoAccion.MoverIzquierda:
                        playerController.IniciarMoverIzquierdaVirtual();
                        break;
                    case TipoAccion.MoverDerecha:
                        playerController.IniciarMoverDerechaVirtual();
                        break;
                    case TipoAccion.Pausa:
                        var pauseMenu = FindFirstObjectByType<PauseMenu>();
                        if (pauseMenu != null)
                        {
                            pauseMenu.AlternarPausa();
                        }
                        break;
                    case TipoAccion.AbrirInventario:
                        var pauseMenuInv = FindFirstObjectByType<PauseMenu>();
                        if (pauseMenuInv != null)
                        {
                            pauseMenuInv.AbrirInventarioDesdeMobile();
                        }
                        break;
                    case TipoAccion.ReiniciarNivel:
                        var pauseMenuReset = FindFirstObjectByType<PauseMenu>();
                        if (pauseMenuReset != null)
                        {
                            pauseMenuReset.ReiniciarNivel();
                        }
                        break;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (playerController != null)
            {
                switch (accion)
                {
                    case TipoAccion.Saltar:
                        playerController.DetenerSaltoVirtual();
                        break;
                    case TipoAccion.Agacharse:
                        playerController.DetenerAbajoVirtual();
                        break;
                    case TipoAccion.Dash:
                        // El dash es un impulso instantáneo, no necesita detenerse
                        break;
                    case TipoAccion.MoverIzquierda:
                        playerController.DetenerMoverIzquierdaVirtual();
                        break;
                    case TipoAccion.MoverDerecha:
                        playerController.DetenerMoverDerechaVirtual();
                        break;
                }
            }
        }
    }
}
