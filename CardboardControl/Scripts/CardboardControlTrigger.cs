using UnityEngine;
using System.Collections;
using CardboardControlDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlTrigger : MonoBehaviour {
    // �]�w���U�åB�ֳt��} Gvr ���s�����ɶ��A�|�P�w���I���ƥ�Ĳ�o
    public float clickSpeedThreshold = 0.4f;
    // �ϥΨƥ�N�o�ɶ�
    public bool useEventCooldowns = true;
    // ���U Gvr ���s�ɬO�_�˸m�_��
    public bool vibrateOnDown = false;
    // ��} Gvr ���s�ɬO�_�˸m�_��
    public bool vibrateOnUp = false;
    // �b�]�w�ɶ������U�åB�ֳt��} Gvr ���s���A�|�P�w���I���ƥ�Ĳ�o
    // �I���ƥ�Ĳ�o�ɬO�_�˸m�_��
    public bool vibrateOnClick = true;
    // �ϥκ��K
    public bool useMagnet = true;
    // �ϥ�Ĳ�I
    public bool useTouch = true;
    // �ƹ����� = Gvr ���s
    public KeyCode triggerKey = KeyCode.Mouse0;
    // �C�L Debug �T��
    public bool printDebugInfo = false;

    // ���R���K���
    private ParsedMagnetData magnet;
    // ���RĲ�I���
    private ParsedTouchData touch;
    // enum �O�C�|�A�w�]�Ĥ@�Ӭ� 0�A�̧�����
    // Gvr ���s���A
    private enum TriggerState {
        Up, Down
    }
    // �ثe Gvr ���s���A�A�w�]����} Gvr ���s
    private TriggerState currentTriggerState = TriggerState.Up;
    // ���� Gvr ���s�ɶ��A�w�]�� 0 ��
    private float clickStartTime = 0f;
    // debug ���ơA�w�]�� 0 �� ??? �ݤ���
    private int debugThrottle = 0;
    // �C FPS debug ���ơA�w�]�� 5 �� ??? �ݤ���
    private int FRAMES_PER_DEBUG = 5;

    // �����}�l�����ɶ�
    private float LongClickTimeLog = 0f;
    // �O�_�i�H���������ɶ�
    private bool CalcTime = true;
    // �O�_���� Gvr ���s
    private bool LongClick = false;

    private CardboardControl cardboard;
    public CardboardControlDelegate OnUp = delegate { };
    public CardboardControlDelegate OnDown = delegate { };
    public CardboardControlDelegate OnClick = delegate { };
    public CardboardControlDelegate OnLongClick = delegate { };

    public void Start() {
        cardboard = gameObject.GetComponent<CardboardControl>();
        magnet = new ParsedMagnetData();
        touch = new ParsedTouchData();
    }

    public void Update() {
        magnet.Update();
        touch.Update();
        if (useTouch)
            CheckTouch();
        if (useMagnet)
            CheckMagnet();
        CheckLongClick();
        CheckKey();
    }

    public void FixedUpdate() {
        if (printDebugInfo)
            PrintDebug();
    }

    private void CheckLongClick() {
        if (Input.GetKey(triggerKey)) {
            //Debug.Log("���U���� " + triggerKey);

            // ���U����ɡA�����}�l�����ɶ�(�u�p��@��)
            if (CalcTime) {
                // �����}�l�����ɶ�
                LongClickTimeLog = Time.realtimeSinceStartup;
                // �N �O�_�i�H���������ɶ� ���A�令 false�A����������ɭ��s�����}�l�����ɶ�
                CalcTime = false;
            }

            // ��������ƥ�G����W�L�]�w���
            if (Time.realtimeSinceStartup - LongClickTimeLog >= clickSpeedThreshold) {
                //Debug.Log("������� " + triggerKey);
                // �N �O�_���� Gvr ���s ���A�令 true
                LongClick = true;
            }

        } else {
            // �N �O�_���� Gvr ���s ���A�令 false
            LongClick = false;
            // �N �O�_�i�H���������ɶ� ���A�令 true�A�����U����ɡA������}�l�����ɶ�
            CalcTime = true;
            // �N�p������ɶ����s�k�s
            LongClickTimeLog = 0;
        }
    }

    private bool KeyFor(string direction) {
        switch (direction) {
            case "down":
                return Input.GetKeyDown(triggerKey);
            case "up":
                return Input.GetKeyUp(triggerKey);
            case "longclick":
                return LongClick;
            default:
                return false;
        }
    }

    private void CheckKey() {
        if (KeyFor("down") && cardboard.EventReady("OnDown"))
            ReportDown();
        if (KeyFor("up") && cardboard.EventReady("OnUp"))
            ReportUp();
        if (KeyFor("longclick") && cardboard.EventReady("OnLongClick"))
            ReportLongClick();
    }

    private void CheckMagnet() {
        if (magnet.IsDown() && cardboard.EventReady("OnDown"))
            ReportDown();
        if (magnet.IsUp() && cardboard.EventReady("OnUp"))
            ReportUp();
        if (magnet.IsDown() && cardboard.EventReady("OnLongClick"))
            ReportLongClick();
    }

    private void CheckTouch() {
        if (touch.IsDown() && cardboard.EventReady("OnDown"))
            ReportDown();
        if (touch.IsUp() && cardboard.EventReady("OnUp"))
            ReportUp();
        if (magnet.IsDown() && cardboard.EventReady("OnLongClick"))
            ReportLongClick();
    }

    private bool IsTouching() {
        return Input.touchCount > 0;
    }

    private void ReportDown() {
        if (currentTriggerState == TriggerState.Up) {
            currentTriggerState = TriggerState.Down;
            OnDown(this);
            if (vibrateOnDown)
                Handheld.Vibrate();
            clickStartTime = Time.time;
        }
    }

    private void ReportUp() {
        if (currentTriggerState == TriggerState.Down) {
            currentTriggerState = TriggerState.Up;
            OnUp(this);
            if (vibrateOnUp)
                Handheld.Vibrate();
            CheckForClick();
        }
    }

    private void CheckForClick() {
        bool withinClickThreshold = SecondsHeld() <= clickSpeedThreshold;
        
        clickStartTime = 0f;
        if (withinClickThreshold && cardboard.EventReady("OnClick"))
            ReportClick();
    }

    private void ReportClick() {
        OnClick(this);
        if (vibrateOnClick)
            Handheld.Vibrate();
    }

    private void ReportLongClick() {
        OnLongClick(this);
    }

    public float SecondsHeld() {
        return Time.time - clickStartTime;
    }

    // �O�_���� Gvr ���s
    //public bool TiggerHold() {
    //    return SecondsHeld() > clickSpeedThreshold;
    //}

    public bool IsHeld() {
        return (currentTriggerState == TriggerState.Down);
    }

    public void ResetMagnetState() {
        magnet.ResetState();
    }

    private void PrintDebug() {
        debugThrottle++;
        if (debugThrottle >= FRAMES_PER_DEBUG) {
            magnet.PrintDebug();
            touch.PrintDebug();
            debugThrottle = 0;
        }
    }
}
