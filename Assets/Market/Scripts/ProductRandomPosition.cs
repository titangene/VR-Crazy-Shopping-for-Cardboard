using System;
using UnityEngine;

public class ProductRandomPosition : MonoBehaviour {
    /// <summary>
    /// 每排有幾列
    /// </summary>
    public int col = 6;
    /// <summary>
    /// 一個商品貨架有幾個商品
    /// </summary>
    public int ProductCount = 9;
    /// <summary>
    /// 第一列 Z 軸的位置
    /// </summary>
    public float First_Z = 5.23f;
    /// <summary>
    /// 第二列 Z 軸開始的位置
    /// </summary>
    private float Second_Z;
    /// <summary>
    /// 最後一列 Z 軸的位置
    /// </summary>
    private float Final_Z;
    /// <summary>
    /// 第一列與第二列之間的距離
    /// </summary>
    public float First_Second_D = 1.456f;
    /// <summary>
    /// 兩排之間的距離
    /// </summary>
    public float TwoCol_D = 2.104f;
    /// <summary>
    /// 相鄰兩列之間的距離
    /// </summary>
    public float TwoRow_D = 0.776f;
    /// <summary>
    /// 不同區域(不相鄰的列)之間的距離
    /// </summary>
    public float TwoArea_D = 6f;


    /// <summary>
    /// 載入 CabinetGroup.prefab
    /// </summary>
    private GameObject CabinetPrefab;
    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    private GameObject CabinetGroup;
    /// <summary>
    /// 商品 ID
    /// </summary>
    private int ProductId = 1;
    /// <summary>
    /// 商品貨架放置位置
    /// </summary>
    private Vector3 V_Position;
    /// <summary>
    /// 商品貨架放置角度
    /// </summary>
    private Quaternion V_Rotation;

    void Start () {
        // 建立第 1 列商品貨價
        SetCabinetPosition( 1, -1 * TwoRow_D - TwoArea_D, First_Z, 180);
        SetCabinetPosition( 1, 0f,  First_Z, 180);
        SetCabinetPosition( 1, TwoRow_D + TwoArea_D, First_Z, 180);
        // 計算第二列 Z 軸開始的位置
        Second_Z = First_Z + First_Second_D;
        // 建立第 2~6 列商品貨價
        SetCabinetPosition(col, -2 * TwoRow_D - TwoArea_D, Second_Z, -90);
        SetCabinetPosition(col, -1 * TwoArea_D, Second_Z, 90);
        SetCabinetPosition(col, -1 * TwoRow_D, Second_Z, -90);
        SetCabinetPosition(col, TwoRow_D, Second_Z, 90);
        SetCabinetPosition(col, TwoArea_D, Second_Z, -90);
        SetCabinetPosition(col, 2 * TwoRow_D + TwoArea_D, Second_Z, 90);
        // 計算最後一列 Z 軸的位置
        Final_Z = Second_Z + TwoCol_D * (col - 1) + First_Second_D;
        // 建立第 7 列商品貨價
        SetCabinetPosition( 1, -1 * TwoRow_D - TwoArea_D, Final_Z, 0);
        SetCabinetPosition( 1,      0f, Final_Z, 0);
        SetCabinetPosition( 1, TwoRow_D + TwoArea_D, Final_Z, 0);
    }

    /// <summary>
    /// 設定商品貨價的數量、位置、角度
    /// </summary>
    /// <param name="count">數量</param>
    /// <param name="position_X">X 軸位置</param>
    /// <param name="position_Z">Z 軸位置</param>
    /// <param name="rotation_Y">Y 軸角度</param>
    public void SetCabinetPosition(int count, float position_X, float position_Z, float rotation_Y) {
        for (int i = 0; i < count; i++) {
            // 設定商品貨架的位置、角度 (X 軸位置、Z 軸位置、Y 軸角度)
            V_Position = new Vector3(position_X, 0, position_Z);
            V_Rotation = Quaternion.Euler(0, rotation_Y, 0);

            // Instantiate 化商品貨架物件
            InstantiateProduct();

            position_Z += TwoCol_D;
        }
    }

    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    public void InstantiateProduct() {
        // 載入 CabinetGroup.prefab
        CabinetPrefab = Resources.Load("Cabinet", typeof(GameObject)) as GameObject;
        // Instantiate 化商品貨架物件
        CabinetGroup = Instantiate(CabinetPrefab, V_Position, V_Rotation) as GameObject;

        // CabinetGroup 內所有的子物件
        Array Cabinet = CabinetGroup.GetComponentsInChildren(typeof(Transform));

        // 在 CabinetGroup 內找出物件名稱內有 Pro_Price 的物件，並將其改為 Pro_Pricexxxx
        ChildObjectRename(Cabinet, "Pro_Price");
        // 在 CabinetGroup 內找出物件名稱內有 Pro_Name 的物件，並將其改為 Pro_Namexxxx
        ChildObjectRename(Cabinet, "Pro_Name");
        // 在 CabinetGroup 內找出物件名稱內有 Pro_Position 的物件，並將其改為 Pro_Positionxxxx
        ChildObjectRename(Cabinet, "Pro_Position");
        // 在 CabinetGroup 內找出物件名稱內有 Pro_Obj 的物件，並將其改為 Pro_Objxxxx
        ChildObjectRename(Cabinet, "Pro_Obj");

        ProductId += ProductCount;
    }

    /// <summary>
    /// 找出物件名稱為 ObjName，將物件名稱改為 ObjNamexxxx，
    /// EX：ProObj0001
    /// </summary>
    /// <param name="Obj">想要尋找某子物件的物件</param>
    /// <param name="ObjChildName">想要尋找的子物件名稱</param>
    private void ChildObjectRename(Array Obj, string ObjChildName) {
        foreach (Transform child in Obj) {
            if (child.name.Contains(ObjChildName)) {
                child.name = ObjChildName + ProductId.ToString().PadLeft(4, '0');
                ProductId++;
            }
        }

        ProductId -= ProductCount;
    }
}
