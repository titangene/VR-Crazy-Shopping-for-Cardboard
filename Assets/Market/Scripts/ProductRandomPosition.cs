using System;
using UnityEngine;

public class ProductRandomPosition : MonoBehaviour {
    /// <summary>
    /// 載入 CabinetGroup.prefab
    /// </summary>
    private GameObject CabinetPrefab;
    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    private GameObject CabinetGroup;
    /// <summary>
    /// 商品資料 ID
    /// </summary>
    private int ProInfoId = 1;
    /// <summary>
    /// 商品物件 ID
    /// </summary>
    private int ProObjId = 1;
    /// <summary>
    /// 商品貨架放置位置
    /// </summary>
    private Vector3 V_Position;
    /// <summary>
    /// 商品貨架放置角度
    /// </summary>
    private Quaternion V_Rotation;

    void Start () {
        V_Position = new Vector3(2, 0, 0.6f);
        V_Rotation = Quaternion.Euler(0, -90, 0);

        InstantiateProduct();

        V_Position = new Vector3(2, 0, 3.13f);
        V_Rotation = Quaternion.Euler(0, -90, 0);

        InstantiateProduct();
    }

    public void InstantiateProduct() {
        // 載入 CabinetGroup.prefab
        CabinetPrefab = Resources.Load("CabinetGroup", typeof(GameObject)) as GameObject;
        // Instantiate 化商品貨架物件
        CabinetGroup = Instantiate(CabinetPrefab, V_Position, V_Rotation) as GameObject;

        // CabinetGroup 內所有的子物件
        Array Cabinet = CabinetGroup.GetComponentsInChildren(typeof(Transform));
        // 在 CabinetGroup 內找出物件名稱內有 ProObj 的物件，並將其改為 ProObjxxxx
        ChildObjectRename(Cabinet, "ProObj", ProObjId);
        // 在 CabinetGroup 內找出物件名稱內有 ProInfo 的物件，並將其改為 ProInfoxxxx
        ChildObjectRename(Cabinet, "ProInfo", ProInfoId);
    }

    /// <summary>
    /// 找出物件名稱為 ObjName，將物件名稱改為 ObjNamexxxx，
    /// EX：ProObj0001
    /// </summary>
    /// <param name="Obj">想要尋找某子物件的物件</param>
    /// <param name="ObjName">想要尋找的子物件名稱</param>
    private void ChildObjectRename(Array Obj, string ObjName, int ProductID) {
        foreach (Transform child in Obj) {
            if (child.name.Contains(ObjName)) {
                child.name = ObjName + ProductID.ToString().PadLeft(4, '0');
                ProductID++;
            }
        }
    }

    void Update () {
	    
	}
}
