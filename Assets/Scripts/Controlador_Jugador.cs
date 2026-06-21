using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5.0f;
    private Vector2 inputsMovimiento;
    private Rigidbody2D rb;
    private Animator animator; // Para controlar la velocidad de tu animación

    [Header("Mecánica del Escudo")]
    public Transform pivoteEscudo; // Arrastra aquí el objeto hijo que es el escudo
    public float velocidadGiro = 300.0f;
    public float rangoDeteccion = 12.0f;

    [Header("Depuración Visual")]
    private bool mostrarVectores = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Capturar inputs de movimiento
        inputsMovimiento.x = Input.GetAxisRaw("Horizontal");
        inputsMovimiento.y = Input.GetAxisRaw("Vertical");

        // Controlar la animación: Si se mueve, la animación corre; si no, se frena
        if (animator != null)
        {
            bool seEstaMoviendo = inputsMovimiento.magnitude > 0;
            animator.speed = seEstaMoviendo ? 1f : 0f;
        }

        // Alternar modo depuración con la tecla V
        if (Input.GetKeyDown(KeyCode.V))
        {
            mostrarVectores = !mostrarVectores;
        }

        GiroAsertivo();
    }

    void FixedUpdate()
    {
        // Movimiento físico libre de inercia
        rb.MovePosition(rb.position + inputsMovimiento.normalized * velocidad * Time.fixedDeltaTime);
    }

    // APLICACIÓN DE PRODUCTO CRUZ 2D PARA EL GIRO AUTOMÁTICO
    void GiroAsertivo()
    {
        Transform amenaza = BuscarAmenazaMasCercana();
        if (amenaza == null || pivoteEscudo == null) return;

        // Vector dirección desde el escudo hacia la amenaza
        Vector2 dirHaciaAmenaza = (Vector2)amenaza.position - (Vector2)pivoteEscudo.position;
        dirHaciaAmenaza.Normalize();

        // Vector que indica hacia dónde apunta el escudo actualmente
        Vector2 dirActualEscudo = pivoteEscudo.up;

        // Fórmula matemática del Producto Cruz en 2D (Determinante)
        float productoCruz2D = (dirActualEscudo.x * dirHaciaAmenaza.y) - (dirActualEscudo.y * dirHaciaAmenaza.x);

        if (mostrarVectores)
        {
            Debug.DrawLine(pivoteEscudo.position, pivoteEscudo.position + (Vector3)dirActualEscudo * 2f, Color.cyan);
            Debug.DrawLine(pivoteEscudo.position, amenaza.position, Color.red);
        }

        // Si el resultado es positivo la amenaza está a la izquierda; si es negativo, a la derecha
        if (productoCruz2D > 0.03f)
        {
            pivoteEscudo.Rotate(0, 0, velocidadGiro * Time.deltaTime);
        }
        else if (productoCruz2D < -0.03f)
        {
            pivoteEscudo.Rotate(0, 0, -velocidadGiro * Time.deltaTime);
        }
    }

    Transform BuscarAmenazaMasCercana()
    {
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, rangoDeteccion);
        Transform masCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (var col in colisiones)
        {
            if (col.CompareTag("Insulto") || col.CompareTag("Enemigo"))
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < distanciaMinima)
                {
                    distanciaMinima = dist;
                    masCercano = col.transform;
                }
            }
        }
        return masCercano;
    }
}