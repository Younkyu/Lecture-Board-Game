using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentAnswersPanel : MonoBehaviour
{
    public Text questionText;
    public Text teacherAnswerText;
    public Text studentAnswerText;
    public Text titleText;
    public Image saBackground; //student answer
    private bool forQuestionList; //else, for student list

    //private Color32 red;
    //private Color32 green;
    private Color32 defaultColor;

    public GameObject listContent;
    public GameObject studentListButton;
    //private GameReport gameReport;

    //for student answer list
    public int questionIndex;

    //for question list
    public int studentId;
    public string username;

    // Start is called before the first frame update
    void Start()
    {
        //gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        //red = new Color32(255, 138, 146, 255);
        //green = new Color32(198, 255, 138, 255);
        defaultColor = saBackground.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadPanel(int questionIndex) //teacher UI use
    {
        forQuestionList = false;
        this.questionIndex = questionIndex;
        GameReport.TeacherData data = GameReport.reportData[questionIndex];
        setQA(data.question, data.answer);
        foreach (KeyValuePair<int, GameReport.StudentData> kvp in data.studentAnswers)
        {
            if (kvp.Value.selfGrade != 0)
                addStudentAnswer(kvp.Value.username, kvp.Value.myAnswer, kvp.Value.selfGrade);
        }
        titleText.text = "Student Answers (" + listContent.transform.childCount + ")";
    }

    public void loadPanel(int id, string username) //teacher UI use
    {
        forQuestionList = true;
        studentId = id;
        this.username = username;
        int qIndex = 0;
        foreach (int index in GameReport.postedQuestions)
        {
            GameReport.TeacherData teachData = GameReport.reportData[index];
            GameReport.StudentData stuData = teachData.studentAnswers[id];
            GameObject newStudent = GameObject.Instantiate(studentListButton);
            newStudent.GetComponent<StudentAnswerButton>().setup(id, username, qIndex++, teachData.question, teachData.answer, 
                stuData.myAnswer, stuData.selfGrade, this);
            newStudent.transform.parent = listContent.transform;
            newStudent.SetActive(true);
        }
        titleText.text = "Question List (" + listContent.transform.childCount + ") - " + username;
    }
    //int id, string username, int questionIndex, string question, string answer, string studentAns, int selfGrade, StudentAnswersPanel sa
    //public void loadPanel_noLabel(int questionIndex) //end game UI teacher use
    //{
    //    this.questionIndex = questionIndex;
    //    GameReport.TeacherData data = GameReport.reportData[questionIndex];
    //    setQA(data.question, data.answer);
    //    foreach (KeyValuePair<int, GameReport.StudentData> kvp in data.studentAnswers)
    //    {
    //        addStudentAnswer(kvp.Value.username, kvp.Value.myAnswer, kvp.Value.selfGrade);
    //    }
    //    titleText.text = "Student Answers (" + listContent.transform.childCount + ")";
    //}

    public void setQA(string q, string a)
    {
        questionText.text = "Q: " + q;
        teacherAnswerText.text = "A: " + a;
    }

    private void setQA_noLabel(string q, string a)
    {
        questionText.text = q;
        teacherAnswerText.text = a;
    }

    public void addStudentAnswer(string username, string answer, int selfGrade)
    {
        
        GameObject newStudent = GameObject.Instantiate(studentListButton);
        newStudent.GetComponent<StudentAnswerButton>().setup(username, answer, selfGrade, this);
        newStudent.transform.parent = listContent.transform;
        newStudent.SetActive(true);
    }

    public void close()
    {
        for (int i = 0; i < listContent.transform.childCount; i++)
        {
            Destroy(listContent.transform.GetChild(i).gameObject);
        }
        studentAnswerText.text = "";
        saBackground.color = defaultColor;
        gameObject.SetActive(false);
    }
}
