using System.Collections.Generic;
using UnityEngine;
using Shatter.Levels;

namespace Shatter.Utils
{
    /// <summary>
    /// Genera un nivel procedural simple de bloques con Rigidbody2D estatico y BoxColliders2D
    /// para prototipar sin necesidad de Tilemap/Tile assets.
    /// Produce: piso continuo con huecos, escalones, plataforma flotante, pared para wall-jump,
    /// zona baja, one-way platforms.
    /// </summary>
    public class PlaceholderTileGenerator2D : MonoBehaviour
    {
        [Header("Capas")]
        [SerializeField] private string nombreCapaSuelo = "Ground";
        [SerializeField] private string nombreCapaPared = "Wall";
        [SerializeField] private string nombreCapaUnaDireccion = "OneWay";

        [Header("Visual")]
        [SerializeField] private Color colorSuelo = new Color(0.22f, 0.22f, 0.28f);
        [SerializeField] private Color colorPared = new Color(0.35f, 0.2f, 0.35f);
        [SerializeField] private Color colorUnaDireccion = new Color(0.8f, 0.7f, 0.3f);
        [SerializeField] private float tamanoTile = 1f;

        private Transform raizNivel;
        private Sprite spriteTileCacheado;

        public Transform RaizNivel => raizNivel;

        public void Generar()
        {
            if (raizNivel == null)
            {
                var go = new GameObject("_RaizNivel");
                raizNivel = go.transform;
            }

            spriteTileCacheado = PlaceholderSpriteGenerator2D.RectanguloSolido(32, 32, Color.white, "TileBlanco");

            // Piso principal (con huecos a proposito)
            for (int x = -15; x <= 40; x++)
            {
                if (x >= 10 && x <= 12) continue; // hueco 1
                if (x >= 20 && x <= 21) continue; // hueco 2
                GenerarTile(new Vector2(x * tamanoTile, -3f), colorSuelo, nombreCapaSuelo);
            }

            // Escalones ascendentes
            GenerarTile(new Vector2(4f, -2f), colorSuelo, nombreCapaSuelo);
            GenerarTile(new Vector2(5f, -1f), colorSuelo, nombreCapaSuelo);
            GenerarTile(new Vector2(6f, 0f), colorSuelo, nombreCapaSuelo);

            // Plataforma flotante
            for (int i = 0; i < 4; i++)
                GenerarTile(new Vector2(14f + i, 1f), colorSuelo, nombreCapaSuelo);

            // Pared alta para wall-jump
            for (int y = -2; y <= 4; y++)
                GenerarTile(new Vector2(25f, y), colorPared, nombreCapaPared);

            // Zona baja (techo) para crouch
            GenerarTile(new Vector2(30f, -1f), colorSuelo, nombreCapaSuelo);
            GenerarTile(new Vector2(31f, -1f), colorSuelo, nombreCapaSuelo);
            GenerarTile(new Vector2(32f, -1f), colorSuelo, nombreCapaSuelo);

            // One-way platforms
            GenerarOneWay(new Vector2(17f, 3f));
            GenerarOneWay(new Vector2(18f, 3f));
            GenerarOneWay(new Vector2(19f, 3f));

            // Paredes de fin de nivel
            for (int y = -2; y <= 6; y++)
            {
                GenerarTile(new Vector2(-15f, y), colorPared, nombreCapaPared);
                GenerarTile(new Vector2(40f, y), colorPared, nombreCapaPared);
            }
        }

        private GameObject GenerarTile(Vector2 posicion, Color color, string nombreCapa)
        {
            var go = new GameObject("Tile");
            go.transform.SetParent(raizNivel, false);
            go.transform.position = posicion;
            int capa = LayerMask.NameToLayer(nombreCapa);
            if (capa < 0)
            {
                // Fallback si layers no configuradas
                if (nombreCapa == "Ground" || nombreCapa == nombreCapaSuelo) capa = 6;
                else if (nombreCapa == "Wall" || nombreCapa == nombreCapaPared) capa = 7;
                else if (nombreCapa == "OneWay" || nombreCapa == nombreCapaUnaDireccion) capa = 8;
            }
            if (capa >= 0) go.layer = capa;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spriteTileCacheado;
            sr.color = color;
            sr.sortingOrder = 3;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(tamanoTile, tamanoTile);
            return go;
        }

        private GameObject GenerarOneWay(Vector2 posicion)
        {
            var go = GenerarTile(posicion, colorUnaDireccion, nombreCapaUnaDireccion);
            go.name = "TileOneWay";
            go.AddComponent<OneWayPlatform>();
            return go;
        }
    }
}
