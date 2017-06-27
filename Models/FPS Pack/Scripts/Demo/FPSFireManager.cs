using UnityEngine;
using System.Collections;

public class FPSFireManager : MonoBehaviour
{
    public ImpactInfo[ ] ImpactElemets = new ImpactInfo[ 0 ];
    public float BulletDistance = 100;
    public GameObject ImpactEffect;
    public GameObject M4A1_Sopmod;
    private Animator M4A1_Anim;
    //public GameObject Shell_Copper;
    //private Animator M4A1_Shell;
    float ShootInterval = .1f;

    private void Start()
    {
        //M4A1_Anim = M4A1_Sopmod.GetComponent<Animator>();
        //M4A1_Shell = Shell_Copper.GetComponent<Animator>();
    }
    void Update ( )
    {
        //if (Input.GetMouseButtonDown(0)) {
        //    RaycastHit hit;
        //       var ray = new Ray(transform.position, transform.forward);
        //       if (Physics.Raycast(ray, out hit, BulletDistance)) {
        //           var effect = GetImpactEffect(hit.transform.gameObject);
        //           if (effect==null)
        //               return;
        //           ImpactEffect.SetActive(false);
        //           ImpactEffect.SetActive(true);
        //           var effectIstance = Instantiate(effect, hit.point, new Quaternion()) as GameObject;
        //           effectIstance.transform.LookAt(hit.point + hit.normal);
        //           Destroy(effectIstance, 4);
        //       }
        //}
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            InvokeRepeating( "Shoot" , 0 , ShootInterval );
        }
        else if ( Input.GetMouseButtonUp( 0 ) )
        {
            CancelInvoke( "Shoot"  );
        }
    }


    void Shoot ( )
    {
        //RaycastHit hit;
        //ImpactEffect.SetActive( false );
        ////M4A1_Anim.Play("M4A1_Anim");
        //ImpactEffect.SetActive( true );
        ////M4A1_Shell.Play("M4A1_Shell");
        //var ray = new Ray( transform.position , transform.forward );
        //if ( Physics.Raycast( ray , out hit , BulletDistance ) )
        //{
        //    //Debug.Log( hit.collider.name );
        //    DinoHurt dinoHurt = hit.transform.GetComponent<DinoHurt>();

        //    var effect = Resources.Load<GameObject>( "Blood11" );
        //    var effectIstance = Instantiate( effect , hit.point , new Quaternion() ) as GameObject;
        //    effectIstance.transform.LookAt(/* hit.point*/ /*+*/hit.normal );

        //    //if ( dinoHurt )
        //    //    dinoHurt.Hit();
        //    if (hit.transform.GetComponent<Rigidbody>()) {
        //        BodyPartGetHurt bodypartgethurt = hit.transform.GetComponent<BodyPartGetHurt>();
        //        if (bodypartgethurt)
        //        {
        //            bodypartgethurt.hitposition = ray.direction;
        //            bodypartgethurt.oriposition = transform.position;
        //            bodypartgethurt.Hit();
        //        }
        //    }
        //}
    }


    [System.Serializable]
    public class ImpactInfo
    {
        public MaterialType.MaterialTypeEnum MaterialType;
        public GameObject ImpactEffect;
    }

    GameObject GetImpactEffect ( GameObject impactedGameObject )
    {
        var materialType = impactedGameObject.GetComponent<MaterialType>();
        if ( materialType == null )
            return null;
        foreach ( var impactInfo in ImpactElemets )
        {
            if ( impactInfo.MaterialType == materialType.TypeOfMaterial )
                return impactInfo.ImpactEffect;
        }
        return null;
    }
}
