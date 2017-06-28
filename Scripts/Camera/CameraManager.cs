using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using y_Network;
using System;
using star.framework.Extensions;

public class CameraManager : Photon.MonoBehaviour
{

    // Use this for initialization
    public GameObject FollowCamera;

    public CameraMode cameraMode;
    public ObservedMode observedMode;

    public bool IsDisableGui = false;

    [Header("第一人稱觀察者設定")]
    public NetWork_ObservedCamera FirstPersonCamera;
    [Header("第三人稱觀察者設定")]
    public NetWork_ObservedCamera ThirdPersonCamera;
    static CameraManager _inst;

    [HideInInspector]
    public Transform target;

    private string selectedTargetID = string.Empty;
    private int selectedViewType = 0;
    private string TargetID = string.Empty;
    private string Name = string.Empty;
    private bool isalive = true;
    private int playerHP = 0;
    private string temp1 = string.Empty;
    private GUIStyle Selectedguistyle;
    private GUIStyle Defaultguistyle;
    private string _StatusString = string.Empty;
    private int second;
    private int _playerIndex;
    private Text TimeText;
    private int eventID;
    public ccMessage m_GameMessage = new ccMessage();

    List<Transform> playerList;

    static public CameraManager inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = (CameraManager)FindObjectOfType(typeof(CameraManager));
                if (_inst == null)
                {
                    Debug.LogError("No GameMain object exists");
                    return null;
                }
            }
            return _inst;
        }
    }

    public void setStatusString(string statusString)
    {
        _StatusString = statusString;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        playerList = PlayerFinder.GetPlayerTransList();
        _playerIndex = 0;
        setStatusString("test");

        TimeText = GameObject.Find("HighScoreCanvas/Text").GetComponent<Text>();
        //ccMessage.f_AddListener(GameState.RPC_Restart, Restart);
        //ccMessage.f_AddListener(GameState.RPC_GameOver, Over);
        //ccMessage.f_AddListener(GameState.RPC_GameComplete, Over);
    }

    void Restart(object data)
    {
        second = 0;
        eventID = ccEngine.ccUpdateEvent.Instance.f_RegEvent(1800, test, 0,null,true);
    }

    void Over(object data)
    {     
        ccEngine.ccUpdateEvent.Instance.f_UnRegEvent(eventID);
    }

    private void test(object data, float data2)
    {
        float sec = 1800 - data2;
        TimeText.text = GetTimeMmSs(sec);
    }



    string GetTimeMmSs(float time)
    {
        TimeSpan interval = TimeSpan.FromSeconds(time);

        string timeText = new DateTime(interval.Ticks).ToString("mm:ss:ff");
        return timeText;
    }

    void SetNextFollowingPlayer()
    {
        if ( playerList.Count == 1 )
        {
            CancelInvoke( "SetNextFollowingPlayer" );
            return;
        }

        if (_playerIndex + 1 == playerList.Count)
        {
            _playerIndex = 0;
        }
        else
        {
            _playerIndex = _playerIndex + 1;
        }
       
        if ( !playerList[_playerIndex].GetComponent<PlayerControl>().IsPlayerAlive())
        {
            SetNextFollowingPlayer();
        }
        else
        {
            target = playerList[_playerIndex].transform.Find("Head").transform;
            setLutifyColor(playerList[_playerIndex].gameObject);
        }
    }

    void Update()
    {

        if (FollowCamera.activeSelf)
        {
            if (cameraMode == CameraMode.ThirdPerson)
            {
                ThirdPersonCamera.ThirdPersonCamera(FollowCamera, target);
            }
            else
            {
                FirstPersonCamera.FirstPersonCamera(FollowCamera, target);
            }
        }

        keyEvent();

    }


    void keyEvent()
    {

        GameObject obj = GameObject.Find("FramesPerSecondCanvas");
        if (!IsDisableGui)
        {
            if (!obj.transform.Find("FramesPerSecondText").GetComponent<Text>().enabled)
            {
                obj.transform.Find("FramesPerSecondText").GetComponent<Text>().enabled = true;
            }
        }
        else
        {
            if (obj.transform.Find("FramesPerSecondText").GetComponent<Text>().enabled)
            {
                obj.transform.Find("FramesPerSecondText").GetComponent<Text>().enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            selectedViewType = 0;
            FollowCamera.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            selectedViewType = 1;
            cameraMode = CameraMode.FirstPerson;
            FollowCamera.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            selectedViewType = 2;
            cameraMode = CameraMode.ThirdPerson;
            FollowCamera.SetActive(true);
        }


        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            if (observedMode != ObservedMode.Auto)
            {
                IsDisableGui = true;
                observedMode = ObservedMode.Auto;
                InvokeRepeating("SetNextFollowingPlayer", 0f, 8f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            IsDisableGui = false;
            observedMode = ObservedMode.Manual;
            CancelInvoke("SetNextFollowingPlayer");
        }
    }


    public void FixPlayerOffset()
    {
        Vector3 headsetV3 = CameraRigFinder.GetHead_Eyes().transform.position;
        Vector3 cameraRigV3 = CameraRigFinder.GetCameraRig().transform.position;

        int playerIndex = PhotonNetwork.player.ID;
        //Transform playerStandSpace = GameObjectFinder.GetObjTransform("Player Stand Space");

        //Vector3 playerPosition = playerStandSpace.GetChild(playerIndex - 1).position;
        Vector3 playerPosition = PlayerFinder.GetPlayerStandPlace(playerIndex).position;

        Vector3 v1;
        v1.x = headsetV3.x;
        v1.y = 0;
        v1.z = headsetV3.z;

        Vector3 v2;
        v2.x = playerPosition.x;
        v2.y = 0;
        v2.z = playerPosition.z;

        Vector3 centerPointV3;
        centerPointV3.x = cameraRigV3.x;
        centerPointV3.y = 0;
        centerPointV3.z = cameraRigV3.z;

        Vector3 offset1 = centerPointV3 - v1;
        Vector3 offset2 = centerPointV3 - v2;

        Vector3 finalOffset = offset1 - offset2;

        CameraRigFinder.GetCameraRig().transform.position = CameraRigFinder.GetCameraRig().transform.position + finalOffset;

    }

    void setLutifyColor(GameObject target)
    {
        if (target.GetComponent<PlayerControl>().IsPlayerAlive())
        {
            FollowCamera.GetComponent<Lutify>().Blend = 0;
        }
        else
        {
            FollowCamera.GetComponent<Lutify>().Blend = 1;
        }
    }


    void OnGUI()
    {
        if (!IsDisableGui)
        {
            Selectedguistyle = new GUIStyle(GUI.skin.button);
            Selectedguistyle.normal.textColor = Color.green;
            Selectedguistyle.onHover.textColor = Color.black;

            Defaultguistyle = new GUIStyle(GUI.skin.button);
            Defaultguistyle.onHover.textColor = Color.black;


            GUI.Button(new Rect(0, 0, 100, 50), "本機畫面\n(num1)", (selectedViewType == 0) ? Selectedguistyle : Defaultguistyle);


            GUI.Button(new Rect(100, 0, 100, 50), "第一人稱視角\n(num2)", (selectedViewType == 1) ? Selectedguistyle : Defaultguistyle);

            GUI.Button(new Rect(200, 0, 100, 50), "第三人稱視角\n(num3)", (selectedViewType == 2) ? Selectedguistyle : Defaultguistyle);

            if (PhotonNetwork.connectedAndReady)
            {
                GUI.Label(new Rect(5, Screen.height - 20, 500, 20), (PhotonNetwork.isMasterClient) ? "(Master)" + PhotonNetwork.player.NickName : "(Client)" + PhotonNetwork.player.NickName);
            }
            else
            {
                GUI.Label(new Rect(5, Screen.height - 20, 100, 20), "(Connecting....)");
            }


            if (playerList.Count > 0)
            {
                if (selectedTargetID == string.Empty)
                {
                    target = playerList[0].transform.Find("Head");
                    selectedTargetID = playerList[0].GetPhotonView().owner.ID.ToString();
                }

                int i = 50;
                foreach (var item in playerList)
                {
                    TargetID = item.GetPhotonView().owner.ID.ToString();
                    Name = item.GetPhotonView().owner.NickName.ToString();
                    isalive = item.GetComponent<PlayerControl>().IsPlayerAlive();
                    playerHP = item.GetComponent<PlayerControl>().getPlayerHp();
                    temp1 = (TargetID == PhotonNetwork.player.ID.ToString()) ? "【本機】" : "【玩家】  ";
                    temp1 += (isalive) ? "存活" : "死亡";
                    temp1 += " 血量:" + playerHP + "\n";
                    temp1 += "  切換此玩家畫面 (按" + TargetID + ")\n";


                    if ((GUI.Button(new Rect(0, i, 200, 50), temp1 + Name, (selectedTargetID == TargetID) ? Selectedguistyle : Defaultguistyle) || Input.GetKeyDown(TargetID)) && observedMode == ObservedMode.Manual)
                    {
                        target = item.transform.Find("Head");
                        setLutifyColor(item.gameObject);
                        selectedTargetID = TargetID;
                    }

                    i = i + 50;
                }
            }
        }

    }
}

public enum CameraMode
{
    FirstPerson,
    ThirdPerson
}

public enum ObservedMode
{
    Manual,
    Auto
}