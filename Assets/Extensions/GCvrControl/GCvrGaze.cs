using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GCvrGaze : MonoBehaviour {

    //GazeInputModule GazeInput;
    //GameObject gazeObject = null;

    /// <summary>
    /// 自定準心觀看範圍，預設 2.8f
    /// </summary>
    public float DistanceRange = 2.8f;
    public LayerMask mask = -1;

    private GvrHead head;
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
        //GazeInput = GameObject.Find("EventSystem").GetComponent<GazeInputModule>();

        StereoController stereoController = Camera.main.GetComponent<StereoController>();
        head = stereoController.Head;
    }

    void LateUpdate() {
        /*
        gazeObject = GazeInput.GetCurrentGameObject();

        string gazeObjName;

        if (gazeObject == null) {
            gazeObjName = "Nothing";
        } else {
            gazeObjName = gazeObject.name;
        }

        Debug.Log(gazeObjName);
        */

        FindGazeTarget_Range();
        FindGazeTarget_Unlimited();
        Calc_TargetObj_Player_Distance();
        ChangeTargetEventTrigger("Pro_Obj");

        if (Input.GetMouseButtonDown(0)) {
            //Debug.Log(GetCurrentGameObject());
            Debug.Log("目標物件名稱：" + GetCurrentObjName_Range());
            Debug.Log("目前準心對準物件的碰撞位置：" + GetIntersectPosition_Range());

            Calc_TargetObj_Player_Distance();

            Debug.Log("玩家與目標物件的距離：" + GetTargetObj_Player_Distance());
            Debug.Log("是否超過範圍：" + IsOverRange());
        }
    }

    /// <summary>
    /// 無範圍限制：建立射線 與 設定目前準心對準的物件(目標物件)
    /// </summary>
    public void FindGazeTarget_Unlimited() {
        Ray ray = head.Gaze;
        // 射線碰撞參數
        RaycastHit hit;
        // 準心是否對準物件
        bool hitResult = false;

        // 準心是否對準物件
        hitResult = Physics.Raycast(ray, out hit, Camera.main.farClipPlane, mask);

        if (hitResult) {
            // 取得目前準心對準的物件
            targetObj_Unlimited = hit.collider.gameObject;
            intersectPosition_Unlimited = transform.position + transform.forward * hit.distance;
        } else {
            // Nothing? Reset variables.
            intersectPosition_Unlimited = Vector3.zero;
        }
    }

    /// <summary>
    /// 有範圍限制：建立射線 與 設定目前準心對準的物件(目標物件)
    /// </summary>
    public void FindGazeTarget_Range() {
        Ray ray = head.Gaze;
        // 射線碰撞參數
        RaycastHit hit;
        // 準心是否對準物件
        bool hitResult = false;

        // 畫出射線，可以知道射線的位置與方向
        Debug.DrawRay(head.transform.position, head.transform.forward * DistanceRange);
        // 準心是否對準物件
        hitResult = Physics.Raycast(ray, out hit, DistanceRange, mask);

        if (hitResult) {
            // 取得目前準心對準的物件
            targetObj_Range = hit.collider.gameObject;
            intersectPosition_Range = transform.position + transform.forward * hit.distance;
        } else {
            // Nothing? Reset variables.
            intersectPosition_Range = Vector3.zero;
        }
    }

    /// <summary>
    /// 計算目標物件與玩家的距離
    /// </summary>
    public void Calc_TargetObj_Player_Distance() {
        if (targetObj_Unlimited != null) {
            // 畢氏定理 distance = sqrt(X^2 + Y^2 + Z^2)
            Vector3 TargetObjPosition = targetObj_Unlimited.transform.position;
            Vector3 PlayerPosition = head.transform.position;

            float disX = Mathf.Pow(TargetObjPosition.x - PlayerPosition.x, 2);
            float disY = Mathf.Pow(TargetObjPosition.y - PlayerPosition.y, 2);
            float disZ = Mathf.Pow(TargetObjPosition.z - PlayerPosition.z, 2);

            TargetObj_Player_Distance = Mathf.Sqrt(disX + disY + disZ);
        }
    }

    public void ChangeTargetEventTrigger(string ObjName) {
        if (targetObj_Unlimited != null && targetObj_Unlimited.name.Contains(ObjName)) {
            EventTrigger targetGazeSwitch = targetObj_Unlimited.GetComponent<EventTrigger>();

            if (IsOverRange()) {               
                targetGazeSwitch.enabled = false;
            } else {
                targetGazeSwitch.enabled = true;
            }
        }
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
