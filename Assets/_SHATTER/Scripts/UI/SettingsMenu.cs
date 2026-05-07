using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shatter.Core; // Para acceder al AudioManager

namespace Shatter.UI
{
    [System.Serializable]
    public struct KeyIconMapping
    {
        public KeyCode key;
        public Sprite icon;
    }

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

        [Header("Iconos de Teclas (Controles)")]
        [Tooltip("Imagen que se muestra cuando el botón se presiona (aplastado) esperando una tecla")]
        public Sprite botonAplastadoIcon;
        [Tooltip("Imagen por defecto si la tecla asignada no tiene un icono configurado")]
        public Sprite iconoPorDefecto;
        [Tooltip("La tipografía (Font) que se usará para los textos del menú de controles (ej. fuente pixelada)")]
        public Font fuenteControles;
        [Tooltip("Lista de iconos específicos por tecla (ej. Space -> Sprite Barra Espaciadora)")]
        public KeyIconMapping[] iconosDeTeclas;
        [Tooltip("Prefab de la fila de control (opcional). Si lo arrastras, podrás diseñar tus botones libremente en el Canvas de Unity.")]
        public GameObject filaControlPrefab;
        [Header("Configuración de Layout")]
        [Tooltip("Posición Y donde inicia el primer control (ej: -180f). Ajusta este valor en el Inspector para alejarlo del título.")]
        public float startY = -180f;
        [Tooltip("Espacio vertical de separación entre cada fila de control (ej: 75f). Ajusta esto en el Inspector para separarlos más.")]
        public float spacingY = 75f;
        private Resolution[] resolutions;

        private static int ultimoFrameEsc = -1;

        private void Start()
        {
            // Ocultar panel de ajustes y controles al inicio
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);

            ConfigurarResoluciones();
            CargarAjustes();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Prevenir múltiples ejecuciones en el mismo frame (ej. si PauseMenu también lo detecta)
                if (Time.frameCount == ultimoFrameEsc) return;

