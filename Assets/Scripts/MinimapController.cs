using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapController : MonoBehaviour, IPointerClickHandler
{

    public Material cameraBoxMaterial;

    public Camera minimap;
    private Plane _groundPlane;

    public float lineWidth;

    private Vector3 GetCameraFrustumPoint(Vector3 position)
    {
        var positionRay = Camera.main.ScreenPointToRay(position);
        float distance;
        if (_groundPlane.Raycast(positionRay, out distance))
        {
            return positionRay.GetPoint(distance);
        }

        return new Vector3();
    }

    private void Awake()
    {
        _groundPlane = new Plane(Vector3.forward, Vector3.zero);
    }

    public void OnPostRender()
    {

        
        Vector3 minViewportPoint = minimap.WorldToViewportPoint(GetCameraFrustumPoint(new Vector3(0f, 0f)));
        Vector3 maxViewportPoint = minimap.WorldToViewportPoint(GetCameraFrustumPoint(new Vector3(Screen.width, Screen.height)));

        float minX = minViewportPoint.x;
        float minY = minViewportPoint.y;

        float maxX = maxViewportPoint.x;
        float maxY = maxViewportPoint.y;

        GL.PushMatrix();
        {
            cameraBoxMaterial.SetPass(0);
            GL.LoadOrtho();

            GL.Begin(GL.QUADS);
            GL.Color(Color.red);
            {

                GL.Vertex(new Vector3(minX, minY + lineWidth, 0));
                GL.Vertex(new Vector3(minX, minY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, minY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, minY + lineWidth, 0));


                GL.Vertex(new Vector3(minX + lineWidth, minY, 0));
                GL.Vertex(new Vector3(minX - lineWidth, minY, 0));
                GL.Vertex(new Vector3(minX - lineWidth, maxY, 0));
                GL.Vertex(new Vector3(minX + lineWidth, maxY, 0));



                GL.Vertex(new Vector3(minX, maxY + lineWidth, 0));
                GL.Vertex(new Vector3(minX, maxY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, maxY - lineWidth, 0));
                GL.Vertex(new Vector3(maxX, maxY + lineWidth, 0));

                GL.Vertex(new Vector3(maxX + lineWidth, minY, 0));
                GL.Vertex(new Vector3(maxX - lineWidth, minY, 0));
                GL.Vertex(new Vector3(maxX - lineWidth, maxY, 0));
                GL.Vertex(new Vector3(maxX + lineWidth, maxY, 0));

            }
            GL.End();
        }
        GL.PopMatrix();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Minimap clicked at: " + eventData.position);
        var miniMapRect = GetComponent<RectTransform>().rect;
        var screenRect = new Rect(
            transform.position.x, 
            transform.position.y, 
            miniMapRect.width, miniMapRect.height);
    
        var mousePos = Input.mousePosition;
        mousePos.y -= screenRect.y;
        mousePos.x -= screenRect.x;

        var camPos = new Vector3(
            mousePos.x *  (GameManager.Instance.Grid.size.x * GameManager.Instance.Grid.cellSize.x / screenRect.width),
            mousePos.y *  (GameManager.Instance.Grid.size.y * GameManager.Instance.Grid.cellSize.y / screenRect.height),
            Camera.main.transform.position.z);
        Camera.main.transform.position = camPos;
    }
}