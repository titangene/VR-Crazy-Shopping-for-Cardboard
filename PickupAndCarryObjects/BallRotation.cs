using UnityEngine;
using System.Collections;

public class BallRotation : MonoBehaviour {

	public float smooth = 5.0f;
    public float tiltAngle = 50.0f;


    void Update () {
        float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
        Debug.Log(tiltAroundZ + ", " + tiltAroundX);
        Quaternion target0 = Quaternion.Euler(-tiltAroundX, tiltAroundZ, 0);
        // Dampen towards the target rotation
        //向target旋轉阻尼
        transform.rotation = Quaternion.Slerp(transform.rotation, target0, Time.deltaTime * smooth);


        if (Input.GetKey(KeyCode.T)) {
            transform.Translate(0, 0, 3 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.G)) {
            transform.Translate(0, 0, -3 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.F)) {
            transform.Translate(-3 * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.H)) {
            transform.Translate(3 * Time.deltaTime, 0, 0);
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }
}
