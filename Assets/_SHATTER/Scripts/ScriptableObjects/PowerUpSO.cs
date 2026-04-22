using UnityEngine;

namespace Shatter.ScriptableObjects
{
    public enum TipoPowerUp
    {
        LatidoCalmado,      // Ralentiza enemigos en radio
        DestelloGuia,       // Revela coleccionables
        EscudoRespiracion,  // Ignora 1 golpe
        MemoriaClara,       // +velocidad
        FocoClaridad        // +salto
    }

    [CreateAssetMenu(menuName = "Shatter/Power Up", fileName = "PowerUp")]
    public class PowerUpSO : ScriptableObject
    {
        [Header("Metadatos")]
        public string nombrePowerUp;
        [TextArea] public string descripcion;
        public Sprite icono;
        public Color colorTinte = Color.white;

        [Header("Tipo / Duracion")]
        public TipoPowerUp tipo;
        public float duracion = 10f;

        [Header("Parametros numericos")]
        public float radio = 6f;
        public float magnitud = 0.5f; // slowdown %, speed boost %, etc.

        [Header("Audio / VFX")]
        public AudioClip sonidoActivacion;
        public GameObject prefabVfxActivacion;
    }
}
