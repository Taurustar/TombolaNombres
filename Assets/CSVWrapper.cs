using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class CSVWrapper : MonoBehaviour
{
    /// <summary>
    /// Converts a CSV information to the JSON for this purpose. 
    /// </summary>
    /// <param name="csv"></param>
    /// <returns></returns>
    public static string CSVToJSON(string csv)
    {
        if (!File.Exists(Application.streamingAssetsPath + "/data.csv"))
            return null;
        
        string firstLine = csv.Split("\n")[0];
        string csvData = csv.Remove(0, firstLine.Length);
        
        int nameIndex = -1;
        int idIndex = -1;
        List<StartSorteo.Participante> listaParticipantes = new List<StartSorteo.Participante>();
        if (csvData.Contains(";")) //for a weird reason some comma delimited files end with a semicolon instead. I applied this fix here to accept those files as well
        {
            for (int i = 0; i < firstLine.Split(";").Length; i++)
            {
                if (firstLine.Split(";")[i] == "Nombre completo")
                {
                    nameIndex = i;
                    break;
                }
            }
        
            
            for (int j = 0; j < firstLine.Split(";").Length; j++)
            {
                if (firstLine.Split(";")[j] == "Id")
                {
                    idIndex = j;
                    break;
                }
            }
            
            
            foreach (string data in csvData.Split("\n"))
            {
                if (string.IsNullOrWhiteSpace(data) || string.IsNullOrWhiteSpace(data)) continue;
            
                StartSorteo.Participante participante = new StartSorteo.Participante
                {
                    Nombre = data.Split(";")[nameIndex],
                    Id = data.Split(";")[idIndex],
                    Ganador = "false"
                };
           
                listaParticipantes.Add(participante);
            }
        }
        else
        {
            for (int i = 0; i < firstLine.Split(",").Length; i++)
            {
                if (firstLine.Split(",")[i] == "Nombre completo")
                {
                    nameIndex = i;
                    break;
                }
            }
        
            
            for (int j = 0; j < firstLine.Split(",").Length; j++)
            {
                if (firstLine.Split(",")[j] == "Id" || firstLine.Split(",")[j] == "Id")
                {
                    idIndex = j;
                    break;
                }
            }
            
            foreach (string data in csvData.Split("\n"))
            {
                if (string.IsNullOrWhiteSpace(data) || string.IsNullOrWhiteSpace(data)) continue;
            
                StartSorteo.Participante participante = new StartSorteo.Participante
                {
                    Nombre = data.Split(",")[nameIndex],
                    Id = data.Split(",")[idIndex],
                    Ganador = "false"
                };
           
                listaParticipantes.Add(participante);
            }
            
        }

        

        StartSorteo.ParticipanteCollection col = new StartSorteo.ParticipanteCollection
        {
            participantes = listaParticipantes.ToArray()
        };


        return JsonUtility.ToJson(col);

    }
    
}
