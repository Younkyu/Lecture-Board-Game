using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;
using System;

public class MarkAnswer : MonoBehaviour
{
    private GameObject teacherUI;
    public PlayerGrouping manager;

    void Start()
    {
        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(sendResult);
    }

    void Update()
    {
        if (GameLiftManager.GetInstance().m_PeerId == 1 && GameObject.Find("TeacherUI") != null)
            teacherUI = GameObject.Find("TeacherUI").transform.Find("Canvas").Find("TabPages").Find("InGame").Find("Scroll View").Find("Viewport").Find("Content").gameObject;
    }

    public void sendResult(string _id, float[] _f)
    {
        Debug.Log("RECEIVED BY " + GameLiftManager.GetInstance().m_PeerId);
        if (GameLiftManager.GetInstance().m_PeerId == 1)
        {
            /*string question = "";
            bool correct = false;
            bool next = false;
            foreach (float f in _f)
            {
                if (f == -1)
                {
                    next = true;
                }
                else
                {
                    if (!next)
                    {
                        question += System.Convert.ToChar((int)f);
                    }
                    else if ((int)f == 0)
                    {
                        correct = false;
                    }
                    else if ((int)f == 1)
                    {
                        correct = true;
                    }
                }
            }

            Debug.Log(question + "   " + correct);*/

            // TRANSFER DATA TO TEACHER UI
            Transform btn = teacherUI.transform.GetChild((int)_f[0]);
            GameReport.TeacherData qData = GameReport.reportData[(int)_f[0]];
            string stat = btn.Find("Stats").GetComponent<Text>().text;
            //string[] values = stat.Split('/');
            //int[] numbers = Array.ConvertAll(values, int.Parse);
            //int answers = numbers[1] + 1;
            //int corrects = numbers[0];
            //int incorrects = numbers[2] - corrects;
            qData.numAnswered++;
            if (_f[1] == 1)
            {
                //corrects = numbers[0] + 1;
                qData.numCorrect++;
            }
            else
            {
                //incorrects = numbers[2] + 1;
                qData.numIncorrect++;
            }

            //Debug.Log(stat);
            //Debug.Log(answers);
            //Debug.Log(corrects);
            //Debug.Log(incorrects);

            stat = qData.numCorrect + "/" + qData.numAnswered + "/" + manager.playerCount;
            Debug.Log(stat);
            btn.Find("Stats").GetComponent<Text>().text = stat;
            //TeacherButton teacherBtn = btn.GetComponent<TeacherButton>();
            //teacherBtn.updateGameReportStats(answers, corrects, incorrects);


            /*foreach (Transform btn in teacherUI.transform)
            {
                if (question == ("Question: " + btn.Find("Text").GetComponent<Text>().text))
                {
                    string stat = btn.Find("Stats").GetComponent<Text>().text;
                    string[] values = stat.Split('/');
                    int[] numbers = Array.ConvertAll(values, int.Parse);

                    int answers = numbers[1] + 1;
                    int corrects = numbers[0];
                    int incorrects = numbers[2];
                    if (correct)
                    {
                        corrects = numbers[0] + 1;
                    }
                    else
                    {
                        incorrects = numbers[2] + 1;
                    }

                    Debug.Log(stat);
                    Debug.Log(answers);
                    Debug.Log(corrects);
                    Debug.Log(incorrects);

                    stat = corrects + "/" + answers + "/" + manager.playerCount;
                    Debug.Log(stat);
                    btn.Find("Stats").GetComponent<Text>().text = stat;
                    TeacherButton teacherBtn = btn.GetComponent<TeacherButton>();
                    teacherBtn.updateGameReportStats(answers, corrects, incorrects);
                }
            }*/
        }
    }
    public void mark(int questionIndex, bool isCorrect)
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[2];
            sendValue[0] = questionIndex;
            if (isCorrect)
            {
                sendValue[1] = 1;
            }
            else
            {
                sendValue[1] = 0;
            }


            /*float[] sendValue = new float[questionTxt.Length + 2];
            int index = 0;
            foreach (char c in questionTxt)
            {
                sendValue[index] = c;
                index++;
            }

            // register separator
            sendValue[index] = -1;
            index++;

            // register as correct/incorrect
            if (isCorrect)
            {
                sendValue[index] = 1;
            }
            else
            {
                sendValue[index] = 0;
            }*/

            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
