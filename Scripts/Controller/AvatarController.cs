using UnityEngine;
using System.Collections;
using VRTK;
using star.framework.Extensions;

public class AvatarController : Photon.MonoBehaviour
{

    public GameObject Avatar_Head;
    public GameObject Avatar_RightHand;
    public GameObject Avatar_LeftHand;
    public GameObject AvatarModel;
    public GameObject Lazer;

    private Vector3 ikPositionOffset;
    private PlayerControl playercontrol;
    // Use this for initialization
    void Start()
    {
        playercontrol = GetComponent<PlayerControl>();
        ikPositionOffset = new Vector3();
        if (photonView.isMine)
        {
            CameraRigFinder.GetEventLeftController().TriggerPressed += OnLeftTriggerPressed;
            CameraRigFinder.GetEventRightController().TriggerPressed += OnRightTriggerPressed;
            CameraRigFinder.GetEventLeftController().TriggerReleased += OnLeftTriggerReleased;
            CameraRigFinder.GetEventRightController().TriggerReleased += OnRightTriggerReleased;
            CameraRigFinder.GetEventRightController().TouchpadPressed += OnRightTouchpadPressed;
            CameraRigFinder.GetEventRightController().TouchpadReleased += OnRightTouchpadReleased;
            CameraRigFinder.GetEventRightController().GripPressed += OnGripPressed;
            CameraRigFinder.GetEventRightController().GripReleased += OnGripReleased;

            //AvatarModel.SetActive(false);
            AvatarModel.layer = 17;
            playercontrol.OnPlayerDie += OnPlayerDie;
            // Avatar_RightHand.GetComponent<FPSFireManager_network>().onTriggerPressed();
        }
        else
        {
            Lazer.layer = 17;
        }


    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.isWriting)
        //    stream.SendNext(ikPositionOffset);
        //else
        //    ikPositionOffset = (Vector3)stream.ReceiveNext();
    }

    void OnPlayerDie(object data)
    {
        Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerReleased();
    }


    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            Avatar_Head.SetPostionAndRotation(CameraRigFinder.GetHead_Eyes());
            Avatar_RightHand.SetPostionAndRotation(CameraRigFinder.GetActualRighitController());
            Avatar_LeftHand.SetPostionAndRotation(CameraRigFinder.GetActualLeftController());

            //ikPositionOffset.x = AvatarModel.transform.position.x;
            //ikPositionOffset.y = CameraRigFinder.GetCameraRig().transform.position.y; ;
            //ikPositionOffset.z = AvatarModel.transform.position.z;

            if (playercontrol.IsPlayerAlive())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Avatar_RightHand.GetComponent<FPSFireManager_network>().onTriggerPressed();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Avatar_RightHand.GetComponent<FPSFireManager_network>().onTriggerReleased();
                }

                if (Input.GetMouseButtonUp(1))
                {
                    Avatar_RightHand.GetComponent<FPSFireManager_network>().onReloadPressed();
                }

                if (Input.GetKeyDown(KeyCode.O))
                {
                    Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerPressed();
                }

                if (Input.GetKeyUp(KeyCode.O))
                {
                    Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerReleased();
                }
            }

        }

        //AvatarModel.transform.position = ikPositionOffset;
    }

    void OnLeftTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        //Avatar_LeftHand.GetComponent<HandModelController>().isTriggerPressed = true;
    }

    void OnGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerPressed();
    }

    void OnGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerReleased();
    }

    void OnRightTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (photonView.isMine)
        {
            Avatar_RightHand.GetComponent<FPSFireManager_network>().onTriggerPressed();
        }
    }

    void OnLeftTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        //Avatar_LeftHand.GetComponent<HandModelController>().isTriggerPressed = false;
    }

    void OnRightTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (photonView.isMine)
        {
            Avatar_RightHand.GetComponent<FPSFireManager_network>().onTriggerReleased();
        }
    }

    void OnRightTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (photonView.isMine)
        {
            Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerPressed();
        }
    }

    void OnRightTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (photonView.isMine)
        {
            Avatar_RightHand.GetComponent<FPSFireManager_network>().onShellTriggerReleased();
        }
    }


}
