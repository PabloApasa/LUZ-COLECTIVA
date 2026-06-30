using UnityEngine;

public class ControlJugador_ProductoCruz : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5.0f;
    private Vector2 inputsMovimiento;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Mecánica del Escudo (Producto Cruz)")]
    public Transform pivoteEscudo; // El objeto hijo "PivoteEscudo"
    public float velocidadGiroEscudo = 400.0f; // Qué tan rápido reacciona el escudo
    public float rangoDeteccionMatematica = 12.0f; // Radio para buscar insultos

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Forzamos rotación cero al inicio
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // 1. Captura de inputs estándar
        inputsMovimiento.x = Input.GetAxisRaw("Horizontal");
        inputsMovimiento.y = Input.GetAxisRaw("Vertical");

        // Flip visual del personaje según su dirección de caminata
        if (inputsMovimiento.x < 0) spriteRenderer.flipX = true;
        else if (inputsMovimiento.x > 0) spriteRenderer.flipX = false;

        // Control de reproducción de la animación
        if (animator != null)
        {
            bool seEstaMoviendo = inputsMovimiento.magnitude > 0;
            animator.speed = seEstaMoviendo ? 1f : 0f;
        }

        // 2. EJECUCIÓN GEOMÉTRICA: Orientación automática del escudo
        GiroAsertivoDelEscudo();
    }

    void FixedUpdate()
    {
        // Movimiento físico libre de inercia
        rb.MovePosition(rb.position + inputsMovimiento.normalized * velocidad * Time.fixedDeltaTime);

        // BLOQUEO ESTRICTO: El cuerpo del jugador NUNCA debe rotar en Z
        transform.rotation = Quaternion.identity;
    }

    // --- APLICACIÓN DE LA FÓRMULA DEL PRODUCTO CRUZ 2D ---
    void GiroAsertivoDelEscudo()
    {
        Transform amenazaCercana = BuscarAmenazaMasCercana();

        // Si no hay insultos en el mapa, el escudo se queda en su última posición
        if (amenazaCercana == null || pivoteEscudo == null) return;

        // Paso A: Resta Vectorial para obtener la dirección hacia el peligro
        Vector2 dirHaciaAmenaza = (Vector2)amenazaCercana.position - (Vector2)pivoteEscudo.position;
        dirHaciaAmenaza.Normalize();

        // Paso B: Obtenemos el vector que representa el frente actual del escudo (eje Y local)
        Vector2 dirActualEscudo = pivoteEscudo.up;

        // Paso C: FÓRMULA DEL PRODUCTO CRUZ EN 2D (Determinante)
        // Cruz = (Ax * By) - (Ay * Bx)
        float productoCruz2D = (dirActualEscudo.x * dirHaciaAmenaza.y) - (dirActualEscudo.y * dirHaciaAmenaza.x);

        // Paso D: Rotación angular en base al signo del resultado
        // Si el resultado es positivo, la amenaza está a la izquierda (rotación positiva en Z)
        if (productoCruz2D > 0.02f)
        {
            pivoteEscudo.Rotate(0, 0, velocidadGiroEscudo * Time.deltaTime);
        }
        // Si es negativo, la amenaza está a la derecha (rotación negativa en Z)
        else if (productoCruz2D < -0.02f)
        {
            pivoteEscudo.Rotate(0, 0, -velocidadGiroEscudo * Time.deltaTime);
        }
    }

    // Escáner matemático para encontrar el insulto teledirigido más próximo
    Transform BuscarAmenazaMasCercana()
    {
        // Buscamos todos los proyectiles activos en la escena
        GameObject[] insultosActivos = GameObject.FindGameObjectsWithTag("Insulto");
        Transform objetivoMasCercano = null;
        float distanciaMinima = rangoDeteccionMatematica;

        foreach (GameObject insulto in insultosActivos)
        {
            // Magnitud de la resta de vectores para calcular distancias exactas
            Vector2 diferencia = (Vector2)insulto.transform.position - (Vector2)transform.position;
            float distancia = diferencia.magnitude;

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                objetivoMasCercano = insulto.transform;
            }
        }

        return objetivoMasCercano;
    }
}