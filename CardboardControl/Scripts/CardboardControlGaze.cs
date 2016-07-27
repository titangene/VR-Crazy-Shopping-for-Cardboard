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
    /// �۩w�Ǥ��[�ݽd��A�w�] 2.8f
    /// </summary>
    public float DistanceRange = 2.8f;

    private GameObject previousObject = null;
    private GameObject currentObject = null;
    private float gazeStartTime = 0f;
    private GvrHead head;
    private RaycastHit hit;
    private bool isHeld;
    private bool stared = false;

    // �ثe�Ǥ߹�Ǫ�����(���d�򭭨�)
    private GameObject currentObjectRange = null;
    // �g�u�I���Ѽ�(���d�򭭨�)
    private RaycastHit hitRange;
    // �Ǥ߬O�_��Ǫ���(���d�򭭨�)
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
        // �Ǥ߬O�_��Ǫ���(�L���d��A�w�])
        isHeld = Physics.Raycast(Ray(), out hit, maxDistance, layerMask);

        // �e�X�g�u�A�i�H���D�g�u����m�P��V(���d�򭭨�)
        Debug.DrawRay(head.transform.position, head.transform.forward * DistanceRange);
        // �Ǥ߬O�_��Ǫ���(���d�򭭨�)
        isHeldRange = Physics.Raycast(Ray(), out hitRange, DistanceRange, layerMask);

        CheckGaze();
    }

    private void CheckGaze() {
        if (GazeChanged() && cardboard.EventReady("OnChange"))
            ReportGazeChange();
        if (!stared && Staring() && cardboard.EventReady("OnStare"))
            ReportStare();
        if (cardboard.EventReady("OnUpdate"))
            // �Y���˹�Ǥߦ��L��Ǫ���(���d�򭭨�)
            ReportGazeUpdate();
        currentObject = Object();
        // �ثe�Ǥ߹�Ǫ�����(���d�򭭨�)
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
    /// �Ǥ߬O�_��Ǫ���(���d�򭭨�)
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
    /// �g�u�I���Ѽ�(���d�򭭨�)
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
    /// �Ǥ߹�Ǫ�����(���d�򭭨�)
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
