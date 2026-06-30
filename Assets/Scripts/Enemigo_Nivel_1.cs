using UnityEngine;

public class IANivel1_PersecucionCercana : MonoBehaviour
{
    [Header("Configuración de Persecución")]
    public float velocidadEnemigo = 3.0f;
    public float rangoDeteccionMatematica = 20.0f;
    public float distanciaMinimaDetencion = 0.6f; // Para que se detenga justo al tocarlo

    private Transform aliadoObjetivo;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Evitamos que el enemigo rote o se caiga de cabeza por físicas
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. ELECCIÓN DEL OBJETIVO: Busca y selecciona al aliado MÁS CERCANO en este fotograma
        BuscarAliadoMasCercano();

        if (aliadoObjetivo == null) return;

        // 2. RESTA VECTORIAL: Vector de desplazamiento (Destino - Origen)
        Vector2 vectorHaciaAliado = (Vector2)aliadoObjetivo.position - (Vector2)transform.position;

        // 3. MAGNITUD: Distancia real en metros (Teorema de Pitágoras)
        float distanciaEscalar = vectorHaciaAliado.magnitude;

        // Si todavía no ha llegado al aliado, aplica el movimiento
        if (distanciaEscalar > distanciaMinimaDetencion)
        {
            // 4. NORMALIZACIÓN: Vector Unitario (longitud = 1, conserva la dirección pura)
            Vector2 direccionNormalizada = vectorHaciaAliado.normalized;

            // 5. PRODUCTO POR UN ESCALAR: Multiplicamos el Vector Dirección por los escalares de Velocidad y Tiempo
            Vector2 vectorDesplazamiento = direccionNormalizada * velocidadEnemigo * Time.deltaTime;

            // Aplicamos el desplazamiento vectorialmente a la posición física
            rb.MovePosition(rb.position + vectorDesplazamiento);
        }
    }

    // ALGORITMO MATEMÁTICO DE SELECCIÓN POR CERCANÍA
    void BuscarAliadoMasCercano()
    {
        // Busca a todos los estudiantes con el Tag "Aliado" en la escena
        GameObject[] todosLosAliados = GameObject.FindGameObjectsWithTag("Aliado");

        Transform objetivoMasCercano = null;

        // Inicializamos la menor distancia con el rango máximo de visión del enemigo
        float menorDistanciaRegistrada = rangoDeteccionMatematica;

        // Reemplaza la línea problemática:
        // foreach (GameObject aliado Individual in todosLosAliados)
        // Por la siguiente línea corregida:
        foreach (GameObject aliadoIndividual in todosLosAliados)
        {
            // Resta de vectores para medir la distancia hacia este aliado específico
            Vector2 vectorDiferencia = (Vector2) aliadoIndividual.transform.position - (Vector2)transform.position;
            float distanciaAlAliado = vectorDiferencia.magnitude;

            // COMPARACIÓN ESCONTRADA: Si la distancia a este aliado es menor que la del anterior...
            if (distanciaAlAliado < menorDistanciaRegistrada)
            {
                // ...este aliado se convierte en nuestro nuevo objetivo fijado
                menorDistanciaRegistrada = distanciaAlAliado;
                objetivoMasCercano = aliadoIndividual.transform;
            }
        }

        // Asignamos el ganador final al objetivo que perseguirá en el Update
        aliadoObjetivo = objetivoMasCercano;
    }
}