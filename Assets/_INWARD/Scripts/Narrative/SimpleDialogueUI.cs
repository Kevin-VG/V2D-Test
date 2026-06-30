using System.Collections;
using Shatter.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Shatter.Narrative
{
    public class SimpleDialogueUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image moodOverlay;
        [SerializeField] private Image panel;
        [SerializeField] private Text speakerText;
        [SerializeField] private Text bodyText;
        [SerializeField] private float defaultDuration = 4.5f;
        [SerializeField] private bool freezePlayerDuringDialogue = true;
        [SerializeField] private Vector2 panelMargin = new Vector2(44f, 28f);
        [SerializeField] private float panelHeight = 150f;
        [SerializeField] private Image finalPanel;
        [SerializeField] private Text finalTitleText;
        [SerializeField] private Text finalBodyText;
        [SerializeField] private float finalFadeDuration = 4.4f;
        [SerializeField] private float finalTextFadeDelay = 1.85f;
        [SerializeField] private float finalHoldPulseDuration = 4f;
        [SerializeField] private Image introPanel;
        [SerializeField] private Text introTitleText;
        [SerializeField] private Text introBodyText;

        private Coroutine activeRoutine;
        private Coroutine overlayRoutine;
        private Coroutine panelPulseRoutine;
        private Coroutine finalRoutine;
        private Coroutine introRoutine;
        private PlayerController2D frozenPlayer;

        public bool IsShowing =>
            (panel != null && panel.gameObject.activeSelf)
            || (introPanel != null && introPanel.gameObject.activeSelf)
            || (finalPanel != null && finalPanel.gameObject.activeSelf);

        private void Awake()
        {
            EnsureUI();
            HideImmediate();
        }

        public void Show(string speaker, string body, float duration = -1f, PlayerController2D playerToFreeze = null)
        {
            EnsureUI();

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(ShowRoutine(speaker, body, duration > 0f ? duration : defaultDuration, playerToFreeze));
        }

        public void HideImmediate()
        {
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
            }

            if (moodOverlay != null)
            {
                moodOverlay.gameObject.SetActive(false);
            }

            if (overlayRoutine != null)
            {
                StopCoroutine(overlayRoutine);
                overlayRoutine = null;
            }

            if (panelPulseRoutine != null)
            {
                StopCoroutine(panelPulseRoutine);
                panelPulseRoutine = null;
            }

            if (finalRoutine != null)
            {
                StopCoroutine(finalRoutine);
                finalRoutine = null;
            }

            if (introRoutine != null)
            {
                StopCoroutine(introRoutine);
                introRoutine = null;
            }

            if (panel != null)
            {
                panel.rectTransform.localScale = Vector3.one;
            }

            if (introPanel != null)
            {
                introPanel.gameObject.SetActive(false);
            }

            if (frozenPlayer != null)
            {
                frozenPlayer.ReanudarMovimiento();
                frozenPlayer = null;
            }
        }

        public void ShowFinalScreen(string title, string body, PlayerController2D playerToFreeze = null)
        {
            EnsureUI();

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                activeRoutine = null;
            }

            HideImmediate();

            if (playerToFreeze != null)
            {
                frozenPlayer = playerToFreeze;
                frozenPlayer.DetenerMovimiento();
            }

            finalTitleText.text = title;
            finalBodyText.text = body;
            finalRoutine = StartCoroutine(ShowFinalRoutine());
        }

        public void ShowLevelIntro(string title, string body, float fadeIn, float hold, float fadeOut, PlayerController2D playerToFreeze = null)
        {
            EnsureUI();
            HideImmediate();

            if (playerToFreeze != null)
            {
                frozenPlayer = playerToFreeze;
                frozenPlayer.DetenerMovimiento();
            }

            introTitleText.text = title;
            introBodyText.text = body;
            introRoutine = StartCoroutine(ShowLevelIntroRoutine(fadeIn, hold, fadeOut));
        }

        private IEnumerator ShowRoutine(string speaker, string body, float duration, PlayerController2D playerToFreeze)
        {
            if (freezePlayerDuringDialogue && playerToFreeze != null)
            {
                frozenPlayer = playerToFreeze;
                frozenPlayer.DetenerMovimiento();
            }

            speakerText.text = speaker;
            bodyText.text = body;
            overlayRoutine = StartCoroutine(PulseMoodOverlay(duration));
            panelPulseRoutine = StartCoroutine(PulsePanel(duration));
            panel.gameObject.SetActive(true);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
                {
                    break;
                }

                yield return null;
            }

            HideImmediate();
            activeRoutine = null;
        }

        private void EnsureUI()
        {
            if (canvas == null)
            {
                canvas = GetComponentInChildren<Canvas>();
            }

            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("DialogueCanvas_N5");
                canvasObject.transform.SetParent(transform, false);
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 40;
                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1280f, 720f);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            if (moodOverlay == null)
            {
                GameObject overlayObject = new GameObject("MemoryMoodOverlay");
                overlayObject.transform.SetParent(canvas.transform, false);
                moodOverlay = overlayObject.AddComponent<Image>();
                moodOverlay.color = new Color(1f, 0.72f, 0.24f, 0f);
                moodOverlay.raycastTarget = false;

                RectTransform rect = moodOverlay.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                overlayObject.SetActive(false);
            }

            if (panel == null)
            {
                GameObject panelObject = new GameObject("DialoguePanel");
                panelObject.transform.SetParent(canvas.transform, false);
                panel = panelObject.AddComponent<Image>();
                panel.color = new Color(0.05f, 0.06f, 0.04f, 0.78f);

                RectTransform rect = panel.rectTransform;
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.offsetMin = new Vector2(panelMargin.x, panelMargin.y);
                rect.offsetMax = new Vector2(-panelMargin.x, panelMargin.y + panelHeight);
            }

            if (speakerText == null)
            {
                speakerText = CreateText("Speaker", panel.transform, 22, FontStyle.Bold, new Color(1f, 0.78f, 0.28f, 1f));
                RectTransform rect = speakerText.rectTransform;
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.offsetMin = new Vector2(28f, -42f);
                rect.offsetMax = new Vector2(-28f, -12f);
            }

            if (bodyText == null)
            {
                bodyText = CreateText("Body", panel.transform, 26, FontStyle.Normal, new Color(1f, 0.96f, 0.82f, 1f));
                RectTransform rect = bodyText.rectTransform;
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.offsetMin = new Vector2(28f, 18f);
                rect.offsetMax = new Vector2(-28f, -50f);
            }

            if (finalPanel == null)
            {
                GameObject finalObject = new GameObject("FinalScreen");
                finalObject.transform.SetParent(canvas.transform, false);
                finalPanel = finalObject.AddComponent<Image>();
                finalPanel.color = new Color(0f, 0f, 0f, 0f);

                RectTransform rect = finalPanel.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                finalTitleText = CreateText("FinalTitle", finalObject.transform, 48, FontStyle.Bold, new Color(1f, 0.84f, 0.38f, 1f));
                finalTitleText.alignment = TextAnchor.MiddleCenter;
                RectTransform titleRect = finalTitleText.rectTransform;
                titleRect.anchorMin = new Vector2(0.12f, 0.52f);
                titleRect.anchorMax = new Vector2(0.88f, 0.68f);
                titleRect.offsetMin = Vector2.zero;
                titleRect.offsetMax = Vector2.zero;

                finalBodyText = CreateText("FinalBody", finalObject.transform, 26, FontStyle.Normal, new Color(1f, 0.95f, 0.8f, 1f));
                finalBodyText.alignment = TextAnchor.MiddleCenter;
                RectTransform bodyRect = finalBodyText.rectTransform;
                bodyRect.anchorMin = new Vector2(0.16f, 0.34f);
                bodyRect.anchorMax = new Vector2(0.84f, 0.5f);
                bodyRect.offsetMin = Vector2.zero;
                bodyRect.offsetMax = Vector2.zero;

                finalObject.SetActive(false);
            }

            if (introPanel == null)
            {
                GameObject introObject = new GameObject("LevelIntroScreen");
                introObject.transform.SetParent(canvas.transform, false);
                introPanel = introObject.AddComponent<Image>();
                introPanel.color = new Color(0f, 0f, 0f, 0f);

                RectTransform rect = introPanel.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                introTitleText = CreateText("IntroTitle", introObject.transform, 42, FontStyle.Bold, new Color(1f, 0.86f, 0.48f, 1f));
                introTitleText.alignment = TextAnchor.MiddleCenter;
                RectTransform titleRect = introTitleText.rectTransform;
                titleRect.anchorMin = new Vector2(0.12f, 0.53f);
                titleRect.anchorMax = new Vector2(0.88f, 0.66f);
                titleRect.offsetMin = Vector2.zero;
                titleRect.offsetMax = Vector2.zero;

                introBodyText = CreateText("IntroBody", introObject.transform, 24, FontStyle.Normal, new Color(1f, 0.95f, 0.82f, 1f));
                introBodyText.alignment = TextAnchor.MiddleCenter;
                RectTransform bodyRect = introBodyText.rectTransform;
                bodyRect.anchorMin = new Vector2(0.18f, 0.41f);
                bodyRect.anchorMax = new Vector2(0.82f, 0.52f);
                bodyRect.offsetMin = Vector2.zero;
                bodyRect.offsetMax = Vector2.zero;

                introObject.SetActive(false);
            }
        }

        private IEnumerator ShowLevelIntroRoutine(float fadeIn, float hold, float fadeOut)
        {
            introPanel.gameObject.SetActive(true);

            Color panelColor = introPanel.color;
            Color titleColor = introTitleText.color;
            Color bodyColor = introBodyText.color;
            panelColor.a = 1f;
            titleColor.a = 0f;
            bodyColor.a = 0f;
            introPanel.color = panelColor;
            introTitleText.color = titleColor;
            introBodyText.color = bodyColor;

            float textFade = Mathf.Max(0.1f, fadeIn);
            for (float t = 0f; t < textFade; t += Time.deltaTime)
            {
                float a = Mathf.SmoothStep(0f, 1f, t / textFade);
                titleColor.a = a;
                bodyColor.a = a;
                introTitleText.color = titleColor;
                introBodyText.color = bodyColor;
                yield return null;
            }

            titleColor.a = 1f;
            bodyColor.a = 1f;
            introTitleText.color = titleColor;
            introBodyText.color = bodyColor;

            for (float t = 0f; t < hold; t += Time.deltaTime)
            {
                float pulse = 1f + Mathf.Sin(t * 1.8f) * 0.01f;
                introTitleText.rectTransform.localScale = new Vector3(pulse, pulse, 1f);
                yield return null;
            }

            introTitleText.rectTransform.localScale = Vector3.one;
            for (float t = 0f; t < fadeOut; t += Time.deltaTime)
            {
                float normalized = Mathf.Clamp01(t / fadeOut);
                float textAlpha = Mathf.SmoothStep(1f, 0f, normalized);
                titleColor.a = textAlpha;
                bodyColor.a = textAlpha;
                panelColor.a = Mathf.SmoothStep(1f, 0f, normalized);
                introTitleText.color = titleColor;
                introBodyText.color = bodyColor;
                introPanel.color = panelColor;
                yield return null;
            }

            introPanel.gameObject.SetActive(false);
            if (frozenPlayer != null)
            {
                frozenPlayer.ReanudarMovimiento();
                frozenPlayer = null;
            }

            introRoutine = null;
        }

        private IEnumerator ShowFinalRoutine()
        {
            finalPanel.gameObject.SetActive(true);

            Color panelColor = finalPanel.color;
            Color titleColor = finalTitleText.color;
            Color bodyColor = finalBodyText.color;
            titleColor.a = 0f;
            bodyColor.a = 0f;
            finalTitleText.color = titleColor;
            finalBodyText.color = bodyColor;

            for (float t = 0f; t < finalFadeDuration; t += Time.deltaTime)
            {
                float normalized = Mathf.Clamp01(t / finalFadeDuration);
                panelColor.a = Mathf.SmoothStep(0f, 1f, normalized);
                finalPanel.color = panelColor;

                if (t >= finalTextFadeDelay)
                {
                    float textT = Mathf.Clamp01((t - finalTextFadeDelay) / Mathf.Max(0.1f, finalFadeDuration - finalTextFadeDelay));
                    float textAlpha = Mathf.SmoothStep(0f, 1f, textT);
                    titleColor.a = textAlpha;
                    bodyColor.a = textAlpha;
                    finalTitleText.color = titleColor;
                    finalBodyText.color = bodyColor;
                }

                yield return null;
            }

            panelColor.a = 1f;
            titleColor.a = 1f;
            bodyColor.a = 1f;
            finalPanel.color = panelColor;
            finalTitleText.color = titleColor;
            finalBodyText.color = bodyColor;

            RectTransform titleRect = finalTitleText.rectTransform;
            for (float t = 0f; t < finalHoldPulseDuration; t += Time.deltaTime)
            {
                float pulse = 1f + Mathf.Sin(t * 1.7f) * 0.018f;
                titleRect.localScale = new Vector3(pulse, pulse, 1f);
                yield return null;
            }

            titleRect.localScale = Vector3.one;
            finalRoutine = null;
        }

        private IEnumerator PulseMoodOverlay(float duration)
        {
            if (moodOverlay == null)
            {
                yield break;
            }

            moodOverlay.gameObject.SetActive(true);
            Color color = moodOverlay.color;
            const float peakAlpha = 0.26f;
            const float holdAlpha = 0.09f;
            const float fadeIn = 0.26f;
            const float fadeOut = 0.55f;

            for (float t = 0f; t < fadeIn; t += Time.deltaTime)
            {
                color.a = Mathf.Lerp(0f, peakAlpha, t / fadeIn);
                moodOverlay.color = color;
                yield return null;
            }

            float holdTime = Mathf.Max(0f, duration - fadeIn - fadeOut);
            for (float t = 0f; t < holdTime; t += Time.deltaTime)
            {
                color.a = holdAlpha + Mathf.Sin(Time.time * 2.2f) * 0.018f;
                moodOverlay.color = color;
                yield return null;
            }

            for (float t = 0f; t < fadeOut; t += Time.deltaTime)
            {
                color.a = Mathf.Lerp(holdAlpha, 0f, t / fadeOut);
                moodOverlay.color = color;
                yield return null;
            }

            color.a = 0f;
            moodOverlay.color = color;
            moodOverlay.gameObject.SetActive(false);
            overlayRoutine = null;
        }

        private IEnumerator PulsePanel(float duration)
        {
            if (panel == null)
            {
                yield break;
            }

            RectTransform rect = panel.rectTransform;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 3.4f) * 0.006f;
                rect.localScale = new Vector3(pulse, pulse, 1f);
                yield return null;
            }

            rect.localScale = Vector3.one;
            panelPulseRoutine = null;
        }

        private static Text CreateText(string name, Transform parent, int size, FontStyle style, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null)
            {
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(14, size - 8);
            text.resizeTextMaxSize = size;
            text.lineSpacing = 1f;
            return text;
        }
    }
}
