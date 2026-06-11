using UnityEngine;

namespace Shatter.UI
{
    /// <summary>
    /// Activa el HUD móvil únicamente en dispositivos móviles o durante pruebas en el editor.
    /// </summary>
    public class MobileHUDActivator : MonoBehaviour
    {
        [Tooltip("Si está activo, los controles móviles también se verán en el Editor de Unity para poder probarlos.")]
        [SerializeField] private bool activarEnEditor = true;

        private void Awake()
        {
            #if UNITY_EDITOR
            gameObject.SetActive(activarEnEditor);
            #elif UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);
            #else
            // Ocultar controles móviles en compilaciones de PC (Windows/Mac/Linux)
            gameObject.SetActive(false);
            #endif
        }
    }
}
