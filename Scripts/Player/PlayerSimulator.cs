using UnityEngine;
using System.Collections;

/// <summary>
/// 在Editor中控制角色攻擊、移動。
/// </summary>
public class PlayerSimulator : Photon.PunBehaviour
{
    [SerializeField]
    private Transform offset;
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    bool bInit;

    Transform RightControllerTrans;

    // Use this for initialization
    void Start ( )
    {
        if ( photonView.isMine == false )
        {
            this.enabled = false;
            return;
        }
        bool bSimlatorMode = true/*DouduckGame.DouduckGameCore.GetSystem<GameMainSystem>().IsPCSimlatorMode*/;

        if ( Application.isEditor == false )
            bSimlatorMode = false;

        if ( SteamVR.active && SteamVR.usingNativeSupport ||
            bSimlatorMode == false )
        {
            this.enabled = false;
            return;
        }
        offset.localRotation = Quaternion.Euler( Vector3.zero );
        RightControllerTrans = CameraRigFinder.GetActualRighitController().transform;
        RightControllerTrans.localPosition = Vector3.up * -0.2f;
        Vector3 rot = RightControllerTrans.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    // Update is called once per frame
    void Update ( )
    {
        float mouseX = Input.GetAxis( "Mouse X" );
        float mouseY = -Input.GetAxis( "Mouse Y" );

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp( rotX , -clampAngle / 4 , clampAngle / 4 );
        rotY = Mathf.Clamp( rotY , -clampAngle / 2 , clampAngle / 2 );
        Quaternion localRotation = Quaternion.Euler( rotX , rotY - 90 , 0.0f );
        RightControllerTrans.rotation = localRotation;
    }


    void LateUpdate ( )
    {
        //因AvatarController 在Update中才執行，所以在他之後。
        if ( bInit == false )
        {
            bInit = true;
            CameraRigFinder.GetCameraRig().transform.position += Vector3.up * 1.5f;
        }
    }
}
