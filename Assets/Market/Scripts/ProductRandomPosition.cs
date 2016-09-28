using System;
using UnityEngine;

public class ProductRandomPosition : MonoBehaviour {
    /// <summary>
    /// 每排有幾列
    /// </summary>
    public int col = 5;
    /// <summary>
    /// 第一列 Z 軸開始的位置
    /// </summary>
    public float FirstPosition_Z = 5.23f;
    /// <summary>
    /// 第二列 Z 軸開始的位置
    /// </summary>
    public float SecondPosition_Z = 7;
    /// <summary>
    /// 最後一列 Z 軸開始的位置
    /// </summary>
    public float FinalPosition_Z = 18.9f;


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
        SetCabinetPosition(  1, -7.757f,  FirstPosition_Z, 180);
        SetCabinetPosition(  1,      0f,  FirstPosition_Z, 180);
        SetCabinetPosition(  1,  7.757f,  FirstPosition_Z, 180);
        // 建立第 2~6 列商品貨價
        SetCabinetPosition(col, -8.514f, SecondPosition_Z, -90);
        SetCabinetPosition(col,     -7f, SecondPosition_Z,  90);
        SetCabinetPosition(col, -0.757f, SecondPosition_Z, -90);
        SetCabinetPosition(col,  0.757f, SecondPosition_Z,  90);
        SetCabinetPosition(col,      7f, SecondPosition_Z, -90);
        SetCabinetPosition(col,  8.514f, SecondPosition_Z,  90);
        // 建立第 7 列商品貨價
        SetCabinetPosition(  1, -7.757f,  FinalPosition_Z,   0);
        SetCabinetPosition(  1,      0f,  FinalPosition_Z,   0);
        SetCabinetPosition(  1,  7.757f,  FinalPosition_Z,   0);
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

            position_Z += 2.53f;
        }

        position_Z = 7;
    }

    /// <summary>
    /// Instantiate 化商品貨架物件
    /// </summary>
    public void InstantiateProduct() {
        // 載入 CabinetGroup.prefab
        CabinetPrefab = Resources.Load("CabinetGroup", typeof(GameObject)) as GameObject;
        // Instantiate 化商品貨架物件
        CabinetGroup = Instantiate(CabinetPrefab, V_Position, V_Rotation) as GameObject;

        // CabinetGroup 內所有的子物件
        Array Cabinet = CabinetGroup.GetComponentsInChildren(typeof(Transform));
        // 在 CabinetGroup 內找出物件名稱內有 ProObj 的物件，並將其改為 ProObjxxxx
        ChildObjectRename(Cabinet, "ProObj");
        // 在 CabinetGroup 內找出物件名稱內有 ProInfo 的物件，並將其改為 ProInfoxxxx
        ChildObjectRename(Cabinet, "ProInfo");

        ProductId += 6;
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

        ProductId -= 6;
    }
}
