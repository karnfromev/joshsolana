using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SolanaWalletInfo 
{
    private static string accountAddress;

    public static string AccountAddress { get => accountAddress; set => accountAddress = value; }
}