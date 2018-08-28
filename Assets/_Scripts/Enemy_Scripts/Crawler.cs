/*Alex Greff
19/01/2016
Crawler
NOTE: this class is unimplemented in the final version
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Enemy {
    private Vector3 offset = new Vector3(0, -0.5f);

    private float turnSpeed = 5;

    public override float TurnSpeed {
        get { return turnSpeed; }
        set { turnSpeed = value; }
    }

    new void Start () {
        base.Start();
    }

    protected override void Update () {
        base.Update();

        Vector2 down = -transform.up;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + offset, down, 0.3f, layerMask);

        Vector2 chosenNormal = Vector2.zero;
        
        if (hit.transform == null) {
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position + offset, transform.right, layerMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + offset, -transform.right, layerMask);

            if (hitRight.transform != null) {
                if (Vector2.Distance(transform.position, hitRight.point) > Vector2.Distance(transform.position, hitLeft.point)) {
                    chosenNormal = hitRight.normal;
                }
                else {
                    chosenNormal = hitLeft.normal;
                }
            }
            else {
                if (hitLeft.transform != null)
                    chosenNormal = hitLeft.normal;
            }
        }

        if (chosenNormal != Vector2.zero) {
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, chosenNormal);
            Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                360);
            transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);
        }

        transform.Translate(transform.right * 2 * Time.deltaTime);

        if (hit.transform == null) return;

        

        print(hit.transform.name);

        Debug.DrawRay(hit.point, hit.normal, Color.red, 100f);

        
    }

    protected override void FixedUpdate () {
        base.FixedUpdate();
    }
}
