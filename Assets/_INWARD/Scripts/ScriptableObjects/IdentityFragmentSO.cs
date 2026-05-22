using UnityEngine;

namespace Shatter.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Shatter/Identity Fragment", fileName = "IdentityFragment")]
    public class IdentityFragmentSO : ScriptableObject
    {
        [Header("Metadatos")]
        public string nombreFragmento = "Nuevo Fragmento";
        [TextArea] public string descripcion;
        public Sprite icono;

        [Header("Visual")]
        public Color colorTinte = Color.white;
        public GameObject prefabParticula;

        [Header("Efectos")]
        [Tooltip("Multiplicador de velocidad horizontal (1 = sin cambio)")]
        public float multiplicadorVelocidad = 1f;
        [Tooltip("Saltos extra adicionales al doble salto base")]
        public int saltosExtra = 0;
        [Tooltip("Reduccion de dano recibido (0..1)")]
        [Range(0f, 0.75f)] public float reduccionDano = 0f;
    }
}
