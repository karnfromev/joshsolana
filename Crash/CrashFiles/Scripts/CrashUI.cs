using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CrashUI : MonoBehaviour {
    public InputField inputField;
    public InputField autoBatInputField;

    public GameObject playerInfoUi;
    public GameObject parentUserInfo;
    [Header("Message UI")] public GameObject addCashMessage;
    public GameObject betAmountMessage;
    private GameObject _temp;
    public GameObject autoCashField;
    public GameObject betAmountField;

    public Button betButton;

    [HideInInspector] public bool betisPlaced;
    private bool _userInfoUpdated;
    private bool _cashOutIsActive;
    [HideInInspector] public bool cashOutisUsed;

    public static CrashUI Instance;

    public float cashInWallet;
    private float _count;

    private Text _text, _multiPlier;
    public bool coroutineisRunning;
    public List<GameObject> userInfo = new List<GameObject>();
    public float _totalCashInRound = 0;

    private void Awake() {
        if (Instance == null)
            Instance = this;
    }

    enum MyEnum {
        Add,
        Subtract
    }


    private void Start() {
        PlayerPrefs.DeleteKey("JoshToken");
        cashInWallet = JoshTokenWallet.GetCash(); // Show cash in inceptor
        Reset(); // Default Bet
    }

    public void DoubleBet() {
        float DoubleBet = int.Parse(inputField.text) * 2;
        inputField.text = DoubleBet + "";
    }

    public void HalfBet() {
        float HalfBet = int.Parse(inputField.text) / 2;
        inputField.text = HalfBet + "";
    }

    public void MaxBet() {
        // total money in wallet
        var totalBet = JoshTokenWallet.GetCash();
        inputField.text = "" + totalBet;
    }

    IEnumerator TurnMessageoff() {
        yield return new WaitForSeconds(2f);
        addCashMessage.SetActive(false);
        betAmountMessage.SetActive(false);
    }

    public void PlaceBet() {
        if (Crash.Instance.isCrashed || _userInfoUpdated) {
            return;
        }

        if (!cashOutisUsed && betisPlaced) {
            return;
        }


        if (_temp == null && !betisPlaced && Crash.Instance.isGame) {
            //Bet is not placed, inside a game
            return;
        }

        if (autoBatInputField.text == "" || inputField.text == "") {
            // Shows message when bet amount or auto bet amount is empty
            betAmountMessage.SetActive(true);
            betAmountMessage.transform.GetChild(0).GetComponent<Text>().text =
                "Invalid Bet Amount\nor\nAuto-cash Value";
            StartCoroutine(TurnMessageoff());
            return;
        }


        if (float.Parse(autoBatInputField.text) <= 1f || float.Parse(inputField.text) <= 0f) {
            // Shows message when Bet amount is in negative or auto-bet amount is equal or less than 1
            betAmountMessage.SetActive(true);
            betAmountMessage.transform.GetChild(0).GetComponent<Text>().text =
                "Bet Amount is less than 0\nor\nAuto-cash Value is less than 1";
            StartCoroutine(TurnMessageoff());
            return;
        }

        if (_cashOutIsActive) { // Executes when check out button is active
            float multiplierValue = Crash.Instance._timer;
            float cashToAdd = float.Parse(inputField.text) * multiplierValue;
            WalletManager.Instance.CashManager(cashToAdd, WalletManager.MyEnum.Add);
            _totalCashInRound += cashToAdd;
            print(_totalCashInRound + "".Color("red"));
            _cashOutIsActive = false;
            _text.text = "+ " + cashToAdd.ToString("F2");
            _multiPlier.text = Crash.Instance._timer.ToString("F2") + "x";
            _text.color = _multiPlier.color = Color.green;
            ResetBet();
            return;
        }


        if (JoshTokenWallet.GetCash() < float.Parse(inputField.text)) {
            // Show Message when input field bet is more than cash in the wallet
            addCashMessage.SetActive(true);
            StartCoroutine(TurnMessageoff());
            return;
        }


        if (Crash.Instance.isGame) {
            // Executes when place bet is called second time within same round and shows cash out option account to multiplier 
            float timeToWait = Crash.Instance._timer;

            if (timeToWait <= 8) { // multiplier less than 8 then cash out wait time is 3
                timeToWait = 3;
            }
            else if (timeToWait > 8 && timeToWait <= 14) { // multiplier less than 14 then cash out wait time is 6
                timeToWait = 6;
            }
            else { // multiplier more than 14 then cash out wait time is 3
                timeToWait = 9;
            }

            _count = timeToWait;
            StartCoroutine(Wait());
        }

        betisPlaced = true;
        AddBet();
        if (!Crash.Instance.isGame) {
            Starting("Starting...");
        }
    }


    public IEnumerator Wait() {
        Starting("Wait.. " + _count + " sec");

        if (!coroutineisRunning) {
            coroutineisRunning = true;
        }

        yield return new WaitForSeconds(1);
        _count--;
        if (_count <= 0) {
            coroutineisRunning = false;
            CashOut(); // Show cash-out button when waiting time is over
            yield break;
        }

        StartCoroutine(Wait());
    }


    public void StopCoroutines() {
        StopAllCoroutines(); // Stop All the coroutines to prevent bugs
        coroutineisRunning = false;
    }

    public void Reset() { // Reset bet to default when game is restarted
        autoBatInputField.text = "25";
    }

    void ChangeButtonLayout(Color color, string txt, Color textColor) { // Change button color and text
        betButton.GetComponent<Image>().color = color;
        Text _text = betButton.transform.GetChild(0).GetComponent<Text>();
        _text.text = txt;
        _text.color = textColor;
    }

    private void Update() {
        if (betisPlaced && !_userInfoUpdated) {
            if (Crash.Instance._timer >= float.Parse(autoBatInputField.text) && !coroutineisRunning) {
                // Calls when auto bets meets the multiplier 
                _userInfoUpdated = true; // avoid auto cash out to use multiple times
                UpdateUserInfoOnCrash(true);
                //Profit in Game
            }
            else if (Crash.Instance.isCrashed) {
                // calls when multiplier get crashed after bet is placed or below the auto bet amount
                _userInfoUpdated = true;
                UpdateUserInfoOnCrash(false);
                //Loss in Game
            }
        }
    }
    

    public void ResetBet() { // On Second Call
        if (Crash.Instance.isGame) {
            betAmountField.SetActive(false);
        }

        _userInfoUpdated = betisPlaced = _cashOutIsActive = cashOutisUsed = false;
        ChangeButtonLayout(new Color(1, 0.7099965f, 0, 1), "Place a Bet",
            new Color(0.1698113f, 0.1698113f, 0.1698113f));
    }

    public void Starting(string message) { // Change color of button to gray
        InteractController(true);
        ChangeButtonLayout(new Color(0.1037736f, 0.1037736f, 0.1037736f, 1), message, new Color(0.5f, 0.5f, 0.5f));
    }

    public void CashOut() {
        InteractController(true);
        cashOutisUsed = _cashOutIsActive = true;
        ChangeButtonLayout(new Color(0f, 0.9f, 0, 1), "Cash Out", new Color(0.1698113f, 0.1698113f, 0.1698113f));
    }

    public void InteractController(bool condition) { // Turn on or off the interaction with the input field
        autoCashField.SetActive(condition);
        betAmountField.SetActive(condition);
    }

    void AddBet() { // Spawn user info ui 
        UpdateWalletOnBet(MyEnum.Subtract);
        _temp = Instantiate(playerInfoUi, parentUserInfo.transform, false);
        _text = _temp.transform.GetChild(1).GetComponent<Text>();
        _multiPlier = _temp.transform.GetChild(2).GetComponent<Text>();
        
        _text.text = "-" + inputField.text;
        _multiPlier.text = "Bet Amount";
        _text.color = Color.red;

        userInfo.Add(_temp);

        InteractController(true);
    }

    public void UpdateUserInfoOnCrash(bool isProfit) {
        if (isProfit) { // Profit
            _text.text = "+" + CalculateProfit();
            _multiPlier.text = autoBatInputField.text + "x";
            _text.color = _multiPlier.color = Color.green;
            WalletManager.Instance.CashManager(cashInWallet, WalletManager.MyEnum.Add);
            _totalCashInRound += cashInWallet;
            Starting("Place a Bet");
        }

        else { //Loss
            print(_totalCashInRound + "".Color("red"));
            _text.text ="-" + _totalCashInRound; // + previous all profits
            _multiPlier.text = "Loss";
            WalletManager.Instance.CashManager(_totalCashInRound, WalletManager.MyEnum.Substract);
            _text.color = Color.red;
     
        }
    }

    float CalculateProfit() { // Calculates profit on auto cash
        cashInWallet = (float.Parse(inputField.text) * float.Parse(autoBatInputField.text));
        return cashInWallet;
    }

    void UpdateWalletOnBet(MyEnum myEnum) { // Update wallet information
        switch (myEnum) {
            case MyEnum.Add:
                JoshTokenWallet.AddCash(float.Parse(inputField.text));
                break;
            case MyEnum.Subtract:
                JoshTokenWallet.SubtractCash(float.Parse(inputField.text));
                break;
        }

        WalletManager.Instance.totalCash.text = JoshTokenWallet.GetCash() + " J.O.S.H Tokens";
    }

    public void ToMainMenu() {
        SceneManager.LoadScene("MFPS/Scenes/MainMenu");
    }
}