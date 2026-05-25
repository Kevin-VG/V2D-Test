using System.Collections;
using UnityEngine;

namespace Shatter.CameraSystem
{
    /// <summary>
    /// Trigger 2D que realiza una transición suave (cross-fade) entre dos capas de fondo (Fase 1 y Fase 2)
    /// al momento de que el jugador cruza una zona del mapa.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BackgroundTransitionTrigger : MonoBehaviour
    {
        [Header("Capas de Fondo")]
        [Tooltip("El objeto contenedor de la Fase 1 (se desvanecerá hasta ser invisible).")]
        [SerializeField] private GameObject capaFase1;
        [Tooltip("El objeto contenedor de la Fase 2 (aparecerá desde invisible hasta opaco).")]
        [SerializeField] private GameObject capaFase2;

        [Header("Ajustes de Transición")]
        [Tooltip("Duración de la transición suave en segundos.")]
        [SerializeField] private float duracionTransicion = 2f;
        [Tooltip("¿Desactivar por completo el objeto de la Fase 1 al finalizar la transición para optimizar rendimiento?")]
        [SerializeField] private bool desactivarFase1AlTerminar = true;

        private SpriteRenderer[] spritesFase1;
        private SpriteRenderer[] spritesFase2;
        private bool transicionIniciada = false;

        private void Start()
        {
            // Nos aseguramos de que el BoxCollider2D de este Trigger esté bien configurado
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;

            // Obtenemos todos los SpriteRenderers que estén dentro de las jerarquías de cada Fase
            if (capaFase1 != null) spritesFase1 = capaFase1.GetComponentsInChildren<SpriteRenderer>();
            if (capaFase2 != null) spritesFase2 = capaFase2.GetComponentsInChildren<SpriteRenderer>();

            // Inicializamos la Fase 2 como completamente invisible al iniciar el nivel
            SetAlpha(spritesFase2, 0f);
            if (capaFase2 != null) capaFase2.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Imprime en consola CUALQUIER objeto que toque el trigger para diagnosticar la física
            Debug.Log($"[DEBUG] Algo tocó el trigger: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

            // Solo se activa si el jugador colisiona con el trigger
            if (!transicionIniciada && collision.CompareTag("Player"))
            {
                Debug.Log("[DEBUG] ¡Jugador detectado! Iniciando cross-fade de fondo.");
                StartCoroutine(EjecutarCrossFade());
            }
        }

        private IEnumerator EjecutarCrossFade()
        {
            transicionIniciada = true;

            // Activamos la Fase 2 para que empiece a recibir el fade-in
            if (capaFase2 != null) capaFase2.SetActive(true);

            float tiempoTranscurrido = 0f;

            // Guardamos los colores base de origen para respetar transparencias originales de cada sprite
            Color[] coloresOriginalesFase1 = ObtenerColoresOriginales(spritesFase1);
            Color[] coloresOriginalesFase2 = ObtenerColoresOriginales(spritesFase2);

            while (tiempoTranscurrido < duracionTransicion)
            {
                tiempoTranscurrido += Time.deltaTime;
                float porcentaje = Mathf.Clamp01(tiempoTranscurrido / duracionTransicion);

                // 1. Desvanecer la Fase 1 (de 100% a 0%)
                ModificarOpacidadLote(spritesFase1, coloresOriginalesFase1, 1f - porcentaje);

                // 2. Mostrar la Fase 2 (de 0% a 100%)
                ModificarOpacidadLote(spritesFase2, coloresOriginalesFase2, porcentaje);

                yield return null;
            }

            // Aseguramos los estados finales exactos al terminar la corrutina
            ModificarOpacidadLote(spritesFase1, coloresOriginalesFase1, 0f);
            ModificarOpacidadLote(spritesFase2, coloresOriginalesFase2, 1f);

            // Desactivar Fase 1 por completo para ahorrar llamadas de dibujo (draw calls)
            if (desactivarFase1AlTerminar && capaFase1 != null)
            {
                capaFase1.SetActive(false);
            }
        }

        private Color[] ObtenerColoresOriginales(SpriteRenderer[] sprites)
        {
            if (sprites == null) return new Color[0];
            Color[] colores = new Color[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                colores[i] = sprites[i].color;
            }
            return colores;
        }

        private void SetAlpha(SpriteRenderer[] sprites, float alpha)
        {
            if (sprites == null) return;
            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    Color color = sprite.color;
                    color.a = alpha;
                    sprite.color = color;
                }
            }
        }

        private void ModificarOpacidadLote(SpriteRenderer[] sprites, Color[] coloresOriginales, float factorOpacidad)
        {
            if (sprites == null) return;
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null && i < coloresOriginales.Length)
                {
                    Color colorDestino = coloresOriginales[i];
                    colorDestino.a = coloresOriginales[i].a * factorOpacidad;
                    sprites[i].color = colorDestino;
                }
            }
        }
    }
}
