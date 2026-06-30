using UnityEngine;

public class IANivel2_AtaqueMatematico : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public GameObject prefabProyectil;
    public float cadenciaDisparo = 2.5f; // Tiempo en segundos entre ataques
    public float rangoDeVisionMatematica = 12.0f; // Distancia máxima para detectar al jugador
    private float cronometroDisparo;

    private Transform objetivoJugador;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Buscamos al jugador por su Tag único al iniciar
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            objetivoJugador = jugador.transform;
        }
    }

    void Update()
    {
        if (objetivoJugador == null) return;

        // --- DEMOSTRACIÓN MATEMÁTICA DE VECTORES ---

        // 1. RESTA VECTORIAL: Calculamos la distancia y el sentido entre los dos puntos
        // Vector = Destino (Posición del Jugador) - Origen (Posición de este Enemigo)
        Vector2 vectorHaciaJugador = (Vector2)objetivoJugador.position - (Vector2)transform.position;

        // 2. MAGNITUD (Teorema de Pitágoras): Obtenemos el escalar de distancia (la hipotenusa)
        // ||V|| = √(x² + y²)
        float distanciaEscalar = vectorHaciaJugador.magnitude;

        // 3. FILTRO DE RANGO: El enemigo solo ataca si el jugador está dentro de su radio de visión
        if (distanciaEscalar <= rangoDeVisionMatematica)
        {
            cronometroDisparo += Time.deltaTime;

            if (cronometroDisparo >= cadenciaDisparo)
            {
                cronometroDisparo = 0f;

                // 4. NORMALIZACIÓN (Vector Unitario): Reducimos el vector para que su longitud valga exactamente 1
                // Esto nos da la dirección pura de disparo sin importar qué tan lejos esté el objetivo
                Vector2 direccionDisparoNormalizada = vectorHaciaJugador.normalized;

                // Ejecutamos la acción pasando el vector calculado
                DispararInsulto(direccionDisparoNormalizada);
            }
        }
    }

    void DispararInsulto(Vector2 direccionCalculada)
    {
        if (prefabProyectil == null) return;

        // Instanciamos el proyectil en la posición actual del enemigo
        GameObject clonProyectil = Instantiate(prefabProyectil, transform.position, Quaternion.identity);

        // Buscamos el script del proyectil que creamos anteriormente
        ProyectilInsulto scriptProyectil = clonProyectil.GetComponent<ProyectilInsulto>();

        if (scriptProyectil != null)
        {
            // Le inyectamos la dirección normalizada. El proyectil multiplicará este vector 
            // por su propia velocidad (Producto de un Vector por un Escalar) para moverse de forma lineal.
            scriptProyectil.InicializarProyectil(direccionCalculada);
        }
    }

    // Dibujo matemático en el editor de Unity para comprobar el rango de ataque
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeVisionMatematica);
    }
}