using UnityEngine;

namespace Shatter.ScriptableObjects
{
    public enum TipoDebuff
    {
        VisionTunel,         // Vineta + lento
        PasosDePlomo,        // Salto reducido
        ControlesInvertidos  // A<->D
    }

    [CreateAssetMenu(menuName = "Shatter/Debuff", fileName = "Debuff")]
    public class DebuffSO : ScriptableObject
    {
        public string nombreDebuff;
        [TextArea] public string descripcion;
        public Sprite icono;
        public Color colorTinte = new Color(0.6f, 0.2f, 0.8f, 1f);

        public TipoDebuff tipo;
        public float duracion = 4f;

        [Range(0f, 1f)] public float multiplicadorVelocidad = 0.6f;  // usado por VisionTunel/Pasos
        [Range(0f, 1f)] public float multiplicadorSalto = 0.5f;      // usado por Pasos

        public AudioClip sonidoAplicacion;
    }
}
