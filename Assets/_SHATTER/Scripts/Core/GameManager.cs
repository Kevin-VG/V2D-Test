using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shatter.Core
{
    /// <summary>
    /// Singleton central del juego. Progreso, destellos, estado, checkpoints.
    /// Requisito UPN: patron Singleton.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum EstadoJuego { Jugando, Pausado, FinDeJuego, Cinematica }

        [Header("Progreso")]
        [SerializeField] private int destellosDeLucidez;
        [SerializeField] private int nivelActual = 1;
        [SerializeField] private int muertes;

        [Header("Estado")]
        [SerializeField] private EstadoJuego estado = EstadoJuego.Jugando;

        private Vector3 ultimoCheckpoint;
        private bool tieneCheckpoint;

        public int DestellosDeLucidez => destellosDeLucidez;
        public int NivelActual => nivelActual;
        public int Muertes => muertes;
        public EstadoJuego Estado => estado;
        public Vector3 UltimoCheckpoint => ultimoCheckpoint;
        public bool TieneCheckpoint => tieneCheckpoint;

        public event Action<int> AlCambiarDestellos;
        public event Action AlMorirJugador;
        public event Action<int> AlCompletarNivel;
        public event Action<EstadoJuego> AlCambiarEstado;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ------ DESTELLOS ------
        public void AgregarDestellos(int cantidad)
        {
            if (cantidad == 0) return;
            destellosDeLucidez = Mathf.Max(0, destellosDeLucidez + cantidad);
            AlCambiarDestellos?.Invoke(destellosDeLucidez);
        }

        public void ReiniciarDestellos()
        {
            destellosDeLucidez = 0;
            AlCambiarDestellos?.Invoke(destellosDeLucidez);
        }

        // ------ CHECKPOINTS ------
        public void EstablecerCheckpoint(Vector3 posicionMundo)
        {
            ultimoCheckpoint = posicionMundo;
            tieneCheckpoint = true;
        }

        public Vector3 ObtenerPosicionRespawn(Vector3 alternativa)
        {
            return tieneCheckpoint ? ultimoCheckpoint : alternativa;
        }

        // ------ MUERTE / RESPAWN ------
        public void NotificarMuerteJugador()
        {
            muertes++;
            AlMorirJugador?.Invoke();
        }

        // ------ ESTADO ------
        public void EstablecerEstado(EstadoJuego nuevoEstado)
        {
            if (estado == nuevoEstado) return;
            estado = nuevoEstado;
            Time.timeScale = estado == EstadoJuego.Pausado ? 0f : 1f;
            AlCambiarEstado?.Invoke(estado);
        }

        public void AlternarPausa()
        {
            EstablecerEstado(estado == EstadoJuego.Pausado ? EstadoJuego.Jugando : EstadoJuego.Pausado);
        }

        // ------ NIVEL ------
        public void CompletarNivel()
        {
            AlCompletarNivel?.Invoke(nivelActual);
        }

        public void CargarNivel(int indiceNivel)
        {
            nivelActual = indiceNivel;
            tieneCheckpoint = false;
            SceneManager.LoadScene("SampleScene"); // por ahora todo el prototipo va en SampleScene
        }

        public void ReiniciarEscenaActual()
        {
            tieneCheckpoint = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
