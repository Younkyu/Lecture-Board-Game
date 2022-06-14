using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class MinMax : MonoBehaviour
{
    public Button min;
    public Button max;

    Vector2 minScale;
    Vector2 maxScale;
    Vector3 minLocation;
    Vector3 maxLocation;
    RectTransform rect;
    Vector3 posDif;
    Vector2 sizeDif;

    public RectTransform child;
    Vector2 childDif;
    public RectTransform grandchild;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        min.onClick.AddListener(minimize);
        max.onClick.AddListener(maximize);
        maxLocation = rect.position;
        maxScale = rect.sizeDelta;
        minLocation = new Vector3(80,0,0);
        minScale = new Vector2(150, 260);
        max.gameObject.SetActive(false);
        posDif = new Vector3 (350-80,0,0);
        sizeDif = new Vector2 (400,0);
        childDif = new Vector2 (360,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void minimize(){
        max.gameObject.SetActive(true);
        min.gameObject.SetActive(false);
        rect.sizeDelta -= sizeDif;
        rect.position -= posDif;
        child.sizeDelta -= sizeDif;
        child.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(140,225);
        //grandchild.gameObject.GetComponent<RectTransform>().sizeDelta -= sizeDif;
        for (int i=0;i<grandchild.gameObject.transform.childCount; i++){
            grandchild.gameObject.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(120,30);
            grandchild.gameObject.transform.GetChild(i).GetComponent<RectTransform>().position -= new Vector3(180,0,0);
            grandchild.gameObject.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(20,30);
            grandchild.gameObject.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>().position -= new Vector3(40,0,0);
            grandchild.gameObject.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = "!";
        }
    }

    void maximize(){
        min.gameObject.SetActive(true);
        max.gameObject.SetActive(false);
        rect.position += posDif;
        rect.sizeDelta += sizeDif;
        child.sizeDelta += sizeDif;
        child.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(500,225);
        //grandchild.gameObject.GetComponent<RectTransform>().sizeDelta += sizeDif;
        for (int i=0;i<grandchild.gameObject.transform.childCount; i++){
            grandchild.gameObject.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(450,30);
            grandchild.gameObject.transform.GetChild(i).GetComponent<RectTransform>().position += new Vector3(180,0,0);
            grandchild.gameObject.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(80,30);
            grandchild.gameObject.transform.GetChild(i).GetChild(1).GetComponent<RectTransform>().position += new Vector3(40,0,0);
            grandchild.gameObject.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = "NEW!";
        }
    }
}
