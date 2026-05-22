using System;
using UnityEngine;

namespace Shatter.Core
{
    /// <summary>
    /// Bus de eventos estaticos para desacoplar sistemas.
    /// </summary>
    public static class GameEvents
    {
        // Jugador
        public static event Action<int> AlGolpearJugador;              // cantidad de dano
        public static event Action AlMorirJugador;
        public static event Action AlReaparecerJugador;
        public static event Action<Vector2> AlSaltarJugador;

        // Coleccionables
        public static event Action<int> AlRecogerDestello;             // destellos agregados
        public static event Action<string> AlRecogerPowerUp;           // etiqueta del power-up
        public static event Action<string> AlEquiparFragmento;         // id del fragmento
        public static event Action<string> AlDesequiparFragmento;

        // Enemigos
        public static event Action<GameObject> AlDerrotarEnemigo;

        // Power-ups / Debuffs
        public static event Action<string, float> AlActivarPowerUp;    // etiqueta, duracion
        public static event Action<string> AlExpirarPowerUp;
        public static event Action<string, float> AlAplicarDebuff;     // etiqueta, duracion
        public static event Action<string> AlExpirarDebuff;

        // Camara
        public static event Action<float, float> AlAgitarCamara;       // intensidad, duracion

        // --- Helpers para lanzar eventos ---
        public static void LanzarGolpeJugador(int dano) => AlGolpearJugador?.Invoke(dano);
        public static void LanzarMuerteJugador() => AlMorirJugador?.Invoke();
        public static void LanzarReaparicionJugador() => AlReaparecerJugador?.Invoke();
        public static void LanzarSaltoJugador(Vector2 velocidad) => AlSaltarJugador?.Invoke(velocidad);

        public static void LanzarDestelloRecogido(int cantidad) => AlRecogerDestello?.Invoke(cantidad);
        public static void LanzarPowerUpRecogido(string etiqueta) => AlRecogerPowerUp?.Invoke(etiqueta);
        public static void LanzarFragmentoEquipado(string id) => AlEquiparFragmento?.Invoke(id);
        public static void LanzarFragmentoDesequipado(string id) => AlDesequiparFragmento?.Invoke(id);

        public static void LanzarEnemigoDerotado(GameObject enemigo) => AlDerrotarEnemigo?.Invoke(enemigo);

        public static void LanzarPowerUpActivado(string etiqueta, float duracion) => AlActivarPowerUp?.Invoke(etiqueta, duracion);
        public static void LanzarPowerUpExpirado(string etiqueta) => AlExpirarPowerUp?.Invoke(etiqueta);
        public static void LanzarDebuffAplicado(string etiqueta, float duracion) => AlAplicarDebuff?.Invoke(etiqueta, duracion);
        public static void LanzarDebuffExpirado(string etiqueta) => AlExpirarDebuff?.Invoke(etiqueta);

        public static void LanzarAgitacionCamara(float intensidad, float duracion) => AlAgitarCamara?.Invoke(intensidad, duracion);
    }
}
