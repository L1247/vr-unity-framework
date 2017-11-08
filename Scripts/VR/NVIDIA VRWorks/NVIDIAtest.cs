using UnityEngine;

public class NVIDIAtest : MonoBehaviour
{
    private NVIDIA.VRWorks m_VRWorks;
    private GUIStyle m_GUIStyle = new GUIStyle();
    public bool ShowOnGUI;
    void Awake ( )
    {
        m_GUIStyle.normal.textColor = new Color( 0 , 0 , 0 );
        m_GUIStyle.fontSize = 14;
        m_GUIStyle.fontStyle = FontStyle.Bold;
    }

    // Use this for initialization
    void Start ( )
    {
        m_VRWorks = Camera.main.GetComponent<NVIDIA.VRWorks>();

        if ( m_VRWorks.IsFeatureAvailable( NVIDIA.VRWorks.Feature.SinglePassStereo ) )
        {
            m_VRWorks.SetActiveFeature( NVIDIA.VRWorks.Feature.LensMatchedShading );
            //m_VRWorks.SetActiveFeature( NVIDIA.VRWorks.Feature.MultiResolution );
        }
    }

    //// Update is called once per frame
    //void Update () {
    //       print( m_VRWorks.GetActiveFeature() );
    //}
    private void OnGUI ( )
    {
        if ( ShowOnGUI )
        {
            if ( m_VRWorks == null )
                m_VRWorks = Camera.main.GetComponent<NVIDIA.VRWorks>();
            string mode = "None";
            NVIDIA.VRWorks.Feature feature = m_VRWorks.GetActiveFeature();
            if ( feature == NVIDIA.VRWorks.Feature.LensMatchedShading )
            {
                mode = "SPS + LMS";
            }
            else if ( feature == NVIDIA.VRWorks.Feature.SinglePassStereo )
            {
                mode = "SPS";
            }
            else if ( feature == NVIDIA.VRWorks.Feature.MultiResolution )
            {
                mode = "MRS";
            }
            GUI.Label( new Rect( 10 , 60 , 300 , 30 ) , "VRWorks: " + mode , m_GUIStyle );
        }
    }
}
