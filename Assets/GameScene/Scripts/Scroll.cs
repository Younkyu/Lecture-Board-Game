using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class Scroll : MonoBehaviour
{
    //public Text txt;
    public static GameObject button;
    public ManageCSV qa;
    private static int number = 1;
    ASLpanel m_ASLObject;
    public static bool imported;
    public static bool startGame = false;
    public GameObject prefabButton;
    public GameObject min;
    private bool isHost = false;
    private GameReport gameReport;
    public StudentStats studentStats;
    public SoundManagerScript notification;
    private bool soundOn = true;

    public GameObject StudentPanel;
    private Color32 green = new Color32(53, 159, 76, 100);
    private Color32 red = new Color32(255, 8, 0, 100);

    // Start is called before the first frame update
    IEnumerator Start()
    {
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        isHost = GameLiftManager.GetInstance().m_PeerId == 1;
        m_ASLObject = gameObject.GetComponent<ASLpanel>();
        if(min==null){
            min = GameObject.Find("Minimize");
        }
        //button = GameObject.Find("Button");
        yield return new WaitForSeconds(1);
        /*if(qa!=null){
            string q = ManageCSV.grid[0,number];
            string a = ManageCSV.grid[1,number];
            while(q!=""){
                createButton(q, a);
                q = ManageCSV.grid[0,number];
                a = ManageCSV.grid[1,number];
            } 
        }*/
        GameObject.Find("DataSend").GetComponent<SendNewQuestion>().studentUI = gameObject;
        if(!isHost){
            notification = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
        }
    }

    public void buttonSetup()
    {
        string q = ManageCSV.grid[0, number];
        string a = ManageCSV.grid[1, number];
        while (q != "")
        {
            if(isHost){
                GameObject newButton = newQ(q, a);
                // newButton.GetComponent<Image>().color = new Color32(157, 241, 146, 255);
                newButton.GetComponent<TeacherButton>().published = false;
            } else {
                createButton(q, a, number - 1);
            }
            q = ManageCSV.grid[0, number];
            a = ManageCSV.grid[1, number];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startGame && imported)
        {
            startGame = false;
            Debug.Log("IMPORTED");
            buttonSetup();
        }
    }

    //For studentUI
    public void createButton(string q, string a, int questionIndex)
    {
        GameObject go = gameObject;
        //Get pos of last button and size of button
        int i = go.transform.childCount - 1;
        GameObject newButton;
        if(i>-1){
            var rectTransform = go.transform.GetChild(i).GetComponent<RectTransform>();
            Vector3 pos = rectTransform.position;
            Vector2 size = rectTransform.sizeDelta;
            //Change size of "content"
            rectTransform = go.GetComponent<RectTransform>();
            rectTransform.sizeDelta += new Vector2(0, size.y);
            //m_ASLObject.increaseScale(new Vector2(0,size.y));
            pos -= new Vector3(0, size.y, 0);
            //size += new Vector2(0,size.y);
            //m_ASLObject.SendAndSetLocalScale(new Vector3(size.x, size.y,0));
            //create new button
            newButton = Instantiate(prefabButton) as GameObject;
            newButton.transform.SetParent(rectTransform.transform, false);
            newButton.GetComponent<RectTransform>().position = pos;
            if(!min.activeSelf){
                newButton.transform.GetComponent<RectTransform>().sizeDelta = size;
                newButton.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(20,30);
                newButton.transform.GetChild(1).GetComponent<RectTransform>().position -= new Vector3(40,0,0);
                newButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "!";
            }
        } else {
            Vector3 pos = new Vector3(180,-20,0);
            newButton = Instantiate(prefabButton) as GameObject;
            newButton.GetComponent<RectTransform>().position = pos;
            newButton.transform.SetParent(go.transform, false);
            if(!min.activeSelf){
                newButton.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(120,30);
                newButton.GetComponent<RectTransform>().localPosition -= new Vector3(180,0,0);
                newButton.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(20,30);
                newButton.transform.GetChild(1).GetComponent<RectTransform>().position -= new Vector3(40,0,0);
                newButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "!";
            }
        }
        ButtonBehavior buttonBehavior = newButton.GetComponent<ButtonBehavior>();
        buttonBehavior.setQA(q, a);
        studentStats.updateNumQuestions();
        buttonBehavior.questionIndex = questionIndex;
        gameReport.createStudentData(GameLiftManager.GetInstance().m_PeerId, GameLiftManager.GetInstance().m_Username, q, a, questionIndex);
        number++;
        StudentPanel.GetComponent<Image>().color = red;
        //SoundOn changed to false to prevent multiple notification sounds when questions are published all at once
        if(notification!=null&&soundOn){
            notification.playSound();
            soundOn = false;
        }
        StartCoroutine(UIcolorreset());
    }

    IEnumerator UIcolorreset()
    {
        //yield on a new YieldInstruction that waits for 2 seconds.
        yield return new WaitForSeconds(2);

        StudentPanel.GetComponent<Image>().color = green;
        soundOn = true;
    }

    //For TeacherUI when they make a new question
    public GameObject newQ(string q, string a)
    {
        GameObject go = gameObject;
        //Get pos of last button and size of button
        int i = go.transform.childCount - 1;
        GameObject newButton;
        if(i>-1){
            var rectTransform = go.transform.GetChild(i).GetComponent<RectTransform>();
            Vector3 pos = rectTransform.position;
            Vector2 size = rectTransform.sizeDelta;
            //Change size of "content"
            rectTransform = go.GetComponent<RectTransform>();
            rectTransform.sizeDelta += new Vector2(0, size.y);
            //m_ASLObject.increaseScale(new Vector2(0,size.y));
            pos -= new Vector3(0, size.y, 0);
            //size += new Vector2(0,size.y);
            //m_ASLObject.SendAndSetLocalScale(new Vector3(size.x, size.y,0));
            //create new button
            newButton = Instantiate(prefabButton) as GameObject;
            newButton.transform.SetParent(rectTransform.transform, false);
            newButton.GetComponent<RectTransform>().position = pos;
        } else {
            Vector3 pos = new Vector3(210,-23,0);
            newButton = Instantiate(prefabButton) as GameObject;
            newButton.GetComponent<RectTransform>().position = pos;
            newButton.transform.SetParent(go.transform, false);
        }
        TeacherButton teacherButton = newButton.GetComponent<TeacherButton>();
        teacherButton.setUp(true, gameReport.createTeacherData(q, a));
        teacherButton.setQA(q, a);
        number++;
        return newButton;
    }

}