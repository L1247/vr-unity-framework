using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using star.framework.system;
public class FPSFireManager_network : Photon.MonoBehaviour
{
    public ImpactInfo[] ImpactElemets = new ImpactInfo[0];
    public float BulletDistance = 100;
    public int MaxBullet = 70;
    public GameObject ImpactEffect;
    public GameObject M4A1_Sopmod;
    public GameObject Shell_Copper;
    public Transform BulletStart;
    public PlayerControl playerControl;
    public FireShellController fshellController;
    public float ReloadAngle = 30;

    private Animator M4A1_Anim;
    private Animator M4A1_Shell;
    private Animator M4A1_Magazine;
    private int _nowBullet;
    private bool isReload = false;
    private bool isReloading = false;
    private BloodEffectID bloodID;

    star.framework.system.SoundManager sm;
    float ShootInterval = 0.1f;
    float noBulleteSoundLength;
    bool isNoBulletSoundPlayEnd;

    void Start()
    {
        sm = star.framework.system.SoundManager.instance;
        M4A1_Anim = M4A1_Sopmod.GetComponent<Animator>();
        M4A1_Shell = Shell_Copper.GetComponent<Animator>();
        M4A1_Magazine = M4A1_Sopmod.transform.Find("M4A1_Sopmod_Magazine").GetComponent<Animator>();
        //AudioClip ac = sm.GetAudioClip(GameEM.SoundList.玩家_沒子彈);
        //noBulleteSoundLength = ac.length;
        init();
    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (transform.eulerAngles.x > ReloadAngle && transform.eulerAngles.x < ReloadAngle + 20 && _nowBullet != MaxBullet)
            {
                GunReload();
            }
        }
    }

 
    public void init()
    {
        isReload = false;
        isReloading = false;
        isNoBulletSoundPlayEnd = true;
        _nowBullet = MaxBullet;
        fshellController.init();
        CancelInvoke("Shoot");
    }

    public void onTriggerPressed()
    {
        InvokeRepeating("Shoot", 0, ShootInterval);
    }

    public void onTriggerReleased()
    {
        CancelInvoke("Shoot");
    }

    public void onShellTriggerPressed()
    {
        fshellController.AimShellTarget();
    }

    public void onShellTriggerReleased()
    {
        fshellController.FireShell();
    }

    public void onReloadPressed()
    {
        GunReload();
    }

    void Shoot()
    {
        if (playerControl.IsPlayerAlive())
        {

            if (!isReload)
            {
                bloodID = (BloodEffectID)Random.Range(0, 2);
                photonView.RPC("RpcShoot", PhotonTargets.All, bloodID);
            }

            //if (photonView.isMine)
            //{
                CheckBullet();
            //}
        }
    }

    void GunReload()
    {
        if (playerControl.IsPlayerAlive() && !isReloading)
        {
            //sm.PlaySound(GameEM.SoundList.玩家_換子彈);

            isReload = true;
            isReloading = true;
            _nowBullet = MaxBullet;
            Invoke("reloading", 0.4f);
            M4A1_Anim.Play("M4A1_Magazine");
        }
    }


    [PunRPC]
    void RpcShoot(BloodEffectID bloodID)
    {
        //sm.PlaySound(GameEM.SoundList.玩家_開槍);
        M4A1_Anim.Play("M4A1_Anim");
        M4A1_Shell.Play("M4A1_Shell");
        ImpactEffect.SetActive(false);
        ImpactEffect.SetActive(true);
        GameObject bullet = SmartPool.Spawn("bullet");

        if (!bullet) return;
        fire(bullet, bloodID, BulletStart);
        if ( PhotonNetwork.isMasterClient )
        {
            RecordManager.PlayerShot( photonView );
        }
    }

    public void fire(GameObject bullet, BloodEffectID bloodID, Transform bulletStart)
    {
        bulletController bb = bullet.GetComponent<bulletController>();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();



        bullet.transform.position = bulletStart.position;
        bullet.transform.rotation = bulletStart.rotation;

        bb.bulletInit();
        bb.DestroyBullet();
        bb.bloodEffectID = bloodID;
        rb.AddForce(bulletStart.up * 15000f);
    }

    void CheckBullet()
    {
        if (!isReload && _nowBullet > 0)
        {
            //扣子彈
            _nowBullet = _nowBullet - 1;
            if (_nowBullet == 0)
            {
                RecordManager.AddPlayTimes();
                isReload = true;
            }
        }
        else if (!isReloading)
        {
            //沒子彈
            if (isNoBulletSoundPlayEnd)
            {
                M4A1_Magazine.Play("Hint", 0);
                //sm.PlaySound(GameEM.SoundList.玩家_沒子彈);
                isNoBulletSoundPlayEnd = false;
                Invoke("noBulletSoundDelay", noBulleteSoundLength);
            }
        }
    }

    void noBulletSoundDelay()
    {
        isNoBulletSoundPlayEnd = true;
    }

    void reloading()
    {
        M4A1_Magazine.Play("Idle");
        isReload = false;
        isReloading = false;
    }

    [System.Serializable]
    public class ImpactInfo
    {
        public MaterialType.MaterialTypeEnum MaterialType;
        public GameObject ImpactEffect;
    }

    GameObject GetImpactEffect(GameObject impactedGameObject)
    {
        var materialType = impactedGameObject.GetComponent<MaterialType>();
        if (materialType == null)
            return null;
        foreach (var impactInfo in ImpactElemets)
        {
            if (impactInfo.MaterialType == materialType.TypeOfMaterial)
                return impactInfo.ImpactEffect;
        }
        return null;
    }
}
