using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
/// <summary>
/// 找出 Layer
/// </summary>
public class Find : MonoBehaviour {
    /// <summary>
    /// 找出所有物件
    /// </summary>
    private Array AllGameObjArray;
    /// <summary>
    /// 找出某物件內的所有子物件
    /// </summary>
    private Array MoreGameObjArray;

    void Start() {
        // 找出所有物件
        FindAllGameObj();
    }

    /// <summary>
    /// 找出所有物件
    /// </summary>
    public void FindAllGameObj() {
        AllGameObjArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
    }

    /// <summary>
    /// 找出某物件內的所有子物件
    /// </summary>
    public void FindMoreGameObj(GameObject obj) {
        MoreGameObjArray = obj.GetComponentsInChildren(typeof(Transform));
    }

    /// <summary>
    /// 將所有是某 Layer 的物件放入某子物件內
    /// </summary>
    /// <param name="FindLayerName">要找的 Layer 名稱</param>
    /// <param name="ObjectParent">要放入某子物件內</param>
    public void PlacedObjectParent(string FindLayerName, Transform ObjectParent) {
        // 找出所有物件
        FindAllGameObj();
        foreach (GameObject GameObj in AllGameObjArray) {
            // 找出所有是某 Layer 的物件
            if (GameObj.layer == LayerMask.NameToLayer(FindLayerName)) {
                // 將所有是某 Layer 的物件放入某子物件內
                GameObj.transform.parent = ObjectParent;
            }
        }
    }

    /// <summary>
    /// 將某物件內所有名為 objName 的子物件 開啟 / 關閉 Collider 和 Rigidbody
    /// </summary>
    /// <param name="obj">要找的物件</param>
    /// <param name="objName">該物件內所有名為 objName 的子物件名稱</param>
    /// <param name="switcher">開啟 / 關閉 Collider 和 Rigidbody</param>
    public void Collider_UseGravitySwitch(GameObject obj, string objName, bool switcher) {
        FindMoreGameObj(obj);
        foreach (Transform child in MoreGameObjArray) {
            if (child.name.Contains(objName)) {
                child.GetComponent<Collider>().enabled = switcher;
                child.GetComponent<Rigidbody>().useGravity = switcher;
            }
        }
    }

    /// <summary>
    /// 將所有是某 Tag 的物件 開啟 or 關閉 EventTrigger
    /// </summary>
    /// <param name="FindTag">找出所有是某 Tag 的物件</param>
    /// <param name="EventTriggerSwitch">將物件的 EventTrigger 開啟 or 關閉</param>
    public void EventTriggerSwitch(string FindTag, bool EventTriggerSwitch) {
        // 找出所有物件
        FindAllGameObj();
        foreach (GameObject GameObj in AllGameObjArray) {
            // 找出所有是某 Tag 的物件
            if (GameObj.tag == FindTag) {
                // 將物件的 EventTrigger 開啟 or 關閉
                GameObj.GetComponent<EventTrigger>().enabled = EventTriggerSwitch;
            }
        }
    }
}
