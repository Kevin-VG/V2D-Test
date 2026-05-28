#if SHATTER_GPGS
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif
using System;
using System.IO;
using System.Text;
using UnityEngine;
using Shatter.Core;

namespace Shatter.Systems
{
    /// <summary>
    /// Administrador de guardado en la nube usando Google Play Games (Saved Games API).
    /// Habilitado mediante el símbolo de compilación 'SHATTER_GPGS'.
    /// </summary>
    public class PlayGamesSaveManager : MonoBehaviour
    {
        public static PlayGamesSaveManager Instance { get; private set; }

        [Header("Configuración de Guardado")]
        [SerializeField] private string nombreArchivoNube = "inward_cloud_save";

        private bool autenticado = false;

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

        private void Start()
        {
            InicializarPlayGames();
        }

        public void InicializarPlayGames()
        {
#if SHATTER_GPGS
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames() // Habilita el guardado en la nube
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            IniciarSesion();
#else
            Debug.LogWarning("[GPGS Nube] El plugin de Google Play Games no está activo en este compilado. Añade el símbolo 'SHATTER_GPGS' en Player Settings una vez instalado el plugin.");
#endif
        }

        public void IniciarSesion()
        {
#if SHATTER_GPGS
            Social.localUser.Authenticate((bool éxito) =>
            {
                autenticado = éxito;
                if (éxito)
                {
                    Debug.Log("[GPGS Nube] Autenticación de Google Play exitosa.");
                    CargarDesdeNube(); // Sincroniza al iniciar sesión
                }
                else
                {
                    Debug.LogWarning("[GPGS Nube] Falló la autenticación de Google Play.");
                }
            });
#endif
        }

        // ------ ESCRIBIR EN LA NUBE ------
        public void GuardarEnNube()
        {
            if (!autenticado)
            {
                Debug.Log("[GPGS Nube] No se puede guardar en la nube: Usuario no autenticado.");
                return;
            }

#if SHATTER_GPGS
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(
                nombreArchivoNube,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnSavedGameOpenedForWrite
            );
#endif
        }

#if SHATTER_GPGS
        private void OnSavedGameOpenedForWrite(SavedGameRequestStatus estado, ISavedGameMetadata metadatos)
        {
            if (estado == SavedGameRequestStatus.Success)
            {
                string rutaLocal = Path.Combine(Application.persistentDataPath, "progreso_jugador.json");
                if (!File.Exists(rutaLocal))
                {
                    Debug.LogWarning("[GPGS Nube] No hay archivo local de guardado para subir.");
                    return;
                }

                string jsonDatos = File.ReadAllText(rutaLocal);
                byte[] bytesAEnviar = Encoding.UTF8.GetBytes(jsonDatos);

                SavedGameMetadataUpdate actualización = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription($"Guardado el {DateTime.Now} - Nivel: {GameManager.Instance.NivelActual}")
                    .Build();

                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.CommitUpdate(metadatos, actualización, bytesAEnviar, OnSavedGameCommitted);
            }
            else
            {
                Debug.LogError($"[GPGS Nube] Error al abrir archivo en la nube para escribir: {estado}");
            }
        }

        private void OnSavedGameCommitted(SavedGameRequestStatus estado, ISavedGameMetadata metadatos)
        {
            if (estado == SavedGameRequestStatus.Success)
            {
                Debug.Log("[GPGS Nube] Progreso respaldado con éxito en Google Play Cloud.");
            }
            else
            {
                Debug.LogError($"[GPGS Nube] Fallo al subir progreso a la nube: {estado}");
            }
        }
#endif

        // ------ LEER DE LA NUBE ------
        public void CargarDesdeNube()
        {
            if (!autenticado)
            {
                Debug.Log("[GPGS Nube] No se puede cargar de la nube: Usuario no autenticado.");
                return;
            }

#if SHATTER_GPGS
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(
                nombreArchivoNube,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnSavedGameOpenedForRead
            );
#endif
        }

#if SHATTER_GPGS
        private void OnSavedGameOpenedForRead(SavedGameRequestStatus estado, ISavedGameMetadata metadatos)
        {
            if (estado == SavedGameRequestStatus.Success)
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.ReadBinaryData(metadatos, OnSavedGameDataRead);
            }
            else
            {
                Debug.LogError($"[GPGS Nube] Error al abrir archivo en la nube para leer: {estado}");
            }
        }

        private void OnSavedGameDataRead(SavedGameRequestStatus estado, byte[] datos)
        {
            if (estado == SavedGameRequestStatus.Success)
            {
                string jsonDatos = Encoding.UTF8.GetString(datos);
                if (string.IsNullOrEmpty(jsonDatos))
                {
                    Debug.Log("[GPGS Nube] El archivo en la nube está vacío (primer guardado).");
                    return;
                }

                // Sobrescribimos el archivo local con el de la nube
                string rutaLocal = Path.Combine(Application.persistentDataPath, "progreso_jugador.json");
                File.WriteAllText(rutaLocal, jsonDatos);

                // Notificar al GameManager para que actualice sus variables en memoria
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.CargarProgreso();
                    Debug.Log("[GPGS Nube] Progreso de la nube sincronizado localmente con éxito.");
                }
            }
            else
            {
                Debug.LogError($"[GPGS Nube] Error al leer datos del archivo en la nube: {estado}");
            }
        }
#endif
    }
}
