using UnityEngine;

namespace Shatter.Player
{
    /// <summary>
    /// Puente entre el PlayerController2D y un Animator opcional.
    /// Si no hay Animator, solo voltea el sprite segun facing.
    /// </summary>
    public class PlayerAnimatorBridge : MonoBehaviour
    {
        [SerializeField] private PlayerController2D controlador;
        [SerializeField] private Animator animador;
        [SerializeField] private SpriteRenderer renderizadorSprite;

        private static readonly int HashVelocidad = Animator.StringToHash("velocidad");
        private static readonly int HashVertical = Animator.StringToHash("vy");
        private static readonly int HashEnSuelo = Animator.StringToHash("enSuelo");
        private static readonly int HashDash = Animator.StringToHash("dash");
        private static readonly int HashDeslizPared = Animator.StringToHash("deslizPared");
        private static readonly int HashAgachado = Animator.StringToHash("estaAgachado");

        private void Reset()
        {
            controlador = GetComponent<PlayerController2D>();
            animador = GetComponentInChildren<Animator>();
            renderizadorSprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void Awake()
        {
            if (controlador == null) controlador = GetComponent<PlayerController2D>();
            if (animador == null) animador = GetComponentInChildren<Animator>();
            if (renderizadorSprite == null) renderizadorSprite = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (controlador == null) return;

            if (renderizadorSprite != null)
            {
                // Volteamos el sprite horizontalmente según la dirección del movimiento
                renderizadorSprite.flipX = controlador.Direccion < 0;
            }

            if (animador == null || !animador.isActiveAndEnabled || animador.runtimeAnimatorController == null) return;

            // Enviamos todos los parámetros físicos del PlayerController2D al Animator de Unity
            animador.SetFloat(HashVelocidad, Mathf.Abs(controlador.VelocidadX));
            animador.SetFloat(HashVertical, controlador.VelocidadY);
            animador.SetBool(HashEnSuelo, controlador.EstaEnSuelo);
            animador.SetBool(HashDash, controlador.EstaHaciendoDash);
            animador.SetBool(HashDeslizPared, controlador.EstaDeslizandoPared);
            animador.SetBool(HashAgachado, controlador.EstaAgachado); // Habilitamos parámetro para animaciones reales de Crouch-Idle y Crouch-Walk
        }
    }
}

