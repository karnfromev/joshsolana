using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;


public class Crash : MonoBehaviour {
    public Queue<String> hashString = new Queue<string>();

    public long number = 10000;

    public Text Timer;
    public Text HashValue;

    [FormerlySerializedAs("totalTime")] public float waitTime = 5;

    private int _count;
    private int count;
    public int queueLimit = 25;

    public GameObject LoadingScreenUI;
    public GameObject RocketUI;
    public GameObject RocketPrefab;
    private GameObject rocket;
    public GameObject HashInfo;

    public bool isCrashed;
    public bool isGame;

    [HideInInspector] public float _timer = 1;
    [Header("Hash to be generated")] public double hashValue;

    public float animationSpeed = 1;

    public GameObject parentPreviousMultiplier;
    public GameObject prevPrefav;

    HMAC _hMac;

    string _gameHash = "23dd1a36f3bfb43c6e1b801a342a1b08f0b11a3e0bb4432c2218962b2b3268c9";
    string salt = "0000000000000000000fa3b65e43e4240d71762a5bf397d5304b2596d116859c";

    public static Crash Instance;

    private float timer;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    } //Create the Instance

    private void Start() {
        timer = waitTime;
    } // Assign Time

    public void CreatHash() {
        Debug.Log("Hash -->" + BuildHash(_gameHash) + "Value -->" + GetHashResult(_gameHash, salt));

        if (hashString.Contains(_gameHash)) {
            count++;
            Debug.LogError("Count " + count);
        }
    } // Generate Hash

    void PushHash(string item) {
        if (hashString.Count == queueLimit) {
            print("DEQUEUE".Bold().Color("red"));
            hashString.Dequeue(); // Remove first hash Enqueue in Queue
            Destroy(parentPreviousMultiplier.transform.GetChild(0).gameObject);
        }

        print("QUEUE".Bold().Color("green"));
        hashString.Enqueue(item);
        print("UpdateUserInfoOnCrash  " +  CrashUI.Instance._totalCashInRound);
    } // Stores Hash Upto 100

    private void Update() {
        if (!isGame) {
            waitTime -= Time.deltaTime;
            string round = waitTime.ToString("F2"); // Round value to 2 decimal
            Timer.text = round.ToString();

            if (waitTime <= 0) {
                if (_count != number) {
                    _count++;
                    CreatHash();

                    if (CrashUI.Instance.betisPlaced) {
                        CrashUI.Instance.CashOut();
                    }
                    else {
                        CrashUI.Instance.Starting("Place a Bet");
                    }
                    waitTime = timer;
                    UiController(false);
                    rocket = Instantiate(RocketPrefab, RocketUI.transform, true); // Spawn rocket4
                    rocket.GetComponent<Animator>().speed = animationSpeed;
                    rocket.SetActive(true);
                }
            }
        }


        if (isGame) {
            if (!isCrashed) {
                hashValue = GetHashResult(_gameHash, salt);
                _timer += Time.deltaTime * ((float) hashValue / 10);
                string round = _timer.ToString("F2");
                HashValue.text = round + "x";

                if (_timer >= hashValue) {
                    _timer = 1;
                    isCrashed = true;
                    HashValue.text = hashValue + "x";
                    PushHash(_gameHash); // Push Hash in Queue on Crash
                    OnCrash();
                    StartCoroutine(Wait());
                    CrashUI.Instance.InteractController(false);
                }
            }
        }
    }

    void UiController(bool isActive) {
        LoadingScreenUI.SetActive(isActive);
        RocketUI.SetActive(!isActive);
        isGame = !isActive;
        if (!isGame) {
            CrashUI.Instance.ResetBet();// Reset all to default when round get reset
        }
    } // Switches UI 

    void OnCrash() {
        CrashUI.Instance.Starting("Restarting...");
        CrashUI.Instance.StopCoroutines(); // Stop Coroutine in CrashUI after the crashed is placed
        HashValue.color = Color.red; // Change text to red when hash value is reached
        rocket.GetComponent<SpriteRenderer>().sprite = null; // Change image rocket to boom
        rocket.transform.GetChild(2).gameObject.SetActive(true); // Boom on
        rocket.transform.eulerAngles = Vector3.zero; // Reset Rotation
        rocket.GetComponent<Animator>().enabled = false; // Disable rocket animation
        rocket.transform.GetChild(1).gameObject.SetActive(false); // Set off Particle Effect 
        Destroy(rocket, 2f); // Destroy rocket after 2 sec
    } // Calls when Rocket get crash

    IEnumerator Wait() {
        yield return new WaitForSeconds(5f);
        CrashUI.Instance._totalCashInRound = 0;
        
        GameObject prevMulti = Instantiate(prevPrefav, parentPreviousMultiplier.transform, false);

        PreviousMultiplierHandler multiplierPrefab = prevMulti.GetComponent<PreviousMultiplierHandler>();
        multiplierPrefab.Hash = _gameHash; // Latest Value Enqueue in Queue
        multiplierPrefab.crashPoint = GetHashResult(_gameHash, salt);

        Text text = prevMulti.transform.GetChild(0).GetComponent<Text>();
        text.text = hashValue + "x";

        if (hashValue > 2) {
            text.color = Color.green;
        }
        else if (hashValue < 2) {
            text.color = Color.red;
        }
        else {
            text.color = Color.white;
        }

        hashValue = 0;
        UiController(true);
        HashValue.color = Color.white;
        isCrashed = false;
        

        for (int i = 0; i < CrashUI.Instance.userInfo.Count; i++) {
            Destroy(CrashUI.Instance.userInfo[i]);
        }

    } // Wait to restart the game After rocket is crash to update the wallet information

    #region Old Hash Function

    public double get_result(string gameHash, string salt) {
        UTF8Encoding encoder = new UTF8Encoding();
        Byte[] gameHashb = encoder.GetBytes(gameHash);
        Byte[] saltb = encoder.GetBytes(salt);
        HMACSHA256 hmSha1 = new HMACSHA256(gameHashb);
        Byte[] hashMe = encoder.GetBytes("");
        hmSha1.TransformBlock(saltb, 0, saltb.Length, null, 0);
        Byte[] hmBytes = hmSha1.ComputeHash(hashMe);
        string h = ToHexString(hmBytes);
        BigInteger check = (BigInteger.Parse(h, System.Globalization.NumberStyles.HexNumber) % 33);
        if (check == 0) {
            return 1f;
        }

        BigInteger check2 = (BigInteger.Parse(h.Substring(0, 13), System.Globalization.NumberStyles.HexNumber));
        double c = (double) check2;
        var e = Math.Pow(2, 52);
        return (Mathf.Floor((float) ((100 * e - c) / (e - c) / 1f)) / 100.0);
    }


    public String Get_Hash(string hashCode) {
        UTF8Encoding encoder = new UTF8Encoding();
        Byte[] hash = encoder.GetBytes(hashCode);
        SHA256 hMACSHA256 = SHA256.Create();
        Byte[] hmBytes = hMACSHA256.ComputeHash(hash);
        hMACSHA256.TransformBlock(hash, 0, hash.Length, null, 0);
        _gameHash = ToHexString(hmBytes);
        return _gameHash;
    }

    public string GenerateHash(string rawData) {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create()) {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString("x2"));
            }


            _gameHash = builder.ToString();
            return _gameHash;
        }
    }

    public static string ToHexString(byte[] array) {
        return String.Concat(Array.ConvertAll(array, x => x.ToString("x2")));
    }

    #endregion

    #region New Hash Function

    double GetHashResult(string rawData, string salt) {
        byte[] keyByte = new ASCIIEncoding().GetBytes(rawData);
        byte[] messageBytes = new ASCIIEncoding().GetBytes(salt);
        byte[] hashmessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);

        String.Concat(Array.ConvertAll(hashmessage, x => x.ToString("x2")));

        int hs = Convert.ToInt32(100 / 5);

        if (divisible(rawData, hs)) {
            return 1.00;
        }

        var h = Convert.ToInt64(rawData.Substring(0, 13), 16);
        var e = Math.Pow(2, 52);
        return Math.Floor((100 * e - h) / (e - h)) / 100.0;
    }

    bool divisible(string hash, int mod) {
        var val = 0;
        var o = (hash.Length) % 4;
        for (var i = o > 0 ? o - 4 : 0; i < hash.Length; i += 4) {
            val = ((val << 16) + Convert.ToInt32(hash.Substring(i, 4), 16)) % mod;
        }

        return val == 0;
    }

    string BuildHash(string rawData) {
        byte[] messageBytes = new ASCIIEncoding().GetBytes(rawData); //ASCII Encoding Algorithm
        byte[] hashmessage = SHA256.Create().ComputeHash(messageBytes);
        _gameHash = String.Concat(Array.ConvertAll(hashmessage, x => x.ToString("x2")));
        return _gameHash;
    }

    #endregion
}