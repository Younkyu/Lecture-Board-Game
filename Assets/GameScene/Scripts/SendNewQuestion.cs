using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class SendNewQuestion : MonoBehaviour
{
    public InputField question;
    public InputField answer;
    public GameObject studentUI;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(readQuestion);
    }

    public void setDataSend()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1){
            GameObject studentUI = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").gameObject;
            //.Find("Canvas").Find("StudentPanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        }
    }

    private void readQuestion(string _id, float[] _f)
    {
        string floats = "SendNewQuestion - Floats received: ";
        for (int i = 0; i < _f.Length; i++)
        {
            floats += _f[i].ToString();
            if (_f.Length - 1 != i)
            {
                floats += ", ";
            }
        }
        Debug.Log(floats);

        string printQuestion = "";
        string printAnswer = "";
        int questionIndex = -1;
        int separatorCount = 0;
        foreach (float f in _f)
        {
            if (f == -1)
            {
                separatorCount++;
            }
            else
            {
                switch (separatorCount)
                {
                    case 0:
                        printQuestion += System.Convert.ToChar((int)f);
                        break;
                    case 1:
                        printAnswer += System.Convert.ToChar((int)f);
                        break;
                    case 2:
                        questionIndex = (int)f;
                        break;
                    default:
                        break;
                }
            }
        }

        GameReport.qPosted++;
        GameReport.postedQuestions.Add(questionIndex);

        // DO SOMETHING WITH THE CREATED QUESTION AND ANSWER HERE
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            Debug.Log(GameLiftManager.GetInstance().m_PeerId + " Received Question: " + printQuestion);
            Debug.Log(GameLiftManager.GetInstance().m_PeerId + " Received Answer: " + printAnswer);
            if(studentUI==null){
                GameObject studentUI = GameObject.Find("StudentUI").transform.Find("GroupWorld(Clone)").Find("Canvas").Find("StudentPanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
            }
            studentUI.GetComponent<Scroll>().createButton(printQuestion, printAnswer, questionIndex);
        } else
        {
            GameReport.updateQuestionPostedNumber(questionIndex, GameReport.qPosted);
        }
    }

    public void sendQuestion(string q, string a, int questionIndex)
    {
        //question = GameObject.Find("");
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            // one additional float length is for question and answer separator (negative value)
            // +2 -> negative separator and questionIndex for GameReport
            float[] sendValue = new float[q.Length + 1 + a.Length + 2];
            int index = 0;
            // register question
            foreach (char c in q)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            // register answer
            foreach (char c in a)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            //register question index for GameReport
            sendValue[index] = questionIndex;
            index++;

            // send float array
            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
