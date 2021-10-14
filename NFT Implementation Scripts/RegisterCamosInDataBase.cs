using System;
using UnityEngine;

public class RegisterCamosInDataBase : MonoBehaviour {
    private bl_CamoTypes _blCamoTypes = new bl_CamoTypes();

    public void RegisterToDB(String keyName) {
        bl_ShopItemData data = new bl_ShopItemData();
        bl_ShopPurchase sp = new bl_ShopPurchase();

        data.ID = 1;
        data.Type = ShopItemType.WeaponCamo;
        
        sp.ID = _blCamoTypes.camoInfo[keyName];
        sp.TypeID = (int) data.Type;
        
        if (!bl_DataBase.Instance.LocalUser.ShopData.ShopPurchases.Contains(sp)) {
            Debug.Log("DATABASE DOESN'T CONTAINS " + keyName + "<-- ADD IT");
            bl_DataBase.Instance.LocalUser.ShopData.AddPurchase(sp);
        }

    }

    public void RemoveToDB(String keyName) {
        bl_ShopItemData data = new bl_ShopItemData();
        bl_ShopPurchase sp = new bl_ShopPurchase();

        data.ID = 1; // Camo id = 1
        data.Type = ShopItemType.WeaponCamo;

        sp.ID = _blCamoTypes.camoInfo[keyName];
        sp.TypeID = (int) data.Type;

        if (bl_DataBase.Instance.LocalUser.ShopData.ShopPurchases.Contains(sp)) {
            Debug.Log("DATABASE CONTAINS " + keyName + "<-- REMOVED IT");
            bl_DataBase.Instance.LocalUser.ShopData.RemovePurchase(sp);
        }
    }

    public void RemoveAllDB() {
        foreach (var items in _blCamoTypes.camoInfo) {
            RemoveToDB(items.Key);
        }

        Debug.Log("ALL CAMOS REMOVED");
    }
}