using UnityEngine;
using UnityEngine.EventSystems;

public class GCvrGaze : MonoBehaviour {
    /// <summary>
    /// 自定準心觀看範圍，預設 2.8f
    /// </summary>
    public float DistanceRange = 2.8f;
    public LayerMask mask = -1;

    /// <summary>
    /// 射線碰撞參數
    /// </summary>
    private RaycastHit hit;
    /// <summary>
    /// 準心是否對準物件
    /// </summary>
    bool hitResult = false;
    /// <summary>
    /// 無範圍限制：目前準心對準的物件(目標物件)
    /// </summary>
    private GameObject targetObj_Unlimited = null;
    /// <summary>
    /// 有範圍限制：目前準心對準的物件(目標物件)
    /// </summary>
    private GameObject targetObj_Range = null;
    /// <summary>
    /// 無範圍限制：目前準心對準物件的碰撞位置
    /// </summary>
    private Vector3 intersectPosition_Unlimited;
    /// <summary>
    /// 有範圍限制：目前準心對準物件的碰撞位置
    /// </summary>
    private Vector3 intersectPosition_Range;
    /// <summary>
    /// 目標物件與玩家的距離
    /// </summary>
    private float TargetObj_Player_Distance = 0;

    void Start() {
    }

    void LateUpdate() {
        // 有範圍限制：建立射線 與 設定目前準心對準的物件(目標物件)
        FindGazeTarget_Range();
        // 無範圍限制：建立射線 與 設定目前準心對準的物件(目標物件)
        FindGazeTarget_Unlimited();
        // 計算目標物件與玩家的距離
        Calc_TargetObj_Player_Distance();
        // 開啟 / 關閉目標物件之 EventTrigger
        ChangeTargetEventTrigger("Pro_Obj");

        if (Input.GetMouseButtonDown(0)) {
            /*
            Debug.Log(GetCurrentGameObject());
            Debug.Log("目標物件名稱：" + GetCurrentObjName_Range());
            Debug.Log("目前準心對準物件的碰撞位置：" + GetIntersectPosition_Range());

            Calc_TargetObj_Player_Distance();

            Debug.Log("玩家與目標物件的距離：" + GetTargetObj_Player_Distance());
            Debug.Log("是否超過範圍：" + IsOverRange());
            */

            // 商品與人物角色的距離超過設定範圍
            if (IsOverRange() && targetObj_Unlimited.name.Contains("Pro_Obj"))
                Debug.Log("商品超過可拿取範圍");
        }
    }

    /// <summary>
    /// 無範圍限制：建立射線 與 設定目前準心對準的物件(目標物件)
    /// </summary>
    private void FindGazeTarget_Unlimited() {
        // 射線 (射線原點 (Main Camera), 射線軸向 (Main Camera 向前))
        Ray ray = new Ray(transform.position, transform.forward);
        // 射線碰撞參數
        RaycastHit hit_U;
        // 準心是否對準物件
        bool hitResult_U = false;

        // 準心是否對準物件
        // 物理射線 (射線參數, 射線碰撞參數, 射線距離)
        hitResult_U = Physics.Raycast(ray, out hit_U, Camera.main.farClipPlane, mask);

        // 如果準心有對準物件
        if (hitResult_U) {
            // 取得目前準心對準的物件
            targetObj_Unlimited = hit_U.collider.gameObject;
            // 設定目前準心對準物件的碰撞位置
            intersectPosition_Unlimited = transform.position + transform.forward * hit_U.distance;

        // 如果準心沒有對準物件
        } else {
            // 將 目前準心對準物件的碰撞位置 重設為 Vector3(0, 0, 0)
            intersectPosition_Unlimited = Vector3.zero;
        }
    }

    /// <summary>
    /// 有範圍限制：建立射線 與 設定目前準心對準的物件(目標物件)
    /// </summary>
    private void FindGazeTarget_Range() {
        // 射線 (射線原點 (Main Camera), 射線軸向 (Main Camera 向前))
        Ray ray = new Ray(transform.position, transform.forward);

        // 畫出射線，可以知道射線的位置與方向
        Debug.DrawRay(transform.position, transform.forward * DistanceRange);
        // 準心是否對準物件
        // 物理射線 (射線參數, 射線碰撞參數, 射線距離)
        hitResult = Physics.Raycast(ray, out hit, DistanceRange, mask);

        // 如果準心有對準物件
        if (hitResult) {
            // 取得目前準心對準的物件
            targetObj_Range = hit.collider.gameObject;
            // 設定目前準心對準物件的碰撞位置
            intersectPosition_Range = transform.position + transform.forward * hit.distance;

        // 如果準心沒有對準物件
        } else {
            // 將 目前準心對準物件的碰撞位置 重設為 Vector3(0, 0, 0)
            intersectPosition_Range = Vector3.zero;
        }
    }

