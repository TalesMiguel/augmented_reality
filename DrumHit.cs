using UnityEngine;

// O NOME ABAIXO TEM QUE SER IGUAL AO NOME DO ARQUIVO
public class DrumHit : MonoBehaviour
{
    private AudioSource audioSource;
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        // Tenta pegar os componentes automaticamente
        audioSource = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
        
        if (rend != null) originalColor = rend.material.color;
        
        // Debug para sabermos se o script carregou
        Debug.Log("Script de bateria iniciado no objeto: " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        // so toca com a baqueta"
        if (other.CompareTag("Baqueta")) 
        {
            Debug.Log("Baqueta bateu!");
            
            if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
            
            if (rend != null) {
                rend.material.color = Color.red;
                Invoke("ResetColor", 0.1f);
            }
        }
    }

    void ResetColor()
    {
        if (rend != null) rend.material.color = originalColor;
    }
}
