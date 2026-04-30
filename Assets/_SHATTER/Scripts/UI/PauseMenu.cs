using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Shatter.Core;
using Shatter.Player;
using Shatter.ScriptableObjects;

namespace Shatter.UI
{
    /// <summary>
    /// Menu de pausa + Inventario Emocional (equipar Fragmentos de Identidad).
    /// Versión refactorizada para diseño visual en el Editor de Unity.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private KeyCode teclaAlternar = KeyCode.Escape;

        [Header("Paneles UI (Asignar en el Inspector)")]
        [Tooltip("El panel principal del Menú de Pausa")]
        [SerializeField] private GameObject panelPausa;
        
        [Tooltip("El panel del Inventario Emocional")]
        [SerializeField] private GameObject panelInventario;

        [Tooltip("El panel de Ajustes (Opcional)")]
        [SerializeField] private GameObject panelAjustes;

        [Header("Inventario UI")]
        [Tooltip("El objeto vacío dentro del panel de inventario donde aparecerán los botones de los fragmentos")]
        [SerializeField] private Transform contenedorFragmentos;

        private bool estaAbierto;
        private IdentityManager gestorIdentidad;

        private void Start()
        {
            // Ocultar los paneles al iniciar el nivel
            if (panelPausa != null) panelPausa.SetActive(false);
            if (panelInventario != null) panelInventario.SetActive(false);
            if (panelAjustes != null) panelAjustes.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(teclaAlternar))
            {
                // Si ajustes está abierto, cerrarlo primero
                if (panelAjustes != null && panelAjustes.activeSelf)
                {
                    CerrarAjustes();
                    return;
                }

                // Si el inventario está abierto, la tecla escape lo cierra
                if (panelInventario != null && panelInventario.activeSelf) 
                { 
                    CerrarInventario(); 
                    return; 
                }
                
                AlternarPausa();
            }
        }

        public void AlternarPausa()
        {
            estaAbierto = !estaAbierto;
            
            if (panelPausa != null) panelPausa.SetActive(estaAbierto);
            
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.AlternarPausa();
            }
            else
            {
                // Fallback de seguridad si no hay GameManager en la escena
                Time.timeScale = estaAbierto ? 0f : 1f; 
            }
        }

        // ------- MÉTODOS PARA LOS BOTONES DEL MENÚ DE PAUSA -------

        public void ReanudarJuego()
        {
            if (estaAbierto) AlternarPausa();
        }

        public void ReiniciarNivel()
        {
            Time.timeScale = 1f; // Asegurar que el tiempo vuelva a la normalidad
            if (GameManager.Instance != null) 
            { 
                GameManager.Instance.EstablecerEstado(GameManager.EstadoJuego.Jugando); 
                GameManager.Instance.ReiniciarEscenaActual(); 
            }
            else 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void IrAlMenuPrincipal()
        {
            Time.timeScale = 1f;
            // Cambia "MainMenu" por el nombre exacto de la escena de tu menú principal
            SceneManager.LoadScene("Menu Principal"); 
        }

        public void AbrirAjustes()
        {
            if (panelPausa != null) panelPausa.SetActive(false);
            if (panelAjustes != null) panelAjustes.SetActive(true);
        }

        public void CerrarAjustes()
        {
            if (panelAjustes != null) panelAjustes.SetActive(false);
            if (panelPausa != null) panelPausa.SetActive(true);
        }

        public void SalirDelJuego()
        {
            Debug.Log("Saliendo del juego...");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        // ------- LÓGICA DEL INVENTARIO EMOCIONAL -------

        public void AbrirInventario()
        {
            if (gestorIdentidad == null)
            {
                var jugador = GameObject.FindGameObjectWithTag("Player");
                if (jugador != null) gestorIdentidad = jugador.GetComponent<IdentityManager>();
            }
            
            if (panelPausa != null) panelPausa.SetActive(false); // Ocultar pausa detrás
            if (panelInventario != null) panelInventario.SetActive(true);
            
            ActualizarInventario();
        }

        public void CerrarInventario()
        {
            if (panelInventario != null) panelInventario.SetActive(false);
            if (panelPausa != null) panelPausa.SetActive(true); // Mostrar menú de pausa otra vez
        }

        private void ActualizarInventario()
        {
            if (contenedorFragmentos == null)
            {
                Debug.LogWarning("Falta asignar el Contenedor de Fragmentos en el Inspector.");
                return;
            }

            // Limpiar botones previos en el contenedor
            foreach (Transform child in contenedorFragmentos)
            {
                Destroy(child.gameObject);
            }

            if (gestorIdentidad == null) return;

            int i = 0;
            foreach (var fragmento in gestorIdentidad.Descubiertos)
            {
                int indice = i;
                var capturado = fragmento;
                bool equipado = gestorIdentidad.EstaEquipado(fragmento);
                
                string etiqueta = (equipado ? "[✓] " : "[ ] ") + fragmento.nombreFragmento;
                
                // Seguimos usando código para generar los botones de la lista ya que es dinámica, 
                // pero ahora se generan dentro del contenedor que tú decidas visualmente.
                var btn = CrearBotonInventario(contenedorFragmentos, etiqueta, new Vector2(0, -50 - (i * 60)), () => {
                    if (gestorIdentidad.EstaEquipado(capturado)) gestorIdentidad.Desequipar(capturado);
                    else gestorIdentidad.Equipar(capturado);
                    ActualizarInventario();
                });
                
                btn.name = "BtnFrag_" + indice;
                
                // Tinte del color del fragmento
                var img = btn.GetComponent<Image>();
                if (img != null) img.color = new Color(fragmento.colorTinte.r, fragmento.colorTinte.g, fragmento.colorTinte.b, 0.8f);
                i++;
            }
        }

        // Generador de botones dinámico exclusivo para los fragmentos del inventario
        private Button CrearBotonInventario(Transform padre, string etiqueta, Vector2 posicionAnclada, System.Action alHacerClick)
        {
            var go = new GameObject("Btn_" + etiqueta);
            go.transform.SetParent(padre, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f); // Anclado arriba al centro
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(380, 52);
            rt.anchoredPosition = posicionAnclada;
            
            var img = go.AddComponent<Image>();
            img.color = new Color(0.25f, 0.25f, 0.35f, 0.9f);
            
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => alHacerClick?.Invoke());

            var etiquetaGo = new GameObject("Etiqueta");
            etiquetaGo.transform.SetParent(go.transform, false);
            
            // Usamos Text Legacy para mantener compatibilidad con tu sistema anterior
            var t = etiquetaGo.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.text = etiqueta;
            t.fontSize = 22;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            
            var lrt = t.rectTransform;
            lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
            
            return btn;
        }
    }
}
