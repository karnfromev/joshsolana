using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using AllArt.Solana.Example;
using UnityEngine;
using UnityEngine.Serialization;


public class NftManagerSolana : MonoBehaviour {

    public class Response {
        public string image;
    }


    public async void Start() {
        string accountAddress =  SolanaWalletInfo.AccountAddress; // Solana
        Debug.Log(accountAddress);
        RegisterCamosInDataBase registerCamosInDataBase = new RegisterCamosInDataBase();
        registerCamosInDataBase.RemoveAllDB();


        for (int i = 0; i < bl_SolanaTokenAddress.NFTsInfo.Count; i++) {
            string mintAddress = bl_SolanaTokenAddress.NFTsInfo.ElementAt(i).Value;
            string mintName =  bl_SolanaTokenAddress.NFTsInfo.ElementAt(i).Key;
            
            

           int balance = await SimpleWallet.instance.GetBalance(accountAddress, mintAddress);


            Debug.Log(mintName + " is available " + balance);


            if (balance > 0) {
                registerCamosInDataBase.RegisterToDB(mintName);
                Debug.Log(mintName + " IS ADDED TO DATABASE");
            }
            else {
                registerCamosInDataBase.RemoveToDB(mintName);
                Debug.Log(mintName + " IS REMOVED FROM DATABASE");

            }
        }
    }
}
