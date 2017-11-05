using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explo : MonoBehaviour
{

    public float force;
    public float radius = 5;
    public LayerMask layerMask = -1;
    public LayerMask layerMaskforTriggerEnter = -1;
    public ForceMode forcemode;
    public float MoveSpeed;
    public Vector3[] points;
    public GameObject ExploEffect;
    public int Damage;


    private Collider[] ExploColliderArray;
    private bool isExplo = false;
    private float upwardModifier;
    private Vector3 ExploPos;
    float bezierCurveLocation = 0;
    // Use this for initialization


    void Start()
    {
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    // //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}

    // Update is called once per frame
    void Update()
    {
        //Vector3 Pos = transform.position;
        //Debug.DrawLine(new Vector3(Pos.x, Pos.y, Pos.z - radius), new Vector3(Pos.x, Pos.y, Pos.z + radius), Color.yellow);

        if (bezierCurveLocation < 1 && !isExplo)
        {
            Vector3 pos = Bezier.GetPoint(points[0], points[1], points[2], points[3], bezierCurveLocation);

            Vector3 temp = pos - transform.position;
            Vector3 temp2 = temp - Vector3.down;
            MoveSpeed += temp2.magnitude;

            bezierCurveLocation += MoveSpeed * 0.05f * Time.deltaTime;
            transform.LookAt(Bezier.GetPoint(points[0], points[1], points[2], points[3], bezierCurveLocation));
            transform.position = pos;
        }

        if (bezierCurveLocation >= 1 && !isExplo)
        {
            DoExplo();
        }
    }



    public void SetPoints(Vector3[] bezierpoints)
    {
        points = bezierpoints;
    }
    // OnCollisionEnter
    /*
    void OnCollisionEnter(Collision collision)
    {
        Instantiate(ExploEffect, transform.position, Quaternion.identity);
        if (PhotonNetwork.isMasterClient)
        {
            ExploPos = transform.position;
            isExplo = true;
            ExploColliderArray = Physics.OverlapSphere(ExploPos, radius, layerMask.value);

            foreach (Collider col in ExploColliderArray)
            {
                Debug.Log(col.gameObject.name);
                HPController _HPController = col.gameObject.GetComponentInParent<HPController>();
                TrexController _TrexController = col.gameObject.GetComponentInParent<TrexController>();
                if (_HPController != null && _TrexController == null)
                {
                    Ragdollmanagement _ragdollman = _HPController.gameObject.GetComponent<Ragdollmanagement>();
                    Vector3 ExploVector = col.gameObject.transform.position - ExploPos;
                    _HPController.setHP(0);
                    _ragdollman.setLastHitInfo(ExploVector, ExploPosName, force, forcemode);
                }

            }

            PhotonNetwork.Destroy(gameObject);
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        int bitRepresentation = 1 << other.gameObject.layer;
        if ((bitRepresentation & layerMaskforTriggerEnter) != 0)
        {
            //other.GetComponent<BodyPartGetHurt>().bodypartHit(Damage);
            DoExplo();
        }
    }
    void DoExplo()
    {
        Instantiate(ExploEffect, transform.position, Quaternion.identity);
        //sm.PlaySound(GameEM.SoundList.爆破_榴彈爆炸);
        //BuzzFloorAdapter t_Buzzfloor = GameObjectFinder.GetObj("BuzzFloorAdapter").GetComponent<BuzzFloorAdapter>();
        //t_Buzzfloor.do_s3Button(1f);
        
        if (PhotonNetwork.isMasterClient)
        {
            ExploPos = transform.position;
            isExplo = true;
            ExploColliderArray = Physics.OverlapSphere(ExploPos, radius, layerMask.value);

            foreach (Collider col in ExploColliderArray)
            {
                //Debug.Log(col.gameObject.name);
                HPController _HPController = col.gameObject.GetComponentInParent<HPController>();
                //TrexController _TrexController = col.gameObject.GetComponentInParent<TrexController>();

                //if (_HPController != null && _TrexController != null)
                //{
                //    _HPController.setHit(Damage);
                //}
                //else if (col.gameObject.name.Contains("Bomb_"))
                //{
                //    _HPController.setHP(0);
                //}
                //else if (_HPController != null && _TrexController == null)
                //{
                //    Ragdollmanagement _ragdollman = _HPController.gameObject.GetComponent<Ragdollmanagement>();
                //    Vector3 ExploVector = col.gameObject.transform.position - ExploPos;
                //    _ragdollman.setLastHitInfo(ExploVector, null, force, forcemode);
                //    _HPController.setHP(0);
                //}
            }

        }
        Destroy(this.gameObject);
    }
}
