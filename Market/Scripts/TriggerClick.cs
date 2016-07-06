using UnityEngine;
using System.Collections;

public class TriggerClick : MonoBehaviour {
    private static CardboardControl cardboard;

    void Start() {
        /*
        * Start by capturing the helper script on CardboardControlManager
        * This script has access to all the controls and their delegates
        *
        * Unity provides a good primer on delegates here:
        * http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
        */
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
        // 當 cardboard 按下按鈕 When the trigger goes down
        cardboard.trigger.OnDown += CardboardDown;
        // 當 cardboard 放開按鈕 When the trigger comes back up
        cardboard.trigger.OnUp += CardboardUp;

        // When the magnet or touch goes down and up within the "click threshold" time
        // That click speed threshold is configurable in the Inspector
        // 在時間內按下並且放開按鈕間的時間在設定時間內，會判定為點擊觸發
        cardboard.trigger.OnClick += CardboardClick;

        // When the thing we're looking at changes, determined by a gaze
        // The gaze distance and layer mask are public as configurable in the Inspector
        // 目光(gaze)看的東西會改變
        cardboard.gaze.OnChange += CardboardGazeChange;

        // 持續盯著某個物體 When we've been staring at an object
        cardboard.gaze.OnStare += CardboardStare;

        // 當裝置旋轉到縱向模式 When we rotate the device into portrait mode
        cardboard.box.OnTilt += CardboardMagnetReset;
    }

    // 偵測到按下 Gvr 按鈕
    private void CardboardDown(object sender) {
        Debug.Log("偵測到按下 Gvr 按鈕");   //Trigger went down
        ChangeObjectColor("SphereDown");
    }

    // 偵測到放開 Gvr 按鈕
    private void CardboardUp(object sender) {
        Debug.Log("偵測到放開 Gvr 按鈕");   // Trigger came up
        ChangeObjectColor("SphereUp");
    }

    // 偵測到點一下 Gvr 按鈕事件(按下並在幾秒內快速放開 Gvr 按鈕)
    private void CardboardClick(object sender) {
        ChangeObjectColor("SphereClick");
        // 找到 Counter 的文字
        TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
        // 預設是 0，如果偵測試點擊事件就會加 1
        int increment = int.Parse(textMesh.text) + 1;
        // 將加 1 的數字設定至 Counter 文字上
        textMesh.text = increment.ToString();

        // With the cardboard object, we can grab information from various controls
        // If the raycast doesn't find anything then the focused object will be null
        // 如果目光沒盯住物體就是nothing
        string name = cardboard.gaze.IsHeld() ? cardboard.gaze.Object().name : "nothing";
        // 目光盯了幾秒
        float count = cardboard.gaze.SecondsHeld();
        // Debug.Log("We've focused on " + name + " for " + count + " seconds.");
        Debug.Log("你正在看 " + name + " 物體已經 " + count + " 秒了");

        // If you need more raycast data from cardboard.gaze, the RaycastHit is exposed as gaze.Hit()
    }

    private void CardboardGazeChange(object sender) {
        // You can grab the data from the sender instead of the CardboardControl object
        // 目光盯住的物體
        CardboardControlGaze gaze = sender as CardboardControlGaze;
        // We can access to the object we're looking at
        // gaze.IsHeld will make sure the gaze.Object() isn't null
        if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {
            ChangeObjectColor(gaze.Object().name);
            // 碰到 HighlightCube 這個方塊準心(cardboard.reticle)會變成紅色
            if (gaze.Object().name == "HighlightCube") {
                // Highlighting can help identify which objects can be interacted with
                // The reticle is hidden by default but we already toggled that in the Inspector
                cardboard.reticle.Highlight(Color.red);
            }
        }
        // We also can access to the last object we looked at
        // gaze.WasHeld() will make sure the gaze.PreviousObject() isn't null
        if (gaze.WasHeld() && gaze.PreviousObject().name.Contains("Cube")) {
            // 重設上一個目光盯住的方塊顏色(白)
            ResetObjectColor(gaze.PreviousObject().name);
            // Use these to undo reticle hiding and highlighting
            // 顯示準心
            cardboard.reticle.Show();
            // 將準心由紅轉為原本顏色(白)
            cardboard.reticle.ClearHighlight();
        }

        // Be sure to set the Reticle Layer Mask on the CardboardControlManager
        // to grow the reticle on the objects you want. The default is everything.

        // Not used here are gaze.Forward(), gaze.Right(), and gaze.Rotation()
        // which are useful for things like checking the view angle or shooting projectiles
    }

    private void CardboardStare(object sender) {
        CardboardControlGaze gaze = sender as CardboardControlGaze;
        if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {
            // Be sure to hide the cursor when it's not needed
            cardboard.reticle.Hide();
        }
    }

    // 當裝置旋轉到縱向模式
    private void CardboardMagnetReset(object sender) {
        // Resetting the magnet will reset the polarity if up and down are confused
        // This occasionally happens when the device is inserted into the enclosure
        // or if the magnetometer readings are weak enough to cut in and out
        Debug.Log("Device tilted");
        cardboard.trigger.ResetMagnetState();
        ResetSpheres();
    }

    // 改變方塊顏色(隨機)
    private void ChangeObjectColor(string name) {
        GameObject obj = GameObject.Find(name);
        Color newColor = RandomColor();
        obj.GetComponent<Renderer>().material.color = newColor;
    }

    // 重設方塊顏色(白)：指定方塊
    private void ResetObjectColor(string name) {
        GameObject.Find(name).GetComponent<Renderer>().material.color = Color.white;
    }

    // 重設方塊顏色(白)：所有方塊
    private void ResetSpheres() {
        string[] spheres = { "SphereDown", "SphereUp", "SphereClick" };
        foreach (string sphere in spheres) {
            GameObject obj = GameObject.Find(sphere);
            obj.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    // 產生隨機顏色
    private Color RandomColor() {
        return new Color(Random.value, Random.value, Random.value);
    }



    /*
    * During our game we can utilize data from the CardboardControl API
    */
    void Update() {
        TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();

        // trigger.IsHeld() is true when the trigger has gone down but not back up yet
        if (cardboard.trigger.IsHeld()) {
            textMesh.GetComponent<Renderer>().enabled = true;
            // trigger.SecondsHeld() is the number of seconds we've held the trigger down
            textMesh.text = cardboard.trigger.SecondsHeld().ToString("#.##");
        } else {
            textMesh.GetComponent<Renderer>().enabled = Time.time % 1 < 0.5;
        }
    }

    /*
    * Be sure to unsubscribe before this object is destroyed
    * so the garbage collector can clean everything up
    */
    void OnDestroy() {
        cardboard.trigger.OnDown -= CardboardDown;
        cardboard.trigger.OnUp -= CardboardUp;
        cardboard.trigger.OnClick -= CardboardClick;
        cardboard.gaze.OnChange -= CardboardGazeChange;
        cardboard.gaze.OnStare -= CardboardStare;
        cardboard.box.OnTilt -= CardboardMagnetReset;
    }
}
