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
        // 找到 CardboardControlManager 中的 CardboardControl.cs Script
        cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();

        // When the trigger goes down
        // 按下 Gvr 按鈕時
        cardboard.trigger.OnDown += CardboardDown;

        // When the trigger comes back up
        // 放開 Gvr 按鈕時
        cardboard.trigger.OnUp += CardboardUp;

        // When the magnet or touch goes down and up within the "click threshold" time
        // That click speed threshold is configurable in the Inspector
        // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
        cardboard.trigger.OnClick += CardboardClick;

        // When the thing we're looking at changes, determined by a gaze
        // The gaze distance and layer mask are public as configurable in the Inspector
        // 改變目光(gaze)看的東西
        cardboard.gaze.OnChange += CardboardGazeChange;

        // When we've been staring at an object
        // 持續盯著某個物體
        cardboard.gaze.OnStare += CardboardStare;

        // When we rotate the device into portrait mode
        // 當裝置旋轉到縱向模式
        cardboard.box.OnTilt += CardboardMagnetReset;
    }

    // 按下 Gvr 按鈕時
    private void CardboardDown(object sender) {
        // Debug.Log("Trigger went down");
        //Debug.Log("偵測到按下 Gvr 按鈕");
        ChangeObjectColor("SphereDown");
    }

    // 放開 Gvr 按鈕時
    private void CardboardUp(object sender) {
        // Debug.Log("Trigger came up");
        //Debug.Log("偵測到放開 Gvr 按鈕");
        ChangeObjectColor("SphereUp");
    }

    // 在設定時間內按下並且快速放開 Gvr 按鈕間，會判定為點擊事件觸發
    private void CardboardClick(object sender) {
        ChangeObjectColor("SphereClick");
        // 找到 SphereClick 內的 Counter 的文字物件
        TextMesh Counter = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
        // 預設是 0，如果偵測是點擊事件就會加 1
        int increment = int.Parse(Counter.text) + 1;
        // 將加 1 的數字設定至 Counter 文字物件上
        Counter.text = increment.ToString();

        // If you need more raycast data from cardboard.gaze, the RaycastHit is exposed as gaze.Hit()
    }

    // 改變目光(gaze)看的東西
    private void CardboardGazeChange(object sender) {
        // You can grab the data from the sender instead of the CardboardControl object
        // 目光盯住的物體
        CardboardControlGaze gaze = sender as CardboardControlGaze;
        // We can access to the object we're looking at
        // gaze.IsHeld will make sure the gaze.Object() isn't null
        // 目光盯住的物體時 gaze.IsHeld() = true，目光沒有盯住的物體時 = false
        if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {
            ChangeObjectColor(gaze.Object().name);
            // 碰到 HighlightCube 這個方塊準心(cardboard.reticle)會變成紅色
            if (gaze.Object().name == "HighlightCube") {
                // Highlighting can help identify which objects can be interacted with
                // The reticle is hidden by default but we already toggled that in the Inspector
                // 準心(cardboard.reticle)會變成紅色
                //cardboard.reticle.Highlight(Color.red);
            }
        }
        // We also can access to the last object we looked at
        // gaze.WasHeld() will make sure the gaze.PreviousObject() isn't null
        // 存取上一個目光盯住的物體在 gaze.WasHeld()
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

    // 持續盯著某個物體
    private void CardboardStare(object sender) {
        CardboardControlGaze gaze = sender as CardboardControlGaze;
        if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {
            // Be sure to hide the cursor when it's not needed
            //cardboard.reticle.Hide();
        }
    }

    // 當裝置旋轉到縱向模式
    private void CardboardMagnetReset(object sender) {
        // Resetting the magnet will reset the polarity if up and down are confused
        // This occasionally happens when the device is inserted into the enclosure
        // or if the magnetometer readings are weak enough to cut in and out
        Debug.Log("Device tilted");
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
        // 找到 SphereDown 內的 Counter 的文字物件
        TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();
        // trigger.IsHeld() is true when the trigger has gone down but not back up yet
        // 按下 Gvr 按鈕時 trigger.IsHeld() = true，放開 Gvr 按鈕時 = false
        if (cardboard.trigger.IsHeld()) {
            // 顯示SphereDown 內的 Counter 的文字物件
            textMesh.GetComponent<Renderer>().enabled = true;
            // trigger.SecondsHeld() is the number of seconds we've held the trigger down
            textMesh.text = cardboard.trigger.SecondsHeld().ToString("#.##");
        } else {
            // 閃爍SphereDown 內的 Counter 的文字物件
            textMesh.GetComponent<Renderer>().enabled = Time.time % 1 < 0.5;
        }

        /*
         * 原本在 CardboardClick(object sender) 下面
         */
        // With the cardboard object, we can grab information from various controls
        // If the raycast doesn't find anything then the focused object will be null
        // 如果目光沒盯住物體就是nothing
        string name = cardboard.gaze.IsHeld() ? cardboard.gaze.Object().name : "nothing";
        // 找到 SphereClick 內的 TimeCounter 的文字物件
        TextMesh TimeCounter = GameObject.Find("SphereClick/TimeCounter").GetComponent<TextMesh>();
        // 目光盯了幾秒
        float count = cardboard.gaze.SecondsHeld();
        // 將 目光盯了幾秒 的數字設定至 TimeCounter 文字物件上
        TimeCounter.text = name + "：" + count.ToString("#.###");
    }

    /*
    * Be sure to unsubscribe before this object is destroyed
    * so the garbage collector can clean everything up
    * 當掛載該 Script 的物件被銷毀時，使垃圾回收器可以清理一切 ??? 看不懂
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
