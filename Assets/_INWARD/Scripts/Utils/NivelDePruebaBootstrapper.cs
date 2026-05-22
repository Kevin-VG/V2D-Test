using UnityEngine;
using UnityEngine.UI;
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
    /// Genera un nivel de prueba completo con 17 zonas que testean TODAS las mecanicas.
    /// Recorrido lineal de izquierda a derecha con senalizacion por zona.
    /// Adjuntar a un GameObject vacio "TestBootstrapper" en SampleScene.
    /// </summary>
    public class NivelDePruebaBootstrapper : MonoBehaviour
    {
        [Header("Sistemas")]
        [SerializeField] private bool crearManagers = true;
        [SerializeField] private bool crearJugador = true;
        [SerializeField] private bool crearCamara = true;
        [SerializeField] private bool crearParallax = true;
        [SerializeField] private bool crearHUD = true;

        [Header("Zonas (todas activas por defecto)")]
        [SerializeField] private bool zona00_Caminar = true;
        [SerializeField] private bool zona01_Salto = true;
        [SerializeField] private bool zona02_Coyote = true;
        [SerializeField] private bool zona03_DobleSalto = true;
        [SerializeField] private bool zona04_Agacharse = true;
        [SerializeField] private bool zona05_OneWay = true;
        [SerializeField] private bool zona06_WallJump = true;
        [SerializeField] private bool zona07_Dash = true;
        [SerializeField] private bool zona08_Checkpoint = true;
        [SerializeField] private bool zona09_Walker = true;
        [SerializeField] private bool zona10_Flyer = true;
        [SerializeField] private bool zona11_Peligros = true;
        [SerializeField] private bool zona12_PowerUps = true;
        [SerializeField] private bool zona13_Debuffs = true;
        [SerializeField] private bool zona14_Fragmentos = true;
        [SerializeField] private bool zona15_Desafio = true;
        [SerializeField] private bool zona16_Meta = true;

        [Header("Layers")]
        [SerializeField] private string capaSuelo = "Ground";
        [SerializeField] private string capaPared = "Wall";
        [SerializeField] private string capaUnaDireccion = "OneWay";

        // --- Colores ---
        private readonly Color colorSuelo = new Color(0.22f, 0.22f, 0.28f);
        private readonly Color colorPared = new Color(0.35f, 0.2f, 0.35f);
        private readonly Color colorOneWay = new Color(0.8f, 0.7f, 0.3f);
        private readonly Color colorPinchos = new Color(0.7f, 0.15f, 0.15f);
        private readonly Color colorMortal = new Color(0.9f, 0.1f, 0.1f);
        private readonly Color colorSeguro = new Color(0.2f, 0.5f, 0.3f);
        private readonly Color colorMeta = new Color(0.85f, 0.75f, 0.3f);
        private readonly Color colorLetrero = new Color(1f, 0.95f, 0.3f);

        // --- Estado ---
        private GameObject jugador;
        private Transform raizNivel;
        private Sprite spriteTile;
        private float pisoY = -3f; // Y base del piso

        // --- Debuffs reutilizables ---
        private DebuffSO debuffVisionTunel;
        private DebuffSO debuffPasosDePlomo;
        private DebuffSO debuffControlesInvertidos;

        private void Awake()
        {
            // Preparar
            AsegurarEventSystem();
            raizNivel = new GameObject("_RaizNivel").transform;
            spriteTile = PlaceholderSpriteGenerator2D.RectanguloSolido(32, 32, Color.white, "TileBlanco");
            CrearDebuffsCompartidos();

            // Sistemas
            if (crearManagers) CrearManagers();
            if (crearJugador) CrearJugador();
            if (crearCamara) ConfigurarCamara();
            if (crearParallax) CrearParallax();
            if (crearHUD) CrearHUD();

            // Bordes laterales del nivel completo
            GenerarParedVertical(-26f, pisoY - 1, pisoY + 10);
            GenerarParedVertical(156f, pisoY - 1, pisoY + 10);

            // Zonas
            if (zona00_Caminar) CrearZona00_Caminar();
            if (zona01_Salto) CrearZona01_SaltoBasico();
            if (zona02_Coyote) CrearZona02_CoyoteYBuffer();
            if (zona03_DobleSalto) CrearZona03_DobleSalto();
            if (zona04_Agacharse) CrearZona04_Agacharse();
            if (zona05_OneWay) CrearZona05_OneWay();
            if (zona06_WallJump) CrearZona06_WallJump();
            if (zona07_Dash) CrearZona07_Dash();
            if (zona08_Checkpoint) CrearZona08_Checkpoint();
            if (zona09_Walker) CrearZona09_Walker();
            if (zona10_Flyer) CrearZona10_Flyer();
            if (zona11_Peligros) CrearZona11_Peligros();
            if (zona12_PowerUps) CrearZona12_PowerUps();
            if (zona13_Debuffs) CrearZona13_Debuffs();
            if (zona14_Fragmentos) CrearZona14_Fragmentos();
            if (zona15_Desafio) CrearZona15_DesafioCombinado();
            if (zona16_Meta) CrearZona16_Meta();
        }

        // ========================================================
        //  DEBUFFS COMPARTIDOS
        // ========================================================
        private void CrearDebuffsCompartidos()
        {
            debuffVisionTunel = ScriptableObject.CreateInstance<DebuffSO>();
            debuffVisionTunel.nombreDebuff = "Vision Tunel";
            debuffVisionTunel.tipo = TipoDebuff.VisionTunel;
            debuffVisionTunel.duracion = 4f;
            debuffVisionTunel.multiplicadorVelocidad = 0.6f;

            debuffPasosDePlomo = ScriptableObject.CreateInstance<DebuffSO>();
            debuffPasosDePlomo.nombreDebuff = "Pasos de Plomo";
            debuffPasosDePlomo.tipo = TipoDebuff.PasosDePlomo;
            debuffPasosDePlomo.duracion = 5f;
            debuffPasosDePlomo.multiplicadorVelocidad = 0.5f;
            debuffPasosDePlomo.multiplicadorSalto = 0.5f;

            debuffControlesInvertidos = ScriptableObject.CreateInstance<DebuffSO>();
            debuffControlesInvertidos.nombreDebuff = "Controles Invertidos";
            debuffControlesInvertidos.tipo = TipoDebuff.ControlesInvertidos;
            debuffControlesInvertidos.duracion = 3f;
        }

        // ========================================================
        //  SISTEMAS BASE (identicos a SceneBootstrapper)
        // ========================================================
        private void CrearManagers()
        {
            if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
            if (ObjectPool.Instance == null) new GameObject("ObjectPool").AddComponent<ObjectPool>();
            if (AudioManager.Instance == null) new GameObject("AudioManager").AddComponent<AudioManager>();
        }

        private void CrearJugador()
        {
            jugador = new GameObject("Player");
            jugador.tag = "Player";
            jugador.transform.position = new Vector3(-22f, pisoY + 3f, 0f);

            var visual = PlaceholderSpriteGenerator2D.CrearPlaceholderJugador();
            visual.transform.SetParent(jugador.transform, false);

            var rb = jugador.AddComponent<Rigidbody2D>();
            rb.gravityScale = 5f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var box = jugador.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.7f, 1.2f);

            var controlador = jugador.AddComponent<PlayerController2D>();
            EstablecerCapasControlador(controlador);

            jugador.AddComponent<PlayerHealth>();
            jugador.AddComponent<PlayerAnimatorBridge>();
            jugador.AddComponent<InteractionSystem>();
            jugador.AddComponent<PowerUpManager>();
            jugador.AddComponent<DebuffManager>();
            jugador.AddComponent<IdentityManager>();
        }

        private void EstablecerCapasControlador(PlayerController2D controlador)
        {
            int capaIdSuelo = LayerMask.NameToLayer(capaSuelo);
            int capaIdPared = LayerMask.NameToLayer(capaPared);
            int capaIdOneWay = LayerMask.NameToLayer(capaUnaDireccion);

            // Validar que las layers existen; si no, advertir y usar fallbacks seguros
            if (capaIdSuelo < 0 || capaIdPared < 0 || capaIdOneWay < 0)
            {
                Debug.LogWarning("[NivelDePrueba] LAYERS NO CONFIGURADAS. " +
                    "Crea las layers Ground(6), Wall(7), OneWay(8) en Edit > Project Settings > Tags and Layers. " +
                    "Usando layers 6/7/8 como fallback.");
                if (capaIdSuelo < 0) capaIdSuelo = 6;
                if (capaIdPared < 0) capaIdPared = 7;
                if (capaIdOneWay < 0) capaIdOneWay = 8;
            }

            int suelo = 1 << capaIdSuelo;
            int pared = 1 << capaIdPared;
            int oneWay = 1 << capaIdOneWay;

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(PlayerController2D).GetField("capaSuelo", flags)?.SetValue(controlador, (LayerMask)suelo);
            typeof(PlayerController2D).GetField("capaPared", flags)?.SetValue(controlador, (LayerMask)pared);
            typeof(PlayerController2D).GetField("capaUnaDireccion", flags)?.SetValue(controlador, (LayerMask)oneWay);
        }

        private void ConfigurarCamara()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 7f;
            cam.backgroundColor = new Color(0.05f, 0.06f, 0.1f);

            // Posicionar la camara sobre el jugador inmediatamente
            if (jugador != null)
                cam.transform.position = jugador.transform.position + new Vector3(0f, 1.2f, -10f);

            var seguimiento = cam.GetComponent<CameraFollow2D>() ?? cam.gameObject.AddComponent<CameraFollow2D>();
            if (jugador != null) seguimiento.EstablecerObjetivo(jugador.transform);
        }

        private void CrearParallax()
        {
            var raiz = new GameObject("RaizParallax");
            raiz.transform.position = new Vector3(0f, 0f, 10f);
            AgregarCapaParallax(raiz, "Cielo", new Vector3(0, 2, 20), new Color(0.12f, 0.18f, 0.35f), new Color(0.3f, 0.35f, 0.55f), 0.02f, -50);
            AgregarCapaParallax(raiz, "Lejos", new Vector3(0, 1, 15), new Color(0.15f, 0.12f, 0.25f), new Color(0.35f, 0.25f, 0.45f), 0.15f, -40);
            AgregarCapaParallax(raiz, "Medio", new Vector3(0, 0, 10), new Color(0.18f, 0.15f, 0.22f), new Color(0.4f, 0.3f, 0.4f), 0.35f, -30);
            AgregarCapaParallax(raiz, "Cerca", new Vector3(0, -1, 5), new Color(0.12f, 0.1f, 0.18f), new Color(0.25f, 0.2f, 0.3f), 0.65f, -20);
        }

        private void AgregarCapaParallax(GameObject raiz, string nombre, Vector3 pos, Color inf, Color sup, float factor, int orden)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(raiz.transform);
            go.transform.position = pos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.GradienteVertical(256, 128, sup, inf, nombre);
            sr.sortingOrder = orden;
            sr.transform.localScale = new Vector3(8f, 4f, 1f);
            var capa = go.AddComponent<ParallaxLayer>();
            capa.factorParallaxX = factor;
            capa.factorParallaxY = factor * 0.2f;
        }

        private void CrearHUD()
        {
            var canvasGo = new GameObject("HUD_Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            canvasGo.AddComponent<HUDManager>();
            canvasGo.AddComponent<PauseMenu>();
        }

        // ========================================================
        //  ZONA 0: CAMINAR (x: -25 a -12)
        // ========================================================
        private void CrearZona00_Caminar()
        {
            GenerarPisoSeccion(-25, -12, pisoY);
            GenerarLetrero(new Vector2(-22f, pisoY + 4f), "ZONA 0: CAMINAR", colorLetrero);
            GenerarLetrero(new Vector2(-22f, pisoY + 3.2f), "Muevete con A / D", Color.white);

            for (int i = 0; i < 5; i++)
                GenerarDestello(new Vector2(-21f + i * 2f, pisoY + 1.5f));
        }

        // ========================================================
        //  ZONA 1: SALTO BASICO + VARIABLE (x: -12 a -2)
        // ========================================================
        private void CrearZona01_SaltoBasico()
        {
            // Piso con hueco
            GenerarPisoSeccion(-12, -8, pisoY);
            // Hueco de 2 tiles (-8 a -6)
            GenerarPisoSeccion(-6, -2, pisoY);

            // Plataformas a alturas crecientes
            GenerarTile(new Vector2(-5f, pisoY + 1f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(-4f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(-3f, pisoY + 3f), colorSuelo, capaSuelo);

            // Destello alto (solo con salto mantenido)
            GenerarDestello(new Vector2(-7f, pisoY + 4.5f));
            // Destellos normales
            GenerarDestello(new Vector2(-5f, pisoY + 2.5f));
            GenerarDestello(new Vector2(-3f, pisoY + 4.5f));

            GenerarLetrero(new Vector2(-10f, pisoY + 4f), "ZONA 1: SALTO", colorLetrero);
            GenerarLetrero(new Vector2(-10f, pisoY + 3.2f), "Espacio (mantener = mas alto)", Color.white);
        }

        // ========================================================
        //  ZONA 2: COYOTE TIME + JUMP BUFFER (x: -2 a 8)
        // ========================================================
        private void CrearZona02_CoyoteYBuffer()
        {
            // Plataforma que termina abruptamente
            GenerarPisoSeccion(-2, 1, pisoY);
            // Hueco de 3 tiles (1 a 4) — requiere coyote time
            // Plataforma destino
            GenerarPisoSeccion(4, 8, pisoY);

            // Destello en el arco del salto coyote
            GenerarDestello(new Vector2(2.5f, pisoY + 3f));
            GenerarDestello(new Vector2(6f, pisoY + 1.5f));

            GenerarLetrero(new Vector2(0f, pisoY + 4f), "ZONA 2: COYOTE TIME", colorLetrero);
            GenerarLetrero(new Vector2(0f, pisoY + 3.2f), "Salta DESPUES de salir del borde", Color.white);
        }

        // ========================================================
        //  ZONA 3: DOBLE SALTO (x: 8 a 18)
        // ========================================================
        private void CrearZona03_DobleSalto()
        {
            GenerarPisoSeccion(8, 10, pisoY);
            // Hueco grande de 5 tiles (10 a 15) — imposible sin doble salto
            GenerarPisoSeccion(15, 18, pisoY);

            // Plataforma alta que requiere doble salto vertical
            GenerarTile(new Vector2(17f, pisoY + 4f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(18f, pisoY + 4f), colorSuelo, capaSuelo);

            // Destellos en arco
            GenerarDestello(new Vector2(11f, pisoY + 3f));
            GenerarDestello(new Vector2(12.5f, pisoY + 5f));
            GenerarDestello(new Vector2(14f, pisoY + 3f));
            GenerarDestello(new Vector2(17.5f, pisoY + 5.5f));

            GenerarLetrero(new Vector2(9f, pisoY + 4f), "ZONA 3: DOBLE SALTO", colorLetrero);
            GenerarLetrero(new Vector2(9f, pisoY + 3.2f), "Espacio x2 en el aire", Color.white);
        }

        // ========================================================
        //  ZONA 4: AGACHARSE (x: 18 a 26)
        // ========================================================
        private void CrearZona04_Agacharse()
        {
            GenerarPisoSeccion(18, 26, pisoY);

            // Techo bajo (tunel) de x:20 a x:24, a 1.1 tiles sobre el piso
            // Player de pie = 1.2 alto, agachado = 0.6 alto → solo pasa agachado
            for (int x = 20; x <= 24; x++)
                GenerarTile(new Vector2(x, pisoY + 1.1f), colorPared, capaSuelo);

            // Destellos dentro del tunel (a media altura del agachado)
            for (int i = 0; i < 4; i++)
                GenerarDestello(new Vector2(20.5f + i, pisoY + 0.8f));

            GenerarLetrero(new Vector2(19f, pisoY + 4f), "ZONA 4: AGACHARSE", colorLetrero);
            GenerarLetrero(new Vector2(19f, pisoY + 3.2f), "Mantener S para agacharte", Color.white);
        }

        // ========================================================
        //  ZONA 5: ONE-WAY + DROP-THROUGH (x: 26 a 36)
        // ========================================================
        private void CrearZona05_OneWay()
        {
            GenerarPisoSeccion(26, 36, pisoY);

            // 3 niveles de plataformas one-way
            for (int x = 29; x <= 33; x++)
            {
                GenerarOneWay(new Vector2(x, pisoY + 2f));   // Nivel 1
                GenerarOneWay(new Vector2(x, pisoY + 4f));   // Nivel 2
                GenerarOneWay(new Vector2(x, pisoY + 6f));   // Nivel 3
            }

            // Destellos en cada nivel
            GenerarDestello(new Vector2(31f, pisoY + 3f));
            GenerarDestello(new Vector2(31f, pisoY + 5f));
            GenerarDestello(new Vector2(31f, pisoY + 7f));

            GenerarLetrero(new Vector2(27f, pisoY + 8f), "ZONA 5: ONE-WAY", colorLetrero);
            GenerarLetrero(new Vector2(27f, pisoY + 7.2f), "Sube desde abajo. S+Espacio para caer", Color.white);
        }

        // ========================================================
        //  ZONA 6: WALL SLIDE + WALL JUMP (x: 36 a 46)
        // ========================================================
        private void CrearZona06_WallJump()
        {
            // Piso de entrada
            GenerarPisoSeccion(36, 38, pisoY);

            // Pozo vertical: 2.5 tiles de separacion entre paredes, 10 de alto
            float paredIzq = 38f;
            float paredDer = 40.5f;
            for (int y = (int)pisoY - 1; y <= (int)pisoY + 10; y++)
            {
                GenerarTile(new Vector2(paredIzq, y), colorPared, capaPared);
                GenerarTile(new Vector2(paredDer, y), colorPared, capaPared);
            }

            // Piso del pozo (para no caer al vacio)
            GenerarTile(new Vector2(39f, pisoY - 1), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(40f, pisoY - 1), colorSuelo, capaSuelo);

            // Destellos en zigzag ascendente (alternando lados)
            GenerarDestello(new Vector2(38.7f, pisoY + 1.5f));
            GenerarDestello(new Vector2(40f, pisoY + 3f));
            GenerarDestello(new Vector2(38.7f, pisoY + 4.5f));
            GenerarDestello(new Vector2(40f, pisoY + 6f));
            GenerarDestello(new Vector2(39.3f, pisoY + 8f));

            // Plataforma de salida arriba a la derecha
            GenerarPisoSeccion(41, 46, pisoY + 10);
            // Rampa de bajada
            GenerarTile(new Vector2(42f, pisoY + 9f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(43f, pisoY + 8f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(44f, pisoY + 7f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(45f, pisoY + 6f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(46f, pisoY + 5f), colorSuelo, capaSuelo);

            GenerarLetrero(new Vector2(37f, pisoY + 4f), "ZONA 6: WALL JUMP", colorLetrero);
            GenerarLetrero(new Vector2(37f, pisoY + 3.2f), "Contra la pared + Espacio", Color.white);
        }

        // ========================================================
        //  ZONA 7: DASH (x: 46 a 56)
        // ========================================================
        private void CrearZona07_Dash()
        {
            // Bajada al nivel del piso desde la Zona 6
            GenerarPisoSeccion(46, 48, pisoY + 4);
            GenerarTile(new Vector2(47f, pisoY + 3f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(48f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(49f, pisoY + 1f), colorSuelo, capaSuelo);
            GenerarPisoSeccion(49, 50, pisoY);

            // Hueco 1 con pinchos (4 tiles, requiere dash)
            GenerarPeligro(new Vector2(52f, pisoY - 1.5f), new Vector2(4f, 0.5f), false);
            GenerarPisoSeccion(54, 56, pisoY);

            // Destello en la trayectoria del dash
            GenerarDestello(new Vector2(52f, pisoY + 1f));

            GenerarLetrero(new Vector2(47f, pisoY + 6f), "ZONA 7: DASH", colorLetrero);
            GenerarLetrero(new Vector2(47f, pisoY + 5.2f), "Shift / K para dash", Color.white);
        }

        // ========================================================
        //  ZONA 8: CHECKPOINT + RESPAWN (x: 56 a 62)
        // ========================================================
        private void CrearZona08_Checkpoint()
        {
            GenerarPisoSeccion(56, 62, pisoY);

            // Checkpoint
            GenerarCheckpoint(new Vector2(58f, pisoY + 1f));

            // Cluster de destellos
            GenerarDestello(new Vector2(57f, pisoY + 1.5f));
            GenerarDestello(new Vector2(58f, pisoY + 2.5f));
            GenerarDestello(new Vector2(59f, pisoY + 1.5f));

            // Pozo mortal despues del checkpoint para probar respawn
            // Hueco en x:60-61
            GenerarPeligro(new Vector2(60.5f, pisoY - 3f), new Vector2(2f, 0.5f), true);
            // Quitar piso en x:60 y 61 — no generar tiles ahi

            GenerarLetrero(new Vector2(57f, pisoY + 4f), "ZONA 8: CHECKPOINT", colorLetrero);
            GenerarLetrero(new Vector2(57f, pisoY + 3.2f), "Toca la bandera. Cae al pozo rojo", Color.white);
        }

        // ========================================================
        //  ZONA 9: WALKER ENEMY (x: 62 a 72)
        // ========================================================
        private void CrearZona09_Walker()
        {
            GenerarPisoSeccion(62, 72, pisoY);

            // Walker 1: para recibir dano y sentir el debuff
            GenerarWalker(new Vector2(65f, pisoY + 1f), debuffVisionTunel);

            // Walker 2: para practicar stomp (plataforma elevada para saltar)
            GenerarTile(new Vector2(68f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(69f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarWalker(new Vector2(70f, pisoY + 1f), debuffVisionTunel);

            GenerarDestello(new Vector2(66f, pisoY + 1.5f));
            GenerarDestello(new Vector2(71f, pisoY + 1.5f));

            GenerarLetrero(new Vector2(63f, pisoY + 4f), "ZONA 9: ENEMIGO WALKER", colorLetrero);
            GenerarLetrero(new Vector2(63f, pisoY + 3.2f), "Salta encima para derrotar (stomp)", Color.white);
        }

        // ========================================================
        //  ZONA 10: FLYER ENEMY (x: 72 a 82)
        // ========================================================
        private void CrearZona10_Flyer()
        {
            GenerarPisoSeccion(72, 82, pisoY);

            // Plataformas para ganar altura
            GenerarTile(new Vector2(74f, pisoY + 1f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(75f, pisoY + 2f), colorSuelo, capaSuelo);

            // Flyer 1 con Vision Tunel
            GenerarFlyer(new Vector2(76f, pisoY + 3.5f), new Vector2(3f, 0f), debuffVisionTunel);

            // Plataformas para segundo flyer
            GenerarTile(new Vector2(79f, pisoY + 1f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(80f, pisoY + 2f), colorSuelo, capaSuelo);

            // Flyer 2 con Controles Invertidos
            GenerarFlyer(new Vector2(80f, pisoY + 4f), new Vector2(2f, 1f), debuffControlesInvertidos);

            GenerarDestello(new Vector2(77f, pisoY + 4.5f));
            GenerarDestello(new Vector2(81f, pisoY + 5f));

            GenerarLetrero(new Vector2(73f, pisoY + 6f), "ZONA 10: ENEMIGO VOLADOR", colorLetrero);
            GenerarLetrero(new Vector2(73f, pisoY + 5.2f), "Usa plataformas para alcanzarlo", Color.white);
        }

        // ========================================================
        //  ZONA 11: PELIGROS (x: 82 a 92)
        // ========================================================
        private void CrearZona11_Peligros()
        {
            GenerarPisoSeccion(82, 84, pisoY);

            // Checkpoint antes de la zona peligrosa
            GenerarCheckpoint(new Vector2(83f, pisoY + 1f));

            // Pinchos suaves (dano -1) en el suelo de x:85 a x:88
            GenerarPisoSeccion(85, 88, pisoY);
            GenerarPeligro(new Vector2(86.5f, pisoY + 0.6f), new Vector2(3f, 0.3f), false);

            // Plataformas para esquivar pinchos
            GenerarTile(new Vector2(85f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(86f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(87f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(88f, pisoY + 2f), colorSuelo, capaSuelo);

            // Piso despues de pinchos
            GenerarPisoSeccion(89, 90, pisoY);

            // Pozo mortal (instant kill) x:90-91
            GenerarPeligro(new Vector2(90.5f, pisoY - 2f), new Vector2(2f, 0.5f), true);

            // Piso final
            GenerarPisoSeccion(92, 92, pisoY);

            GenerarLetrero(new Vector2(83f, pisoY + 4f), "ZONA 11: PELIGROS", colorLetrero);
            GenerarLetrero(new Vector2(83f, pisoY + 3.2f), "Pinchos=-1 vida. Rojo=muerte", Color.white);
        }

        // ========================================================
        //  ZONA 12: POWER-UPS (x: 92 a 112)
        // ========================================================
        private void CrearZona12_PowerUps()
        {
            GenerarPisoSeccion(92, 112, pisoY);

            // 12a: Latido Calmado (x: 92-96)
            var latido = ScriptableObject.CreateInstance<PowerUpSO>();
            latido.nombrePowerUp = "Latido Calmado";
            latido.tipo = TipoPowerUp.LatidoCalmado;
            latido.duracion = 10f;
            latido.radio = 6f;
            latido.magnitud = 0.5f;
            GenerarPowerUp(new Vector2(93f, pisoY + 1.5f), latido, new Color(0.3f, 0.85f, 0.9f));
            GenerarWalker(new Vector2(95f, pisoY + 1f), debuffVisionTunel);
            GenerarLetrero(new Vector2(93f, pisoY + 4f), "Latido Calmado", colorLetrero);
            GenerarLetrero(new Vector2(93f, pisoY + 3.2f), "Ralentiza enemigos cercanos", Color.white);

            // 12b: Destello Guia (x: 96-100)
            var guia = ScriptableObject.CreateInstance<PowerUpSO>();
            guia.nombrePowerUp = "Destello Guia";
            guia.tipo = TipoPowerUp.DestelloGuia;
            guia.duracion = 15f;
            guia.radio = 12f;
            GenerarPowerUp(new Vector2(97f, pisoY + 1.5f), guia, new Color(1f, 1f, 0.5f));
            // Destellos "ocultos" (lejos del camino)
            GenerarDestello(new Vector2(98f, pisoY + 5f));
            GenerarDestello(new Vector2(99f, pisoY + 6f));
            GenerarLetrero(new Vector2(97f, pisoY + 4f), "Destello Guia", colorLetrero);
            GenerarLetrero(new Vector2(97f, pisoY + 3.2f), "Revela coleccionables ocultos", Color.white);

            // 12c: Escudo Respiracion (x: 100-104)
            var escudo = ScriptableObject.CreateInstance<PowerUpSO>();
            escudo.nombrePowerUp = "Escudo Respiracion";
            escudo.tipo = TipoPowerUp.EscudoRespiracion;
            escudo.duracion = 999f;
            GenerarPowerUp(new Vector2(101f, pisoY + 1.5f), escudo, new Color(0.4f, 1f, 0.5f));
            GenerarWalker(new Vector2(103f, pisoY + 1f), debuffPasosDePlomo);
            GenerarLetrero(new Vector2(101f, pisoY + 4f), "Escudo Respiracion", colorLetrero);
            GenerarLetrero(new Vector2(101f, pisoY + 3.2f), "Bloquea el proximo debuff", Color.white);

            // 12d: Memoria Clara (x: 104-108)
            var memoria = ScriptableObject.CreateInstance<PowerUpSO>();
            memoria.nombrePowerUp = "Memoria Clara";
            memoria.tipo = TipoPowerUp.MemoriaClara;
            memoria.duracion = 10f;
            memoria.magnitud = 0.5f;
            GenerarPowerUp(new Vector2(105f, pisoY + 1.5f), memoria, new Color(0.5f, 0.9f, 1f));
            GenerarDestello(new Vector2(106f, pisoY + 1.5f));
            GenerarDestello(new Vector2(107f, pisoY + 1.5f));
            GenerarLetrero(new Vector2(105f, pisoY + 4f), "Memoria Clara", colorLetrero);
            GenerarLetrero(new Vector2(105f, pisoY + 3.2f), "+50% velocidad. Corre!", Color.white);

            // 12e: Foco Claridad (x: 108-112)
            var foco = ScriptableObject.CreateInstance<PowerUpSO>();
            foco.nombrePowerUp = "Foco Claridad";
            foco.tipo = TipoPowerUp.FocoClaridad;
            foco.duracion = 15f;
            GenerarPowerUp(new Vector2(109f, pisoY + 1.5f), foco, new Color(0.9f, 0.5f, 1f));
            // Plataforma alta que requiere el salto extra
            GenerarTile(new Vector2(111f, pisoY + 5f), colorSuelo, capaSuelo);
            GenerarDestello(new Vector2(111f, pisoY + 6.5f));
            GenerarLetrero(new Vector2(109f, pisoY + 4f), "Foco Claridad", colorLetrero);
            GenerarLetrero(new Vector2(109f, pisoY + 3.2f), "+1 salto extra. Alcanza arriba!", Color.white);
        }

        // ========================================================
        //  ZONA 13: DEBUFFS (x: 112 a 124)
        // ========================================================
        private void CrearZona13_Debuffs()
        {
            GenerarPisoSeccion(112, 124, pisoY);

            // 13a: Vision Tunel (x: 112-116)
            GenerarWalker(new Vector2(114f, pisoY + 1f), debuffVisionTunel);
            GenerarDestello(new Vector2(115f, pisoY + 1.5f));
            GenerarLetrero(new Vector2(113f, pisoY + 4f), "DEBUFF: Vision Tunel", new Color(1f, 0.4f, 0.4f));
            GenerarLetrero(new Vector2(113f, pisoY + 3.2f), "Lento por 4 segundos", Color.white);

            // 13b: Pasos de Plomo (x: 116-120)
            GenerarWalker(new Vector2(118f, pisoY + 1f), debuffPasosDePlomo);
            GenerarTile(new Vector2(119f, pisoY + 3f), colorSuelo, capaSuelo);
            GenerarDestello(new Vector2(119f, pisoY + 4.5f));
            GenerarLetrero(new Vector2(117f, pisoY + 4f), "DEBUFF: Pasos de Plomo", new Color(1f, 0.4f, 0.4f));
            GenerarLetrero(new Vector2(117f, pisoY + 3.2f), "Salto reducido 5s", Color.white);

            // 13c: Controles Invertidos (x: 120-124)
            GenerarWalker(new Vector2(122f, pisoY + 1f), debuffControlesInvertidos);
            // Plataformas que requieren precision
            GenerarTile(new Vector2(122f, pisoY + 2f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(123f, pisoY + 3f), colorSuelo, capaSuelo);
            GenerarDestello(new Vector2(123f, pisoY + 4.5f));
            GenerarLetrero(new Vector2(121f, pisoY + 5f), "DEBUFF: Invertidos", new Color(1f, 0.4f, 0.4f));
            GenerarLetrero(new Vector2(121f, pisoY + 4.2f), "A<->D por 3 segundos", Color.white);
        }

        // ========================================================
        //  ZONA 14: FRAGMENTOS DE IDENTIDAD (x: 124 a 132)
        // ========================================================
        private void CrearZona14_Fragmentos()
        {
            GenerarPisoSeccion(124, 132, pisoY);

            // Crear los 3 fragmentos
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

            GenerarColeccionableFragmento(new Vector2(125.5f, pisoY + 1.5f), aura);
            GenerarColeccionableFragmento(new Vector2(127f, pisoY + 1.5f), tinta);
            GenerarColeccionableFragmento(new Vector2(128.5f, pisoY + 1.5f), acero);

            // Plataforma alta (probar Tinta de Estrellas +1 salto)
            GenerarTile(new Vector2(130f, pisoY + 5f), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(131f, pisoY + 5f), colorSuelo, capaSuelo);
            GenerarDestello(new Vector2(130.5f, pisoY + 6.5f));

            // Walker (probar Acero Mental reduccion dano)
            GenerarWalker(new Vector2(131f, pisoY + 1f), debuffVisionTunel);

            GenerarLetrero(new Vector2(125f, pisoY + 4f), "ZONA 14: FRAGMENTOS", colorLetrero);
            GenerarLetrero(new Vector2(125f, pisoY + 3.2f), "Recoge y equipa con ESC > Inventario", Color.white);
        }

        // ========================================================
        //  ZONA 15: DESAFIO COMBINADO (x: 132 a 150)
        // ========================================================
        private void CrearZona15_DesafioCombinado()
        {
            // Checkpoint al inicio
            GenerarPisoSeccion(132, 133, pisoY);
            GenerarCheckpoint(new Vector2(132.5f, pisoY + 1f));

            // Seccion 1: Dash sobre pinchos (x: 134-137)
            GenerarPisoSeccion(134, 134, pisoY);
            GenerarPeligro(new Vector2(136f, pisoY - 0.5f), new Vector2(3f, 0.3f), false);
            GenerarPisoSeccion(137, 138, pisoY);

            // Seccion 2: Wall jump shaft angosto (2.5 tiles de separacion)
            for (int y = (int)pisoY; y <= (int)pisoY + 6; y++)
            {
                GenerarTile(new Vector2(138f, y), colorPared, capaPared);
                GenerarTile(new Vector2(140.5f, y), colorPared, capaPared);
            }
            GenerarTile(new Vector2(139f, pisoY - 1), colorSuelo, capaSuelo);
            GenerarTile(new Vector2(140f, pisoY - 1), colorSuelo, capaSuelo);
            GenerarDestello(new Vector2(138.7f, pisoY + 3f));
            GenerarDestello(new Vector2(140f, pisoY + 5f));

            // Plataforma salida del shaft
            GenerarPisoSeccion(142, 143, pisoY + 6);

            // Seccion 3: Doble salto sobre hueco (x: 143-147)
            // Hueco de 4 tiles
            GenerarPisoSeccion(147, 148, pisoY + 6);
            GenerarDestello(new Vector2(145f, pisoY + 9f));

            // Seccion 4: Flyer para stomp (x: 147)
            GenerarFlyer(new Vector2(148f, pisoY + 8f), new Vector2(2f, 0f), debuffControlesInvertidos);

            // Seccion 5: Drop-through one-way (bajada)
            for (int x = 148; x <= 149; x++)
            {
                GenerarOneWay(new Vector2(x, pisoY + 4f));
                GenerarOneWay(new Vector2(x, pisoY + 2f));
            }
            GenerarPisoSeccion(148, 150, pisoY);

            // Seccion 6: Tunel de crouch hacia la meta (techo bajo, solo agachado)
            for (int x = 148; x <= 150; x++)
                GenerarTile(new Vector2(x, pisoY + 1.1f), colorPared, capaSuelo);

            // Destellos de recompensa
            for (int i = 0; i < 5; i++)
                GenerarDestello(new Vector2(133f + i * 3.5f, pisoY + 2f + (i % 2) * 3f));

            GenerarLetrero(new Vector2(132f, pisoY + 4f), "ZONA 15: DESAFIO", colorLetrero);
            GenerarLetrero(new Vector2(132f, pisoY + 3.2f), "Usa TODAS las mecanicas!", Color.white);
        }

        // ========================================================
        //  ZONA 16: META (x: 150 a 155)
        // ========================================================
        private void CrearZona16_Meta()
        {
            // Piso dorado
            for (int x = 150; x <= 155; x++)
                GenerarTile(new Vector2(x, pisoY), colorMeta, capaSuelo);

            // LevelGoal trigger
            GenerarMeta(new Vector2(153f, pisoY + 1f));

            // Destellos finales
            GenerarDestello(new Vector2(151f, pisoY + 2f));
            GenerarDestello(new Vector2(152f, pisoY + 3f));
            GenerarDestello(new Vector2(153f, pisoY + 4f));
            GenerarDestello(new Vector2(154f, pisoY + 3f));
            GenerarDestello(new Vector2(155f, pisoY + 2f));

            GenerarLetrero(new Vector2(152f, pisoY + 6f), "ZONA 16: META", colorMeta);
            GenerarLetrero(new Vector2(152f, pisoY + 5.2f), "Nivel completado!", Color.white);
        }

        // ========================================================
        //  HELPERS — TILES Y GEOMETRIA
        // ========================================================
        private GameObject GenerarTile(Vector2 posicion, Color color, string nombreCapa)
        {
            var go = new GameObject("Tile");
            go.transform.SetParent(raizNivel, false);
            go.transform.position = posicion;
            int capa = LayerMask.NameToLayer(nombreCapa);
            if (capa < 0) capa = ObtenerCapaFallback(nombreCapa);
            go.layer = capa;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spriteTile;
            sr.color = color;
            sr.sortingOrder = 3;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = Vector2.one;
            return go;
        }

        private void GenerarOneWay(Vector2 posicion)
        {
            var go = GenerarTile(posicion, colorOneWay, capaUnaDireccion);
            go.name = "TileOneWay";
            go.AddComponent<OneWayPlatform>();
        }

        private void GenerarParedVertical(float x, float yInicio, float yFin)
        {
            for (float y = yInicio; y <= yFin; y += 1f)
                GenerarTile(new Vector2(x, y), colorPared, capaPared);
        }

        private void GenerarPisoSeccion(int xInicio, int xFin, float y)
        {
            for (int x = xInicio; x <= xFin; x++)
                GenerarTile(new Vector2(x, y), colorSuelo, capaSuelo);
        }

        // ========================================================
        //  HELPERS — ENTIDADES
        // ========================================================
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

        private void GenerarCheckpoint(Vector2 posicion)
        {
            var go = new GameObject("Checkpoint");
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(14, 40, new Color(0.8f, 0.8f, 0.8f), "BanderaCheckpoint");
            sr.sortingOrder = 4;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.6f, 1.5f);
            go.AddComponent<Checkpoint>();
        }

        private void GenerarPeligro(Vector2 posicion, Vector2 tamano, bool instantaneo)
        {
            var go = new GameObject(instantaneo ? "PozoMortal" : "Pinchos");
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            Color c = instantaneo ? colorMortal : colorPinchos;
            int ancho = Mathf.Max(16, (int)(tamano.x * 32));
            int alto = Mathf.Max(8, (int)(tamano.y * 32));
            sr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(ancho, alto, c, instantaneo ? "PozoMortal" : "Pinchos");
            sr.sortingOrder = 4;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = tamano;
            var h = go.AddComponent<Hazard>();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            if (instantaneo)
                typeof(Hazard).GetField("matarInstantaneamente", flags)?.SetValue(h, true);
        }

        private void GenerarWalker(Vector2 posicion, DebuffSO debuff)
        {
            var go = new GameObject("Enemigo_Walker");
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(28, 28, new Color(0.85f, 0.25f, 0.25f), "Walker");
            sr.sortingOrder = 8;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 4f;
            rb.freezeRotation = true;
            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 0.8f);
            var we = go.AddComponent<WalkerEnemy>();

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            int idSuelo = LayerMask.NameToLayer(capaSuelo);
            int idPared = LayerMask.NameToLayer(capaPared);
            if (idSuelo < 0) idSuelo = 6;
            if (idPared < 0) idPared = 7;
            int mascara = (1 << idSuelo) | (1 << idPared);
            typeof(WalkerEnemy).GetField("capaSuelo", flags)?.SetValue(we, (LayerMask)mascara);
            typeof(EnemyBase).GetField("debuffAlContacto", flags)?.SetValue(we, debuff);
        }

        private void GenerarFlyer(Vector2 posicion, Vector2 offsetPatrulla, DebuffSO debuff)
        {
            var go = new GameObject("Enemigo_Flyer");
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.Circulo(28, new Color(0.6f, 0.3f, 0.85f), "Flyer");
            sr.sortingOrder = 8;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
            var fe = go.AddComponent<FlyerEnemy>();

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            typeof(EnemyBase).GetField("debuffAlContacto", flags)?.SetValue(fe, debuff);
            typeof(FlyerEnemy).GetField("offsetPatrulla", flags)?.SetValue(fe, offsetPatrulla);
        }

        private void GenerarColeccionableFragmento(Vector2 posicion, IdentityFragmentSO fragmento)
        {
            var go = new GameObject("Fragmento_" + fragmento.nombreFragmento);
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.Circulo(24, fragmento.colorTinte, "FragmentoCirculo");
            sr.sortingOrder = 7;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;
            var c = go.AddComponent<Collectible>();
            c.Configurar(TipoColeccionable.FragmentoIdentidad, 0, null, fragmento);
        }

        private void GenerarMeta(Vector2 posicion)
        {
            var go = new GameObject("Meta_Nivel");
            go.transform.position = posicion;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator2D.RectanguloSolido(16, 48, colorMeta, "Meta");
            sr.sortingOrder = 5;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1f, 2f);
            go.AddComponent<LevelGoal>();
        }

        private int ObtenerCapaFallback(string nombreCapa)
        {
            // Fallbacks fijos si las layers no estan configuradas en Unity
            if (nombreCapa == capaSuelo || nombreCapa == "Ground") return 6;
            if (nombreCapa == capaPared || nombreCapa == "Wall") return 7;
            if (nombreCapa == capaUnaDireccion || nombreCapa == "OneWay") return 8;
            return 0;
        }

        // ========================================================
        //  HELPERS — LETREROS (World-Space Canvas)
        // ========================================================
        private void GenerarLetrero(Vector2 posicion, string texto, Color color)
        {
            var go = new GameObject("Letrero");
            go.transform.position = new Vector3(posicion.x, posicion.y, 0f);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 20;

            var rt = canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(600, 50);
            rt.localScale = new Vector3(0.01f, 0.01f, 1f);

            var textoGo = new GameObject("Texto");
            textoGo.transform.SetParent(go.transform, false);
            var t = textoGo.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.text = texto;
            t.fontSize = 36;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;

            var trt = t.rectTransform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;
        }

        // ========================================================
        //  EVENT SYSTEM
        // ========================================================
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
