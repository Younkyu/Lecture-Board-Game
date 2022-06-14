// Source: https://github.com/dipen-apptrait/Vertical-drag-drop-listview-unity
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ASL;

public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    /// <summary>Bool that toggles when we send the floats, gets set to false after we send to save bandwidth</summary>
    public bool m_SendFloat = false;
    /// <summary>The floats that will be sent</summary>
    public float[] m_MyFloats = new float[4];

    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPosition;
    private ASLObject currentASL;
    public bool isVertical = true;
    public bool isHorizontal = true;

    private int totalChild;
    private Transform otherTransform;

    void Start()
    {
        currentASL = currentTransform.GetComponent<ASLObject>();
        currentASL._LocallySetFloatCallback(MyFloatFunction);
    }

    void Update()
    {
        if (m_SendFloat)
        {
            currentASL.SendAndSetClaim(() =>
            {
                string floats = "DragController Floats sent: ";
                for (int i = 0; i < m_MyFloats.Length; i++)
                {
                    floats += m_MyFloats[i].ToString();
                    if (m_MyFloats.Length - 1 != i)
                    {
                        floats += ", ";
                    }
                }
                Debug.Log(floats);
                currentASL.SendFloatArray(m_MyFloats);
            });
            m_SendFloat = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameLiftManager.GetInstance().m_PeerId == 1)
        {
            currentPosition = currentTransform.position;
            mainContent = currentTransform.parent.gameObject;
            totalChild = mainContent.transform.childCount;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
            return;
        if (isVertical && isHorizontal)
        {
            currentTransform.position =
                        new Vector3(eventData.position.x, eventData.position.y, currentTransform.position.z);
        }
        else if (isHorizontal)
        {
            currentTransform.position =
                        new Vector3(eventData.position.x, currentTransform.position.y, currentTransform.position.z);
        }
        else if (isVertical)
        {
            currentTransform.position =
                        new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);
        }
        else
        { //if neither vert or horiz, then no drag
            return;
        }


        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);
                int distance = (int)Vector3.Distance(currentTransform.position,
                    otherTransform.position);
                if (distance <= 10)
                {
                    if (isVertical && isHorizontal)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(currentPosition.x, currentPosition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(otherTransformOldPosition.x, otherTransformOldPosition.y,
                            currentTransform.position.z);
                        //currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        //currentPosition = currentTransform.position;
                        m_MyFloats[0] = 1;
                        m_MyFloats[1] = otherTransform.GetSiblingIndex();
                        m_SendFloat = true;
                    }
                    else if (isHorizontal)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(currentPosition.x, otherTransform.position.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(otherTransformOldPosition.x, currentTransform.position.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPosition = currentTransform.position;
                    }
                    else //isVertical
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, currentPosition.y,
                            otherTransform.position.z);
                        currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y,
                            currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPosition = currentTransform.position;
                    }
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
            return;
        currentTransform.position = currentPosition;
    }

    private void changeIndex(GameObject obj)
    {
        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
        currentPosition = currentTransform.position;
    }

    //
    public void MyFloatFunction(string _id, float[] _myFloats)
    {
        switch (_myFloats[0])
        {
            case 1: //change index
                //_myFloats[1] should == otherTransform.GetSiblingIndex()
                currentTransform.SetSiblingIndex((int)_myFloats[1]);
                currentPosition = currentTransform.position;
                break;
            default:
                Debug.Log("DragController MyFloatFunction default case");
                break;
        }
    }
}