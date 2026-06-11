using UnityEngine;

namespace Shatter.Player
{
    public interface IInteractable
    {
        bool PuedeInteractuar { get; }
        string TextoInteraccion { get; }
        void AlInteractuar(GameObject interactor);
    }

    /// <summary>
    /// Detecta IInteractable mas cercano en un radio y ejecuta el interact con E.
    /// </summary>
    public class InteractionSystem : MonoBehaviour
    {
        [SerializeField] private float radioDeteccion = 1.6f;
        [SerializeField] private LayerMask capaInteractuable = ~0;
        // La tecla ahora viene del InputManager

        private readonly Collider2D[] resultados = new Collider2D[8];
        private IInteractable actual;

        public IInteractable Actual => actual;
        public string TextoActual => actual != null ? actual.TextoInteraccion : null;

        private bool virtualInteractPressed;

        public void TriggerVirtualInteraction()
        {
            virtualInteractPressed = true;
        }

        private void Update()
        {
            EncontrarMasCercano();
            if (actual != null && actual.PuedeInteractuar)
            {
                KeyCode tecla = Shatter.Core.InputManager.Instance != null ? Shatter.Core.InputManager.Instance.Interactuar : KeyCode.E;
                if (Input.GetKeyDown(tecla) || virtualInteractPressed)
                {
                    actual.AlInteractuar(gameObject);
                }
            }
            
            // Consumir el input virtual
            virtualInteractPressed = false;
        }

        private void EncontrarMasCercano()
        {
            int cantidad = Physics2D.OverlapCircleNonAlloc(transform.position, radioDeteccion, resultados, capaInteractuable);
            actual = null;
            float mejorDistancia = float.MaxValue;
            for (int i = 0; i < cantidad; i++)
            {
                var col = resultados[i];
                if (col == null) continue;
                var it = col.GetComponentInParent<IInteractable>();
                if (it == null || !it.PuedeInteractuar) continue;
                float d = ((Vector2)col.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (d < mejorDistancia) { mejorDistancia = d; actual = it; }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        }
    }
}
