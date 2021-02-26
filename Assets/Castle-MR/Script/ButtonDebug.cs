using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonDebug : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() {
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
    }

    public void OnClick()
    {
        Debug.Log("=====>DebugLog : Button " + gameObject.name + " is working");
    }

    public void ChangeLanguage(){
        
    }
}
