using System.Collections.Generic;
using UnityEngine;
using Shatter.Core;
using Shatter.ScriptableObjects;

namespace Shatter.Player
{
    /// <summary>
    /// Gestor de Fragmentos de Identidad equipados. Aplica efectos al player.
    /// Requisito UPN: Personalizacion del personaje.
    /// </summary>
    public class IdentityManager : MonoBehaviour
    {
        [SerializeField] private int maximoEquipado = 3;
        [SerializeField] private SpriteRenderer renderizadorObjetivo;
        [SerializeField] private Transform anclaParticulas;

        [SerializeField] private List<IdentityFragmentSO> descubiertos = new List<IdentityFragmentSO>();
        private readonly List<IdentityFragmentSO> equipados = new List<IdentityFragmentSO>();
        private readonly List<GameObject> particulasGeneradas = new List<GameObject>();

        private PlayerController2D controlador;
        private PlayerHealth salud;

        public IReadOnlyList<IdentityFragmentSO> Descubiertos => descubiertos;
        public IReadOnlyList<IdentityFragmentSO> Equipados => equipados;
        public int MaximoEquipado => maximoEquipado;

        private void Awake()
        {
            controlador = GetComponent<PlayerController2D>();
            salud = GetComponent<PlayerHealth>();
            if (renderizadorObjetivo == null) renderizadorObjetivo = GetComponentInChildren<SpriteRenderer>();
            if (anclaParticulas == null) anclaParticulas = transform;
        }

        public void Descubrir(IdentityFragmentSO fragmento)
        {
            if (fragmento == null || descubiertos.Contains(fragmento)) return;
            descubiertos.Add(fragmento);
        }

        public bool EstaEquipado(IdentityFragmentSO fragmento) => equipados.Contains(fragmento);

        public bool Equipar(IdentityFragmentSO fragmento)
        {
            if (fragmento == null || EstaEquipado(fragmento)) return false;
            if (equipados.Count >= maximoEquipado) return false;
            equipados.Add(fragmento);
            RecalcularEfectos();
            GameEvents.LanzarFragmentoEquipado(fragmento.nombreFragmento);
            return true;
        }

        public void Desequipar(IdentityFragmentSO fragmento)
        {
            if (fragmento == null || !EstaEquipado(fragmento)) return;
            equipados.Remove(fragmento);
            RecalcularEfectos();
            GameEvents.LanzarFragmentoDesequipado(fragmento.nombreFragmento);
        }

        private void RecalcularEfectos()
        {
            // Limpiar particulas previas
            foreach (var p in particulasGeneradas) if (p != null) Destroy(p);
            particulasGeneradas.Clear();

            float mulVelocidad = 1f;
            int saltosExtraBono = 0;
            float redDano = 0f;
            Color tinte = Color.white;
            int contador = 0;

            foreach (var f in equipados)
            {
                mulVelocidad *= f.multiplicadorVelocidad;
                saltosExtraBono += f.saltosExtra;
                redDano += f.reduccionDano;
                tinte = (Color)((Vector4)tinte + (Vector4)f.colorTinte); contador++;

                if (f.prefabParticula != null && anclaParticulas != null)
                {
                    var p = Instantiate(f.prefabParticula, anclaParticulas.position, Quaternion.identity, anclaParticulas);
                    particulasGeneradas.Add(p);
                }
            }

            if (contador > 0) tinte = new Color(tinte.r / contador, tinte.g / contador, tinte.b / contador, 1f);
            else tinte = Color.white;

            if (renderizadorObjetivo != null) renderizadorObjetivo.color = tinte;
            if (controlador != null)
            {
                controlador.AplicarMultiplicadorVelocidad(mulVelocidad);
                controlador.EstablecerSaltosExtra(1 + saltosExtraBono); // base 1 + bonus
            }
            if (salud != null) salud.EstablecerReduccionDano(Mathf.Clamp01(redDano));
        }
    }
}
