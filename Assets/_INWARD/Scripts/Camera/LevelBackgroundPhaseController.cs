using System;
using UnityEngine;

namespace Shatter.CameraSystem
{
    /// <summary>
    /// Fades background phase containers according to the horizontal position of a target.
    /// Useful for long 2D levels that change mood by section.
    /// </summary>
    [ExecuteAlways]
    public class LevelBackgroundPhaseController : MonoBehaviour
    {
        [Serializable]
        private class BackgroundPhase
        {
            public string nombre;
            public GameObject contenedor;
            public float xInicioCompleto;
            public float xFinCompleto;
            public float distanciaFade = 30f;

            [NonSerialized] public SpriteRenderer[] renderers = Array.Empty<SpriteRenderer>();
            [NonSerialized] public Color[] coloresOriginales = Array.Empty<Color>();
        }

        [Header("Referencia")]
        [SerializeField] private Transform objetivo;

        [Header("Fases")]
        [SerializeField] private BackgroundPhase[] fases = Array.Empty<BackgroundPhase>();

        [Header("Ajustes")]
        [SerializeField] private bool ocultarRenderersInvisibles = true;

        private void OnEnable()
        {
            ResolverObjetivo();
            ReconstruirCache();
            AplicarFases();
        }

        private void OnValidate()
        {
            ReconstruirCache();
            AplicarFases();
        }

        private void LateUpdate()
        {
            ResolverObjetivo();
            AplicarFases();
        }

        private void ResolverObjetivo()
        {
            if (objetivo != null) return;

            Camera camaraPrincipal = Camera.main;
            if (camaraPrincipal != null)
            {
                objetivo = camaraPrincipal.transform;
            }
        }

        private void ReconstruirCache()
        {
            if (fases == null) return;

            foreach (BackgroundPhase fase in fases)
            {
                if (fase == null || fase.contenedor == null)
                {
                    continue;
                }

                fase.renderers = fase.contenedor.GetComponentsInChildren<SpriteRenderer>(true);
                fase.coloresOriginales = new Color[fase.renderers.Length];

                for (int i = 0; i < fase.renderers.Length; i++)
                {
                    fase.coloresOriginales[i] = fase.renderers[i].color;
                    if (fase.coloresOriginales[i].a <= 0.001f)
                    {
                        fase.coloresOriginales[i].a = 1f;
                    }
                }
            }
        }

        private void AplicarFases()
        {
            if (objetivo == null || fases == null) return;

            float x = objetivo.position.x;
            foreach (BackgroundPhase fase in fases)
            {
                if (fase == null || fase.renderers == null) continue;

                float alpha = CalcularAlpha(fase, x);
                for (int i = 0; i < fase.renderers.Length; i++)
                {
                    SpriteRenderer renderer = fase.renderers[i];
                    if (renderer == null) continue;

                    Color baseColor = i < fase.coloresOriginales.Length ? fase.coloresOriginales[i] : Color.white;
                    baseColor.a *= alpha;
                    renderer.color = baseColor;

                    if (ocultarRenderersInvisibles)
                    {
                        renderer.enabled = alpha > 0.001f;
                    }
                }
            }
        }

        private static float CalcularAlpha(BackgroundPhase fase, float x)
        {
            float fade = Mathf.Max(0.001f, fase.distanciaFade);

            if (x < fase.xInicioCompleto)
            {
                return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(fase.xInicioCompleto - fade, fase.xInicioCompleto, x));
            }

            if (x > fase.xFinCompleto)
            {
                return Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(fase.xFinCompleto, fase.xFinCompleto + fade, x));
            }

            return 1f;
        }
    }
}
