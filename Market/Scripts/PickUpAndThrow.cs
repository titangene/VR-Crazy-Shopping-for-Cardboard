using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PickUpAndThrow : MonoBehaviour {
    // 找到 Head 物件
    public Transform Head;
    // 找到 GvrViewer
    public GvrViewer GvrMain;
    // 往準心方向丟出物體的速度
    public float ThrowSpeed = 8.0f;
    // 商品自轉的速度
    public float RotateSpeed = 1.5f;
    // 商品與人物角色的距離要在此範圍內才可以拿取商品
    public float PickUpRange = 3.2f;
    // 商品與人物角色的距離是否超過設定範圍
    public bool PickUpOverRange = false;
    // 是否拿取商品
    public bool PickingUp = false;

    // 找到商品物件
    private Transform Product;
    // 找到 CardboardControl
    private static CardboardControl cardboard;
    // 準心對準的物體物件
    private static CardboardControlGaze gaze;
    // 準心對準的物體名稱
    private string GazeObjectName;
    // 角色
    private CharacterController controller;
    // 找到商品物件的 EventTrigger
    private EventTrigger ProductTrigger;
    // 計算商品與人物角色的距離
    private Vector3 Product_Player_Distance;
    // 紀錄商品與人物角色的距離
    private float Product_Player;
    // 拿取商品時，固定商品與人物角色為此距離
    private float Product_Player_Setting = 1.3f;
    // 紀錄商品 X 座標
    private float Product_X;
    // 紀錄商品 Y 座標
    private float Product_Y;
    // 紀錄商品 Z 座標
    private float Product_Z;
    // 某物體的剛體
    private Rigidbody RB;

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

    // 準心改變對準的物體時
    private void CardboardGazeChange(object sender) {
        // 檢查商品與人物角色的距離是否在設定範圍內
        PickUpCheck(sender);
    }

    // 準心持續對準某個物體時
    private void CardboardStare(object sender) {
        // 檢查商品與人物角色的距離是否在設定範圍內
        PickUpCheck(sender);
    }

    // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    private void CardboardClick(object sender) {
        // 準心對準商品時 gaze.IsHeld() = true，準心沒有對準時 = false
        // 商品物件名稱為 ProObjxxxx (xxxx 為邊號，EX：ProObj0001)
        if (GazeCheck()) {
            // 按第一次 Gvr 按鈕，商品會跟著玩家頭部方向移動
            if (PickingUp == false && PickUpOverRange == false) {
                //Debug.Log("Taking");
                // 將 是否拿取商品 狀態改成 true
                PickingUp = true;

            // 按第二次 Gvr 按鈕，將商品丟出
            } else if (PickingUp == true) {
                //Debug.Log("Throw");
                // 將 是否拿取商品 狀態改成 false
                PickingUp = false;
                // 將商品丟出
                ThrowProduct();
            
            // 商品與人物角色的距離超過設定範圍
            } else if (PickUpOverRange == true) {
                Debug.Log("商品超過可拿取範圍");
            }
        }
    }

    void Update () {
        // 是否按一下 Gvr 按鈕拿取商品
        if (PickingUp == true) {
            // 商品跟著玩家頭部方向移動
            PickUpProduct();
        }
    }

    // 檢查商品與人物角色的距離是否在設定範圍內，如果有可拿取商品，沒有則無
    private void PickUpCheck(object sender) {
        // 準心對準的物體
        gaze = sender as CardboardControlGaze;
        // Debug 如果準心沒有對準任何東西，會設定對準目標名稱 = nothing
        GazeObjectName = gaze.IsHeld() ? gaze.Object().name : "nothing";
        //Debug.Log(GazeObjectName);

        // Debug 準心沒有對準任何物體(nothing)
        // 檢查準心是否對準物體 && 準心對準的物體名稱是否為 ProObjxxxx
        if (GazeCheck()) {
            // 找到商品物件
            Product = GameObject.Find(GazeObjectName).transform;
            // 找到商品物件的 EventTrigger
            ProductTrigger = Product.GetComponent<EventTrigger>();
        }

        // 檢查準心是否對準物體 && 準心對準的物體名稱是否為 ProObjxxxx
        if (GazeCheck()) {            
            // 更新商品與人物角色的距離
            Product_Player_Distance = Product.position - transform.position;
            // 紀錄商品與人物角色的距離
            Product_Player = Mathf.Sqrt(Mathf.Pow(Product_Player_Distance.x, 2) +
                                        Mathf.Pow(Product_Player_Distance.z, 2));
            //Debug.Log(Product_Player);

            // 如果商品與人物角色的距離在設定範圍內
            // 準心對準商品時，準心不會放大，而且不可拿取商品
            if (Product_Player > PickUpRange) {
                // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 true
                PickUpOverRange = true;
                // 準心不會放大
                ProductTrigger.enabled = false;
            } else {
                // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
                PickUpOverRange = false;
                // 準心會放大
                ProductTrigger.enabled = true;
            }
        } else {
            // 將 商品與人物角色的距離是否超過設定範圍 狀態改成 false
            PickUpOverRange = false;
            // Debug 準心沒有對準任何物體(nothing) 或 對準沒有使用 EventTrigger 的物體
            // 檢查準心是否對準物體 && 準心對準的物體名稱是否為 ProObjxxxx
            if (GazeCheck()) {
                // 準心會放大
                ProductTrigger.enabled = true;
            }
        }
    }

    // 檢查準心是否對準物體 && 準心對準的物體名稱是否為 ProObjxxxx
    // 準心對準商品時 gaze.IsHeld() = true，準心沒有對準時 = false
    // 商品物件名稱為 ProObjxxxx (xxxx 為邊號，EX：ProObj0001)
    private bool GazeCheck() {
        return gaze.IsHeld() && GazeObjectName.Contains("ProObj");
    }

    // 商品跟著玩家頭部方向移動
    private void PickUpProduct() {
        // 找到當前物體的鋼體
        RB = Product.GetComponent<Rigidbody>();
        // 關閉物體的重力
        RB.useGravity = false;
        // 鎖定物理效果影響物體的旋轉和移動
        RB.constraints = RigidbodyConstraints.FreezeAll;
        // 用右手定則，大拇指往 X 軸指，Y 軸與 Z 軸朝其他手指的指向旋轉 phi 角值
        // 攝影機 X 軸旋轉角度 (Y 與 Z 旋轉的 phi 角)
        float Camera_AngleX = Head.transform.eulerAngles.x;
        // 用右手定則，大拇指往 Y 軸指，X 軸與 Z 軸朝其他手指的指向旋轉 theta 角值
        // 攝影機 Y 軸旋轉角度 (X 與 Z 旋轉的 theta 角)
        float Camera_AngleY = Head.transform.eulerAngles.y;
        // Mathf.Deg2Rad 度轉弧度 = (PI * 2) / 360
        // 計算 X_Z 弧度
        float Theta = Camera_AngleY * Mathf.Deg2Rad;
        // 計算 Y_Z 弧度 (正負轉換)
        float Phi = Camera_AngleX * - Mathf.Deg2Rad;
        // 紀錄商品 X 座標：x = r * cos(Phi) * sin(Theta)
        Product_X = Product_Player_Setting * Mathf.Cos(Phi) * Mathf.Sin(Theta);
        // 紀錄商品 Y 座標：y = r * sin(Phi)
        Product_Y = Product_Player_Setting * Mathf.Sin(Phi);
        // 紀錄商品 Z 座標：z = r * cos(Phi) * cos(Theta)
        Product_Z = Product_Player_Setting * Mathf.Cos(Phi) * Mathf.Cos(Theta);
        // 商品會跟著玩家的視角移動位置
        Product.position = new Vector3(Product_X + GvrMain.transform.position.x,
                                       Product_Y + GvrMain.transform.position.y,
                                       Product_Z + GvrMain.transform.position.z);
        // 商品會跟著玩家的視角旋轉角度
        //Product.rotation = Quaternion.Euler(Camera_AngleX, Camera_AngleY, 0);
        // 商品會以每 FPS 固定角度自轉
        Product.Rotate(new Vector3(-30, -30, 30) * Time.deltaTime * RotateSpeed);
    }

    // 將商品丟出
    private void ThrowProduct() {
        // 開啟物體的重力
        RB.useGravity = true;
        // 解除物理效果影響物體旋轉和移動的鎖定                           
        RB.constraints = RigidbodyConstraints.None;
        // 往準心方向丟出物體
        RB.velocity = Head.forward * ThrowSpeed;
    }
}
