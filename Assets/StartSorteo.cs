using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSorteo : MonoBehaviour
{
    [Serializable]
    public class Participante
    {
        public string Id;
        public string Nombre;
        public string Ganador;
    }

    [Serializable]
    public class ParticipanteCollection
    {
        public Participante[] participantes;
    }

    public TMP_InputField field;
    public ParticipanteCollection collection;
    public Button button;
    public GenerateGanadoresList listaGanadores;
    public bool pulling = false;

    [Header("Canvas 2")]
    public Canvas firstCanvas;
    public Canvas nextCanvas;
    public TMP_Text winnerId, winnerName;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !pulling)
        {
            PickGanador();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/data.csv"))
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
            return;
        
        if (!File.Exists(Application.persistentDataPath + "/dataSorteo.json"))
        {
            using (StreamReader sr = File.OpenText(Application.streamingAssetsPath + "/data.csv"))
            {
                string subJson = sr.ReadToEnd();
                /*Debug.Log(subJson);
                string json = "{\"participantes\":[" + subJson.Substring(0, subJson.Length - 1) + "]}";
                Debug.Log(json);*/
                string dataJson = CSVWrapper.CSVToJSON(subJson);
                collection = JsonUtility.FromJson<ParticipanteCollection>(dataJson);
            }
            
            

            using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/dataSorteo.json"))
            {
                sw.Write(JsonUtility.ToJson(collection));
            }

            listaGanadores.GenerateList(collection);
        }
        else
        {
            ParticipanteCollection previousCol;

            using (StreamReader sr = File.OpenText(Application.persistentDataPath + "/dataSorteo.json"))
            {
                string json = sr.ReadToEnd();
                Debug.Log(json);
                previousCol = JsonUtility.FromJson<ParticipanteCollection>(json);
            }

            File.Delete(Application.persistentDataPath + "/dataSorteo.json");

            using (StreamReader sr = File.OpenText(Application.streamingAssetsPath + "/data.csv"))
            {
                string subJson = sr.ReadToEnd();
                /*Debug.Log(subJson);
                string json = "{\"participantes\":[" + subJson.Substring(0, subJson.Length - 1) + "]}";
                Debug.Log(json);*/
                string dataJson = CSVWrapper.CSVToJSON(subJson);
                collection = JsonUtility.FromJson<ParticipanteCollection>(dataJson);
                //collection = JsonUtility.FromJson<ParticipanteCollection>(json);
            }

            foreach(Participante participante in collection.participantes)
            {
                if(previousCol.participantes.Any(x=> x.Id == participante.Id))
                {
                    if(previousCol.participantes.First(x=> x.Id == participante.Id).Ganador == "true")
                    {
                        participante.Ganador = "true";
                    }
                }
            }

            using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/dataSorteo.json"))
            {
                sw.WriteLine(JsonUtility.ToJson(collection));
            }

        }

        /*if(collection == null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        else
        if(collection.participantes.Length < 10)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);*/

        listaGanadores.GenerateList(collection);
    }

    public void PickGanador()
    {
        button.interactable = false;
        pulling = true;
        //button.GetComponent<Animator>().speed = 0;
        StartCoroutine(RollAnimation());
    }

    public IEnumerator RollAnimation()
    {
        string mail = collection.participantes.Where(x => x.Ganador == "false" && !string.IsNullOrWhiteSpace(x.Id) && !string.IsNullOrEmpty(x.Id)).ElementAt(UnityEngine.Random.Range(0, collection.participantes.Count(x => x.Ganador == "false"))).Id;
        
        float secs = 0;
        do
        {
            int i = UnityEngine.Random.Range(0, collection.participantes.Length);
            field.text = collection.participantes[i].Id;
            secs += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (secs < 3);

        field.text = collection.participantes.First(x => x.Id == mail).Id;

        collection.participantes.First(x => x.Id == mail).Ganador = "true";


        using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/dataSorteo.json"))
        {
            sw.WriteLine(JsonUtility.ToJson(collection));
        }

        button.interactable = true;
        //button.GetComponent<Animator>().speed = 1;

        listaGanadores.GenerateList(collection);

        winnerId.text = collection.participantes.First(x => x.Id == mail).Id;
        winnerName.text = collection.participantes.First(x => x.Id == mail).Nombre;
        nextCanvas.enabled = true;
        firstCanvas.enabled = false;
        

        pulling = false;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

}
