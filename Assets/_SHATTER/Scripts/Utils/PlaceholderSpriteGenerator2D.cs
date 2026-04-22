using UnityEngine;

namespace Shatter.Utils
{
    /// <summary>
    /// Genera sprites coloreados en runtime para prototipar sin assets reales.
    /// </summary>
    public static class PlaceholderSpriteGenerator2D
    {
        public static Sprite RectanguloSolido(int ancho, int alto, Color color, string nombre = "RectSolido")
        {
            var textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);
            textura.filterMode = FilterMode.Point;
            var pixeles = new Color[ancho * alto];
            for (int i = 0; i < pixeles.Length; i++) pixeles[i] = color;
            textura.SetPixels(pixeles);
            textura.Apply();
            var sprite = Sprite.Create(textura, new Rect(0, 0, ancho, alto), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = nombre;
            return sprite;
        }

        public static Sprite Circulo(int tamano, Color color, string nombre = "Circulo")
        {
            var textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);
            textura.filterMode = FilterMode.Bilinear;
            Vector2 centro = new Vector2(tamano / 2f, tamano / 2f);
            float radio = tamano / 2f - 1f;
            for (int y = 0; y < tamano; y++)
                for (int x = 0; x < tamano; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), centro);
                    if (d <= radio) textura.SetPixel(x, y, color);
                    else textura.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            textura.Apply();
            var sprite = Sprite.Create(textura, new Rect(0, 0, tamano, tamano), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = nombre;
            return sprite;
        }

        public static Sprite GradienteVertical(int ancho, int alto, Color superior, Color inferior, string nombre = "Gradiente")
        {
            var textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);
            textura.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < alto; y++)
            {
                float t = y / (float)(alto - 1);
                Color c = Color.Lerp(inferior, superior, t);
                for (int x = 0; x < ancho; x++) textura.SetPixel(x, y, c);
            }
            textura.Apply();
            var sprite = Sprite.Create(textura, new Rect(0, 0, ancho, alto), new Vector2(0.5f, 0.5f), 32f);
            sprite.name = nombre;
            return sprite;
        }

        /// <summary>Crea un SpriteRenderer hijo con un rect simple (player humanoide muy simple).</summary>
        public static GameObject CrearPlaceholderJugador(string nombre = "VisualJugador")
        {
            var go = new GameObject(nombre);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RectanguloSolido(24, 40, new Color(0.35f, 0.55f, 0.85f), "RectJugador");
            sr.sortingOrder = 10;
            return go;
        }

        public static GameObject CrearVisualColeccionable(Color color)
        {
            var go = new GameObject("VisualDestello");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = Circulo(24, color, "CirculoDestello");
            sr.sortingOrder = 5;
            return go;
        }
    }
}
