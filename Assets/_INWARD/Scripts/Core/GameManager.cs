using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shatter.Core
{
    /// <summary>
    /// Clase contenedora de los datos que queremos persistir.
    /// </summary>
    [System.Serializable]
    public class DatosGuardados
    {
        public int destellosDeLucidez;
        public int nivelActual;
        public int muertes;
        public bool tieneCheckpoint;
        public float checkpointX;
        public float checkpointY;
        public float checkpointZ;
    }

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
            CargarProgreso(); // Cargar progreso al iniciar el juego
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
            GuardarProgreso(); // Auto-guardado al activar un Checkpoint
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
            GuardarProgreso(); // Auto-guardado al completar el nivel
        }

        // ------ PERSISTENCIA (GUARDADO / CARGA LOCAL MÓVIL/PC) ------
        private string ObtenerRutaGuardado()
        {
            return Path.Combine(Application.persistentDataPath, "progreso_jugador.json");
        }

        public void GuardarProgreso()
        {
            try
            {
                DatosGuardados datos = new DatosGuardados
                {
                    destellosDeLucidez = this.destellosDeLucidez,
                    nivelActual = this.nivelActual,
                    muertes = this.muertes,
                    tieneCheckpoint = this.tieneCheckpoint,
                    checkpointX = this.ultimoCheckpoint.x,
                    checkpointY = this.ultimoCheckpoint.y,
                    checkpointZ = this.ultimoCheckpoint.z
                };

                string json = JsonUtility.ToJson(datos, true);
                File.WriteAllText(ObtenerRutaGuardado(), json);
                Debug.Log($"[Inward Save System] Progreso guardado localmente en: {ObtenerRutaGuardado()}");

                // Auto-sincronizar con la nube (Google Play Games Services)
                if (Shatter.Systems.PlayGamesSaveManager.Instance != null)
                {
                    Shatter.Systems.PlayGamesSaveManager.Instance.GuardarEnNube();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Inward Save System] Error al guardar progreso: {e.Message}");
            }
        }

        public void CargarProgreso()
        {
            try
            {
                string ruta = ObtenerRutaGuardado();
                if (File.Exists(ruta))
                {
                    string json = File.ReadAllText(ruta);
                    DatosGuardados datos = JsonUtility.FromJson<DatosGuardados>(json);

                    this.destellosDeLucidez = datos.destellosDeLucidez;
                    this.nivelActual = datos.nivelActual;
                    this.muertes = datos.muertes;
                    this.tieneCheckpoint = datos.tieneCheckpoint;
                    this.ultimoCheckpoint = new Vector3(datos.checkpointX, datos.checkpointY, datos.checkpointZ);

                    // Disparamos evento para actualizar HUD
                    AlCambiarDestellos?.Invoke(destellosDeLucidez);
                    Debug.Log($"[Inward Save System] Progreso cargado exitosamente desde: {ruta}");
                }
                else
                {
                    Debug.Log("[Inward Save System] No se encontró ningún archivo de guardado previo.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[Inward Save System] Error al cargar progreso: {e.Message}");
            }
        }

        public void BorrarProgreso()
        {
            try
            {
                string ruta = ObtenerRutaGuardado();
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                    Debug.Log("[Inward Save System] Archivo de progreso eliminado.");
                }
                
                // Reiniciar a valores por defecto
                this.destellosDeLucidez = 0;
                this.nivelActual = 1;
                this.muertes = 0;
                this.tieneCheckpoint = false;
                this.ultimoCheckpoint = Vector3.zero;

                AlCambiarDestellos?.Invoke(destellosDeLucidez);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Inward Save System] Error al borrar progreso: {e.Message}");
            }
        }

        public void CargarNivel(int indiceNivel)
        {
            nivelActual = indiceNivel;
            tieneCheckpoint = false;
            SceneManager.LoadScene("Nivel_01"); // Cambiado a Nivel_01
        }

        public void ReiniciarEscenaActual()
        {
            tieneCheckpoint = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
