using UnityEngine;

public class IANivel1 : MonoBehaviour
{
    public GameObject prefabProyectil;
    public float tiempoEntreDisparos = 3.0f;
    private float cronometro;
    private Transform objetivo;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) objetivo = p.transform;
    }

    void Update()
    {
        if (objetivo == null) return;

        cronometro += Time.deltaTime;
        if (cronometro >= tiempoEntreDisparos)
        {
            cronometro = 0f;
            Disparar();
        }
    }

    void Disparar()
    {
        // REGLA MATEMÁTICA: Destino (Objetivo) - Origen (Enemigo) = Vector Dirección
        Vector2 direccionAtaque = (Vector2)objetivo.position - (Vector2)transform.position;

        GameObject clon = Instantiate(prefabProyectil, transform.position, Quaternion.identity);
        ProyectilInsulto script = clon.GetComponent<ProyectilInsulto>();

        if (script != null)
        {
            script.InicializarProyectil(direccionAtaque);
        }
    }
}