using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class FireShellController : Photon.MonoBehaviour {


    public GameObject shell;
    [HideInInspector]
    public Vector3[] _beamPoints;
    public int MaxShell = 1;
    public PlayerControl playercontrol;
    public ShellController shellcontroller;
    private bool isPrepareToFireShell = false;


    private int _nowShell;
    private bool isReload = false;
    private bool isReloading = false;
    private bool isOnFireZone = false;

    star.framework.system.SoundManager sm;

    void Start()
    {
        shellcontroller.DestinationMarkerEnter += ontest1;
        shellcontroller.DestinationMarkerExit +=  ontest2;
        sm = star.framework.system.SoundManager.instance;
    }

    void ontest1(object sender, DestinationMarkerEventArgs e)
    {
        isOnFireZone = true;
    }

    void ontest2(object sender, DestinationMarkerEventArgs e)
    {
        isOnFireZone = false;
    }

    public void AimShellTarget()
    {
        if (playercontrol.IsPlayerAlive() && !isReload)
        {
            shellcontroller.beamActive = true;
            isPrepareToFireShell = true;
        }
    }

    public void FireShell()
    {
        if (playercontrol.IsPlayerAlive() && !isReload && isOnFireZone)
        {
            if (isPrepareToFireShell)
            {
                CheckShell();

                shellcontroller.beamActive = false;
                isPrepareToFireShell = false;
                //產生砲彈
                photonView.RPC("RpcFireShell", PhotonTargets.All, _beamPoints);
                ShellReload();
            }
        }

        isPrepareToFireShell = false;
        shellcontroller.beamActive = false;

    }

    [PunRPC]
    void RpcFireShell(Vector3[] _beamPoints)
    {
        //sm.PlaySound(GameEM.SoundList.爆破_榴彈發射);      
        GameObject obj = Instantiate(shell, shellcontroller.transform.position , shellcontroller.transform.rotation);
        var a = obj.GetComponent<Explo>();
        a.SetPoints(_beamPoints);
    }

    public void init()
    {
        isReload = false;
        isReloading = false;
        isOnFireZone = false;
        //isNoBulletSoundPlayEnd = true;
        _nowShell = MaxShell;
        // CancelInvoke("Shoot");
    }


    void ShellReload()
    {
        if (playercontrol.IsPlayerAlive() && !isReloading)
        {
            //sm.PlaySound(GameEM.SoundList.玩家_換子彈);

            isReload = true;
            isReloading = true;
            _nowShell = MaxShell;
            Invoke("reloading", 5f);
            //M4A1_Anim.Play("M4A1_Magazine");
        }
    }

    void reloading()
    {
        //M4A1_Magazine.Play("Idle");
        isReload = false;
        isReloading = false;
    }

    void CheckShell()
    {
        if (!isReload && _nowShell > 0)
        {
            //扣子彈
            _nowShell = _nowShell - 1;
            if (_nowShell == 0)
            {
                isReload = true;
            }
        }
        else if (!isReloading)
        {
            //沒子彈
            //if (isNoBulletSoundPlayEnd)
            //{
            //    M4A1_Magazine.Play("Hint", 0);
            //    sm.PlaySound(GameEM.SoundList.玩家_沒子彈);
            //    isNoBulletSoundPlayEnd = false;
            //    Invoke("noBulletSoundDelay", noBulleteSoundLength);
            //}
        }
    }


}
