using UnityEngine;

public class BaquetaSetup : MonoBehaviour
{
    public Renderer baquetaEsquerda; // Arraste o cilindro da esquerda
    public Renderer baquetaDireita;  // Arraste o cilindro da direita

    public Material materialAzul;     // Crie/Arraste um material azul
    public Material materialVermelho; // Crie/Arraste um material vermelho

    void Start()
    {
        if(baquetaEsquerda) baquetaEsquerda.material = materialAzul;
        if(baquetaDireita) baquetaDireita.material = materialVermelho;
    }
}
