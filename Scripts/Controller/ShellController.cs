﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ShellController : VRTK_BasePointer
{
    [Tooltip("The length of the projected forward pointer beam, this is basically the distance able to point from the origin position.")]
    public float pointerLength = 10f;
    [Tooltip("The maximum angle in degrees of the origin before the beam curve height is restricted. A lower angle setting will prevent the beam being projected high into the sky and curving back down.")]
    [Range(1, 100)]
    public float beamHeightLimitAngle = 100f;
    [Tooltip("The amount of height offset to apply to the projected beam to generate a smoother curve even when the beam is pointing straight.")]
    public float beamCurveOffset = 1f;
    [Tooltip("A custom Game Object can be applied here to use instead of the default sphere for the beam tracer. The custom Game Object will match the rotation of the object attached to.")]
    public GameObject customPointerTracer;
    [Tooltip("The number of items to render in the beam bezier curve. A high number here will most likely have a negative impact of game performance due to large number of rendered objects.")]
    public int pointerDensity = 10;
    [Tooltip("The size of the ground pointer cursor. This number also affects the size of the objects in the bezier curve beam. The larger the radius, the larger the objects will be.")]
    public float pointerCursorRadius = 0.5f;
    [Tooltip("Beam's radius / 8")]
    public float BeamSizeOffset = 5f;
    [Tooltip("Rescale each pointer tracer element according to the length of the Bezier curve.")]
    public bool rescalePointerTracer = false;
    [Tooltip("A cursor is displayed on the ground at the location the beam ends at, it is useful to see what height the beam end location is, however it can be turned off by toggling this.")]
    public bool showPointerCursor = true;
    [Tooltip("The pointer cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
    public bool pointerCursorMatchTargetRotation = false;
    [Tooltip("A custom Game Object can be applied here to use instead of the default flat cylinder for the pointer cursor.")]
    public GameObject customPointerCursor;
    [Tooltip("A custom Game Object can be applied here to appear only if the teleport is allowed (its material will not be changed ).")]
    public GameObject validTeleportLocationObject = null;

    public PhotonView photonview;
    private VRTK_CurveGenerator curvedBeam;
    public FireShellController fscontoller;

    [HideInInspector]
    public  bool beamActive = false;
    private GameObject curvedBeamContainer;
    private Vector3 fixedForwardBeamForward;
    private const float BEAM_ADJUST_OFFSET = 0.00001f;
    private GameObject pointerCursor;
    private Vector3 contactNormal;
    private GameObject validTeleportLocationInstance = null;


    protected override void Start()
    {
        if (photonview.isMine)
        {
            base.OnEnable();
            InitPointer();
            TogglePointer(false);
            beamActive = false;
        }
    }

    protected override void Update()
    {
        if (photonview.isMine)
        {
            base.Update();
            curvedBeam.TogglePoints(beamActive);
            pointerCursor.SetActive(beamActive);
            if (beamActive)
            {
                var jointPosition = ProjectForwardBeam();
                var downPosition = ProjectDownBeam(jointPosition);
                fscontoller._beamPoints = DisplayCurvedBeam(jointPosition, downPosition);
                SetPointerCursor(downPosition);
            }
        }   
    }

    private void TogglePointerCursor(bool state)
    {
        var pointerCursorState = (showPointerCursor && state ? showPointerCursor : false);
        pointerCursor.gameObject.SetActive(pointerCursorState);
        base.TogglePointer(state);
    }

    private void SetPointerCursor(Vector3 cursorPosition)
    {
        destinationPosition = cursorPosition;

        if (pointerContactTarget != null)
        {
            TogglePointerCursor(true);
            pointerCursor.transform.position = cursorPosition;
            if (pointerCursorMatchTargetRotation)
            {
                pointerCursor.transform.rotation = Quaternion.FromToRotation(Vector3.up, contactNormal);
            }
            base.UpdateDependencies(pointerCursor.transform.position);
            UpdatePointerMaterial(pointerHitColor);
            if (validTeleportLocationInstance != null)
            {
                validTeleportLocationInstance.SetActive(ValidDestination(pointerContactTarget, destinationPosition));
            }
        }
        else
        {
            TogglePointerCursor(false);
            UpdatePointerMaterial(pointerMissColor);
        }
    }

    private GameObject CreateCursor()
    {
        var cursorYOffset = 0.02f;
        var cursor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        var cursorRenderer = cursor.GetComponent<MeshRenderer>();

        cursor.transform.localScale = new Vector3(pointerCursorRadius, cursorYOffset, pointerCursorRadius);
        cursorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        cursorRenderer.receiveShadows = false;
        cursorRenderer.material = pointerMaterial;
        Destroy(cursor.GetComponent<CapsuleCollider>());
        return cursor;
    }

    protected override void InitPointer()
    {
        pointerCursor = (customPointerCursor ? Instantiate(customPointerCursor) : CreateCursor());
        if (validTeleportLocationObject != null)
        {
            validTeleportLocationInstance = Instantiate(validTeleportLocationObject);
            validTeleportLocationInstance.name = string.Format("[{0}]BasePointer_BezierPointer_TeleportBeam", gameObject.name);
            validTeleportLocationInstance.transform.SetParent(pointerCursor.transform);
            validTeleportLocationInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
            validTeleportLocationInstance.SetActive(false);
        }

        pointerCursor.name = string.Format("[{0}]BasePointer_BezierPointer_PointerCursor", gameObject.name);
        VRTK_PlayerObject.SetPlayerObject(pointerCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
        pointerCursor.layer = LayerMask.NameToLayer("Ignore Raycast");
        pointerCursor.SetActive(false);

        curvedBeamContainer = new GameObject(string.Format("[{0}]BasePointer_BezierPointer_CurvedBeamContainer", gameObject.name));
        VRTK_PlayerObject.SetPlayerObject(curvedBeamContainer, VRTK_PlayerObject.ObjectTypes.Pointer);
        curvedBeamContainer.SetActive(false);
        curvedBeam = curvedBeamContainer.gameObject.AddComponent<VRTK_CurveGenerator>();
        curvedBeam.transform.SetParent(null);
        curvedBeam.Create(pointerDensity, BeamSizeOffset, customPointerTracer, rescalePointerTracer);

        base.InitPointer();
    }


    private Vector3[] DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
    {
        Vector3[] beamPoints = new Vector3[]
        {
                GetOriginPosition(),
                jointPosition + new Vector3(0f, beamCurveOffset, 0f),
                downPosition,
                downPosition,
        };
        var tracerMaterial = (customPointerTracer ? null : pointerMaterial);
        curvedBeam.SetPoints(beamPoints, tracerMaterial, currentPointerColor);
        if (pointerVisibility != pointerVisibilityStates.Always_Off)
        {
            curvedBeam.TogglePoints(true);
        }

        return beamPoints;
    }


    private Vector3 ProjectForwardBeam()
    {
        var attachedRotation = Vector3.Dot(Vector3.up, transform.forward.normalized);
        var calculatedLength = pointerLength;
        var useForward = GetOriginForward();
        if ((attachedRotation * 100f) > beamHeightLimitAngle)
        {
            useForward = new Vector3(useForward.x, fixedForwardBeamForward.y, useForward.z);
            var controllerRotationOffset = 1f - (attachedRotation - (beamHeightLimitAngle / 100f));
            calculatedLength = (pointerLength * controllerRotationOffset) * controllerRotationOffset;

        }
        else
        {
            fixedForwardBeamForward = GetOriginForward();
        }

        var actualLength = calculatedLength;
        Ray pointerRaycast = new Ray(GetOriginPosition(), useForward);

        RaycastHit collidedWith;
        var hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith, calculatedLength, ~layersToIgnore);

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != collidedWith.collider))
        {
            pointerContactDistance = 0f;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            pointerContactDistance = collidedWith.distance;
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && pointerContactDistance < calculatedLength)
        {
            actualLength = pointerContactDistance;
        }

        //Use BEAM_ADJUST_OFFSET to move point back and up a bit to prevent beam clipping at collision point
        return (pointerRaycast.GetPoint(actualLength - BEAM_ADJUST_OFFSET) + (Vector3.up * BEAM_ADJUST_OFFSET));
    }


    private Vector3 ProjectDownBeam(Vector3 jointPosition)
    {
        Vector3 downPosition = Vector3.zero;
        Ray projectedBeamDownRaycast = new Ray(jointPosition, Vector3.down);
        RaycastHit collidedWith;

        var downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith, float.PositiveInfinity, ~layersToIgnore);

        if (!downRayHit || (pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != collidedWith.collider))
        {
            if (pointerContactRaycastHit.collider != null)
            {
                base.PointerOut();
            }
            pointerContactTarget = null;
            pointerContactRaycastHit = new RaycastHit();
            contactNormal = Vector3.zero;
            downPosition = projectedBeamDownRaycast.GetPoint(0f);
        }

        if (downRayHit)
        {
            pointerContactTarget = collidedWith.transform;
            pointerContactRaycastHit = collidedWith;
            contactNormal = collidedWith.normal;
            downPosition = projectedBeamDownRaycast.GetPoint(collidedWith.distance);
            base.PointerIn();
        }
        return downPosition;
    }

}