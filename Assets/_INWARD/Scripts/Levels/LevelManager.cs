using UnityEngine;
using Shatter.Core;

namespace Shatter.Levels
{
    /// <summary>
    /// Clase base para managers de nivel. Define spawn, goal, conteo.
    /// </summary>
    public abstract class LevelManager : MonoBehaviour
    {
        [SerializeField] protected Transform puntoSpawn;
        [SerializeField] protected Transform meta;
        [SerializeField] protected int totalDestellosEnNivel;

        public Vector3 PosicionSpawn => puntoSpawn != null ? puntoSpawn.position : transform.position;

        public virtual void IniciarNivel()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.EstablecerEstado(GameManager.EstadoJuego.Jugando);
        }

        public virtual void CompletarNivel()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CompletarNivel();
            Debug.Log($"[LevelManager] Nivel completado: {GetType().Name}");
        }
    }
}
