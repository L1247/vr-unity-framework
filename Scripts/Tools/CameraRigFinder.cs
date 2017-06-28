using UnityEngine;
using VRTK;

/// <summary>
/// For CamerigInfo
/// </summary>
public class CameraRigFinder 
{
    ///  /// <summary>
    /// Return CameraRig
    /// </summary>
    /// <returns></returns>
    public static GameObject GetCameraRig (  )
    {
        return VRTK_SDKManager.instance.actualBoundaries;
    }

    /// <summary>
    /// Return Camera (eye)
    /// </summary>
    /// <returns></returns>
    public static GameObject GetHead_Eyes (  )
    {
        return VRTK_SDKManager.instance.actualHeadset;
    }

    /// <summary>
    /// Return Camera (ears)
    /// </summary>
    /// <returns></returns>
    public static GameObject GetHead_Ears ( )
    {
        GameObject eyes = GetHead_Eyes();
        
        //Head/ears
        return eyes.transform.parent.Find( "Camera (ears)").gameObject;
    }

    /// <summary>
    /// Return Controller (left)
    /// </summary>
    /// <returns></returns>
    public static GameObject GetActualLeftController (  )
    {
        return VRTK_SDKManager.instance.actualLeftController;
    }

    /// <summary>
    /// Controller (right)
    /// </summary>
    /// <returns></returns>
    public static GameObject GetActualRighitController (  )
    {
        return VRTK_SDKManager.instance.actualRightController;
    }

    /// <summary>
    /// Return [VRTK]/LeftController
    /// </summary>
    /// <returns></returns>
    public static GameObject GetScriptLeftController (  )
    {
        return VRTK_SDKManager.instance.scriptAliasLeftController;
    }

    /// <summary>
    /// Return [VRTK]/RightController
    /// </summary>
    /// <returns></returns>
    public static GameObject GetScriptRighitController (  )
    {
        return VRTK_SDKManager.instance.scriptAliasRightController;
    }

    /// <summary>
    /// Return Controller (left)/Model
    /// </summary>
    /// <returns></returns>
    public static GameObject GetModelLeftController (  )
    {
        return VRTK_SDKManager.instance.modelAliasLeftController;
    }

    /// <summary>
    /// Return Controller (right)/Model
    /// </summary>
    /// <returns></returns>
    public static GameObject GetModelRighitController (  )
    {
        return VRTK_SDKManager.instance.modelAliasRightController;
    }

    /// <summary>
    /// A reference to the GameObject that contains VRTK_ControllerEvents that apply to the Left Hand Controller
    /// </summary>
    /// <returns></returns>
    public static VRTK_ControllerEvents GetEventLeftController ( )
    {
        return GetScriptLeftController().GetComponent<VRTK_ControllerEvents>();
    }

    /// <summary>
    /// A reference to the GameObject that contains VRTK_ControllerEvents that apply to the Right Hand Controller
    /// </summary>
    /// <returns></returns>
    public static VRTK_ControllerEvents GetEventRightController ( )
    {
        return GetScriptRighitController().GetComponent<VRTK_ControllerEvents>();
    }

}
