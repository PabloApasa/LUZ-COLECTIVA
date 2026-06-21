using UnityEngine;
using UnityEngine.UI;

public class ProyectilInsulto : MonoBehaviour
{
    public float velocidad = 5.0f;
    private Vector2 direccionTrayectoria;
    private bool estaPurificado = false;

    [Header("Configuración Visual")]
    public Text meshTexto; // Si usas un componente de texto flotante en el objeto
    private string[] frasesAsertivas = { "¡Tú puedes!", "Vales mucho", "No estás solo", "Respeto" };

    public void InicializarProyectil(Vector2 direccion)
    {
        direccionTrayectoria = direccion.normalized;
    }

    void Update()
    {
        // Se desplaza linealmente en base a su vector de dirección
        transform.Translate(direccionTrayectoria * velocidad * Time.deltaTime, Space.World);
    }

    // APLICACIÓN DEL PRODUCTO PUNTO PARA EL FILTRO DE COLISIÓN
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Escudo") && !estaPurificado)
        {
            Vector2 frenteEscudo = collision.transform.up;

            // Producto Punto entre el frente del escudo y la dirección inversa del proyectil
            float alineacion = Vector2.Dot(frenteEscudo, -direccionTrayectoria);

            // 0.707 representa un ángulo de 45° (cobertura total de 90° del cono de luz)
            if (alineacion > 0.707f)
            {
                PurificarProyectil();
            }
            else
            {
                // Entró por un punto ciego (Ángulo incorrecto)
                GestorJuego.instancia.ModificarAnimoEscolar(-10);
                Destroy(gameObject);
            }
        }
        else if (collision.CompareTag("Player") || collision.CompareTag("Aliado"))
        {
            if (!estaPurificado)
            {
                GestorJuego.instancia.ModificarAnimoEscolar(-15);
            }
            else
            {
                // Si ya fue purificado, sana y suma ánimo al golpear un aliado
                GestorJuego.instancia.ModificarAnimoEscolar(5);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstaculo") && estaPurificado)
        {
            // Mecánica Nivel 2: Los insultos buenos rompen los muros del laberinto digital
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void PurificarProyectil()
    {
        estaPurificado = true;
        direccionTrayectoria = -direccionTrayectoria; // REFLEXIÓN VECTORIAL DIRECTA INVERSA
        velocidad *= 1.3f; // Feedback visual de aceleración

        if (meshTexto != null)
        {
            meshTexto.text = frasesAsertivas[Random.Range(0, frasesAsertivas.Length)];
            meshTexto.color = Color.cyan;
        }

        GestorJuego.instancia.SumarPuntos(15);
    }
}