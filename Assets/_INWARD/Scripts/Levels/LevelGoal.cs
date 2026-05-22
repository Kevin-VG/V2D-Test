using UnityEngine;

namespace Shatter.Levels
{
    /// <summary>
    /// Trigger de fin de nivel.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LevelGoal : MonoBehaviour
    {
        [SerializeField] private LevelManager gestorNivel;

        private void Awake()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
            if (gestorNivel == null) gestorNivel = FindObjectOfType<LevelManager>();
        }

        private void OnTriggerEnter2D(Collider2D otro)
        {
            if (!otro.CompareTag("Player")) return;
            if (gestorNivel != null) gestorNivel.CompletarNivel();
        }
    }
}
