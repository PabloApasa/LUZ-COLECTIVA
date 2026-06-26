using UnityEngine;

public class SeguirJugador : MonoBehaviour
{
    private Transform objetivoJugador;

    [Header("Configuraci�n de Escolta")]
    public float velocidadSeguimiento = 4.0f;
    public float distanciaSegura = 1.8f;

    [Header("Estado del Aliado")]
    public bool rescatado = false; // Controla si ya fue alcanzado por el jugador
    public float radioDeRescate = 2.0f; // Distancia a la que el jugador debe acercarse para activarlo

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            objetivoJugador = jugador.transform;
        }

        // Congelar rotaci�n f�sica desde el c�digo por seguridad
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (objetivoJugador == null) return;

        // Asegurar que el aliado tampoco rote de cabeza
        transform.rotation = Quaternion.identity;

        // MATEM�TICA DE DETECCI�N Y DISTANCIA
        Vector2 vectorHaciaJugador = (Vector2)objetivoJugador.position - (Vector2)transform.position;
        float distanciaActual = vectorHaciaJugador.magnitude;

        // FASE 1: Si no ha sido rescatado, verifica si el jugador est� lo suficientemente cerca
        if (!rescatado)
        {
            if (distanciaActual <= radioDeRescate)
            {
                rescatado = true;
                Debug.Log("�Aliado Rescatado! Comenzando escolta.");
            }

            // Se queda quieto esperando
            if (animator != null) animator.speed = 0.3f; // Animaci�n muy lenta de espera (Idle)
            return;
        }

        // FASE 2: Comportamiento de persecuci�n (Solo si ya fue rescatado)
        if (distanciaActual > distanciaSegura)
        {
            Vector2 direccionNormalizada = vectorHaciaJugador.normalized;

            // Orientaci�n visual del aliado (Mirar a izquierda o derecha seg�n a d�nde camina)
            if (direccionNormalizada.x < 0 && spriteRenderer != null) spriteRenderer.flipX = true;
            else if (direccionNormalizada.x > 0 && spriteRenderer != null) spriteRenderer.flipX = false;

            // Movimiento f�sico cinem�tico hacia el jugador
            rb.MovePosition(rb.position + direccionNormalizada * velocidadSeguimiento * Time.deltaTime);

            if (animator != null) animator.speed = 1f; // Animaci�n normal de caminata
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null) animator.speed = 0f; // Freno al estar cerca
        }
    }
}