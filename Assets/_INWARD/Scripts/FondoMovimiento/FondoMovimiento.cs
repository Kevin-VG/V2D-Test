using UnityEngine;

/// <summary>
/// Desplaza la textura del material de forma automática (tiempo) y reactiva (parallax de cámara).
/// Excelente técnica de alto rendimiento para fondos infinitos en 2D sin duplicar GameObjects.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class FondoMovimiento : MonoBehaviour
{
    [Header("1. Movimiento Automático (Auto Scroll)")]
    [Tooltip("Velocidad de desplazamiento constante (ej: nubes o niebla moviéndose solas).")]
    [SerializeField] private Vector2 velocidadAuto = new Vector2(0.02f, 0f);

    [Header("2. Movimiento Parallax (Seguimiento de Cámara)")]
    [Tooltip("¿Activar el efecto parallax basado en el movimiento de la cámara?")]
    [SerializeField] private bool usarParallax = true;
    [Tooltip("El factor de parallax. 0 = se mueve pegado a la pantalla (infinito). 0.1 = montañas lejanas. 0.5 = árboles cercanos.")]
    [SerializeField] private Vector2 factorParallax = new Vector2(0.1f, 0f);
    [Tooltip("Referencia a la cámara. Si se deja vacío, buscará la Main Camera automáticamente.")]
    [SerializeField] private Transform camara;

    private Vector2 offsetAuto;
    private Material material;
    private Renderer miRenderer;

    // Cacheamos los IDs del Shader para evitar GC Alloc
    private static readonly int PropMainTex = Shader.PropertyToID("_MainTex");
    private static readonly int PropBaseMap = Shader.PropertyToID("_BaseMap");

    private bool tieneMainTex;
    private bool tieneBaseMap;

    void Awake()
    {
        miRenderer = GetComponent<Renderer>();
        if (miRenderer != null)
        {
            material = miRenderer.material;
            tieneMainTex = material.HasProperty(PropMainTex);
            tieneBaseMap = material.HasProperty(PropBaseMap);
        }
        else
        {
            Debug.LogError($"[FondoMovimiento] No se encontró ningún Renderer en {gameObject.name}.", this);
            enabled = false;
        }

        // Buscar cámara automáticamente si no se asignó en el Inspector
        if (camara == null)
        {
            var mainCam = Camera.main;
            if (mainCam != null) camara = mainCam.transform;
        }
    }

    void Update()
    {
        if (material == null) return;

        // 1. Calculamos el desplazamiento automático continuo
        offsetAuto += velocidadAuto * Time.deltaTime;
        offsetAuto.x %= 1.0f;
        offsetAuto.y %= 1.0f;

        // 2. Calculamos el desplazamiento por Parallax (seguimiento de la cámara)
        Vector2 offsetParallax = Vector2.zero;
        if (usarParallax && camara != null)
        {
            offsetParallax.x = camara.position.x * factorParallax.x;
            offsetParallax.y = camara.position.y * factorParallax.y;
        }

        // 3. Combinamos ambos efectos
        Vector2 offsetFinal = offsetAuto + offsetParallax;
        offsetFinal.x %= 1.0f;
        offsetFinal.y %= 1.0f;

        // Aplicamos el desplazamiento de la textura
        if (tieneBaseMap)
        {
            material.SetTextureOffset(PropBaseMap, offsetFinal); // URP (Lit/Unlit Shaders)
        }
        
        if (tieneMainTex)
        {
            material.SetTextureOffset(PropMainTex, offsetFinal); // Clásico (Unlit/Transparent, etc.)
        }
    }

    private void OnDestroy()
    {
        // Liberamos la instancia del material creado en runtime para evitar memory leaks
        if (material != null)
        {
            Destroy(material);
        }
    }
}
