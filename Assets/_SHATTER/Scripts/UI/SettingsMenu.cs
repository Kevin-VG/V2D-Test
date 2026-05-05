using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shatter.Core; // Para acceder al AudioManager

namespace Shatter.UI
{
    /// <summary>
    /// Script para gestionar el menú de ajustes (Settings).
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Contenedores UI")]
        [Tooltip("El panel principal que contiene los botones del menú")]
        public GameObject mainPanel;
        [Tooltip("El panel que contiene todas las opciones de ajustes")]
        public GameObject settingsPanel;
        [Tooltip("El panel que contiene los controles del juego")]
        public GameObject controlsPanel;

        [Header("Elementos de Ajustes")]
        public TMP_Dropdown languageDropdown;
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        public Toggle screenShakeToggle;
        public Slider masterVolumeSlider;
        public Slider volumeSlider; // Music Volume
        public Slider sfxVolumeSlider;

        [Header("Audio de UI")]
        [Tooltip("El AudioSource encargado de reproducir los efectos de sonido de la UI")]
        public AudioSource uiAudioSource;
        [Tooltip("Sonido que se reproduce al abrir un menú o panel")]
        public AudioClip openMenuSound;
        [Tooltip("Sonido que se reproduce al cerrar un menú o panel")]
        public AudioClip closeMenuSound;

        [Header("Textos para Traducir (Localización)")]
        [Tooltip("Textos de la Pantalla de Inicio")]
        public TMP_Text subtitleText;

        [Tooltip("Textos del Menú Principal")]
        public TMP_Text playBtnText;
        public TMP_Text settingsBtnText;
        public TMP_Text creditsBtnText;
        public TMP_Text exitBtnText;

        [Tooltip("Textos del Menú de Pausa")]
        public TMP_Text pauseTitleText;
        public TMP_Text resumeBtnText;
        public TMP_Text inventoryBtnText;
        public TMP_Text restartBtnText;

        [Tooltip("Textos del Inventario Emocional")]
        public TMP_Text inventoryTitleText;
        public TMP_Text inventorySubtitleText;
        public TMP_Text closeInventoryBtnText;
        
        [Tooltip("Textos del Menú de Ajustes")]
        public TMP_Text settingsTitleText;
        public TMP_Text languageLabelText;
        public TMP_Text resolutionLabelText;
        public TMP_Text fullscreenLabelText;
        public TMP_Text screenShakeLabelText;
        public TMP_Text masterVolumeLabelText;
        public TMP_Text volumeLabelText; // Music Label
        public TMP_Text sfxVolumeLabelText;
        public TMP_Text backBtnText;
        public TMP_Text openControlsBtnText;

        [Tooltip("Textos del Panel de Controles")]
        public TMP_Text controlsTitleText;
        public TMP_Text backFromControlsBtnText;

        private Resolution[] resolutions;

        private void Start()
        {
            // Ocultar panel de ajustes y controles al inicio
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);

