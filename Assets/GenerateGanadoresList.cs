using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateGanadoresList : MonoBehaviour
{

    public GameObject listItem;
   public void GenerateList(StartSorteo.ParticipanteCollection collection)
    {
        foreach(var v in GetComponentsInChildren<TMP_Text>())
        {
            Destroy(v.gameObject);
        }

        foreach(StartSorteo.Participante participante in collection.participantes)
        {
            if(participante.Ganador == "true")
            {
                GameObject item = Instantiate(listItem, transform);
                item.GetComponent<TMP_Text>().text = participante.Nombre + " " + participante.Id;
            }
        }
    }
}
