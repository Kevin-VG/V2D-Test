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
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private KeyCode teclaAlternar = KeyCode.Escape;

        private GameObject panel;
        private GameObject panelInventario;
        private bool estaAbierto;
        private IdentityManager gestorIdentidad;

        private void Start()
        {
            ConstruirPanel();
            panel.SetActive(false);
            panelInventario.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(teclaAlternar))
            {
                if (panelInventario.activeSelf) { panelInventario.SetActive(false); return; }
                AlternarPausa();
            }
        }

        private void AlternarPausa()
        {
            estaAbierto = !estaAbierto;
            panel.SetActive(estaAbierto);
            if (GameManager.Instance != null) GameManager.Instance.AlternarPausa();
        }

        // ------- CONSTRUCCION UI -------
        private void ConstruirPanel()
        {
            panel = CrearPanel("PanelPausa", new Color(0f, 0f, 0f, 0.75f));

            CrearEtiqueta(panel.transform, "PAUSA", new Vector2(0, 220), 48, Color.white);
            CrearBoton(panel.transform, "Reanudar",     new Vector2(0, 120),  () => AlternarPausa());
            CrearBoton(panel.transform, "Inventario Emocional", new Vector2(0, 50),   AbrirInventario);
            CrearBoton(panel.transform, "Reiniciar Nivel", new Vector2(0, -20), () => {
                if (GameManager.Instance != null) { GameManager.Instance.EstablecerEstado(GameManager.EstadoJuego.Jugando); GameManager.Instance.ReiniciarEscenaActual(); }
                else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
            CrearBoton(panel.transform, "Salir",         new Vector2(0, -90),  () => Application.Quit());

            ConstruirPanelInventario();
        }

        private void ConstruirPanelInventario()
        {
            panelInventario = CrearPanel("PanelInventario", new Color(0.1f, 0.08f, 0.15f, 0.92f));
            CrearEtiqueta(panelInventario.transform, "FRAGMENTOS DE IDENTIDAD", new Vector2(0, 260), 30, Color.white);
            CrearEtiqueta(panelInventario.transform, "Click para equipar/desequipar (max. 3)", new Vector2(0, 220), 18, new Color(0.8f, 0.8f, 0.85f));

            var botonCerrar = CrearBoton(panelInventario.transform, "Cerrar", new Vector2(0, -260), () => panelInventario.SetActive(false));
        }

        private void AbrirInventario()
        {
            if (gestorIdentidad == null)
            {
                var jugador = GameObject.FindGameObjectWithTag("Player");
                if (jugador != null) gestorIdentidad = jugador.GetComponent<IdentityManager>();
            }
            panelInventario.SetActive(true);
            ActualizarInventario();
        }

        private void ActualizarInventario()
        {
            // Limpiar botones previos
            var paraDestruir = new System.Collections.Generic.List<Transform>();
            foreach (Transform t in panelInventario.transform)
                if (t.name.StartsWith("BtnFrag_")) paraDestruir.Add(t);
            foreach (var t in paraDestruir) Destroy(t.gameObject);

            if (gestorIdentidad == null) return;

            int i = 0;
            foreach (var fragmento in gestorIdentidad.Descubiertos)
            {
                int indice = i;
                var capturado = fragmento;
                bool equipado = gestorIdentidad.EstaEquipado(fragmento);
                string etiqueta = (equipado ? "[✓] " : "[ ] ") + fragmento.nombreFragmento;
                var btn = CrearBoton(panelInventario.transform, etiqueta, new Vector2(0, 150 - i * 60), () => {
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

        // ------- HELPERS -------
        private GameObject CrearPanel(string nombre, Color color)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        private Text CrearEtiqueta(Transform padre, string texto, Vector2 posicionAnclada, int tamano, Color color)
        {
            var go = new GameObject("Etiqueta_" + texto);
            go.transform.SetParent(padre, false);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.text = texto;
            t.fontSize = tamano;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            var rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(600, 60);
            rt.anchoredPosition = posicionAnclada;
            return t;
        }

        private Button CrearBoton(Transform padre, string etiqueta, Vector2 posicionAnclada, System.Action alHacerClick)
        {
            var go = new GameObject("Btn_" + etiqueta);
            go.transform.SetParent(padre, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(380, 52);
            rt.anchoredPosition = posicionAnclada;
            var img = go.AddComponent<Image>();
            img.color = new Color(0.25f, 0.25f, 0.35f, 0.9f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => alHacerClick?.Invoke());

            var etiquetaGo = new GameObject("Etiqueta");
            etiquetaGo.transform.SetParent(go.transform, false);
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
