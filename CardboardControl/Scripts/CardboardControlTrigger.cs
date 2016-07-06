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

    private CardboardControl cardboard;
    public CardboardControlDelegate OnUp = delegate { };
    public CardboardControlDelegate OnDown = delegate { };
    public CardboardControlDelegate OnClick = delegate { };


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
        CheckKey();
    }

    public void FixedUpdate() {
        if (printDebugInfo)
            PrintDebug();
    }

    private bool KeyFor(string direction) {
        switch (direction) {
            case "down":
            return Input.GetKeyDown(triggerKey);
            case "up":
            return Input.GetKeyUp(triggerKey);
            default:
            return false;
        }
    }

    private void CheckKey() {
        if (KeyFor("down") && cardboard.EventReady("OnDown"))
            ReportDown();
        if (KeyFor("up") && cardboard.EventReady("OnUp"))
            ReportUp();
    }

    private void CheckMagnet() {
        if (magnet.IsDown() && cardboard.EventReady("OnDown"))
            ReportDown();
        if (magnet.IsUp() && cardboard.EventReady("OnUp"))
            ReportUp();
    }

    private void CheckTouch() {
        if (touch.IsDown() && cardboard.EventReady("OnDown"))
            ReportDown();
        if (touch.IsUp() && cardboard.EventReady("OnUp"))
            ReportUp();
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

    public float SecondsHeld() {
        return Time.time - clickStartTime;
    }

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