                // 1. Si los controles están abiertos, volver a Ajustes
                if (controlsPanel != null && controlsPanel.activeInHierarchy)
                {
                    ultimoFrameEsc = Time.frameCount;
                    CloseControls();
                }
                // 2. Si ajustes está abierto y estamos en el Menú Principal (mainPanel asignado)
                else if (settingsPanel != null && settingsPanel.activeInHierarchy && mainPanel != null)
                {
                    ultimoFrameEsc = Time.frameCount;
                    CloseSettings();
                }
            }
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
            if (controlsPanel != null) 
            {
                controlsPanel.SetActive(true);
                GenerarBotonesControles();
            }
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

        // --- SISTEMA DE REASIGNACION DE TECLAS (KEYBINDINGS VISUALES) ---
        private bool esperandoTecla = false;
        private string accionAReasignar = "";
        private Image imagenBotonReasignando;
        private List<GameObject> botonesGenerados = new List<GameObject>();

        private Sprite ObtenerIconoDeTecla(KeyCode tecla)
        {
            if (iconosDeTeclas != null)
            {
                foreach (var mapping in iconosDeTeclas)
                {
                    if (mapping.key == tecla) return mapping.icon;
                }
            }
            return iconoPorDefecto; // Si no hay dibujo para esta tecla
        }

        private void GenerarBotonesControles()
        {
            if (controlsPanel == null || Shatter.Core.InputManager.Instance == null) return;

            // Limpiar botones anteriores
            foreach (var btn in botonesGenerados)
            {
                if (btn != null) Destroy(btn);
            }
            botonesGenerados.Clear();

            int i = 0;

            foreach (var kvp in Shatter.Core.InputManager.Instance.Teclas)
            {
                string accion = kvp.Key;
                KeyCode tecla = kvp.Value;
                
                // Texto a mostrar, ej: "Saltar"
                string etiqueta = TraducirAccion(accion);
                Sprite iconoTecla = ObtenerIconoDeTecla(tecla);
                
                if (filaControlPrefab != null)
                {
                    // --- FORMA PROFESIONAL: USANDO TU PREFAB DEL CANVAS ---
                    GameObject filaGo = Instantiate(filaControlPrefab, controlsPanel.transform);
                    
                    // Posicionarla en el panel
                    RectTransform rt = filaGo.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        rt.anchorMin = new Vector2(0.5f, 1f); 
                        rt.anchorMax = new Vector2(0.5f, 1f);
                        rt.pivot = new Vector2(0.5f, 1f);
                        rt.anchoredPosition = new Vector2(0, startY - (i * spacingY));
                    }

                    FilaControlUI filaUI = filaGo.GetComponent<FilaControlUI>();
                    if (filaUI != null)
                    {
                        // Configurar textos, imagen y textos de respaldo
                        filaUI.ConfigurarFila(etiqueta, iconoTecla, tecla.ToString(), iconoPorDefecto);
                        
                        // Configurar el click del botón para remapear
                        if (filaUI.botonRemapeo != null)
                        {
                            string acc = accion; // Cachear variable
                            Image imgKey = filaUI.imagenTecla;
                            filaUI.botonRemapeo.onClick.RemoveAllListeners();
                            filaUI.botonRemapeo.onClick.AddListener(() => {
                                if (!esperandoTecla)
                                {
                                    ReproducirSonidoUI(openMenuSound);
                                    IniciarReasignacion(acc, imgKey);
                                }
                            });
                        }
                    }
                    botonesGenerados.Add(filaGo);
                }
                else
                {
                    // --- FORMA POR DEFECTO: GENERADO POR CÓDIGO (FALLBACK) ---
                    var botonGo = CrearBotonControl(controlsPanel.transform, etiqueta, new Vector2(0, startY - (i * spacingY)), accion, iconoTecla, tecla.ToString());
                    botonesGenerados.Add(botonGo);
                }
                
                i++;
            }
        }

        private GameObject CrearBotonControl(Transform padre, string etiqueta, Vector2 posicionAnclada, string accion, Sprite icono, string nombreTeclaStr)
        {
            var go = new GameObject("BtnKey_" + accion);
            go.transform.SetParent(padre, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f); 
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(450, 65); // Botón un poco más grande y ancho para que luzcan los iconos
            rt.anchoredPosition = posicionAnclada;
            
            var img = go.AddComponent<Image>();
            // El fondo del botón será transparente o semitransparente para que destaquen las imagenes
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            // 1. Crear el objeto de Texto (Nombre de la Acción, a la izquierda)
            var etiquetaGo = new GameObject("Text_Action");
            etiquetaGo.transform.SetParent(go.transform, false);
            
            var t = etiquetaGo.AddComponent<Text>();
            if (fuenteControles != null)
            {
                t.font = fuenteControles;
            }
            else
            {
                t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            t.text = etiqueta;
            t.fontSize = 24;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleLeft;
            
            var lrt = t.rectTransform;
            lrt.anchorMin = new Vector2(0.05f, 0f); 
            lrt.anchorMax = new Vector2(0.5f, 1f);
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

            // 2. Crear el objeto de Imagen (Icono de la Tecla, a la derecha con proporción dinámica)
            var iconoGo = new GameObject("Image_Key");
            iconoGo.transform.SetParent(go.transform, false);
            var imgKey = iconoGo.AddComponent<Image>();
            imgKey.sprite = icono;
            imgKey.preserveAspect = true;

            var irt = imgKey.rectTransform;
            
            // Calculamos el alto fijo y el ancho dinámico según el aspecto real de la imagen
            float height = 48f; // Alto perfecto alineado con el botón de 65px
            float aspect = 1f;
            
            Sprite spriteReferencia = (icono != null) ? icono : iconoPorDefecto;
            if (spriteReferencia != null)
            {
                aspect = (float)spriteReferencia.rect.width / spriteReferencia.rect.height;
            }
            float width = height * aspect;

            // Forzar anclaje al centro de la mitad derecha del botón de fila (crece simétricamente hacia ambos lados)
            irt.anchorMin = new Vector2(0.78f, 0.5f);
            irt.anchorMax = new Vector2(0.78f, 0.5f);
            irt.pivot = new Vector2(0.5f, 0.5f);
            irt.sizeDelta = new Vector2(width, height);
            irt.anchoredPosition = Vector2.zero; // Perfectamente centrado en ese anclaje

            // 3. Crear texto de respaldo (si no hay imagen o si está usando el icono gris por defecto)
            if (icono == null || icono == iconoPorDefecto)
            {
                var txtFallback = new GameObject("Text_Fallback").AddComponent<Text>();
                txtFallback.transform.SetParent(iconoGo.transform, false);
                txtFallback.font = t.font;
                txtFallback.text = nombreTeclaStr;
                txtFallback.fontSize = 18; // Tamaño ideal para que quepa bien dentro de la tecla
                txtFallback.color = Color.white; // Blanco para que combine con el pixel art de la piedra
                txtFallback.alignment = TextAnchor.MiddleCenter;
                
                var frt = txtFallback.rectTransform;
                frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
                frt.offsetMin = Vector2.zero; frt.offsetMax = Vector2.zero;
            }
            
            btn.onClick.AddListener(() => {
                if (!esperandoTecla)
                {
                    ReproducirSonidoUI(openMenuSound);
                    IniciarReasignacion(accion, imgKey);
                }
            });

            return go;
        }

        private string TraducirAccion(string accion)
        {
            // Traducir según el idioma actual (0 = Español, 1 = Inglés)
            int idioma = PlayerPrefs.GetInt("LanguagePreference", 0);
            if (idioma == 1) // Inglés
            {
                switch (accion)
                {
                    case "Izquierda": return "Left";
                    case "Derecha": return "Right";
                    case "Abajo": return "Down";
                    case "Saltar": return "Jump";
                    case "Dash": return "Dash";
                    case "Interactuar": return "Interact";
                }
            }
            return accion; // Español es por defecto la clave
        }

        private void IniciarReasignacion(string accion, Image imagenBoton)
        {
            esperandoTecla = true;
            accionAReasignar = accion;
            imagenBotonReasignando = imagenBoton;
            
            if (botonAplastadoIcon != null)
            {
                imagenBotonReasignando.sprite = botonAplastadoIcon;
            }
            else
            {
                // Respaldo visual si no asignan icono de aplastado
                imagenBotonReasignando.color = Color.yellow;
            }
        }

        // OnGUI se llama varias veces por frame y es util para capturar Event.current (teclas raw)
        private void OnGUI()
        {
            if (esperandoTecla)
            {
                Event e = Event.current;
                if (e != null && e.isKey && e.type == EventType.KeyDown && e.keyCode != KeyCode.None)
                {
                    if (e.keyCode != KeyCode.Escape) // Evitar que Escape se asigne accidentalmente
                    {
                        AsignarNuevaTecla(e.keyCode);
                    }
                    else
                    {
                        // Si presionó escape, cancelar la reasignación
                        CancelarReasignacion();
                    }
                }
            }
        }

        private void AsignarNuevaTecla(KeyCode nuevaTecla)
        {
            if (Shatter.Core.InputManager.Instance != null)
            {
                Shatter.Core.InputManager.Instance.ReasignarTecla(accionAReasignar, nuevaTecla);
                ReproducirSonidoUI(closeMenuSound); // Sonido de confirmación
            }
            
            esperandoTecla = false;
            GenerarBotonesControles(); // Regenerar todo para refrescar la UI
        }

        private void CancelarReasignacion()
        {
            esperandoTecla = false;
            GenerarBotonesControles();
        }
    }
}
