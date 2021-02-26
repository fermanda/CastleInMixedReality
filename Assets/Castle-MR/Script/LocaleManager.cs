using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Networking;

public class LocaleManager : MonoBehaviour
{
    public string CurrentLanguage = "Japanese";
    public Dictionary<string, string> localeText;
    private IEnumerator coroutine;
    
    private void Awake() {
        LoadLocalizedText();
    }

    public void ChangeLanguage(){
        if(CurrentLanguage == "English") CurrentLanguage = "Japanese";
        else CurrentLanguage = "English";
        LoadLocalizedText();
    }

    public void LoadLocalizedText(){
        localeText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, CurrentLanguage+".json");

#if UNITY_ANDROID && !UNITY_EDITOR
        var path = "jar:file://" + Application.dataPath + "!/assets/" + CurrentLanguage+".json";
#else
        var path = filePath;
#endif
        coroutine = GetLanguagePack(path);
        StartCoroutine(coroutine);
    }

    private IEnumerator GetLanguagePack(string path){
        var loadingRequest = UnityWebRequest.Get(path);
        Debug.Log("=====>DebugLog : " + path);
        loadingRequest.SendWebRequest();
        while (!loadingRequest.isDone) {
            yield return null;
        }
        
        if (loadingRequest.downloadHandler.data != null  && loadingRequest.downloadHandler.data.Length > 0){
            Debug.Log("=====>DebugLog : Data Received");
        }
        else{
            Debug.Log("=====>DebugLog : Data is Empty");
        }

        string JSONdata = loadingRequest.downloadHandler.text;
        LocalizationData loadeddata = JsonUtility.FromJson<LocalizationData>(JSONdata);
        for (int i = 0; i < loadeddata.items.Length; i++){
            localeText.Add(loadeddata.items[i].key, loadeddata.items[i].value);
        }

        Debug.Log("Locale text data contains " + localeText.Count + " entries");
        Debug.Log(localeText);
        UpdateLanguage();
        yield break;
    }

    public void UpdateLanguage(){
        foreach(KeyValuePair<string, string> entry in localeText){
            Debug.Log("Key: " + entry.Key + ", Value: " + entry.Value);
            GameObject.Find(entry.Key).GetComponentInChildren<TextMeshProUGUI>().text = entry.Value;
        }
    }
}
