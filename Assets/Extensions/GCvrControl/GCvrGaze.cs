using UnityEngine;
using GCvrDelegates;

public class GCvrGaze : MonoBehaviour {
    /// <summary>
    /// 自訂 Gaze 多少時間，超過範圍會判定為 OnStare 事件觸發
    /// </summary>
    public float stareHoldTimeRange = 2.0f;
    /// <summary>
    /// 自訂 Gaze 範圍，預設 2.8f
    /// </summary>
    public float DistanceRange = 2.2f;
    public LayerMask mask = -1;
    /// <summary>
    /// OnChange 時裝置是否會震動
    /// </summary>
    public bool vibrateOnChange = false;
    /// <summary>
    /// OnStare 時裝置是否會震動
    /// </summary>
    public bool vibrateOnStare = false;
    /// <summary>
    /// Main Camera
    /// </summary>
    public Camera cam { get; private set; }

    /// <summary>
    /// 有範圍限制：射線碰撞參數
    /// </summary>
    private RaycastHit hit_Range;
    /// <summary>
    /// 無範圍限制：射線碰撞參數
    /// </summary>
    private RaycastHit hit_Infinity;
    /// <summary>
    /// 有範圍限制：準心是否對準物件
    /// </summary>
    private bool hitResult_Range = false;
    /// <summary>
    /// 無範圍限制：準心是否對準物件
    /// </summary>
    private bool hitResult_Infinity = false;

    /// <summary>
    /// 開始 Gaze 時間
    /// </summary>
    private float gazeStartTime = 0f;
    /// <summary>
    /// 是否持續盯著
    /// </summary>
    private bool isStared = false;

    /// <summary>
    /// 有範圍限制：目前準心對準的物件
    /// </summary>
    private GameObject currentObj_Range = null;
    /// <summary>
    /// 無範圍限制：目前準心對準的物件
    /// </summary>
    private GameObject currentObj_Infinity = null;
    /// <summary>
    /// 有範圍限制：前一個準心對準的物件
    /// </summary>
    private GameObject previousObj_Range = null;
    /// <summary>
    /// 無範圍限制：前一個準心對準的物件
    /// </summary>
    private GameObject previousObj_Infinity = null;

    /// <summary>
    /// 有範圍限制：目前準心對準物件的碰撞位置
    /// </summary>
    private Vector3 intersectPosition_Range;
    /// <summary>
    /// 無範圍限制：目前準心對準物件的碰撞位置
    /// </summary>
    private Vector3 intersectPosition_Infinity;
    /// <summary>
    /// 目標物件與玩家的距離
    /// </summary>
    private float TargetObj_Player_Distance = 0;

    public GCvrDelegate OnChange = delegate { };
    public GCvrDelegate OnStare = delegate { };

    void Awake() {
        cam = GetComponent<Camera>();
    }

    void LateUpdate() {
        // 物理射線 (射線參數, 射線碰撞參數, 射線距離)
        // 無範圍限制：準心是否對準物件
        hitResult_Infinity = Physics.Raycast(Ray(), out hit_Infinity, cam.farClipPlane, mask);
        // 有範圍限制：準心是否對準物件
        hitResult_Range = Physics.Raycast(Ray(), out hit_Range, DistanceRange, mask);
        // 畫出射線，可以知道射線的位置與方向
        Debug.DrawRay(transform.position, transform.forward * DistanceRange);

        CheckGaze();

        // 計算目標物件與玩家的距離
        Calc_TargetObj_Player_Distance();
        // 開啟 / 關閉目標物件之 EventTrigger
        ChangeTargetEventTrigger("Pro_Obj");
    }

    private void CheckGaze() {
        if (GazeChanged())
            ReportGazeChange();
        if (!isStared && Staring())
            ReportStare();

        // 設定 目前準心對準的物件
        currentObj_Range = Obj_Range();
        currentObj_Infinity = Obj_Infinity();
    }

    private bool GazeChanged() {
        // 已改變 GazeObject
        if (currentObj_Infinity != Obj_Infinity()) {
            // 設定 前一個準心對準的物件
            previousObj_Infinity = currentObj_Infinity;
            previousObj_Range = currentObj_Range;
            // 重設 是否持續盯著
            isStared = false;

            return true;
        }
        return false;
    }

    private void ReportGazeChange() {
        OnChange(this);
        // 開始 Gaze 時間
        gazeStartTime = Time.time;
        // 是否讓裝置震動
        if (vibrateOnChange)
            Handheld.Vibrate();
    }

    /// <summary>
    /// 是否正在持續盯著
    /// </summary>
    private bool Staring() {
        return GazeTime() > stareHoldTimeRange;
    }

