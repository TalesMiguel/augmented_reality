using UnityEngine;

public class BaquetaSetup : MonoBehaviour
{
    public Renderer baquetaEsquerda;
    public Renderer baquetaDireita;

    public Material materialAzul; 
    public Material materialVermelho;

    void Start()
    {
        if(baquetaEsquerda) baquetaEsquerda.material = materialAzul;
        if(baquetaDireita) baquetaDireita.material = materialVermelho;
    }
}
