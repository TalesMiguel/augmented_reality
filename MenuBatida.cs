using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuBatida : MonoBehaviour
{
    
    public string nomeDaCena; 
    
    private AudioSource audioSource;
    private Renderer rend;
    private bool jaBateu = false; // pra nao bater 2x sem querer

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Baqueta") && !jaBateu)
        {
            jaBateu = true; 
            
            // Toca o som
            if(audioSource) audioSource.PlayOneShot(audioSource.clip);
            
            // Muda cor para feedback
            if(rend) rend.material.color = Color.green;

            // troca de cena com atraso (para ouvir o som)
            StartCoroutine(CarregarCena());
        }
    }

    IEnumerator CarregarCena()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Verifica se o nome da cena é válido
        if (Application.CanStreamedLevelBeLoaded(nomeDaCena))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
        else
        {
            Debug.LogError("A cena '" + nomeDaCena + "' não foi encontrada no Build Settings!");
            // Reseta para poder bater de novo caso tenha errado o nome
            jaBateu = false;
            if(rend) rend.material.color = Color.white; // Volta a cor
        }
    }
}
