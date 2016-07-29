using UnityEngine;
using System.Collections;

public class ColliderSwitch : MonoBehaviour {
    /// <summary>
    /// 找出所有物件
    /// </summary>
    GameObject[] AllGameObjArray;

    /// <summary>
    /// 將所有是某 Tag 的物件 Collider 開啟 or 關閉
    /// </summary>
    /// <param name="FindTagName">找出所有是某 Tag 的物件</param>
    /// <param name="ColliderSwitch">將物件的 Collider 開啟 or 關閉</param>
    public void CloseCollider(string FindTagName, bool ColliderSwitch) {
        // 找出所有物件
        AllGameObjArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject GameObj in AllGameObjArray) {
            // 找出所有是某 Tag 的物件
            if (GameObj.tag == FindTagName) {
                // 將物件的 Collider 開啟 or 關閉
                GameObj.GetComponent<Collider>().enabled = ColliderSwitch;
            }
        }
    }
}
