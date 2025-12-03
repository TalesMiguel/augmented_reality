using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManagerCores : MonoBehaviour
{
    public List<TamborCores> listaTambores; 
    public AudioSource somGameOver; 

    [Header("Configurações de Dificuldade")]
    public float tempoLimite = 5.0f; 

    [System.Serializable]
    public struct NotaMusical
    {
        public int idTambor;
        public string tagBaquetaNecessaria; 
    }

    private List<NotaMusical> sequenciaCompleta = new List<NotaMusical>();
    private int indiceAtualJogador = 0;
    
    private bool turnoDoJogador = false;
    private float timerInatividade = 0f;
    private bool jogoAcabou = false;

    void Start()
    {
        Invoke("IniciarNovaRodada", 2.0f);
    }

    void Update()
    {
        if (jogoAcabou) return;

        if (turnoDoJogador)
        {
            timerInatividade += Time.deltaTime;

            if (timerInatividade > tempoLimite)
            {
                Debug.Log("Tempo esgotou!");
                GameOver();
            }
        }
    }

    void IniciarNovaRodada()
    {
        turnoDoJogador = false;
        indiceAtualJogador = 0;
        
        AdicionarNotaAleatoria();
        StartCoroutine(DemonstrarSequencia());
    }

    void AdicionarNotaAleatoria()
    {
        NotaMusical novaNota = new NotaMusical();
        novaNota.idTambor = Random.Range(0, listaTambores.Count);
        novaNota.tagBaquetaNecessaria = (Random.Range(0, 2) == 0) ? "BaquetaAzul" : "BaquetaVermelha";
        sequenciaCompleta.Add(novaNota);
    }

    IEnumerator DemonstrarSequencia()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (NotaMusical nota in sequenciaCompleta)
        {
            TamborCores tambor = listaTambores[nota.idTambor];
            Color corVisual = (nota.tagBaquetaNecessaria == "BaquetaAzul") ? Color.blue : Color.red;

            tambor.TocarFeedback(corVisual);
            yield return new WaitForSeconds(0.8f);
        }

        turnoDoJogador = true;
        timerInatividade = 0f; 
        Debug.Log("Sua vez! Repita a sequência.");
    }

    // -------------------------------------------------------------
    // A MUDANÇA PRINCIPAL ESTÁ AQUI EMBAIXO
    // -------------------------------------------------------------
    public void ProcessarBatidaJogador(int idTamborBatido, string tagBaquetaUsada)
    {
        if (jogoAcabou || !turnoDoJogador) return;

        // 1. Reseta o timer
        timerInatividade = 0f;

        // 2. FEEDBACK IMEDIATO:
        // Descobre qual cor o jogador usou (independente de estar certo ou errado)
        Color corUsadaPeloJogador = (tagBaquetaUsada == "BaquetaAzul") ? Color.blue : Color.red;
        
        // Toca o som e mostra a cor no tambor AGORA
        listaTambores[idTamborBatido].TocarFeedback(corUsadaPeloJogador);

        // 3. AGORA verificamos a lógica do jogo
        NotaMusical notaEsperada = sequenciaCompleta[indiceAtualJogador];

        if (idTamborBatido == notaEsperada.idTambor && tagBaquetaUsada == notaEsperada.tagBaquetaNecessaria)
        {
            // ACERTOU! Segue o jogo...
            indiceAtualJogador++;

            if (indiceAtualJogador >= sequenciaCompleta.Count)
            {
                Debug.Log("Rodada completa!");
                turnoDoJogador = false; 
                Invoke("IniciarNovaRodada", 2.0f);
            }
        }
        else
        {
            // ERROU!
            Debug.Log("Errou! Game Over.");
            // O som do Game Over vai tocar por cima do som do tambor (efeito caos)
            GameOver();
        }
    }

    void GameOver()
    {
        jogoAcabou = true;
        turnoDoJogador = false;
        
        if (somGameOver) somGameOver.Play();

        Invoke("ReiniciarCena", 3.0f);
    }

    void ReiniciarCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
