using UnityEngine;
using UnityEngine.UI;

public class WalletManager : MonoBehaviour {
    public enum MyEnum {
        Add,
        Substract
    }
    
    public Text totalCash;
    public static WalletManager Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
    void Start() {
        DisplayCash();
    }
    public void CashManager(float cashReceived , MyEnum operation) {
        print("Enum " + operation + "".Color("red"));
       totalCash.color = Color.white; // Reset color to white
       switch (operation) {
           case MyEnum.Add:
               JoshTokenWallet.AddCash(cashReceived);
               break;
           case MyEnum.Substract:
               JoshTokenWallet.SubtractCash(cashReceived);
               break;
       }
        DisplayCash();
    }
    void DisplayCash() {
        Debug.Log("Get Cash is Called");
        totalCash.text = JoshTokenWallet.GetCash().ToString("F2") + " J.O.S.H Tokens";
    }

}
