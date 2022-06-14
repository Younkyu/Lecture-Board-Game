using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ASL;
using UnityEngine.UI;

public class CameraManipulation : MonoBehaviour
{
    private Vector3 click;
    public float movespeed = .015f;
    public float rotatespeed = .1f;
    public int zoomspeed = 7;
    private float cameraY;

    public enum LookAtCompute
    {
        QuatLookRotation = 0,
        TransformLookAt = 1
    };

    //public GameObject LookAt;
    //private Transform LookAtPosition = null;
    public LookAtCompute ComputeMode = LookAtCompute.QuatLookRotation;
    Vector3 delta = Vector3.zero;
    Vector3 mouseDownPos = Vector3.zero;
    //bool uistart = true;
    public ButtonBehavior first;

    void Start()
    {
        //Debug.Assert(LookAt != null);
        //LookAtPosition = LookAt.transform;
        //LookAt.GetComponent<Renderer>().enabled = false;
        //first.GetComponentInChildren<Text>().text = "Enter the word \"Yes\"";
        //first.setQA("Enter the word \"Yes\"", "Yes");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //if (Application.isEditor){
            //    UnityEditor.EditorApplication.isPlaying = false;
            //}
            Application.Quit();
        }

        /*switch (ComputeMode)
        {
            case LookAtCompute.QuatLookRotation:
                // Viewing vector is from transform.localPosition to the lookat position
                Vector3 V = LookAtPosition.localPosition - transform.localPosition;
                Vector3 W = Vector3.Cross(-V, transform.up);
                Vector3 U = Vector3.Cross(W, -V);
                transform.localRotation = Quaternion.LookRotation(V, U);
                break;

            case LookAtCompute.TransformLookAt:
                transform.LookAt(LookAtPosition);
                break;
        }*/

        //transform.LookAt(LookAtPosition);
        // Mouse Button 1/right click: rotates camera
        // Mouse button 2/Scroll button: moves camera
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            mouseDownPos = Input.mousePosition;
            delta = Vector3.zero;
            cameraY = transform.localPosition.y;
        }
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            delta = mouseDownPos - Input.mousePosition;
            mouseDownPos = Input.mousePosition;
            if (Input.GetMouseButton(1))
            {
                //ProcesssTumble(delta);
            }
            else if(Input.GetMouseButton(2))
            {
                Vector3 d = delta.x * movespeed * transform.right + delta.y * movespeed * transform.up;
                transform.localPosition += d;
                transform.localPosition = new Vector3(
                    Mathf.Clamp(transform.localPosition.x, -9f, 2f), cameraY, Mathf.Clamp(transform.localPosition.z, -2f, 9f));
                //LookAtPosition.localPosition += d;
                //LookAtPosition.transform.localPosition = new Vector3(
                    //Mathf.Clamp(LookAtPosition.transform.localPosition.x, -5f, 5f), 0, Mathf.Clamp(LookAtPosition.transform.localPosition.z, -5f, 5f));
            }
        }

        // Use mouse scroll to zoom
        // Won't work if mouse is over a UI element
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            var input = Input.GetAxis("Mouse ScrollWheel");
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x + (transform.forward.x * zoomspeed * input), -9f, 2f),
                Mathf.Clamp(transform.localPosition.y + (transform.forward.y * zoomspeed * input), 1f, 20f),
                Mathf.Clamp(transform.localPosition.z + (transform.forward.z * zoomspeed * input), -2f, 9f));
        }


    }

    float Direction = 1.0f;
    /*public void ProcesssTumble(Vector3 delta)
    {
        Quaternion q = Quaternion.AngleAxis(-delta.x * rotatespeed * Direction, transform.up);
        Matrix4x4 r = Matrix4x4.Rotate(q);
        Matrix4x4 invP = Matrix4x4.TRS(-LookAtPosition.localPosition, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        Vector3 newCameraPos = r.MultiplyPoint(transform.localPosition);
        transform.localPosition = newCameraPos;

        q = Quaternion.AngleAxis(delta.y * rotatespeed * Direction, transform.right);
        r = Matrix4x4.Rotate(q);
        invP = Matrix4x4.TRS(-LookAtPosition.localPosition, Quaternion.identity, Vector3.one);
        r = invP.inverse * r * invP;
        newCameraPos = r.MultiplyPoint(transform.localPosition);
        if(newCameraPos.y>0)
            transform.localPosition = newCameraPos;

        transform.LookAt(LookAtPosition);
    }*/

}