            ConfigurarResoluciones();
            CargarAjustes();
        }

        private void ConfigurarResoluciones()
        {
            if (resolutionDropdown == null) return;

            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                // Formato: 1920 x 1080
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                // Verificar cuál es la resolución actual
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            
            // Cargar resolución preferida si existe, sino usar la actual
            int resIndex = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
            resolutionDropdown.value = resIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private void CargarAjustes()
        {
            // Pantalla completa
            if (fullscreenToggle != null)
            {
                bool isFullscreen = PlayerPrefs.GetInt("FullscreenPreference", Screen.fullScreen ? 1 : 0) == 1;
                fullscreenToggle.isOn = isFullscreen;
            }

            // Vibración de cámara
            if (screenShakeToggle != null)
            {
                bool shakeEnabled = PlayerPrefs.GetInt("ScreenShakePreference", 1) == 1;
                screenShakeToggle.isOn = shakeEnabled;
            }

            // Volumen Maestro
            if (masterVolumeSlider != null)
            {
                float masterVolume = PlayerPrefs.GetFloat("MasterVolumePreference", 1f);
                masterVolumeSlider.value = masterVolume;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.volumenMaestro = masterVolume;
                    AudioManager.Instance.AplicarVolumenes();
                }
            }

            // Volumen de la música
            if (volumeSlider != null)
            {
                float musicVolume = PlayerPrefs.GetFloat("MusicVolumePreference", 0.8f);
                volumeSlider.value = musicVolume;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.volumenMusica = musicVolume;
                    AudioManager.Instance.AplicarVolumenes();
                }
            }

            // Volumen SFX
            if (sfxVolumeSlider != null)
            {
                float sfxVolume = PlayerPrefs.GetFloat("SFXVolumePreference", 1f);
                sfxVolumeSlider.value = sfxVolume;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.volumenEfectos = sfxVolume;
                }
            }

            // Lenguaje (0 = Español, 1 = Inglés)
            if (languageDropdown != null)
            {
                int languageIndex = PlayerPrefs.GetInt("LanguagePreference", 0); // 0 como defecto (Español)
                languageDropdown.value = languageIndex;
                languageDropdown.RefreshShownValue();

                // Actualizar los textos de la interfaz al idioma correcto al iniciar
                ActualizarTextos(languageIndex);
            }
        }

        // --- MÉTODOS PARA LOS EVENTOS DE LA UI ---

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            PlayerPrefs.SetInt("FullscreenPreference", isFullscreen ? 1 : 0);
        }

        public void SetScreenShake(bool isEnabled)
        {
            PlayerPrefs.SetInt("ScreenShakePreference", isEnabled ? 1 : 0);
        }

        public void SetMasterVolume(float volume)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.volumenMaestro = volume;
                AudioManager.Instance.AplicarVolumenes();
            }
            PlayerPrefs.SetFloat("MasterVolumePreference", volume);
        }

        public void SetVolume(float volume) // Music Volume
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.volumenMusica = volume;
                AudioManager.Instance.AplicarVolumenes();
            }
            PlayerPrefs.SetFloat("MusicVolumePreference", volume);
        }

        public void SetSFXVolume(float volume)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.volumenEfectos = volume;
            }
            PlayerPrefs.SetFloat("SFXVolumePreference", volume);
        }

        public void SetLanguage(int languageIndex)
        {
            // 0 = Español, 1 = Ingles
            PlayerPrefs.SetInt("LanguagePreference", languageIndex);
            
            // Actualizar todos los textos en tiempo real
            ActualizarTextos(languageIndex);
        }

        private void ReproducirSonidoUI(AudioClip clip)
        {
            if (clip == null) return;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.ReproducirEfecto(clip);
            }
            else if (uiAudioSource != null)
            {
                float master = PlayerPrefs.GetFloat("MasterVolumePreference", 1f);
                float sfx = PlayerPrefs.GetFloat("SFXVolumePreference", 1f);
                uiAudioSource.PlayOneShot(clip, master * sfx);
            }
        }

        public void OpenControls()
        {
            ReproducirSonidoUI(openMenuSound);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(true);
        }

        public void CloseControls()
        {
            ReproducirSonidoUI(closeMenuSound);
            if (controlsPanel != null) controlsPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        private void ActualizarTextos(int idioma)
        {
            // 0 = Español, 1 = Inglés
            if (idioma == 0) 
            {
                // ESPAÑOL
                if (subtitleText != null) subtitleText.text = "(toque cualquier letra)";

                if (playBtnText != null) playBtnText.text = "JUGAR";
                if (settingsBtnText != null) settingsBtnText.text = "AJUSTES";
                if (creditsBtnText != null) creditsBtnText.text = "CRÉDITOS";
                if (exitBtnText != null) exitBtnText.text = "SALIR";

                if (resumeBtnText != null) resumeBtnText.text = "REANUDAR";
                if (inventoryBtnText != null) inventoryBtnText.text = "INVENTARIO";
                if (restartBtnText != null) restartBtnText.text = "REINICIAR";

                if (pauseTitleText != null) pauseTitleText.text = "PAUSA";
                if (inventoryTitleText != null) inventoryTitleText.text = "INVENTARIO";
                if (inventorySubtitleText != null) inventorySubtitleText.text = "Click para equipar/desequipar (máx. 3)";
                if (closeInventoryBtnText != null) closeInventoryBtnText.text = "CERRAR";

                if (settingsTitleText != null) settingsTitleText.text = "AJUSTES";
                if (languageLabelText != null) languageLabelText.text = "Idioma";
                if (resolutionLabelText != null) resolutionLabelText.text = "Resolución";
                if (fullscreenLabelText != null) fullscreenLabelText.text = "Pantalla Completa";
                if (screenShakeLabelText != null) screenShakeLabelText.text = "Vibración de Cámara";
                if (masterVolumeLabelText != null) masterVolumeLabelText.text = "Volumen de Juego";
                if (volumeLabelText != null) volumeLabelText.text = "Volumen de Música";
                if (sfxVolumeLabelText != null) sfxVolumeLabelText.text = "Volumen de Efectos";
                if (backBtnText != null) backBtnText.text = "VOLVER";
                if (openControlsBtnText != null) openControlsBtnText.text = "CONTROLES";

                if (controlsTitleText != null) controlsTitleText.text = "CONTROLES";
                if (backFromControlsBtnText != null) backFromControlsBtnText.text = "VOLVER";
            }
            else 
            {
                // ENGLISH
                if (subtitleText != null) subtitleText.text = "(press any key)";

                if (playBtnText != null) playBtnText.text = "PLAY";
                if (settingsBtnText != null) settingsBtnText.text = "SETTINGS";
                if (creditsBtnText != null) creditsBtnText.text = "CREDITS";
                if (exitBtnText != null) exitBtnText.text = "EXIT";

                if (resumeBtnText != null) resumeBtnText.text = "RESUME";
                if (inventoryBtnText != null) inventoryBtnText.text = "INVENTORY";
                if (restartBtnText != null) restartBtnText.text = "RESTART";

                if (pauseTitleText != null) pauseTitleText.text = "PAUSE";
                if (inventoryTitleText != null) inventoryTitleText.text = "INVENTORY";
                if (inventorySubtitleText != null) inventorySubtitleText.text = "Click to equip/unequip (max 3)";
                if (closeInventoryBtnText != null) closeInventoryBtnText.text = "CLOSE";

                if (settingsTitleText != null) settingsTitleText.text = "SETTINGS";
                if (languageLabelText != null) languageLabelText.text = "Language";
                if (resolutionLabelText != null) resolutionLabelText.text = "Resolution";
                if (fullscreenLabelText != null) fullscreenLabelText.text = "Fullscreen";
                if (screenShakeLabelText != null) screenShakeLabelText.text = "Screen Shake";
                if (masterVolumeLabelText != null) masterVolumeLabelText.text = "Game Volume";
                if (volumeLabelText != null) volumeLabelText.text = "Music Volume";
                if (sfxVolumeLabelText != null) sfxVolumeLabelText.text = "SFX Volume";
                if (backBtnText != null) backBtnText.text = "BACK";
                if (openControlsBtnText != null) openControlsBtnText.text = "CONTROLS";

                if (controlsTitleText != null) controlsTitleText.text = "CONTROLS";
                if (backFromControlsBtnText != null) backFromControlsBtnText.text = "BACK";
            }
        }

        // --- NAVEGACIÓN ---

        // Llamar a este método desde el botón "SETTINGS" del Menú Principal
        public void OpenSettings()
        {
            ReproducirSonidoUI(openMenuSound);
            if (mainPanel != null) mainPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        // Llamar a este método desde el botón "VOLVER" (BACK) del menú de ajustes
        public void CloseSettings()
        {
            ReproducirSonidoUI(closeMenuSound);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (mainPanel != null) mainPanel.SetActive(true);
            PlayerPrefs.Save(); // Asegurar que todo se guarde en disco
        }
    }
}
