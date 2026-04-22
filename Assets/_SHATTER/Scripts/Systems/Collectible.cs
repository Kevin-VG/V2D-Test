using UnityEngine;
using Shatter.Core;
using Shatter.Player;
using Shatter.ScriptableObjects;

namespace Shatter.Systems
{
    public enum TipoColeccionable { Destello, PowerUp, FragmentoIdentidad }

    /// <summary>
    /// Coleccionable generico: destello (moneda), power-up, o fragmento de identidad.
    /// Implementa IInteractable (opcional) y deteccion por trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Collectible : MonoBehaviour, IPoolable
    {
        [SerializeField] private TipoColeccionable tipo = TipoColeccionable.Destello;
        [SerializeField] private int cantidadDestellos = 1;
        [SerializeField] private PowerUpSO powerUp;
        [SerializeField] private IdentityFragmentSO fragmentoIdentidad;

        [Header("Visual")]
        [SerializeField] private float amplitudFlotacion = 0.15f;
        [SerializeField] private float velocidadFlotacion = 2f;

        private Vector3 posicionBase;
        private float faseFlotacion;
        private SpriteRenderer sr;
        private float temporizadorResaltado;
        private Color colorOriginal;

        public TipoColeccionable Tipo => tipo;

        public void Configurar(TipoColeccionable t, int cantidad = 1, PowerUpSO p = null, IdentityFragmentSO f = null)
        {
            tipo = t; cantidadDestellos = cantidad; powerUp = p; fragmentoIdentidad = f;
        }

        private void Awake()
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) colorOriginal = sr.color;
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnEnable()
        {
            posicionBase = transform.position;
            faseFlotacion = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            faseFlotacion += Time.deltaTime * velocidadFlotacion;
            transform.position = posicionBase + new Vector3(0f, Mathf.Sin(faseFlotacion) * amplitudFlotacion, 0f);

            if (temporizadorResaltado > 0f)
            {
                temporizadorResaltado -= Time.deltaTime;
                if (sr != null)
                {
                    float p = Mathf.PingPong(Time.time * 4f, 1f);
                    sr.color = Color.Lerp(colorOriginal, Color.yellow, p);
                }
                if (temporizadorResaltado <= 0f && sr != null) sr.color = colorOriginal;
            }
        }

        public void Resaltar(float duracion)
        {
            temporizadorResaltado = Mathf.Max(temporizadorResaltado, duracion);
        }

        private void OnTriggerEnter2D(Collider2D otro)
        {
            if (!otro.CompareTag("Player")) return;
            IntentarRecoger(otro.gameObject);
        }

        private void IntentarRecoger(GameObject jugador)
        {
            switch (tipo)
            {
                case TipoColeccionable.Destello:
                    if (GameManager.Instance != null) GameManager.Instance.AgregarDestellos(cantidadDestellos);
                    GameEvents.LanzarDestelloRecogido(cantidadDestellos);
                    break;
                case TipoColeccionable.PowerUp:
                    var pm = jugador.GetComponent<PowerUpManager>();
                    if (pm != null && powerUp != null) pm.Activar(powerUp);
                    GameEvents.LanzarPowerUpRecogido(powerUp != null ? powerUp.nombrePowerUp : "");
                    break;
                case TipoColeccionable.FragmentoIdentidad:
                    var im = jugador.GetComponent<IdentityManager>();
                    if (im != null && fragmentoIdentidad != null) im.Descubrir(fragmentoIdentidad);
                    break;
            }

            Desactivar();
        }

        private void Desactivar()
        {
            if (ObjectPool.Instance != null && ObjectPool.Instance.TienePool(ObtenerEtiquetaPool()))
                ObjectPool.Instance.Reciclar(ObtenerEtiquetaPool(), gameObject);
            else
                gameObject.SetActive(false);
        }

        private string ObtenerEtiquetaPool()
        {
            return tipo switch
            {
                TipoColeccionable.Destello => "Destello",
                TipoColeccionable.PowerUp => "PowerUpPickup",
                TipoColeccionable.FragmentoIdentidad => "FragmentoIdentidad",
                _ => "Destello"
            };
        }

        public void AlGenerar() { }
        public void AlReciclar() { if (sr != null) sr.color = colorOriginal; temporizadorResaltado = 0f; }
    }
}
