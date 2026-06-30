using UnityEngine;
using UnityEngine.UI;

public class ProyectilInsulto : MonoBehaviour
{
    [Header("Movimiento Base")]
    public float velocidad = 5.0f;
    private Vector2 direccionTrayectoria;

    [Header("Mecánica Teledirigida (Producto Cruz)")]
    public float velocidadGiroProyectil = 120.0f;
    private Transform objetivoJugador;

    public void InicializarProyectil(Vector2 direccionInicial)
    {
        direccionTrayectoria = direccionInicial.normalized;

        float anguloInicial = Mathf.Atan2(direccionTrayectoria.y, direccionTrayectoria.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, anguloInicial - 90);
    }

    void Start()
    {
        GameObject j = GameObject.FindGameObjectWithTag("Player");
        if (j != null) objetivoJugador = j.transform;
    }

    void Update()
    {
        // Persigue al jugador con Producto Cruz mientras exista en el mapa
        if (objetivoJugador != null)
        {
            CalcularGiroTeledirigido();
        }

        // Desplazamiento lineal continuo
        transform.Translate(direccionTrayectoria * velocidad * Time.deltaTime, Space.World);
    }

    void CalcularGiroTeledirigido()
    {
        Vector2 dirHaciaJugador = (Vector2)objetivoJugador.position - (Vector2)transform.position;
        dirHaciaJugador.Normalize();

        Vector2 dirActualProyectil = transform.up;

        // FÓRMULA DEL PRODUCTO CRUZ 2D
        float productoCruz2D = (dirActualProyectil.x * dirHaciaJugador.y) - (dirActualProyectil.y * dirHaciaJugador.x);

        float cambioAngulo = 0f;
        if (productoCruz2D > 0.01f)
        {
            cambioAngulo = velocidadGiroProyectil * Time.deltaTime;
        }
        else if (productoCruz2D < -0.01f)
        {
            cambioAngulo = -velocidadGiroProyectil * Time.deltaTime;
        }

        transform.Rotate(0, 0, cambioAngulo);
        direccionTrayectoria = transform.up;
    }

    // DETECCIÓN DE IMPACTOS MODIFICADA
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Si choca de frente contra el escudo, se neutraliza (Desaparece)
        if (collision.CompareTag("Escudo"))
        {
            Vector2 frenteEscudo = collision.transform.up;

            // Producto Punto para validar si el bloqueo fue frontal (cono de 90°)
            float alineacion = Vector2.Dot(frenteEscudo, -direccionTrayectoria);

            if (alineacion > 0.707f)
            {
                // ¡Bloqueo Exitoso!
                GestorJuego.instancia.SumarPuntos(15);
                Destroy(gameObject); // El proyectil desaparece de la escena
            }
            else
            {
                // El escudo no llegó a cubrir ese ángulo (Impacto lateral/trasero)
                GestorJuego.instancia.ModificarAnimoEscolar(-10);
                Destroy(gameObject);
            }
        }
        // 2. Si choca directamente contra el jugador o un aliado por no bloquearlo a tiempo
        else if (collision.CompareTag("Player") || collision.CompareTag("Aliado"))
        {
            GestorJuego.instancia.ModificarAnimoEscolar(-15);
            Destroy(gameObject);
        }
    }
}