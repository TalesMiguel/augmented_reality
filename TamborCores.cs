using UnityEngine;

public class TamborCores : MonoBehaviour
{
    public int idTambor; // 0, 1 ou 2
    
    // Referências para os DOIS tipos de gerente possíveis
    private GameManagerCores managerCores; // Chefe da cena Simon Says
    private GameManagerRitmo managerRitmo; // Chefe da cena Ritmo
    
    private AudioSource audioSource;
    private Renderer rend;
    private Color corOriginal;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
        if(rend) corOriginal = rend.material.color;

        // O tambor tenta encontrar QUEM está mandando nesta cena
        managerCores = FindFirstObjectByType<GameManagerCores>();
        managerRitmo = FindFirstObjectByType<GameManagerRitmo>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se é uma baqueta colorida
        if (other.CompareTag("BaquetaAzul") || other.CompareTag("BaquetaVermelha"))
        {
            // O tambor decide pra quem mandar a mensagem:
            
            // 1. Se o Gerente de Ritmo existir, avisa ele
            if (managerRitmo != null)
            {
                managerRitmo.ProcessarBatidaJogador(idTambor, other.tag);
            }
            // 2. Se não, se o Gerente de Cores existir, avisa ele
            else if (managerCores != null)
            {
                managerCores.ProcessarBatidaJogador(idTambor, other.tag);
            }
        }
    }

    // Essa função é chamada pelos gerentes para piscar a cor (Funciona igual para os dois)
    public void TocarFeedback(Color corDoFeedback)
    {
        if (audioSource) audioSource.PlayOneShot(audioSource.clip);
        
        if (rend)
        {
            rend.material.color = corDoFeedback;
            CancelInvoke("ResetarCor");
            Invoke("ResetarCor", 0.3f);
        }
    }

    void ResetarCor()
    {
        if(rend) rend.material.color = corOriginal;
    }
}
