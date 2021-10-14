using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WalletManagerSolana : MonoBehaviour {

    public InputField _InputUserName;
    public InputField _InputPassword;
    public static WalletManagerSolana Instance;
    public GetSolanaData data;
    public GameObject MessageUsernameIncorrect;
    public GameObject MessagePasswordIncorrect;
    public GameObject NoInternetCorrection;
    private bool isCoroutineStarted;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
   
    public void OnLogin() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            NoInternetCorrection.SetActive(true);
            if (!isCoroutineStarted)
                StartCoroutine(WaitOff());
            print("No Internet Connection".Color("red"));
            return;
        }

        string debug = "";
        StartCoroutine(data.GetDataFirebase(_InputUserName.text,(user)=>
        {
            if (!user.Password.Equals(_InputPassword.text)) {
                MessagePasswordIncorrect.SetActive(true);
                if (!isCoroutineStarted)
                    StartCoroutine(WaitOff());
                return;
            }
            SolanaWalletInfo.AccountAddress = user.Address;
            SceneManager.LoadScene("Login");

            
        },(check)=>
        {
            if (!check)
            {
                MessageUsernameIncorrect.SetActive(true);
                if (!isCoroutineStarted)
                    StartCoroutine(WaitOff());
                return;
            }
        }));        
    }
    public void OnSigunUp()
    {
        Application.OpenURL("https://solana-30d78.web.app");
    }
    

    IEnumerator WaitOff() {
        isCoroutineStarted = true;
        yield return new WaitForSeconds(2f);
        MessageUsernameIncorrect.SetActive(false);
        MessagePasswordIncorrect.SetActive(false);
        NoInternetCorrection.SetActive(false);
        isCoroutineStarted = false;
    }
}