using UnityEngine;

namespace Shatter.Player
{
    /// <summary>
    /// Platformer 2D avanzado: movimiento con aceleracion, salto variable,
    /// coyote time, jump buffer, doble salto, wall slide + wall jump, dash,
    /// crouch y drop-through desde OneWayPlatforms.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movimiento horizontal")]
        [SerializeField] private float velocidadMaxima = 8f;
        [SerializeField] private float aceleracion = 80f;
        [SerializeField] private float desaceleracion = 70f;
        [SerializeField] private float controlAereo = 0.65f;

        [Header("Salto")]
        [SerializeField] private float fuerzaSalto = 16f;
        [SerializeField] private float multiplicadorCorteSalto = 0.45f;
        [SerializeField] private float tiempoCoyote = 0.12f;
        [SerializeField] private float tiempoBufferSalto = 0.15f;
        [SerializeField] private int saltosExtra = 1;

        [Header("Gravedad")]
        [SerializeField] private float escalaGravedadSubida = 3f;
        [SerializeField] private float escalaGravedadBajada = 5f;
        [SerializeField] private float velocidadCaidaMaxima = 22f;

        [Header("Pared")]
        [SerializeField] private float velocidadDeslizPared = 3f;
        [SerializeField] private Vector2 fuerzaSaltoPared = new Vector2(14f, 16f);
        [SerializeField] private float tiempoBloqueoSaltoPared = 0.15f;

        [Header("Dash")]
        [SerializeField] private float velocidadDash = 22f;
        [SerializeField] private float duracionDash = 0.18f;
        [SerializeField] private float enfriamientoDash = 0.6f;

        [Header("Chequeo Suelo / Pared")]
        [SerializeField] private Vector2 tamanoChequeoSuelo = new Vector2(0.65f, 0.2f);
        [SerializeField] private float offsetChequeoSueloY = -0.65f;
        [SerializeField] private Vector2 tamanoChequeoParedes = new Vector2(0.15f, 0.7f);
        [SerializeField] private float offsetChequeoParedX = 0.42f;
        [SerializeField] private LayerMask capaSuelo;
        [SerializeField] private LayerMask capaPared;
        [SerializeField] private LayerMask capaUnaDireccion;

        [Header("Agacharse")]
        [SerializeField] private float multiplicadorVelocidadAgachado = 0.5f;
        [SerializeField] private Vector2 tamanoColliderNormal = new Vector2(0.7f, 1.2f);
        [SerializeField] private Vector2 offsetColliderNormal = new Vector2(0f, 0f);
        [SerializeField] private Vector2 tamanoColliderAgachado = new Vector2(0.7f, 0.6f);
        [SerializeField] private Vector2 offsetColliderAgachado = new Vector2(0f, -0.3f);

        // --- Estado interno ---
        private Rigidbody2D rb;
        private BoxCollider2D boxCol;
        private bool estaAgachado;

        private float entradaMovimiento;
        private bool saltoPresionadoEsteFrame;
        private bool saltoMantenido;
        private bool dashPresionadoEsteFrame;
        private bool agachadoMantenido;
        private bool abajoMantenido;

        private float contadorCoyote;
        private float contadorBufferSalto;
        private int saltosRestantes;
        private bool saltoUsadoEnSuelo; // evita saltar multiples veces desde el suelo

        private bool estaEnSuelo;
        private bool tocaParedDerecha;
        private bool tocaParedIzquierda;
        private bool estaDeslizandoPared;

        private bool estaHaciendoDash;
        private float temporizadorDash;
        private float temporizadorEnfriamientoDash;
        private Vector2 direccionDash;

        private float temporizadorBloqueoSaltoPared;
        private bool controlesInvertidos;
        private float multiplicadorVelocidad = 1f;
        private bool congelamientoExterno;

        private int direccion = 1; // 1 derecha, -1 izquierda

        // Referencias a subsistemas
        private FocusSystem focusSystem;
        private EmotionalWeight emotionalWeight;

        // Buffers pre-asignados para OverlapBox (evita GC)
        private readonly Collider2D[] resultadosChequeo = new Collider2D[8];

        // --- API publica ---
        public bool EstaEnSuelo => estaEnSuelo;
        public bool EstaHaciendoDash => estaHaciendoDash;
        public bool EstaDeslizandoPared => estaDeslizandoPared;
        public bool EstaAgachado => estaAgachado;
        public float VelocidadX => rb != null ? rb.linearVelocity.x : 0f;
        public float VelocidadY => rb != null ? rb.linearVelocity.y : 0f;
        public int Direccion => direccion;

