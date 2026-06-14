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
            // Solo activar si estamos en plataforma móvil real o si estamos simulando móvil en el editor
            bool debaActivar = Application.isMobilePlatform || PauseMenu.UsarModoMovil();
            gameObject.SetActive(debaActivar);
        }
    }
}
