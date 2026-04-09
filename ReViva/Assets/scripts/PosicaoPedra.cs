using System;
using UnityEngine;

public class PosicaoPedra : MonoBehaviour
{
  public Transform[] posicao_inicial;
  public  Transform[] posicao_final;
   public GameObject pedra;
   public float dificuldade;

    void Awake()
   {
       //posicao_final = new Transform[posicao_inicial.x + dificuldade];
   }

   void Start()
    {
        for (int i = 0; i < posicao_final.Length; i++)
        {
            Instantiate(pedra, posicao_final[i]); 
        }
        
    }

    
    void Update()
    {
        
    }
}
