using UnityEngine;
using Shatter.Core;
using Shatter.Player;
using Shatter.Systems;
using Shatter.CameraSystem;
using Shatter.AI;
using Shatter.Levels;
using Shatter.UI;
using Shatter.ScriptableObjects;

namespace Shatter.Utils
{
    /// <summary>
    /// Ensambla la escena del prototipo completo al dar Play.
    /// Adjuntar a un GameObject vacio "Bootstrapper" en SampleScene.
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("Que crear")]
        [SerializeField] private bool crearManagers = true;
        [SerializeField] private bool crearNivel = true;
        [SerializeField] private bool crearJugador = true;
        [SerializeField] private bool crearCamara = true;
        [SerializeField] private bool crearParallax = true;
        [SerializeField] private bool crearEnemigos = true;
        [SerializeField] private bool crearColeccionables = true;
        [SerializeField] private bool crearPowerUps = true;
        [SerializeField] private bool crearCheckpoint = true;
        [SerializeField] private bool crearPeligro = true;
        [SerializeField] private bool crearHUD = true;
        [SerializeField] private bool crearIdentidadesEjemplo = true;

        [Header("Layers (deben existir en ProjectSettings -> Tags and Layers)")]
        [SerializeField] private string capaSuelo = "Ground";
        [SerializeField] private string capaPared = "Wall";
        [SerializeField] private string capaUnaDireccion = "OneWay";
        [SerializeField] private string capaEnemigo = "Enemy";
        [SerializeField] private string capaInteractuable = "Interactable";

        private GameObject jugador;
        private Level01Manager nivel;
        private PlaceholderTileGenerator2D generadorTiles;

        private void Awake()
        {
            AsegurarEventSystem();
            if (crearManagers) CrearManagers();
            if (crearNivel) CrearNivel();
            if (crearJugador) CrearJugador();
            if (crearCamara) ConfigurarCamara();
            if (crearParallax) CrearParallax();
            if (crearEnemigos) CrearEnemigos();
            if (crearColeccionables) CrearColeccionables();
            if (crearPowerUps) CrearPowerUps();
            if (crearCheckpoint) CrearCheckpoint();
            if (crearPeligro) CrearPeligro();
            if (crearHUD) CrearHUD();
        }

        // --- MANAGERS ---
        private void CrearManagers()
        {
            if (GameManager.Instance == null)
            {
                var gm = new GameObject("GameManager");
                gm.AddComponent<GameManager>();
            }
            if (ObjectPool.Instance == null)
            {
                var op = new GameObject("ObjectPool");
                op.AddComponent<ObjectPool>();
            }
            if (AudioManager.Instance == null)
            {
                var am = new GameObject("AudioManager");
                am.AddComponent<AudioManager>();
            }
        }

        // --- NIVEL ---
        private void CrearNivel()
        {
            var nivelGo = new GameObject("Level01");
            nivel = nivelGo.AddComponent<Level01Manager>();

            var raizTiles = new GameObject("RaizTiles");
            raizTiles.transform.SetParent(nivelGo.transform);
            generadorTiles = raizTiles.AddComponent<PlaceholderTileGenerator2D>();
            generadorTiles.Generar();
        }

        // --- JUGADOR ---
        private void CrearJugador()
        {
            jugador = new GameObject("Player");
            jugador.tag = "Player";
            jugador.transform.position = new Vector3(-10f, 2f, 0f);

            // Visual
            var visual = PlaceholderSpriteGenerator2D.CrearPlaceholderJugador();
            visual.transform.SetParent(jugador.transform, false);

            // Fisicas
            var rb = jugador.AddComponent<Rigidbody2D>();
            rb.gravityScale = 5f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var box = jugador.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.7f, 1.2f);
            box.offset = new Vector2(0f, 0f);

            // Controlador + subsistemas
            var controlador = jugador.AddComponent<PlayerController2D>();
            EstablecerCapasControladorJugador(controlador);

            jugador.AddComponent<PlayerHealth>();
            jugador.AddComponent<FocusSystem>();
            jugador.AddComponent<EmotionalWeight>();
            jugador.AddComponent<PlayerAnimatorBridge>();
            jugador.AddComponent<InteractionSystem>();
            jugador.AddComponent<PowerUpManager>();
            jugador.AddComponent<DebuffManager>();
            var gestorIdentidad = jugador.AddComponent<IdentityManager>();

            if (crearIdentidadesEjemplo) CrearFragmentosIdentidadEjemplo(gestorIdentidad);
        }

        private void EstablecerCapasControladorJugador(PlayerController2D controlador)
        {
            int capaIdSuelo = LayerMask.NameToLayer(capaSuelo);
            int capaIdPared = LayerMask.NameToLayer(capaPared);
            int capaIdOneWay = LayerMask.NameToLayer(capaUnaDireccion);

            if (capaIdSuelo < 0 || capaIdPared < 0 || capaIdOneWay < 0)
            {
                Debug.LogWarning("[SceneBootstrapper] LAYERS NO CONFIGURADAS. " +
                    "Crea las layers Ground(6), Wall(7), OneWay(8) en Edit > Project Settings > Tags and Layers. " +
                    "Usando layers 6/7/8 como fallback.");
                if (capaIdSuelo < 0) capaIdSuelo = 6;
                if (capaIdPared < 0) capaIdPared = 7;
                if (capaIdOneWay < 0) capaIdOneWay = 8;
            }

            int suelo = 1 << capaIdSuelo;
            int pared = 1 << capaIdPared;
            int unaDireccion = 1 << capaIdOneWay;

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(PlayerController2D).GetField("capaSuelo", flags)?.SetValue(controlador, (LayerMask)suelo);
            typeof(PlayerController2D).GetField("capaPared", flags)?.SetValue(controlador, (LayerMask)pared);
            typeof(PlayerController2D).GetField("capaUnaDireccion", flags)?.SetValue(controlador, (LayerMask)unaDireccion);
        }

        // --- CAMARA ---
        private void ConfigurarCamara()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                cam.orthographic = true;
            }
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.backgroundColor = new Color(0.05f, 0.06f, 0.1f);

            var seguimiento = cam.GetComponent<CameraFollow2D>();
            if (seguimiento == null) seguimiento = cam.gameObject.AddComponent<CameraFollow2D>();
            if (jugador != null) seguimiento.EstablecerObjetivo(jugador.transform);
        }

        // --- PARALLAX ---
        private void CrearParallax()
        {
            var raiz = new GameObject("RaizParallax");
            raiz.transform.position = new Vector3(0f, 0f, 10f);

            AgregarCapaParallax(raiz, "Cielo",   new Vector3(0, 2, 20), new Color(0.12f, 0.18f, 0.35f), new Color(0.3f, 0.35f, 0.55f), 0.02f, -50);
            AgregarCapaParallax(raiz, "Lejos",   new Vector3(0, 1, 15), new Color(0.15f, 0.12f, 0.25f), new Color(0.35f, 0.25f, 0.45f), 0.15f, -40);
            AgregarCapaParallax(raiz, "Medio",   new Vector3(0, 0, 10), new Color(0.18f, 0.15f, 0.22f), new Color(0.4f, 0.3f, 0.4f),    0.35f, -30);
            AgregarCapaParallax(raiz, "Cerca",   new Vector3(0,-1, 5),  new Color(0.12f, 0.1f, 0.18f),  new Color(0.25f, 0.2f, 0.3f),   0.65f, -20);
        }

        private void AgregarCapaParallax(GameObject raiz, string nombre, Vector3 posicion, Color inferior, Color superior, float factor, int ordenSorting)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(raiz.transform);
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.GradienteVertical(256, 128, superior, inferior, nombre);
            sr.sortingOrder = ordenSorting;
            sr.transform.localScale = new Vector3(5f, 3f, 1f);

            var capa = go.AddComponent<ParallaxLayer>();
            capa.factorParallaxX = factor;
            capa.factorParallaxY = factor * 0.2f;
        }

        // --- ENEMIGOS ---
        private void CrearEnemigos()
        {
            var visionTunel = ScriptableObject.CreateInstance<DebuffSO>();
            visionTunel.nombreDebuff = "Vision Tunel";
            visionTunel.tipo = TipoDebuff.VisionTunel;
            visionTunel.duracion = 4f;
            visionTunel.multiplicadorVelocidad = 0.6f;

            var invertidos = ScriptableObject.CreateInstance<DebuffSO>();
            invertidos.nombreDebuff = "Controles Invertidos";
            invertidos.tipo = TipoDebuff.ControlesInvertidos;
            invertidos.duracion = 3f;

            var walker = new GameObject("Enemigo_Walker");
            walker.transform.position = new Vector3(2f, -2f, 0f);
            var walkerSr = walker.AddComponent<SpriteRenderer>();
            walkerSr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(28, 28, new Color(0.85f, 0.25f, 0.25f), "Walker");
            walkerSr.sortingOrder = 8;
            var walkerRb = walker.AddComponent<Rigidbody2D>();
            walkerRb.gravityScale = 4f;
            walkerRb.freezeRotation = true;
            var walkerCol = walker.AddComponent<BoxCollider2D>();
            walkerCol.size = new Vector2(0.8f, 0.8f);
            var we = walker.AddComponent<WalkerEnemy>();
            int idS = LayerMask.NameToLayer(capaSuelo); if (idS < 0) idS = 6;
            int idP = LayerMask.NameToLayer(capaPared); if (idP < 0) idP = 7;
            int mascaraSuelo = (1 << idS) | (1 << idP);
            var campoSuelo = typeof(WalkerEnemy).GetField("capaSuelo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (campoSuelo != null) campoSuelo.SetValue(we, (LayerMask)mascaraSuelo);
            var campoDebuff = typeof(EnemyBase).GetField("debuffAlContacto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (campoDebuff != null) campoDebuff.SetValue(we, visionTunel);

            var flyer = new GameObject("Enemigo_Flyer");
            flyer.transform.position = new Vector3(16f, 2f, 0f);
            var flySr = flyer.AddComponent<SpriteRenderer>();
            flySr.sprite = PlaceholderSpriteGenerator2D.Circulo(28, new Color(0.6f, 0.3f, 0.85f), "Flyer");
            flySr.sortingOrder = 8;
            var flyRb = flyer.AddComponent<Rigidbody2D>();
            flyRb.gravityScale = 0f;
            flyRb.freezeRotation = true;
            var flyCol = flyer.AddComponent<CircleCollider2D>();
            flyCol.radius = 0.4f;
            var fe = flyer.AddComponent<FlyerEnemy>();
            if (campoDebuff != null) campoDebuff.SetValue(fe, invertidos);
        }

        // --- COLECCIONABLES ---
        private void CrearColeccionables()
        {
            Vector2[] posiciones = {
                new Vector2(-5f, -1f),
                new Vector2(0f, 0f),
                new Vector2(7f, 1.5f),
                new Vector2(15f, 2.5f),
                new Vector2(28f, 1f),
            };
            foreach (var p in posiciones) GenerarDestello(p);
        }

        private GameObject GenerarDestello(Vector2 posicion)
        {
            var go = new GameObject("Destello");
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.Circulo(20, new Color(1f, 0.95f, 0.4f), "CirculoDestello");
            sr.sortingOrder = 6;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;
            var c = go.AddComponent<Collectible>();
            c.Configurar(TipoColeccionable.Destello, 1);
            return go;
        }

        // --- POWER UPS ---
        private void CrearPowerUps()
        {
            var latido = ScriptableObject.CreateInstance<PowerUpSO>();
            latido.nombrePowerUp = "Latido Calmado";
            latido.tipo = TipoPowerUp.LatidoCalmado;
            latido.duracion = 10f;
            latido.radio = 6f;
            latido.magnitud = 0.5f;

            var escudo = ScriptableObject.CreateInstance<PowerUpSO>();
            escudo.nombrePowerUp = "Escudo de Respiracion";
            escudo.tipo = TipoPowerUp.EscudoRespiracion;
            escudo.duracion = 999f;

            GenerarPowerUp(new Vector2(8f, -1.5f), latido, new Color(0.3f, 0.85f, 0.9f));
            GenerarPowerUp(new Vector2(22f, 1f), escudo, new Color(0.4f, 1f, 0.5f));
        }

        private GameObject GenerarPowerUp(Vector2 posicion, PowerUpSO so, Color color)
        {
            var go = new GameObject("PowerUp_" + so.nombrePowerUp);
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.Circulo(28, color, "CirculoPowerUp");
            sr.sortingOrder = 7;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.45f;
            var c = go.AddComponent<Collectible>();
            c.Configurar(TipoColeccionable.PowerUp, 0, so);
            return go;
        }

        // --- CHECKPOINT ---
        private void CrearCheckpoint()
        {
            var go = new GameObject("Checkpoint");
            go.transform.position = new Vector3(13f, -1f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(14, 40, new Color(0.8f, 0.8f, 0.8f), "BanderaCheckpoint");
            sr.sortingOrder = 4;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.6f, 1.5f);
            go.AddComponent<Checkpoint>();
        }

        // --- PELIGRO ---
        private void CrearPeligro()
        {
            var go = new GameObject("Peligro_Pozo");
            go.transform.position = new Vector3(11f, -6f, 0f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(96, 16, new Color(0.7f, 0.1f, 0.1f), "PozoMortal");
            sr.sortingOrder = 4;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(3f, 0.5f);
            var h = go.AddComponent<Hazard>();
            var campo = typeof(Hazard).GetField("matarInstantaneamente", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (campo != null) campo.SetValue(h, true);
        }

        // --- HUD ---
        private void CrearHUD()
        {
            var canvasGo = new GameObject("HUD_Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            canvasGo.AddComponent<HUDManager>();
            canvasGo.AddComponent<PauseMenu>();
        }

        // --- IDENTIDADES ---
        private void CrearFragmentosIdentidadEjemplo(IdentityManager gestor)
        {
            var aura = ScriptableObject.CreateInstance<IdentityFragmentSO>();
            aura.nombreFragmento = "Aura de la Pua";
            aura.descripcion = "Brillo calido. +5% velocidad.";
            aura.colorTinte = new Color(1f, 0.7f, 0.4f);
            aura.multiplicadorVelocidad = 1.05f;

            var tinta = ScriptableObject.CreateInstance<IdentityFragmentSO>();
            tinta.nombreFragmento = "Tinta de Estrellas";
            tinta.descripcion = "Rastro etereo. +1 salto extra.";
            tinta.colorTinte = new Color(0.6f, 0.7f, 1f);
            tinta.saltosExtra = 1;

            var acero = ScriptableObject.CreateInstance<IdentityFragmentSO>();
            acero.nombreFragmento = "Acero Mental";
            acero.descripcion = "Resistencia. -25% dano recibido.";
            acero.colorTinte = new Color(0.75f, 0.75f, 0.8f);
            acero.reduccionDano = 0.25f;

            gestor.Descubrir(aura);
            gestor.Descubrir(tinta);
            gestor.Descubrir(acero);
        }

        // --- EVENT SYSTEM ---
        private void AsegurarEventSystem()
        {
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
    }
}
