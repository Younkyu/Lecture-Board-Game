using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ASL;

public class StudentStats : MonoBehaviour
{
    public GameObject qListContents;
    public Text statsText;
    private GameReport gameReport;
    //public Selfgrader selfGrader;

    public int numCorrect = 0;
    public int numAnswered = 0;

    // Start is called before the first frame update
    void Start()
    {
        //statsText = transform.GetChild(0).GetComponent<Text>();
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void incrementNumCorrect()
    {
        numCorrect++;
        updateUI();
    }

    public void incrementNumAnswered()
    {
        numAnswered++;
        updateUI();
    }

    public void updateNumQuestions()
    { //GameReport.qPosted should be updated
        updateUI();
    }

    private void updateUI()
    {
        statsText.text = "Stats: " + numCorrect + "/" + numAnswered + "/" + GameReport.qPosted;
    }
    //student send to teacher
    public void sendAnswer(int id, int questionIndex, string studentResponse)
    {
        gameReport.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[3 + studentResponse.Length];
            sendValue[0] = 1;
            sendValue[1] = id;
            sendValue[2] = questionIndex;
            int index = 3;
            foreach (char c in studentResponse)
            {
                sendValue[index++] = c;
            }
            // send float array
            gameReport.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
    //student send to teacher
    public void sendGrade(int id, int questionIndex, int isCorrect)
    {
        gameReport.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[4];
            int index = 0;
            sendValue[0] = 2;
            sendValue[1] = id;
            sendValue[2] = questionIndex;
            sendValue[3] = isCorrect;
            // send float array
            gameReport.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
