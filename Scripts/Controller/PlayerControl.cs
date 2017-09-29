using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PlayerControl : Photon.PunBehaviour
{
    public HPController hpcontroller;
    public FPSFireManager_network fpsscript;
    public float dieEffectOffset = 0.1f;
    public float hurtEffectOffset = 0.2f;
    public GameObject playerModel;
    public GameObject Weapon;
    public GameObject MuzzleFlash_long;
    public GameObject MuzzleFlash_short;

    private Lutify lutifyscript;
    private ScreenOverlay screenoverlay;
    //private EnemyWaveSpawner enemyWaveSpawner;
    private bool isBloodEffect = false;
    private int tempHP;
    private bool _isLowHP;
    private int _id;
    private int ownerID;
    private bool FireUPIsOpen = false;

    public delegate void PlayerControlEvent ( PlayerControl sender );
    public event PlayerControlEvent OnPlayerDie;

    star.framework.system.SoundManager sm;

    void Awake ( )
    {
        ownerID = photonView.owner.ID;
        gameObject.name = gameObject.name + "  " + ownerID;
    }

    void Start ( )
    {

        //ccMessage.f_AddListener( GameState.RPC_Restart , Restart );
        //ccMessage.f_AddListener( DinoGameEM.GameEvent.RPC_FireUP, FireUPRPC );

        sm = star.framework.system.SoundManager.instance;
        lutifyscript = CameraRigFinder.GetHead_Eyes().GetComponent<Lutify>();
        screenoverlay = CameraRigFinder.GetHead_Eyes().GetComponent<ScreenOverlay>();
        
        

        PlayerControlInit();

        if ( hpcontroller == null )
            Debug.LogWarning( "[ " + gameObject.name + " ] Don't Have HPController " );
        else
        {
            hpcontroller.OnDie += OnDie;
            hpcontroller.OnHit += OnHit;
        }

        this.OnPlayerDie += test;

    }

    void PlayerControlInit ( )
    {
        hpcontroller.HPControllerInit();
        fpsscript.init();
        initPlayerControl();
        _isLowHP = false;
        FireUPIsOpen = true;
        FireUPRPC(gameObject);
    }

    void test ( object data )
    {
        print( GetComponent<PhotonView>().owner.NickName + ",DIE" );  
    }

    void Restart ( object data )
    {
        PlayerControlInit();
    }

    void OnGUI ( )
    {
        if (!CameraManager.inst.IsDisableGui)
        {
            if (photonView.isMine)
            {
                GUI.Button(new Rect(300, 0, 100, 50), "開始遊戲\n(num7)");
                GUI.Button(new Rect(400, 0, 100, 50), "調整本機位置\n(num9)");
                GUI.Button(new Rect(500, 0, 100, 50), "調整全體位置\n(num8)");
                GUI.Button(new Rect(600, 0, 100, 50), "槍枝火力增強\n(num5)");
            }
        }

    }


    [PunRPC]
    void FixPlayerOffset ( )
    {
        CameraManager.inst.FixPlayerOffset();
    }

  
    void FireUPRPC(object date)
    {

        if (!FireUPIsOpen)
        {


            ParticleSystem PMuzz_long = MuzzleFlash_long.GetComponent<ParticleSystem>();
            ParticleSystem PMuzz_short = MuzzleFlash_short.GetComponent<ParticleSystem>();

            var PMuzz_long_main = PMuzz_long.main;
            var PMuzz_short_main = PMuzz_short.main;

            PMuzz_long_main.startSize = 0.35f;
            MuzzleFlash_long.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(83f / 255f, 58f / 255f, 33f / 255f, 128f / 255f));


            PMuzz_short_main.startSize = 0.1f;
            MuzzleFlash_short.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(75f / 255f, 49f / 255f, 22f / 255f, 128f / 255f));

            FireUPIsOpen = true;
        }
        else
        {
            ParticleSystem PMuzz_long = MuzzleFlash_long.GetComponent<ParticleSystem>();
            ParticleSystem PMuzz_short = MuzzleFlash_short.GetComponent<ParticleSystem>();

            var PMuzz_long_main = PMuzz_long.main;
            var PMuzz_short_main = PMuzz_short.main;

            PMuzz_long_main.startSize = 0.1f;
            MuzzleFlash_long.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(69f / 255f, 46f / 255f, 24f / 255f, 128f / 255f));


            PMuzz_short_main.startSize = 0.08f;
            MuzzleFlash_short.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", new Color(77f / 255f, 51f / 255f, 28f / 255f, 128f / 255f));

            FireUPIsOpen = false;
        }
    }


    void Update ( )
    {
        if ( !IsPlayerAlive() )
        {
            setPlayerDie();
        }
        else
        {
            setPlayerAlive();
        }

        if (photonView.isMine)
        {
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                //ccMessage.f_Broadcast(GameState.Restart);
                photonView.RPC("FixPlayerOffset", PhotonTargets.All);
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                FixPlayerOffset();
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                photonView.RPC("FixPlayerOffset", PhotonTargets.All);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //ccMessage.f_Broadcast(DinoGameEM.GameEvent.FireUP);
            }
        }
    }

    void OnDie ( object sender )
    {
        //sm.StopMusic();
        //sm.PlaySound(GameEM.SoundList.玩家_死亡);
        ccEngine.ccTimeEvent.Instance.f_UnRegEvent( _id );
        if ( OnPlayerDie != null )
        {
            OnPlayerDie( this );
        }
    }

    void OnHit ( object sender )
    {
        if ( photonView.isMine )
        {
            //if ( hpcontroller.getHP() <= ( hpcontroller.Hp / 1 ) && !_isLowHP )
            //{
            //    _isLowHP = true;
            //    _id = ccEngine.ccTimeEvent.Instance.f_RegEvent( 6f , playsound , true );
            //}
        }
    }

    private void playsound ( object data )
    {
        //sm.PlaySound( GameEM.SoundList.玩家_心跳聲 );
    }



    void OnPhotonSerializeView ( PhotonStream stream , PhotonMessageInfo info )
    {
        if ( stream.isWriting )
        {
            //stream.SendNext( hpcontroller.getHP() );
        }
        else
        {
            tempHP = ( int ) stream.ReceiveNext();
            hpcontroller.SetHP( tempHP );
        }
    }

    public void initPlayerControl ( )
    {
        if ( photonView.isMine )
        {
            hpcontroller.HPControllerInit();
            //tempHP = hpcontroller.getHP();

            if ( lutifyscript )
            {
                lutifyscript.Blend = 0;
            }

        }
    }

    public void ReducePlayerHP ( int reduceNum )
    {
        //if ( photonView.isMine && hpcontroller.getHP() > 0 )
        //{
        //    hpcontroller.setHit( reduceNum );

        //    if ( !isBloodEffect )
        //    {
        //        StartCoroutine( CamBloodEffect() );
        //    }
        //}
    }

    IEnumerator CamBloodEffect ( )
    {

        isBloodEffect = true;
        screenoverlay.enabled = true;

        yield return new WaitForSeconds( hurtEffectOffset );

        isBloodEffect = false;
        screenoverlay.enabled = false;
        //Enum[ ] soundclipList = new Enum[ ] { GameEM.SoundList.玩家_受傷1 , GameEM.SoundList.玩家_受傷2 , GameEM.SoundList.玩家_受傷3 };
        //sm.PlayRandomSound( soundclipList );
    }

    public int getPlayerHp ( )
    {
        return 0;
        //return hpcontroller.getHP();
    }

    public bool IsPlayerAlive ( )
    {
        return false;
        //return ( hpcontroller.getHP() > 0 ) ? true : false;
    }

    private void setPlayerDie ( )
    {
        if ( photonView.isMine )
        {
            if ( IsPlayerAlive() )
                return;

            if ( lutifyscript && lutifyscript.Blend < 1 )
            {
                lutifyscript.Blend = lutifyscript.Blend + dieEffectOffset;
            }
        }
        else
        {
            if ( playerModel && playerModel.activeSelf )
            {
                Weapon.SetActive( false );
                playerModel.SetActive( false );
            }
        }
    }

    private void setPlayerAlive ( )
    {
        if ( photonView.isMine )
        {
            if ( !IsPlayerAlive() )
                return;

            if ( lutifyscript && lutifyscript.Blend > 0 )
            {
                lutifyscript.Blend = lutifyscript.Blend - dieEffectOffset;
            }
        }
        else
        {
            if ( playerModel && !playerModel.activeSelf )
            {
                Weapon.SetActive( true );
                playerModel.SetActive( true );
            }
        }
    }

    public void OnDestroy ( )
    {
        if ( hpcontroller == null )
            Debug.LogWarning( "[ " + gameObject.name + " ] Don't Have HPController " );
        else
            hpcontroller.OnDie -= OnDie;

        //ccMessage.f_RemoveListener( GameState.RPC_Restart , Restart );

        //if ( photonView.isMine && enemyWaveSpawner != null )
        //{
        //    enemyWaveSpawner.UnregisterDieEvent( this );
        //}
    }


    //public void InjectionEnemyWaveSpawner ( EnemyWaveSpawner _enemyWaveSpawner )
    //{
    //    enemyWaveSpawner = _enemyWaveSpawner;
    //}


}
