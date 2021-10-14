using System;
using UnityEngine;
using UnityEngine.UI;

public class PreviousMultiplierHandler : MonoBehaviour {

    private GameObject MessageUI;
    
    [Header("Properties")]
    public string Hash;
    public double crashPoint;
    private void Start() {
        MessageUI = transform.root.GetComponent<Crash>().HashInfo;
    }

    public void OnClick() {
        MessageUI.SetActive(true);
        SetInfo();
    }

    void SetInfo() {
        MessageUI.transform.GetChild(0).GetChild(1).GetChild(0).transform.GetComponent<Text>().text = Hash;
        MessageUI.transform.GetChild(0).GetChild(2).GetChild(0).transform.GetComponent<Text>().text = crashPoint.ToString();
    }
}
