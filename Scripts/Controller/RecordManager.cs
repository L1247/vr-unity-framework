using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 儲存玩家的資訊
/// </summary>
public class RecordManager
{
    static int BulletCount;

    static int NowAllPlayCount;
    public static bool isAddTime = false;
    /// <summary>
    /// ID , PlayCount
    /// </summary>
    static Dictionary<int , int> PlayerDic = new Dictionary<int , int>();

    public static void SetBulletCount ( GameObject aik )
    {
        BulletCount = aik.GetComponentInChildren<FPSFireManager_network>().MaxBullet;
    }

    static int GetAllPlayCount ( )
    {
        return Common.ReadPlayCountTxt();
    }

    static void SetAllPlayCount ( int PlayNum )
    {
        Common.WritePlayCountTxt( PlayNum );
    }

    static void AddPlayCount ( int viewId )
    {
        if ( NowAllPlayCount <= 0 )
        {
            NowAllPlayCount = GetAllPlayCount();
        }
        NowAllPlayCount++;
        int TotalPlayCount = NowAllPlayCount;
        SetAllPlayCount( TotalPlayCount );
    }


    public static void AddPlayTimes()
    {
        //Not in game.
        //if (DouduckGame.DouduckGameCore.GetSystem<GameMainSystem>().IsGaming == false)
        //    return;
        if (!isAddTime)
        {
            isAddTime = true;
            //VerifyCommon.addPlayTimes();
        }
    }

    /// <summary>
    /// 儲存遊玩次數
    /// </summary>
    /// <param name="pv"></param>
    public static void PlayerShot ( PhotonView pv )
    {
        //Not in game.
        //if ( DouduckGame.DouduckGameCore.GetSystem<GameMainSystem>().IsGaming == false )
        //    return;

        int playerID = pv.owner.ID;
        int playCount = -1;
        PlayerDic.TryGetValue( playerID , out playCount );
        if ( playCount == -1 )
        {
            playCount = 0;
            PlayerDic.Add( playerID , 0 );
        }

        if ( playCount >= BulletCount )
        {
            return;
        }

        playCount += 1;
        PlayerDic[ playerID ] = playCount;
        //Debug.Log( playCount );
        if ( playCount >= BulletCount )
        {
            AddPlayCount( playerID );
            //Debug.Log( playerID + " : ADD ALL PlayerCount ; NowAllPlayCount is "+ NowAllPlayCount );
        }
    }

    public static void ClearPlayerShot ( )
    {
        List<int> keys = new List<int>( PlayerDic.Keys );

        foreach ( var key in keys )
        {
            //Debug.Log( PlayerDic[ key ] );
            PlayerDic[ key ] = 0;
            //Debug.Log( PlayerDic[ key ] );
        }
    }
}
