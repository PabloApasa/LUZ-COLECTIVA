using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GestorJuego : MonoBehaviour
{
    public static GestorJuego instancia;

    [Header("Métricas de la Partida")]
    public float temporizador = 150.0f; // 2 minutos y 30 segundos (Métrica solicitada)
    private int animoEscolar = 100;
    private int puntosEmpatia = 0;
    private bool juegoTerminado = false;

    [Header("Conexiones de Interfaz UI")]
    public Slider barraAnimoUI;
    public Text textoTiempoUI;
    public Text textoPuntosUI;
    public GameObject panelFinalUI; // El cuadro de interfaz de Victoria/Derrota
    public Text textoResultadoUI;

    void Awake()
    {
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (barraAnimoUI != null) barraAnimoUI.value = animoEscolar;
        if (panelFinalUI != null) panelFinalUI.SetActive(false);
        Time.timeScale = 1f; // Asegurar que el tiempo corra al iniciar
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (temporizador > 0)
        {
            temporizador -= Time.deltaTime;
            ActualizarUI();
        }
        else
        {
            // Victoria por resistir el tiempo límite de la partida
            TerminarPartida(true);
        }
    }

    void ActualizarUI()
    {
        if (barraAnimoUI != null) barraAnimoUI.value = animoEscolar;

        if (textoTiempoUI != null)
        {
            int minutos = Mathf.FloorToInt(temporizador / 60);
            int segundos = Mathf.FloorToInt(temporizador % 60);
            textoTiempoUI.text = string.Format("{0:0}:{1:00}", minutos, segundos);
        }
    }

    public void ModificarAnimoEscolar(int valor)
    {
        if (juegoTerminado) return;

        animoEscolar = Mathf.Clamp(animoEscolar + valor, 0, 100);

        if (animoEscolar <= 0)
        {
            // Derrota por agotamiento del clima de convivencia
            TerminarPartida(false);
        }
    }

    public void SumarPuntos(int puntos)
    {
        puntosEmpatia += puntos;
        if (textoPuntosUI != null) textoPuntosUI.text = "Empatía: " + puntosEmpatia;
    }

    public void TerminarPartida(bool gano)
    {
        juegoTerminado = true;
        Time.timeScale = 0f; // Detiene por completo las físicas e IAs del juego
        if (panelFinalUI != null) panelFinalUI.SetActive(true);

        if (gano)
        {
            textoResultadoUI.text = "¡ENTORNO CONECTADO EXITOSAMENTE!\nSuperaste el tiempo límite con asertividad.";
            textoResultadoUI.color = Color.green;
        }
        else
        {
            textoResultadoUI.text = "EL SILENCIO PREVALECIÓ\nEl nivel de acoso superó el ánimo escolar.";
            textoResultadoUI.color = Color.red;
        }
    }

    public void BotonReiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}