    private void ReportStare() {
        OnStare(this);
        // 設定 是否持續盯著 狀態為 true
        isStared = true;
        // 是否讓裝置震動
        if (vibrateOnStare)
            Handheld.Vibrate();
    }

    /// <summary>
    /// 計算目標物件與玩家的距離
    /// </summary>
    private void Calc_TargetObj_Player_Distance() {
        if (currentObj_Infinity != null) {
            // 畢氏定理 distance = sqrt(X^2 + Y^2 + Z^2)
            Vector3 TargetObjPosition = currentObj_Infinity.transform.position;
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
        if (currentObj_Infinity != null && currentObj_Infinity.name.Contains(ObjName)) {
            // 目標物件之 EventTrigger
            UnityEngine.EventSystems.EventTrigger targetGazeSwitch =
                currentObj_Infinity.GetComponent<UnityEngine.EventSystems.EventTrigger>();

            // 如果 取得目標物件與玩家的距離已超過限制範圍，關閉 / 開啟 目標物件之 EventTrigger
            if (IsOverRange())
                targetGazeSwitch.enabled = false;
            else
                targetGazeSwitch.enabled = true;
        }
    }

    /// <summary>
    /// 取得射線
    /// </summary>
    public Ray Ray() {
        // 射線 (射線原點 (Main Camera), 射線軸向 (Main Camera 向前))
        return new Ray(transform.position, transform.forward);
    }

    /// <summary>
    /// 有範圍限制：取得射線碰撞參數
    /// </summary>
    public RaycastHit Hit_Range() {
        return hit_Range;
    }

    /// <summary>
    /// 無範圍限制：取得射線碰撞參數
    /// </summary>
    public RaycastHit Hit_Infinity() {
        return hit_Infinity;
    }

    /// <summary>
    /// 取得 Gaze 多少時間
    /// </summary>
    public float GazeTime() {
        return Time.time - gazeStartTime;
    }

    /// <summary>
    /// 有範圍限制：取得準心是否對準物件
    /// </summary>
    public bool IsHitResult_Range() {
        return hitResult_Range;
    }

    /// <summary>
    /// 無範圍限制：取得準心是否對準物件
    /// </summary>
    public bool IsHitResult_Infinity() {
        return hitResult_Infinity;
    }

    /// <summary>
    /// 有範圍限制：取得準心是否在前一次對準物件
    /// </summary>
    public bool WasHitResult_Range() {
        return previousObj_Range != null;
    }

    /// <summary>
    /// 無範圍限制：取得準心是否在前一次對準物件
    /// </summary>
    public bool WasHitResult_Infinity() {
        return previousObj_Infinity != null;
    }

    /// <summary>
    /// 有範圍限制：準心對準的物件
    /// </summary>
    public GameObject Obj_Range() {
        return IsHitResult_Range() ? hit_Range.collider.gameObject : null;
    }

    /// <summary>
    /// 無範圍限制：準心對準的物件
    /// </summary>
    public GameObject Obj_Infinity() {
        return IsHitResult_Infinity() ? hit_Infinity.collider.gameObject : null;
    }

    /// <summary>
    /// 有範圍限制：取得目前準心對準物件的碰撞位置
    /// </summary>
    public Vector3 IntersectPosition_Range() {
        return IsHitResult_Range() ? 
            transform.position + transform.forward * hit_Range.distance : Vector3.zero;
    }

    /// <summary>
    /// 無範圍限制：取得目前準心對準物件的碰撞位置
    /// </summary>
    public Vector3 IntersectPosition_Infinity() {
        return IsHitResult_Infinity() ?
            transform.position + transform.forward * hit_Infinity.distance : Vector3.zero;
    }

    /// <summary>
    /// 有範圍限制：取得目前準心對準的物件
    /// </summary>
    public GameObject CurrentObj_Range() {
        return currentObj_Range;
    }

    /// <summary>
    /// 無範圍限制：取得目前準心對準的物件
    /// </summary>
    public GameObject CurrentObj_Infinity() {
        return currentObj_Infinity;
    }

    /// <summary>
    /// 有範圍限制：取得前一個準心對準的物件
    /// </summary>
    public GameObject PreviousObj_Range() {
        return previousObj_Range;
    }

    /// <summary>
    /// 無範圍限制：取得前一個準心對準的物件
    /// </summary>
    public GameObject PreviousObj_Infinity() {
        return previousObj_Infinity;
    }

    /// <summary>
    /// 取得 目前準心對準的物件名稱、前一個準心對準的物件名稱：
    /// currentObj_Range, currentObj_Infinity, previousObj_Range, previousObj_Infinity
    /// </summary>
    public string GetObjName(GameObject obj) {
        return obj != null ? obj.name : "Nothing";
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
