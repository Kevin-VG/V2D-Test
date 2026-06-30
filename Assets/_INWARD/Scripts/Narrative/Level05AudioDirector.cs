using System.Collections;
using UnityEngine;

namespace Shatter.Narrative
{
    public class Level05AudioDirector : MonoBehaviour
    {
        [SerializeField] private AudioClip ambientLoop;
        [SerializeField] private AudioClip ecoCue;
        [SerializeField] private AudioClip finalSwell;
        [SerializeField, Range(0f, 1f)] private float ambientVolume = 0.28f;
        [SerializeField, Range(0f, 1f)] private float dialogueAmbientVolume = 0.16f;
        [SerializeField, Range(0f, 1f)] private float cueVolume = 0.55f;
        [SerializeField, Range(0f, 1f)] private float finalVolume = 0.48f;
        [SerializeField] private float fadeTime = 0.7f;

        private AudioSource ambientSource;
        private AudioSource cueSource;
        private AudioSource finalSource;
        private Coroutine fadeRoutine;
        private bool ambientStarted;

        private void Awake()
        {
            EnsureSources();
        }

        private void Start()
        {
            StartAmbientIfPossible();
        }

        private void OnEnable()
        {
            ambientStarted = false;
        }

        private void Update()
        {
            if (!ambientStarted)
            {
                StartAmbientIfPossible();
            }
        }

        private void StartAmbientIfPossible()
        {
            EnsureSources();

            if (ambientLoop == null || ambientSource == null || ambientStarted)
            {
                return;
            }

            ambientSource.clip = ambientLoop;
            ambientSource.volume = ambientVolume;
            ambientSource.loop = true;
            ambientSource.Play();

            if (!ambientSource.isPlaying)
            {
                Destroy(ambientSource.gameObject);
                ambientSource = CreateSource("Nivel5_AmbientSource", true);
                ambientSource.clip = ambientLoop;
                ambientSource.volume = ambientVolume;
                ambientSource.loop = true;
                ambientSource.Play();
            }

            ambientStarted = ambientSource.isPlaying;
        }

        public void PlayEcoMoment(float duration)
        {
            EnsureSources();

            if (cueSource != null && ecoCue != null)
            {
                cueSource.PlayOneShot(ecoCue, cueVolume);
            }

            FadeAmbient(dialogueAmbientVolume);
            CancelInvoke(nameof(RestoreAmbient));
            Invoke(nameof(RestoreAmbient), Mathf.Max(1f, duration));
        }

        public void PlayFinalMoment()
        {
            EnsureSources();

            FadeAmbient(0.08f);
            if (finalSource != null && finalSwell != null)
            {
                finalSource.PlayOneShot(finalSwell, finalVolume);
            }
        }

        public void RestoreAmbient()
        {
            FadeAmbient(ambientVolume);
        }

        private void FadeAmbient(float targetVolume)
        {
            EnsureSources();

            if (ambientSource == null)
            {
                return;
            }

            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = StartCoroutine(FadeAmbientRoutine(targetVolume));
        }

        private IEnumerator FadeAmbientRoutine(float targetVolume)
        {
            float startVolume = ambientSource.volume;
            for (float t = 0f; t < fadeTime; t += Time.deltaTime)
            {
                ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, t / fadeTime);
                yield return null;
            }

            ambientSource.volume = targetVolume;
            fadeRoutine = null;
        }

        private AudioSource CreateSource(string sourceName, bool loop)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform, false);
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.dopplerLevel = 0f;
            return source;
        }

        private void EnsureSources()
        {
            if (ambientSource == null)
            {
                ambientSource = FindOrCreateSource("Nivel5_AmbientSource", true);
            }

            if (cueSource == null)
            {
                cueSource = FindOrCreateSource("Nivel5_EcoCueSource", false);
            }

            if (finalSource == null)
            {
                finalSource = FindOrCreateSource("Nivel5_FinalSource", false);
            }
        }

        private AudioSource FindOrCreateSource(string sourceName, bool loop)
        {
            Transform existing = transform.Find(sourceName);
            if (existing != null && existing.TryGetComponent(out AudioSource source))
            {
                source.loop = loop;
                return source;
            }

            return CreateSource(sourceName, loop);
        }
    }
}
