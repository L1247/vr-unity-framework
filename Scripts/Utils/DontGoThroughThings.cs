using UnityEngine;
using System.Collections;
using System;

public class DontGoThroughThings : MonoBehaviour
{
    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 
    public Vector3 lastHitPoint;

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    public Vector3 previousPosition;
    private Rigidbody myRigidbody;
    private Collider myCollider;

    private Vector3 initPosition;


    //initialize values 
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        previousPosition = myRigidbody.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;

        initPosition = transform.parent.position;
    }

    public void Init() {
        myRigidbody.position = initPosition;
        previousPosition = initPosition;
    }

    void FixedUpdate()
    {
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = myRigidbody.position - previousPosition;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                if (!hitInfo.collider)
                    return;
                myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                //try
                //{
                //    if(!hitInfo.transform.GetComponent<BodyPartGetHurt>().hpController.getIsDie())
                //        myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                //}
                //catch (Exception e)
                //{
                //    myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                //}
            }
        }

        previousPosition = myRigidbody.position;
    }
}