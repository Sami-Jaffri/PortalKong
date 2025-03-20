using System;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Portal[] portals = new Portal[2];
    public GameObject portalPrefabA;
    public GameObject portalPrefabB;
    private Camera portalCamera;
    public Camera mainCamera;

    [Serializable]
    public struct Portal
    {
        public Portal(GameObject portalObject,Transform portalDestination, int inceptionCount,bool isActive)
        {
            this.portalObject = portalObject;
            this.portalDestination = portalDestination;
            this.inceptionCount = inceptionCount;
            this.isActive = isActive;
        }
        public GameObject portalObject;   
        public Transform portalDestination;
        public int inceptionCount;
        public bool isActive;
    }

    void Update()
    {
        Debug.Log(portals[0]);
        //UpdatePortals();
    }

    void UpdatePortals()
    {
        if (!portals[0].isActive || !portals[1].isActive)
        {
            return;
        }
        else
        {
            portalCamera = portals[0].portalObject.GetComponent<Camera>();
            for (int i = portals[0].inceptionCount - 1; i >= 0; --i)
            {
                 RenderPortal(portals[0].portalObject, portals[1].portalObject, portals[0].inceptionCount);
            }
       
            portalCamera = portals[1].portalObject.GetComponent<Camera>();
            for (int i = portals[1].inceptionCount - 1; i >= 0; --i)
            {
                RenderPortal(portals[1].portalObject, portals[0].portalObject, portals[1].inceptionCount);
            }
        }
    }

    void RenderPortal(GameObject inPortal, GameObject outPortal, int iteration)
    {
        //Transform inTransform = inPortal.transform;
        //Transform outTransform = outPortal.transform;

        //Transform cameraTransform = portalCamera.transform;
        //cameraTransform.position = transform.position;
        //cameraTransform.rotation = transform.rotation;

        
        //var relativePosition = inTransform.InverseTransformPoint(cameraTransform.position);
        //relativePosition = Quaternion.Euler(0f, 180f, 0f) * relativePosition;
        //cameraTransform.position = outTransform.TransformPoint(relativePosition);

        //Quaternion relativeRotation = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
        //relativeRotation = Quaternion.Euler(0f, 180f, 0f) * relativeRotation;
        //cameraTransform.rotation = outTransform.rotation * relativeRotation;
        

        //Plane p = new Plane(-outTransform.forward, outTransform.position);
        //Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        //Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;
        //var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        //portalCamera.projectionMatrix = newMatrix;
    }
}