        public void DetenerMovimiento() { congelamientoExterno = true; if (rb != null) rb.linearVelocity = Vector2.zero; }
        public void ReanudarMovimiento() { congelamientoExterno = false; }
        public void AplicarMultiplicadorVelocidad(float m) { multiplicadorVelocidad = Mathf.Max(0.1f, m); }
        public void EstablecerControlesInvertidos(bool inv) { controlesInvertidos = inv; }
        public void EstablecerSaltosExtra(int n) { saltosExtra = Mathf.Max(0, n); }
        public int ObtenerSaltosExtra() => saltosExtra;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCol = GetComponent<BoxCollider2D>();
            focusSystem = GetComponent<FocusSystem>();
            emotionalWeight = GetComponent<EmotionalWeight>();

            rb.freezeRotation = true;
            rb.gravityScale = escalaGravedadBajada;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            if (congelamientoExterno) { entradaMovimiento = 0f; return; }

            // --- Lectura de input ---
            float crudo = Input.GetAxisRaw("Horizontal");
            if (controlesInvertidos) crudo = -crudo;
            entradaMovimiento = crudo;

            saltoPresionadoEsteFrame = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
            saltoMantenido = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            dashPresionadoEsteFrame = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K);
            abajoMantenido = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

            // --- Drop through one-way (ANTES de agacharse para no interferir) ---
            if (estaEnSuelo && abajoMantenido && saltoPresionadoEsteFrame)
            {
                if (IntentarCaerPorPlataforma())
                {
                    contadorBufferSalto = 0f;
                    saltoPresionadoEsteFrame = false;
                    agachadoMantenido = false;
                    // No procesar nada mas este frame — el jugador cae
                    return;
                }
            }

            // --- Agacharse (solo si no hizo drop-through) ---
            agachadoMantenido = abajoMantenido && estaEnSuelo;
            if (agachadoMantenido && !estaAgachado)
            {
                estaAgachado = true;
                boxCol.size = tamanoColliderAgachado;
                boxCol.offset = offsetColliderAgachado;
            }
            else if (!agachadoMantenido && estaAgachado)
            {
                Vector2 origenTecho = (Vector2)transform.position + new Vector2(0f, 0.35f);
                bool techoEncima = ChequearOverlap(origenTecho, new Vector2(tamanoColliderNormal.x - 0.1f, 0.4f), capaSuelo | capaPared);
                if (!techoEncima)
                {
                    estaAgachado = false;
                    boxCol.size = tamanoColliderNormal;
                    boxCol.offset = offsetColliderNormal;
                }
                else
                {
                    agachadoMantenido = true;
                }
            }

            // --- Temporizadores ---
            if (contadorCoyote > 0f) contadorCoyote -= Time.deltaTime;
            if (contadorBufferSalto > 0f) contadorBufferSalto -= Time.deltaTime;
            if (temporizadorBloqueoSaltoPared > 0f) temporizadorBloqueoSaltoPared -= Time.deltaTime;
            if (temporizadorEnfriamientoDash > 0f) temporizadorEnfriamientoDash -= Time.deltaTime;

            if (saltoPresionadoEsteFrame) contadorBufferSalto = tiempoBufferSalto;

            // --- Dash ---
            if (dashPresionadoEsteFrame && temporizadorEnfriamientoDash <= 0f && !estaHaciendoDash)
            {
                if (focusSystem == null || focusSystem.PuedeRealizarDash())
                {
                    IniciarDash();
                    if (focusSystem != null) focusSystem.ConsumirDash();
                }
            }

            // --- Corte de salto (salto variable) ---
            if (!saltoMantenido && rb.linearVelocity.y > 0f && !estaHaciendoDash)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * multiplicadorCorteSalto);

            // --- Direccion ---
            if (Mathf.Abs(entradaMovimiento) > 0.1f) direccion = entradaMovimiento > 0f ? 1 : -1;
        }

        private void FixedUpdate()
        {
            VerificarSuelo();
            VerificarParedes();
            ManejarSalto();
            ManejarDeslizPared();
            ManejarMovimientoHorizontal();
            ManejarDash();
            AplicarAjustesGravedad();
            LimitarVelocidadCaida();
        }

        // =====================================================
        //  CHEQUEO SEGURO — filtra el collider del propio jugador
        // =====================================================
        private bool ChequearOverlap(Vector2 centro, Vector2 tamano, LayerMask mascara)
        {
            int cantidad = Physics2D.OverlapBoxNonAlloc(centro, tamano, 0f, resultadosChequeo, mascara);
            for (int i = 0; i < cantidad; i++)
            {
                if (resultadosChequeo[i] != null && resultadosChequeo[i].gameObject != gameObject)
                    return true;
            }
            return false;
        }

        // --------- CHEQUEOS ---------
        private void VerificarSuelo()
        {
            Vector2 origen = (Vector2)transform.position + new Vector2(0f, offsetChequeoSueloY);
            bool enSuelo = ChequearOverlap(origen, tamanoChequeoSuelo, capaSuelo | capaUnaDireccion);

            if (enSuelo && !estaEnSuelo)
            {
                // Acaba de aterrizar
                saltosRestantes = saltosExtra;
                temporizadorEnfriamientoDash = 0f;
                saltoUsadoEnSuelo = false;
            }
            estaEnSuelo = enSuelo;
            if (estaEnSuelo) contadorCoyote = tiempoCoyote;
        }

        private void VerificarParedes()
        {
            Vector2 posDer = (Vector2)transform.position + new Vector2(offsetChequeoParedX, 0f);
            Vector2 posIzq = (Vector2)transform.position + new Vector2(-offsetChequeoParedX, 0f);
            tocaParedDerecha = ChequearOverlap(posDer, tamanoChequeoParedes, capaPared);
            tocaParedIzquierda = ChequearOverlap(posIzq, tamanoChequeoParedes, capaPared);
        }

        // --------- HORIZONTAL ---------
        private void ManejarMovimientoHorizontal()
        {
            if (estaHaciendoDash) return;
            if (temporizadorBloqueoSaltoPared > 0f) return;

            float objetivo = entradaMovimiento * velocidadMaxima * multiplicadorVelocidad * (agachadoMantenido ? multiplicadorVelocidadAgachado : 1f);
            float control = estaEnSuelo ? 1f : controlAereo;
            float tasa = (Mathf.Abs(objetivo) > 0.01f) ? aceleracion : desaceleracion;
            float nuevaX = Mathf.MoveTowards(rb.linearVelocity.x, objetivo, tasa * control * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(nuevaX, rb.linearVelocity.y);
        }

        // --------- SALTO ---------
        private void ManejarSalto()
        {
            if (contadorBufferSalto <= 0f) return;

            // 1) Wall jump: solo si NO esta en suelo y toca pared
            if (!estaEnSuelo && (estaDeslizandoPared || tocaParedIzquierda || tocaParedDerecha))
            {
                int dirPared = tocaParedDerecha ? 1 : -1;
                int dirInput = 0;
                if (entradaMovimiento > 0.1f) dirInput = 1;
                else if (entradaMovimiento < -0.1f) dirInput = -1;

                // Modificacion: Solo salta de la pared si el jugador suelta la tecla hacia la pared
                // o presiona la tecla opuesta. Esto evita "rebotar" en el mismo lugar por error.
                if (dirInput != dirPared)
                {
                    if (focusSystem == null || focusSystem.PuedeRealizarWallJump())
                    {
                        int dirSalto = -dirPared;
                        rb.linearVelocity = new Vector2(fuerzaSaltoPared.x * dirSalto, fuerzaSaltoPared.y);
                        // Aumentamos ligeramente el bloqueo para asegurar que el jugador logre cruzar
                        temporizadorBloqueoSaltoPared = tiempoBloqueoSaltoPared * 1.5f; 
                        contadorBufferSalto = 0f;
                        contadorCoyote = 0f;
                        saltosRestantes = Mathf.Max(saltosRestantes, 0); // no regalar saltos extra
                        direccion = dirSalto;

                        if (focusSystem != null) focusSystem.ConsumirWallJump();
                        Shatter.Core.GameEvents.LanzarSaltoJugador(rb.linearVelocity);
                    }
                    else
                    {
                        contadorBufferSalto = 0f;
                    }
                    return;
                }
            }

            // 2) Salto de suelo / coyote
            if (estaEnSuelo || contadorCoyote > 0f)
            {
                if (saltoUsadoEnSuelo && estaEnSuelo) return; // previene multiples saltos de suelo

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
                contadorBufferSalto = 0f;
                contadorCoyote = 0f;
                if (estaEnSuelo) saltoUsadoEnSuelo = true;
                Shatter.Core.GameEvents.LanzarSaltoJugador(rb.linearVelocity);
                return;
            }

            // 3) Doble salto (en el aire)
            if (saltosRestantes > 0)
            {
                if (focusSystem == null || focusSystem.PuedeRealizarDobleSalto())
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
                    saltosRestantes--;
                    contadorBufferSalto = 0f;
                    if (focusSystem != null) focusSystem.ConsumirDobleSalto();
                    Shatter.Core.GameEvents.LanzarSaltoJugador(rb.linearVelocity);
                }
                else
                {
                    contadorBufferSalto = 0f;
                }
            }
        }

        // --------- DESLIZ EN PARED ---------
        private void ManejarDeslizPared()
        {
            bool estabaDeslizando = estaDeslizandoPared;
            bool presionandoContraPared = (entradaMovimiento > 0.1f && tocaParedDerecha) || (entradaMovimiento < -0.1f && tocaParedIzquierda);
            
            // Ahora se pega a la pared incluso si está subiendo (se eliminó la condición de rb.linearVelocity.y < 0f)
            estaDeslizandoPared = !estaEnSuelo && presionandoContraPared;

            if (estaDeslizandoPared)
            {
                // Si sube por la inercia del salto, frenamos un poco su ascenso para que sienta que "se pegó"
                float velY = rb.linearVelocity.y > 0f ? rb.linearVelocity.y * 0.8f : Mathf.Max(rb.linearVelocity.y, -velocidadDeslizPared);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, velY);
            }

            // Al iniciar desliz, restaurar 1 salto para wall jump
            if (estaDeslizandoPared && !estabaDeslizando)
                saltosRestantes = Mathf.Max(saltosRestantes, 1);
        }

        // --------- DASH ---------
        private void IniciarDash()
        {
            estaHaciendoDash = true;
            temporizadorDash = duracionDash;
            temporizadorEnfriamientoDash = enfriamientoDash;
            float dir = Mathf.Abs(entradaMovimiento) > 0.1f ? Mathf.Sign(entradaMovimiento) : direccion;
            direccionDash = new Vector2(dir, 0f);
            rb.linearVelocity = direccionDash * velocidadDash;
        }

        private void ManejarDash()
        {
            if (!estaHaciendoDash) return;
            rb.linearVelocity = new Vector2(direccionDash.x * velocidadDash, 0f);
            temporizadorDash -= Time.fixedDeltaTime;
            if (temporizadorDash <= 0f)
            {
                estaHaciendoDash = false;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.6f, rb.linearVelocity.y);
            }
        }

        // --------- GRAVEDAD ---------
        private void AplicarAjustesGravedad()
        {
            if (estaHaciendoDash) { rb.gravityScale = 0f; return; }
            if (estaDeslizandoPared) { rb.gravityScale = escalaGravedadSubida * 0.5f; return; }
            rb.gravityScale = rb.linearVelocity.y > 0f ? escalaGravedadSubida : escalaGravedadBajada;
        }

        private void LimitarVelocidadCaida()
        {
            if (rb.linearVelocity.y < -velocidadCaidaMaxima)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -velocidadCaidaMaxima);
        }

        // --------- DROP THROUGH ---------
        private bool IntentarCaerPorPlataforma()
        {
            Vector2 origen = (Vector2)transform.position + new Vector2(0f, offsetChequeoSueloY);
            int cantidad = Physics2D.OverlapBoxNonAlloc(origen, tamanoChequeoSuelo, 0f, resultadosChequeo, capaUnaDireccion);
            for (int i = 0; i < cantidad; i++)
            {
                if (resultadosChequeo[i] == null || resultadosChequeo[i].gameObject == gameObject) continue;
                var plataforma = resultadosChequeo[i].GetComponent<Shatter.Levels.OneWayPlatform>();
                if (plataforma != null) { plataforma.DesactivarColisionTemporal(boxCol); return true; }
            }
            return false;
        }

        // --------- GIZMOS ---------
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(0f, offsetChequeoSueloY), tamanoChequeoSuelo);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(offsetChequeoParedX, 0f), tamanoChequeoParedes);
            Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(-offsetChequeoParedX, 0f), tamanoChequeoParedes);
        }
    }
}
