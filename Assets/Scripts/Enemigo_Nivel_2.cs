using UnityEngine;

public class IANivel2 : MonoBehaviour
{
    private Transform jugador;
    public float velocidadOrbita = 2.5f;
    public float radioOrbita = 5.0f;
    private int direccionGiro = 1; // 1 horario, -1 antihorario

    void Start()
    {
        GameObject j = GameObject.FindGameObjectWithTag("Player");
        if (j != null) jugador = j.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        Vector2 vectorHaciaJugador = (Vector2)jugador.position - (Vector2)transform.position;
        float distancia = vectorHaciaJugador.magnitude;

        // Si está muy lejos, se acerca al pasillo del jugador
        if (distancia > radioOrbita)
        {
            transform.Translate(vectorHaciaJugador.normalized * velocidadOrbita * Time.deltaTime, Space.World);
        }

        // CALCULO TANGENCIAL PARA LA ÓRBITA
        Vector2 orbitaPerpendicular = new Vector2(-vectorHaciaJugador.y, vectorHaciaJugador.x).normalized;
        transform.Translate(orbitaPerpendicular * direccionGiro * velocidadOrbita * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con una pared del laberinto, invierte el sentido usando el signo modificado
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            direccionGiro *= -1;
        }
    }
}