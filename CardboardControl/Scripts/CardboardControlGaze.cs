using UnityEngine;
using System.Collections;
using CardboardControlDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlGaze : MonoBehaviour {
    public float maxDistance = Mathf.Infinity;
    public LayerMask layerMask = Physics.DefaultRaycastLayers;
    public bool useEventCooldowns = false;
    public bool vibrateOnChange = false;
    public bool vibrateOnStare = false;
    public float stareTimeThreshold = 2.0f;
    /// <summary>
    /// 自定準心觀看範圍，預設 2.8f
    /// </summary>
    public float DistanceRange = 2.8f;

    private GameObject previousObject = null;
    private GameObject currentObject = null;
    private float gazeStartTime = 0f;
    private GvrHead head;
    private RaycastHit hit;
    private bool isHeld;
    private bool stared = false;

    // 目前準心對準的物件(有範圍限制)
    private GameObject currentObjectRange = null;
    // 射線碰撞參數(有範圍限制)
    private RaycastHit hitRange;
    // 準心是否對準物件(有範圍限制)
    private bool isHeldRange;

    private CardboardControl cardboard;
    public CardboardControlDelegate OnChange = delegate { };
    public CardboardControlDelegate OnUpdate = delegate { };
    public CardboardControlDelegate OnStare = delegate { };

    public void Start() {
        cardboard = gameObject.GetComponent<CardboardControl>();
        StereoController stereoController = Camera.main.GetComponent<StereoController>();
        head = stereoController.Head;
    }

    public void Update() {
        // 準心是否對準物件(無限範圍，預設)
        isHeld = Physics.Raycast(Ray(), out hit, maxDistance, layerMask);

        // 畫出射線，可以知道射線的位置與方向(有範圍限制)
        Debug.DrawRay(head.transform.position, head.transform.forward * DistanceRange);
        // 準心是否對準物件(有範圍限制)
        isHeldRange = Physics.Raycast(Ray(), out hitRange, DistanceRange, layerMask);

        CheckGaze();
    }

    private void CheckGaze() {
        if (GazeChanged() && cardboard.EventReady("OnChange"))
            ReportGazeChange();
        if (!stared && Staring() && cardboard.EventReady("OnStare"))
            ReportStare();
        if (cardboard.EventReady("OnUpdate"))
            // 即時檢察準心有無對準物件(有範圍限制)
            ReportGazeUpdate();
        currentObject = Object();
        // 目前準心對準的物件(有範圍限制)
        currentObjectRange = ObjectRange();
    }

    private bool Staring() {
        return SecondsHeld() > stareTimeThreshold;
    }

    private bool GazeChanged() {
        if (currentObject != Object()) {
            previousObject = currentObject;
            stared = false;
            return true;
        }
        return false;
    }

    private void ReportGazeChange() {
        OnChange(this);
        if (vibrateOnChange)
            Handheld.Vibrate();
        gazeStartTime = Time.time;
    }

    private void ReportGazeUpdate() {
        OnUpdate(this);
    }

    private void ReportStare() {
        OnStare(this);
        if (vibrateOnStare)
            Handheld.Vibrate();
        stared = true;
    }

    public bool IsHeld() {
        return isHeld;
    }

    /// <summary>
    /// 準心是否對準物件(有範圍限制)
    /// </summary>
    public bool IsHeldRange() {
        return isHeldRange;
    }

    public bool WasHeld() {
        return previousObject != null;
    }

    public float SecondsHeld() {
        return Time.time - gazeStartTime;
    }

    public RaycastHit Hit() {
        return hit;
    }

    /// <summary>
    /// 射線碰撞參數(有範圍限制)
    /// </summary>
    public RaycastHit HitRange() {
        return hitRange;
    }

    public GameObject Object() {
        if (IsHeld()) {
            return hit.transform.gameObject;
        } else {
            return null;
        }
    }

    /// <summary>
    /// 準心對準的物件(有範圍限制)
    /// </summary>
    public GameObject ObjectRange() {
        if (IsHeldRange()) {
            return hitRange.transform.gameObject;
        } else {
            return null;
        }
    }

    public GameObject PreviousObject() {
        return previousObject;
    }

    public Vector3 Forward() {
        return Ray().direction.normalized;
    }

    public Vector3 Right() {
        return Vector3.Cross(Forward(), Vector3.up);
    }

    public Quaternion Rotation() {
        return Quaternion.LookRotation(Forward());
    }

    public Ray Ray() {
        return head.Gaze;
    }
}
