using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class Selfgrader : MonoBehaviour
{
    string question;
    string teacher;
    string student;
    public Button qButton;
    public Button correct;
    public Button incorrect;
    public Button closeButton;
    public Text questionTxt;
    public Text teacherAnswer;
    public Text studentAnswer;
    public MarkAnswer ma;
    public StudentStats stats;
    private PlayerData playerData;
    public bool graded = false;
    public int questionIndex = -1;
    private Color32 normal = new Color32(61, 197, 212, 239);
    // Start is called before the first frame update
    void Start()
    {
        Button btn = correct.GetComponent<Button>();
        btn.onClick.AddListener(markCorrect);
        Button btn2 = incorrect.GetComponent<Button>();
        btn2.onClick.AddListener(markIncorrect);
        Button btn3 = closeButton.GetComponent<Button>();
        btn3.onClick.AddListener(close);

        playerData = GameObject.Find("PlayerDataManager").GetComponent<PlayerData>();
        ma = GameObject.Find("AnswerManager").GetComponent<MarkAnswer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setText(string q, string ta, string submit)
    {
        question = q;
        teacher = ta;
        student = submit;
        questionTxt.text = q;
        teacherAnswer.text = "Teacher's Answer: "+ ta;
        studentAnswer.text = "Your Answer: " + submit;
    }

    public void setText(string q, string a)
    {
        question = q;
        teacher = a;
        questionTxt.text = q;
        teacherAnswer.text = a;
    }

    public void studentSubmit(string a)
    {
        graded = false;
        closeButton.gameObject.SetActive(false);
        student = "Your Answer: " + a;
        studentAnswer.text = "Your Answer: " + a;
        if (qButton!=null){
            stats.incrementNumAnswered();
            stats.sendAnswer(GameLiftManager.GetInstance().m_PeerId, questionIndex, a);
            qButton.GetComponent<ButtonBehavior>().setStudentAnswer(a);
        }
    }

    public void activeButtons(){
        correct.gameObject.SetActive(true);
        incorrect.gameObject.SetActive(true);
    }

    void markCorrect()
    {
        if(qButton!=null){
            qButton.GetComponent<Image>().color = correct.image.color;
            qButton.GetComponent<ButtonBehavior>().answered = true;
            ma.mark(questionIndex, true);
            graded = true;

            stats.incrementNumCorrect();
            stats.sendGrade(GameLiftManager.GetInstance().m_PeerId, questionIndex, 1);
        }
        DiceRoll.movePoints++;
        DiceRoll.starCount += 5;
        closeButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
        GetComponent<Image>().color = normal;
        playerData.sendData();
    }

    void markIncorrect()
    {
        if(qButton!=null){
            qButton.GetComponent<Image>().color = incorrect.image.color;
            qButton.GetComponent<ButtonBehavior>().answered = true;
            ma.mark(questionIndex, false);
            graded = true;

            stats.sendGrade(GameLiftManager.GetInstance().m_PeerId, questionIndex, -1);
        }
        closeButton.gameObject.SetActive(true);
        GetComponent<Image>().color = normal;
        gameObject.SetActive(false);
    }

    void close(){
        if(!closeButton.gameObject.activeSelf){
            closeButton.gameObject.SetActive(true);
        }
        correct.gameObject.SetActive(true);
        incorrect.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
