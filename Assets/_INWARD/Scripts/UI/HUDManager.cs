using UnityEngine;
using UnityEngine.UI;
using Shatter.Core;
using Shatter.Player;
using Shatter.Systems;

namespace Shatter.UI
{
    /// <summary>
    /// HUD basico construido en codigo para el prototipo.
    /// Muestra: destellos, vida, power-up activo, debuff activo, prompt de interaccion.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class HUDManager : MonoBehaviour
    {
        private Text textoDestellos;
        private Text textoVida;
        private Text textoPowerUp;
        private Text textoDebuff;
        private Text textoIndicacion;

        private PlayerHealth salud;
        private PowerUpManager powerUp;
        private DebuffManager debuff;
        private InteractionSystem interaccion;

        private void Start()
        {
            ConstruirHUD();
            Invoke(nameof(BuscarReferenciasJugador), 0.1f);

            if (GameManager.Instance != null)
                GameManager.Instance.AlCambiarDestellos += ActualizarDestellos;
            GameEvents.AlRecogerDestello += _ => ActualizarDestellos(GameManager.Instance != null ? GameManager.Instance.DestellosDeLucidez : 0);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AlCambiarDestellos -= ActualizarDestellos;
        }

        private void ConstruirHUD()
        {
            textoDestellos  = CrearTexto("TextoDestellos",  new Vector2(20, -20),  TextAnchor.UpperLeft,   "✦ 0");
            textoVida       = CrearTexto("TextoVida",       new Vector2(20, -55),  TextAnchor.UpperLeft,   "♥ 4");
            textoPowerUp    = CrearTexto("TextoPowerUp",    new Vector2(20, -90),  TextAnchor.UpperLeft,   "");
            textoDebuff     = CrearTexto("TextoDebuff",     new Vector2(20, -125), TextAnchor.UpperLeft,   "");
            textoIndicacion = CrearTexto("TextoIndicacion", new Vector2(0, 80),    TextAnchor.LowerCenter, "");
        }

        private Text CrearTexto(string nombre, Vector2 posicion, TextAnchor ancla, string contenido)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(transform, false);
            var texto = go.AddComponent<Text>();
            texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (texto.font == null) texto.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            texto.text = contenido;
            texto.fontSize = 22;
            texto.color = Color.white;
            texto.alignment = ancla;
            texto.horizontalOverflow = HorizontalWrapMode.Overflow;
            texto.verticalOverflow = VerticalWrapMode.Overflow;

            var rt = texto.rectTransform;
            if (ancla == TextAnchor.UpperLeft)
            {
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
            }
            else if (ancla == TextAnchor.LowerCenter)
            {
                rt.anchorMin = new Vector2(0.5f, 0f);
                rt.anchorMax = new Vector2(0.5f, 0f);
                rt.pivot = new Vector2(0.5f, 0f);
            }
            rt.anchoredPosition = posicion;
            rt.sizeDelta = new Vector2(600, 40);
            return texto;
        }

        private void BuscarReferenciasJugador()
        {
            var jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador == null) return;
            salud = jugador.GetComponent<PlayerHealth>();
            powerUp = jugador.GetComponent<PowerUpManager>();
            debuff = jugador.GetComponent<DebuffManager>();
            interaccion = jugador.GetComponent<InteractionSystem>();
        }

        private void ActualizarDestellos(int valor)
        {
            if (textoDestellos != null) textoDestellos.text = "✦ " + valor;
        }

        private void Update()
        {
            if (salud == null) BuscarReferenciasJugador();
            if (salud != null && textoVida != null)
                textoVida.text = "♥ " + salud.Actuales + "/" + salud.Maximo;

            if (textoPowerUp != null)
            {
                if (powerUp != null && powerUp.Activo != null)
                    textoPowerUp.text = "★ " + powerUp.Activo.nombrePowerUp + "  " + powerUp.Restante.ToString("F1") + "s";
                else textoPowerUp.text = "";
            }

            if (textoDebuff != null)
            {
                if (debuff != null && debuff.Activo != null)
                    textoDebuff.text = "<color=#ff6666>⚠ " + debuff.Activo.nombreDebuff + "  " + debuff.Restante.ToString("F1") + "s</color>";
                else textoDebuff.text = "";
            }

            if (textoIndicacion != null)
            {
                if (interaccion != null && interaccion.TextoActual != null)
                    textoIndicacion.text = "[E] " + interaccion.TextoActual;
                else textoIndicacion.text = "";
            }
        }
    }
}
