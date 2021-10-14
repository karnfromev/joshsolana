using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSolanaData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
    }
    public IEnumerator GetDataFirebase(string userName,System.Action<User> action,System.Action<bool> check)
    {
        string json= "";
        bool checkString = false;
        User user = null;
        yield return RestClient.Get($"https://solana-30d78-default-rtdb.firebaseio.com/users/{userName}.json")
            .Then(response => {
                json = response.Text;
                user = JsonUtility.FromJson<User>(json);
                action(user);
                checkString = true;
                check(checkString);
                return;
            }).Catch((error)=>
            {
                Debug.LogError(error);
                checkString = false;
                check(checkString);
            });
        
    }
    
}
[System.Serializable]
public class User
{
    public string Address;
    public string Password;
    public string username;

    User(string username,string Password,string Address)
    {
        this.Address = Address;
        this.Password = Password;
        this.username = username;

    }

}