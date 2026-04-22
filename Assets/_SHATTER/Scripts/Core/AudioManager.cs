using UnityEngine;

namespace Shatter.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Fuentes de audio")]
        [SerializeField] private AudioSource fuenteMusica;
        [SerializeField] private AudioSource fuenteEfectos;
        [SerializeField] private AudioSource fuenteAmbiente;

        [Header("Volumenes (0-1)")]
        [Range(0f, 1f)] public float volumenMaestro = 1f;
        [Range(0f, 1f)] public float volumenMusica = 0.8f;
        [Range(0f, 1f)] public float volumenEfectos = 1f;
        [Range(0f, 1f)] public float volumenAmbiente = 0.6f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fuenteMusica == null) fuenteMusica = CrearFuente("FuenteMusica", true);
            if (fuenteEfectos == null) fuenteEfectos = CrearFuente("FuenteEfectos", false);
            if (fuenteAmbiente == null) fuenteAmbiente = CrearFuente("FuenteAmbiente", true);
        }

        private AudioSource CrearFuente(string nombre, bool repetir)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(transform);
            var fuente = go.AddComponent<AudioSource>();
            fuente.loop = repetir;
            fuente.playOnAwake = false;
            return fuente;
        }

        public void ReproducirMusica(AudioClip clip)
        {
            if (clip == null || fuenteMusica == null) return;
            if (fuenteMusica.clip == clip && fuenteMusica.isPlaying) return;
            fuenteMusica.clip = clip;
            fuenteMusica.volume = volumenMaestro * volumenMusica;
            fuenteMusica.Play();
        }

        public void DetenerMusica() { if (fuenteMusica != null) fuenteMusica.Stop(); }

        public void ReproducirEfecto(AudioClip clip, float escalaVolumen = 1f)
        {
            if (clip == null || fuenteEfectos == null) return;
            fuenteEfectos.PlayOneShot(clip, volumenMaestro * volumenEfectos * escalaVolumen);
        }

        public void ReproducirAmbiente(AudioClip clip)
        {
            if (clip == null || fuenteAmbiente == null) return;
            if (fuenteAmbiente.clip == clip && fuenteAmbiente.isPlaying) return;
            fuenteAmbiente.clip = clip;
            fuenteAmbiente.volume = volumenMaestro * volumenAmbiente;
            fuenteAmbiente.Play();
        }

        public void DetenerAmbiente() { if (fuenteAmbiente != null) fuenteAmbiente.Stop(); }

        public void AplicarVolumenes()
        {
            if (fuenteMusica != null) fuenteMusica.volume = volumenMaestro * volumenMusica;
            if (fuenteAmbiente != null) fuenteAmbiente.volume = volumenMaestro * volumenAmbiente;
        }
    }
}
