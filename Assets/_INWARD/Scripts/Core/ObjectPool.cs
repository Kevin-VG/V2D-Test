using System.Collections.Generic;
using UnityEngine;

namespace Shatter.Core
{
    /// <summary>
    /// Interfaz opcional para objetos pooleados (reset de estado al reactivar).
    /// </summary>
    public interface IPoolable
    {
        void AlGenerar();
        void AlReciclar();
    }

    /// <summary>
    /// Pool generico tag-based. Requisito UPN: Object Pooling.
    /// Se usa para power-ups, particulas, proyectiles conceptuales, floating text.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [System.Serializable]
        public class Piscina
        {
            public string etiqueta;
            public GameObject prefab;
            public int tamanoInicial = 8;
            public bool expandible = true;
        }

        [SerializeField] private List<Piscina> piscinas = new List<Piscina>();

        private readonly Dictionary<string, Queue<GameObject>> disponibles = new Dictionary<string, Queue<GameObject>>();
        private readonly Dictionary<string, Piscina> definiciones = new Dictionary<string, Piscina>();
        private Transform raizPool;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            raizPool = new GameObject("_ObjetosPooleados").transform;
            raizPool.SetParent(transform);

            foreach (var piscina in piscinas) RegistrarPool(piscina);
        }

        public void RegistrarPool(Piscina piscina)
        {
            if (piscina == null || string.IsNullOrEmpty(piscina.etiqueta) || piscina.prefab == null) return;
            if (definiciones.ContainsKey(piscina.etiqueta)) return;

            definiciones[piscina.etiqueta] = piscina;
            var cola = new Queue<GameObject>();
            for (int i = 0; i < piscina.tamanoInicial; i++)
                cola.Enqueue(CrearInstancia(piscina));
            disponibles[piscina.etiqueta] = cola;
        }

        public void RegistrarPool(string etiqueta, GameObject prefab, int tamanoInicial = 8, bool expandible = true)
        {
            RegistrarPool(new Piscina { etiqueta = etiqueta, prefab = prefab, tamanoInicial = tamanoInicial, expandible = expandible });
        }

        private GameObject CrearInstancia(Piscina piscina)
        {
            var go = Instantiate(piscina.prefab, raizPool);
            go.name = piscina.prefab.name;
            go.SetActive(false);
            return go;
        }

        public GameObject Generar(string etiqueta, Vector3 posicion, Quaternion rotacion)
        {
            if (!disponibles.TryGetValue(etiqueta, out var cola) || !definiciones.TryGetValue(etiqueta, out var def))
            {
                Debug.LogWarning($"[ObjectPool] Pool '{etiqueta}' no registrado.");
                return null;
            }

            GameObject obj;
            if (cola.Count > 0)
                obj = cola.Dequeue();
            else if (def.expandible)
                obj = CrearInstancia(def);
            else
                return null;

            obj.transform.SetPositionAndRotation(posicion, rotacion);
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
                poolable.AlGenerar();
            return obj;
        }

        public void Reciclar(string etiqueta, GameObject obj)
        {
            if (obj == null) return;
            if (obj.TryGetComponent<IPoolable>(out var poolable))
                poolable.AlReciclar();
            obj.SetActive(false);
            obj.transform.SetParent(raizPool, false);

            if (!disponibles.TryGetValue(etiqueta, out var cola))
            {
                cola = new Queue<GameObject>();
                disponibles[etiqueta] = cola;
            }
            cola.Enqueue(obj);
        }

        public bool TienePool(string etiqueta) => definiciones.ContainsKey(etiqueta);
    }
}
