using UnityEngine;
using System.Collections;
using ExitGames.Demos.DemoAnimator;

namespace y_Network
{
    [System.Serializable]
    public class NetWork_ObservedCamera
    {
        // The distance in the x-z plane to the target
        public float distance = 1;
        // the height we want the camera to be above the target
        public float height = 5;
        // How much we 
        public float heightDamping = 3;
        public float rotationDamping = 3;
        public static int type = 0;
        GameObject playerEye;
        GameObject[] playerList;


        public void FirstPersonCamera(GameObject cameraObj , Transform target)
        {
           
            if (target)
            {
                cameraObj.transform.position = Vector3.Lerp(cameraObj.transform.position, target.position, 5 * Time.deltaTime);
                cameraObj.transform.rotation = Quaternion.Lerp(cameraObj.transform.rotation, target.rotation, 5 * Time.deltaTime);
            }
        }

        public void ThirdPersonCamera(GameObject cameraObj , Transform target)
        {

            if (target)
            {
                // Calculate the current rotation angles
                float wantedRotationAngle = target.eulerAngles.y;
                float wantedHeight = target.position.y + height;

                float currentRotationAngle = cameraObj.transform.eulerAngles.y;
                float currentHeight = cameraObj.transform.position.y;

                // Damp the rotation around the y-axis
                currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

                // Damp the height
                currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

                // Convert the angle into a rotation
                Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

                // Set the position of the camera on the x-z plane to:
                // distance meters behind the target

                Vector3 pos = target.position;
                pos -= currentRotation * Vector3.forward * distance;
                pos.y = currentHeight;
                cameraObj.transform.position = pos;


                // Always look at the target
                cameraObj.transform.LookAt(target);
            }
        }
    }
}
