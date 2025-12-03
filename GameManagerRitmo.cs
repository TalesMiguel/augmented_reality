using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManagerRitmo : MonoBehaviour
{
    public List<TamborCores> listaTambores; 
    public AudioSource audioSourceGeral; 
    public AudioClip somSucesso; 
    public AudioClip somErro;    
    public AudioClip somFaseConcluida; // Som especial quando completar as 3x

    [Header("Configurações")]
    public float tempoLimite = 10.0f; 
    public int repeticoesNecessarias = 3; // Quantas vezes tem que acertar

    // --- ESTRUTURA DOS RUDIMENTOS ---
    [System.Serializable]
    public struct NotaRudimento
    {
        public int idTambor; 
        public bool usarMaoDireita; 
        public float tempoDeEspera; 
    }

    [System.Serializable]
    public struct FaseRudimento
    {
        public string nomeRudimento; 
        public List<NotaRudimento> sequencia;
    }

    public List<FaseRudimento> listaDeRudimentos; 
    // --------------------------------

    private int indiceFaseAtual = 0;
    private int indiceNotaAtual = 0;
    
    // Novo contador de progresso
    private int repeticoesAtuais = 0; 
    
    private bool turnoDoJogador = false;
    private float timerInatividade = 0f;
    private bool jogoAcabou = false;

    void Start()
    {
        Invoke("IniciarFase", 2.0f);
    }

    void Update()
    {
        if (jogoAcabou || !turnoDoJogador) return;

        timerInatividade += Time.deltaTime;
        if (timerInatividade > tempoLimite)
        {
            Debug.Log("Tempo Esgotou!");
            TocarErro();
        }
    }

    void IniciarFase()
    {
        Debug.Log($"Rudimento: {listaDeRudimentos[indiceFaseAtual].nomeRudimento} | Progresso: {repeticoesAtuais}/{repeticoesNecessarias}");
        
        turnoDoJogador = false;
        indiceNotaAtual = 0;
        StartCoroutine(DemonstrarRudimento());
    }

    IEnumerator DemonstrarRudimento()
    {
        yield return new WaitForSeconds(1.0f);

        FaseRudimento fase = listaDeRudimentos[indiceFaseAtual];

        foreach (NotaRudimento nota in fase.sequencia)
        {
            TamborCores tambor = listaTambores[nota.idTambor];
            Color cor = nota.usarMaoDireita ? Color.red : Color.blue;

            tambor.TocarFeedback(cor);
            yield return new WaitForSeconds(nota.tempoDeEspera);
        }

        Debug.Log("Sua vez!");
        turnoDoJogador = true;
        timerInatividade = 0f;
    }

    public void ProcessarBatidaJogador(int idTamborBatido, string tagBaquetaUsada)
    {
        if (jogoAcabou || !turnoDoJogador) return;

        timerInatividade = 0f;

        // 1. Feedback Visual
        Color corUsada = (tagBaquetaUsada == "BaquetaAzul") ? Color.blue : Color.red;
        listaTambores[idTamborBatido].TocarFeedback(corUsada);

        // 2. Validação
        FaseRudimento fase = listaDeRudimentos[indiceFaseAtual];
        NotaRudimento notaEsperada = fase.sequencia[indiceNotaAtual];

        bool tamborCerto = (idTamborBatido == notaEsperada.idTambor);
        
        bool maoCerta = false;
        if (notaEsperada.usarMaoDireita && tagBaquetaUsada == "BaquetaVermelha") maoCerta = true;
        if (!notaEsperada.usarMaoDireita && tagBaquetaUsada == "BaquetaAzul") maoCerta = true;

        if (tamborCerto && maoCerta)
        {
            indiceNotaAtual++;

            // Completou a sequência inteira?
            if (indiceNotaAtual >= fase.sequencia.Count)
            {
                // Incrementa o contador de sucesso
                repeticoesAtuais++;
                Debug.Log($"Acertou! ({repeticoesAtuais}/{repeticoesNecessarias})");

                turnoDoJogador = false;

                // Checa se já completou as 3 vezes necessárias
                if (repeticoesAtuais >= repeticoesNecessarias)
                {
                    // PASSOU DE FASE (NÍVEL)
                    Debug.Log("Fase Concluída! Próximo Rudimento...");
                    if(audioSourceGeral && somFaseConcluida) audioSourceGeral.PlayOneShot(somFaseConcluida);
                    
                    indiceFaseAtual++;
                    repeticoesAtuais = 0; // Zera o contador para a próxima fase

                    if (indiceFaseAtual < listaDeRudimentos.Count)
                    {
                        Invoke("IniciarFase", 2.0f); 
                    }
                    else
                    {
                        Debug.Log("VOCÊ ZEROU O MODO RITMO!");
                        Invoke("VoltarAoMenu", 3.0f);
                    }
                }
                else
                {
                    // ACERTOU MAS AINDA FALTA REPETIR
                    if(audioSourceGeral && somSucesso) audioSourceGeral.PlayOneShot(somSucesso);
                    // Repete a mesma fase
                    Invoke("IniciarFase", 1.5f);
                }
            }
        }
        else
        {
            Debug.Log("Errou! Reiniciando a demonstração...");
            TocarErro();
        }
    }

    void TocarErro()
    {
        if(audioSourceGeral && somErro) audioSourceGeral.PlayOneShot(somErro);
        
        // Zera o progresso da fase atual? 
        
        turnoDoJogador = false;
        Invoke("IniciarFase", 1.5f);
    }

    void VoltarAoMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
