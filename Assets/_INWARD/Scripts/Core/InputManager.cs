using UnityEngine;
using System.Collections.Generic;

namespace Shatter.Core
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InputManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("InputManager");
                        instance = go.AddComponent<InputManager>();
                    }
                }
                return instance;
            }
        }

        [Header("Controles por defecto")]
        public KeyCode MoverIzquierda = KeyCode.A;
        public KeyCode MoverDerecha = KeyCode.D;
        public KeyCode Abajo = KeyCode.S;
        public KeyCode Saltar = KeyCode.Space;
        public KeyCode Dash = KeyCode.LeftShift;
        public KeyCode Interactuar = KeyCode.E;

        // Diccionario para facilitar el remapeo y la UI
        public Dictionary<string, KeyCode> Teclas { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InicializarTeclas();
                CargarTeclas();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Llamado si se crea via AddComponent
        private void Start()
        {
            if (Teclas == null)
            {
                InicializarTeclas();
                CargarTeclas();
            }
        }

        private void InicializarTeclas()
        {
            Teclas = new Dictionary<string, KeyCode>
            {
                { "Izquierda", MoverIzquierda },
                { "Derecha", MoverDerecha },
                { "Abajo", Abajo },
                { "Saltar", Saltar },
                { "Dash", Dash },
                { "Interactuar", Interactuar }
            };
        }

        public void ReasignarTecla(string accion, KeyCode nuevaTecla)
        {
            if (Teclas.ContainsKey(accion))
            {
                Teclas[accion] = nuevaTecla;
                ActualizarVariables(accion, nuevaTecla);
                GuardarTeclas();
            }
        }

        private void ActualizarVariables(string accion, KeyCode tecla)
        {
            switch (accion)
            {
                case "Izquierda": MoverIzquierda = tecla; break;
                case "Derecha": MoverDerecha = tecla; break;
                case "Abajo": Abajo = tecla; break;
                case "Saltar": Saltar = tecla; break;
                case "Dash": Dash = tecla; break;
                case "Interactuar": Interactuar = tecla; break;
            }
        }

        public void GuardarTeclas()
        {
            foreach (var kvp in Teclas)
            {
                PlayerPrefs.SetString("Key_" + kvp.Key, kvp.Value.ToString());
            }
            PlayerPrefs.Save();
        }

        public void CargarTeclas()
        {
            List<string> claves = new List<string>(Teclas.Keys);
            foreach (string clave in claves)
            {
                string teclaGuardada = PlayerPrefs.GetString("Key_" + clave, "");
                if (!string.IsNullOrEmpty(teclaGuardada))
                {
                    if (System.Enum.TryParse(teclaGuardada, out KeyCode codigoTecla))
                    {
                        Teclas[clave] = codigoTecla;
                        ActualizarVariables(clave, codigoTecla);
                    }
                }
            }
        }
    }
}
