using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentAnswerButton : MonoBehaviour
{
    private string studentAnswer;
    private string studentName;
    private StudentAnswersPanel saPanel;
    private EndGameUI endGameUI;
    private Color32 color;
    private Color32 red;
    private Color32 green;
    private Color32 badColor;
    public bool forQuestionList; //else, for student list
    //only for student end game and teacher player list tab
    private string question;
    private string answer;
    public int questionIndex;
    private int selfGrade;
    //only for teacher question list tab
    public int studentId;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(setText);
        color = new Color32(255, 255, 255, 255); //white
        red = new Color32(255, 138, 146, 255);
        green = new Color32(198, 255, 138, 255);
        badColor = new Color32(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Image>().color == badColor)
        {
            GetComponent<Image>().color = getColor(selfGrade);
        }
    }
    //for teacher UI question list tab; list of student answers
    public void setup(string username, string answer, int selfGrade, StudentAnswersPanel sa)
    {
        studentName = username;
        studentAnswer = answer;
        this.selfGrade = selfGrade;
        saPanel = sa;
        GetComponent<Image>().color = color;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = username;
        forQuestionList = false;
    }
    //for student end game UI
    public void setup(int postedNumber, int questionIndex, string question, string answer, string studentAns, int selfGrade, EndGameUI endGameUI)
    {
        studentAnswer = studentAns;
        this.selfGrade = selfGrade;
        this.endGameUI = endGameUI;
        this.question = question;
        this.answer = answer;
        this.questionIndex = questionIndex;
        GetComponent<Image>().color = color;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = "Q" + postedNumber.ToString();
        forQuestionList = true;
    }
    //for teacher UI player list tab; list of questions
    public void setup(int id, string username, int questionIndex, string question, string answer, string studentAns, int selfGrade, StudentAnswersPanel sa)
    {
        studentName = username;
        studentAnswer = studentAns;
        this.selfGrade = selfGrade;
        this.endGameUI = endGameUI;
        this.question = question;
        this.answer = answer;
        this.questionIndex = questionIndex;
        saPanel = sa;
        GetComponent<Image>().color = color;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = "Q" + (questionIndex + 1).ToString();
        forQuestionList = true;
    }

    public void setText()
    {
        if (saPanel != null) //teacher use
        {
            if (forQuestionList)
            {
                saPanel.setQA(question, answer);
                saPanel.saBackground.color = getColor(selfGrade);
                saPanel.studentAnswerText.text = studentName + ": " + studentAnswer;
            } else
            {
                saPanel.saBackground.color = getColor(selfGrade);
                saPanel.studentAnswerText.text = studentName + ": " + studentAnswer;
            }
        }
        else if (endGameUI != null) //student use
        {
            endGameUI.questionText.text = question;
            endGameUI.teacherAnswerText.text = answer;
            endGameUI.saBackground.color = getColor(selfGrade);
            endGameUI.studentAnswerText.text = studentAnswer;
        }
    }

    private Color32 getColor(int selfGrade)
    {
        switch (selfGrade)
        {
            case -1:
                return red;
                break;
            case 1:
                return green;
                break;
            default:
                break;
        }
        return color;
    }
}
