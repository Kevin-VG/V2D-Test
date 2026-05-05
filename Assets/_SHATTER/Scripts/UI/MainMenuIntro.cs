using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Shatter.UI
{
    /// <summary>
    /// Script que maneja la transición de la pantalla de inicio al menú principal.
    /// </summary>
    public class MainMenuIntro : MonoBehaviour
    {
        [Header("Elementos UI")]
        [Tooltip("La imagen de fondo (debe ocupar toda la pantalla)")]
        [SerializeField] private Image backgroundImage;
        
        [Tooltip("El texto del título 'Inward'")]
        [SerializeField] private TMP_Text titleText;
        
        [Tooltip("El texto del subtítulo '(toque cualquier letra)'")]
        [SerializeField] private TMP_Text subtitleText;
        
        [Tooltip("El GameObject que contiene los botones del menú (PLAY, SETTING, CREDIT, EXIT)")]
        [SerializeField] private GameObject menuOptionsContainer;
        
        [Tooltip("El Canvas Group del contenedor de botones (para hacer que aparezcan gradualmente)")]
        [SerializeField] private CanvasGroup menuOptionsCanvasGroup;

        [Header("Configuración")]
        [Tooltip("Duración de la transición en segundos")]
        [SerializeField] private float fadeDuration = 1.5f;

        [Header("Audio")]
        [Tooltip("Audio Source para reproducir el sonido de transición")]
        [SerializeField] private AudioSource audioSource;
        
        [Tooltip("Efecto de sonido al iniciar la transición")]
        [SerializeField] private AudioClip transitionSound;

        [Header("Fuentes")]
        [Tooltip("Fuente inicial (cuando el fondo es negro)")]
        [SerializeField] private TMP_FontAsset initialFont;

        [Tooltip("Tamaño de la fuente inicial")]
        [SerializeField] private float initialFontSize = 100f;

        [Tooltip("Fuente final (cuando el fondo pasa a blanco)")]
        [SerializeField] private TMP_FontAsset targetFont;

        [Tooltip("Tamaño de la fuente final")]
        [SerializeField] private float targetFontSize = 100f;

        private bool hasPressedKey = false;

        private void Start()
        {
            // Estado inicial: Fondo negro, textos blancos, menú invisible pero activo
            if (backgroundImage != null) backgroundImage.color = Color.black;
            if (titleText != null) 
            {
                titleText.color = Color.white;
                if (initialFont != null) titleText.font = initialFont;
                titleText.fontSize = initialFontSize;
            }
            if (subtitleText != null) subtitleText.color = Color.white;
            
            // Asegurarse de que el contenedor esté activo pero invisible mediante el CanvasGroup
            if (menuOptionsContainer != null)
                menuOptionsContainer.SetActive(true);
                
            if (menuOptionsCanvasGroup != null)
            {
                menuOptionsCanvasGroup.alpha = 0f;
                menuOptionsCanvasGroup.interactable = false;
                menuOptionsCanvasGroup.blocksRaycasts = false;
            }
        }

        private void Update()
        {
            // Detectar cualquier tecla o botón del mouse presionado
            if (!hasPressedKey && Input.anyKeyDown)
            {
                hasPressedKey = true;
                
                // Reproducir el sonido de transición
                if (audioSource != null && transitionSound != null)
                {
                    audioSource.PlayOneShot(transitionSound);
                }
                
                StartCoroutine(TransitionRoutine());
            }
            else if (hasPressedKey && Input.GetKeyDown(KeyCode.Escape))
            {
                // Permite regresar entre pantallas con la tecla Escape
                SettingsMenu[] menusAjustes = Resources.FindObjectsOfTypeAll<SettingsMenu>();
                foreach (var menu in menusAjustes)
                {
                    if (menu.gameObject.scene.isLoaded)
                    {
                        if (menu.controlsPanel != null && menu.controlsPanel.activeSelf)
                        {
                            menu.CloseControls();
                            return;
                        }
                        else if (menu.settingsPanel != null && menu.settingsPanel.activeSelf)
                        {
                            menu.CloseSettings();
                            return;
                        }
                    }
                }
            }
        }

        private IEnumerator TransitionRoutine()
        {
            // Ocultar el subtítulo inmediatamente (o se podría hacer un fade también)
            if (subtitleText != null)
                subtitleText.gameObject.SetActive(false);

            float elapsedTime = 0f;
            Color initialBgColor = Color.black;
            Color targetBgColor = Color.white;

            Color initialTitleColor = Color.white;
            Color targetTitleColor = Color.black;

            bool fontChanged = false;

            // Transición suave de colores (Fade) y aparición de los botones
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);

                // Curva suave para la transición (opcional, hace que se vea más orgánico)
                float smoothT = Mathf.SmoothStep(0, 1, t);

                if (backgroundImage != null) 
                    backgroundImage.color = Color.Lerp(initialBgColor, targetBgColor, smoothT);
                
                if (titleText != null) 
                {
                    titleText.color = Color.Lerp(initialTitleColor, targetTitleColor, smoothT);
                    
                    // Cambiar la fuente a la mitad de la transición
                    if (!fontChanged && t >= 0.5f)
                    {
                        if (targetFont != null) titleText.font = targetFont;
                        titleText.fontSize = targetFontSize;
                        fontChanged = true;
                    }
                }

                // Aparecer los botones gradualmente
                if (menuOptionsCanvasGroup != null)
                    menuOptionsCanvasGroup.alpha = smoothT;

                yield return null;
            }

            // Asegurar los colores finales y la visibilidad de los botones
            if (backgroundImage != null) backgroundImage.color = targetBgColor;
            if (titleText != null)
            {
                titleText.color = targetTitleColor;
                if (targetFont != null) titleText.font = targetFont;
                titleText.fontSize = targetFontSize;
            }

            if (menuOptionsCanvasGroup != null)
            {
                menuOptionsCanvasGroup.alpha = 1f;
                menuOptionsCanvasGroup.interactable = true;
                menuOptionsCanvasGroup.blocksRaycasts = true;
            }
        }

        // --- Métodos para los botones (Asignar en el evento OnClick() de cada Botón en el Inspector) ---

        public void OnPlayClicked()
        {
            Debug.Log("Iniciando el juego... Preparando GameManager");
            if (Shatter.Core.GameManager.Instance != null)
            {
                Shatter.Core.GameManager.Instance.CargarNivel(1);
            }
            else
            {
                SceneManager.LoadScene("Nivel_01");
            }
        }

        public void OnSettingsClicked()
        {
            Debug.Log("Abriendo configuraciones...");
            // TODO: Mostrar panel de configuraciones
        }

        public void OnCreditsClicked()
        {
            Debug.Log("Mostrando créditos...");
            // TODO: Mostrar panel de créditos
        }

        public void OnExitClicked()
        {
            Debug.Log("Saliendo del juego...");
            Application.Quit();
            
            #if UNITY_EDITOR
            // Esto permite detener el juego en el editor de Unity
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
