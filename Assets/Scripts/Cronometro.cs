using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cronometro : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI textoCrono;
    [SerializeField] private float tiempo;

    private int tiempoMinutos, tiempoSegundos, tiempoDecimasDeSegundo;

    // Agrega una variable para controlar si el cronómetro debe seguir actualizándose
    public bool cronometroActivo = true;

    void Crono()
    {
        if (cronometroActivo)
        {
            tiempo += Time.deltaTime;

            tiempoMinutos = Mathf.FloorToInt(tiempo / 60);
            tiempoSegundos = Mathf.FloorToInt(tiempo % 60);
            tiempoDecimasDeSegundo = Mathf.FloorToInt((tiempo % 1) * 100);

            textoCrono.text = string.Format ("{0:00}:{1:00}:{2:00}", tiempoMinutos, tiempoSegundos, tiempoDecimasDeSegundo);
        }        
    }

    

    // Update is called once per frame
    void Update()
    {
        Crono();
    }
}