    /// <summary>
    /// 計算目標物件與玩家的距離
    /// </summary>
    private void Calc_TargetObj_Player_Distance() {
        if (targetObj_Unlimited != null) {
            // 畢氏定理 distance = sqrt(X^2 + Y^2 + Z^2)
            Vector3 TargetObjPosition = targetObj_Unlimited.transform.position;
            Vector3 PlayerPosition = transform.position;

            float disX = Mathf.Pow(TargetObjPosition.x - PlayerPosition.x, 2);
            float disY = Mathf.Pow(TargetObjPosition.y - PlayerPosition.y, 2);
            float disZ = Mathf.Pow(TargetObjPosition.z - PlayerPosition.z, 2);

            TargetObj_Player_Distance = Mathf.Sqrt(disX + disY + disZ);
        }
    }

    /// <summary>
    /// 開啟 / 關閉目標物件之 EventTrigger
    /// </summary>
    /// <param name="ObjName">目標物件名稱內需有某名稱才能開啟 / 關閉該物件之 EventTrigger</param>
    private void ChangeTargetEventTrigger(string ObjName) {
        if (targetObj_Unlimited != null && targetObj_Unlimited.name.Contains(ObjName)) {
            // 目標物件之 EventTrigger
            EventTrigger targetGazeSwitch = targetObj_Unlimited.GetComponent<EventTrigger>();

            // 如果 取得目標物件與玩家的距離已超過限制範圍，關閉 目標物件之 EventTrigger
            if (IsOverRange()) {               
                targetGazeSwitch.enabled = false;

            // 如果 取得目標物件與玩家的距離未超過限制範圍，開啟 目標物件之 EventTrigger
            } else {
                targetGazeSwitch.enabled = true;
            }
        }
    }

    /// <summary>
    /// 有範圍限制：取得射線碰撞參數
    /// </summary>
    public RaycastHit GetHit() {
        return hit;
    }

    /// <summary>
    /// 有範圍限制：取得準心是否對準物件
    /// </summary>
    public bool IsHitResult() {
        return hitResult;
    }

    /// <summary>
    /// 有範圍限制：取得目前準心對準的物件
    /// </summary>
    public GameObject GetCurrentObj_Range() {
        return targetObj_Range != null ? targetObj_Range : null;
    }

    /// <summary>
    /// 無範圍限制：取得目前準心對準的物件
    /// </summary>
    public GameObject GetCurrentObj_Unlimited() {
        return targetObj_Unlimited != null ? targetObj_Unlimited : null;
    }

    /// <summary>
    /// 有範圍限制：取得目前準心對準的物件名稱
    /// </summary>
    public string GetCurrentObjName_Range() {
        return targetObj_Range != null ? targetObj_Range.name : "Nothing";
    }

    /// <summary>
    /// 無範圍限制：取得目前準心對準的物件名稱
    /// </summary>
    public string GetCurrentObjName_Unlimited() {
        return targetObj_Unlimited != null ? targetObj_Unlimited.name : "Nothing";
    }

    /// <summary>
    /// 有範圍限制：取得目前準心對準物件的碰撞位置
    /// </summary>
    public Vector3 GetIntersectPosition_Range() {
        return intersectPosition_Range;
    }

    /// <summary>
    /// 無範圍限制：取得目前準心對準物件的碰撞位置
    /// </summary>
    public Vector3 GetIntersectPosition_Unlimited() {
        return intersectPosition_Unlimited;
    }

    /// <summary>
    /// 取得目標物件與玩家的距離
    /// </summary>
    public float GetTargetObj_Player_Distance() {
        return TargetObj_Player_Distance;
    }

    /// <summary>
    /// 取得目標物件與玩家的距離是否已超過限制範圍
    /// </summary>
    public bool IsOverRange() {
        return TargetObj_Player_Distance > DistanceRange ? true : false;
    }
}
