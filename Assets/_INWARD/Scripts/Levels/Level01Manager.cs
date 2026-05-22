using UnityEngine;

namespace Shatter.Levels
{
    public class Level01Manager : LevelManager
    {
        [SerializeField] private string tituloNivel = "Nivel 1 — El Umbral";

        private void Start() { IniciarNivel(); }

        public override void IniciarNivel()
        {
            base.IniciarNivel();
            Debug.Log($"[Level01] Iniciado: {tituloNivel}");
        }
    }
}
