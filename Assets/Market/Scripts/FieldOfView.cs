using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

    public Transform Head;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask[] targetsMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start() {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets() {
        visibleTargets.Clear();

        foreach (LayerMask targetMask in targetsMask) {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(Head.position, viewRadius, targetMask);

            foreach (Collider Targets in targetsInViewRadius) {
                Transform target = Targets.transform;
                Vector3 dirToTarget = (target.position - Head.position).normalized;
                if (Vector3.Angle(Head.forward, dirToTarget) < viewAngle / 2) {
                    float dstToTarget = Vector3.Distance(Head.position, target.position);

                    if (!Physics.Raycast(Head.position, dirToTarget, dstToTarget, obstacleMask)) {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        /*
        Collider[] targetsInViewRadius = Physics.OverlapSphere(Head.position, viewRadius, targetsMask);
        
        for (int i = 0; i < targetsInViewRadius.Length; i++) {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - Head.position).normalized;
                if (Vector3.Angle(Head.forward, dirToTarget) < viewAngle / 2) {
                    float dstToTarget = Vector3.Distance(Head.position, target.position);

                    if (!Physics.Raycast(Head.position, dirToTarget, dstToTarget, obstacleMask)) {
                        visibleTargets.Add(target);
                    }
                }
        }
        */
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += Head.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
