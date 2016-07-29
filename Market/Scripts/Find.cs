using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// 找出 Layer
/// </summary>
public class Find : MonoBehaviour {
    /// <summary>
    /// 要找出的 Layer 名稱
    /// </summary>
    //public string FindLayerName;

    /// <summary>
    /// 要放入某子物件內
    /// </summary>
    //public Transform ObjectParent;

    /// <summary>
    /// 找出所有物件
    /// </summary>
    GameObject[] AllGameObjArray;

    void Start() {
        // 找出所有物件
        AllGameObjArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
    }

    /// <summary>
    /// 將所有是某 Layer 的物件放入某子物件內
    /// </summary>
    /// <param name="FindLayerName">要找出的 Layer 名稱</param>
    /// <param name="ObjectParent">要放入某子物件內</param>
    public void PlacedObjectParent(string FindLayerName, Transform ObjectParent) {
        foreach (GameObject GameObj in AllGameObjArray) {
            // 找出所有是某 Layer 的物件
            if (GameObj.layer == LayerMask.NameToLayer(FindLayerName)) {
                // 將所有是某 Layer 的物件放入某子物件內
                GameObj.transform.parent = ObjectParent;
            }
        }
    }

    /// <summary>
    /// 將所有是某 Tag 的物件 開啟 or 關閉 EventTrigger
    /// </summary>
    /// <param name="FindTag">找出所有是某 Tag 的物件</param>
    /// <param name="EventTriggerSwitch">將物件的 EventTrigger 開啟 or 關閉</param>
    public void EventTriggerSwitch(string FindTag, bool EventTriggerSwitch) {
        foreach (GameObject GameObj in AllGameObjArray) {
            // 找出所有是某 Tag 的物件
            if (GameObj.tag == FindTag) {
                // 將物件的 EventTrigger 開啟 or 關閉
                GameObj.GetComponent<EventTrigger>().enabled = EventTriggerSwitch;
            }
        }
    }
}
