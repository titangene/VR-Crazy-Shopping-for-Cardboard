using UnityEngine;
using System.Collections;
using System;

public class TakeAndThrowProduct : MonoBehaviour {
    // 找到 Head 物件
    public Transform Head;
    // 找到 GvrViewer
    public GvrViewer GvrMain;
    // 找到商品物件
    public Transform Product;
    // 是否拿取商品
    public bool Taking = false;

    // 找到 class CardboardControl
    private static CardboardControl cardboard;
    // 準心對準的物體
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;
    // 角色
    private CharacterController controller;
    // 紀錄商品與人物角色的距離
    private float Product_Player = 1.3f;
    // 紀錄商品 X 座標
    private float Product_X;
    // 紀錄商品 Y 座標
    private float Product_Y;
    // 紀錄商品 Z 座標
    private float Product_Z;

    Rigidbody RB;

    void Start () {
        // 找到 Head 物件
        Head = GvrMain.transform.FindChild("Head");
        // 找到 CharacterController
        controller = GetComponent<CharacterController>();
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        cardboard.trigger.OnClick += CardboardClick;
        // 準心改變對準的物體時
        cardboard.gaze.OnChange += CardboardGazeChange;
        // 準心持續對準某個物體時
        cardboard.gaze.OnStare += CardboardStare;
    }

    // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    private void CardboardClick(object sender) {
        // 準心對準商品時 gaze.IsHeld() = true，準心沒有對準時 = false
        // 商品物件名稱為 ProObjxxxx (xxxx 為邊號，EX：ProObj0001)
        if (gaze.IsHeld() && GazeObjectName.Contains("ProObj")) {
            // 按第一次 Gvr 按鈕，商品會跟著玩家頭部方向移動
            if (Taking == false) {
                Debug.Log("Taking");
                // 將 是否拿取商品 狀態改成 true
                Taking = true;
                // 商品跟著玩家頭部方向移動
                TakeProduct();
            // 按第二次 Gvr 按鈕，將商品丟出
            } else if (Taking == true) {
                Debug.Log("Throw");
                // 將 是否拿取商品 狀態改成 false
                Taking = false;
                // 將商品丟出
                ThrowProduct();
            }
        }
    }

    // 準心改變對準的物體時
    private void CardboardGazeChange(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        Debug.Log(GazeObjectName);
    }

    // 準心持續對準某個物體時
    private void CardboardStare(object sender) {
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        Debug.Log(GazeObjectName);
    }

    void Update () {
        //ProductMove();
        //Debug.Log(Product_Y);
    }

    // 商品跟著玩家頭部方向移動
    private void TakeProduct() {
        // 找到商品物件
        //Product = GameObject.Find(GazeObjectName).transform;
        Product = GameObject.Find("ProObj0007").transform;
        // 攝影機 X 軸旋轉角度
        float Camera_AngleX = Head.transform.eulerAngles.x;
        // Debug
        if (Camera_AngleX > 270.0f) {
            Camera_AngleX = 90 - (Camera_AngleX - 270);
        }
        // 攝影機 Y 軸旋轉角度
        float Camera_AngleY = Head.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        // 計算 X_Z 弧度
        float thetaXZ = Camera_AngleY * Mathf.Deg2Rad;
        // 計算 Y_Z 弧度 (正負轉換)
        float thetaYZ = Camera_AngleX * - Mathf.Deg2Rad;
        // 紀錄購物車 X 座標：x = r * sin(thita)
        Product_X = Product_Player * Mathf.Sin(thetaXZ);
        // 紀錄購物車 Y 座標：y = r * sin(thita)
        Product_Y = Product_Player * Mathf.Cos(thetaYZ);
        // 紀錄購物車 Z 座標：z = r * cos(thita)
        Product_Z = Product_Player * Mathf.Cos(thetaXZ);
        // 購物車會跟著玩家的視角移動位置
        Product.position = new Vector3(Product_X + transform.position.x,
                                       Product_Y,
                                       Product_Z + transform.position.z);
        // 購物車會跟著玩家的視角旋轉角度
        Product.rotation = Quaternion.Euler(0, Camera_AngleY, 0);

        Debug.Log(Camera_AngleX + ", " + Product_Y);
    }

    // 將商品丟出
    private void ThrowProduct() {

    }


    // 商品跟著玩家頭部方向移動
    private void TakeProduct1() {
        Product = GameObject.Find(GazeObjectName).transform;
        //if (!take) {
        //Product.position = new Vector3(0, 1.6f, 1.3f);
        //take = true;
        //}
        // 將物體放在 Head 物件內(子類別)
        Product.parent = Head;
        // 找到當前物體的鋼體
        RB = Product.GetComponent<Rigidbody>();
        // 關閉物體的重力
        RB.useGravity = false;
        // 鎖定物理效果影響物體的旋轉和移動
        RB.constraints = RigidbodyConstraints.FreezeAll;
    }

    // 將商品丟出
    private void ThrowProduct1() {
        // 將物體放在 Head 物件內(子類別)
        Product.parent = null;
        // 找到當前物體的鋼體
        RB = Product.GetComponent<Rigidbody>();
        // 開啟物體的重力
        RB.useGravity = true;
        // 解除物理效果影響物體旋轉和移動的鎖定
        RB.constraints = RigidbodyConstraints.None;
    }
}
