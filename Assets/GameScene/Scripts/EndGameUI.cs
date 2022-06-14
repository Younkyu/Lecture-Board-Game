using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class EndGameUI : MonoBehaviour
{
    public Text questionText;
    public Text teacherAnswerText;
    public Text studentAnswerText;
    public Image saBackground; //student answer
    public Text studentStats;
    public Text questionsPosted; //teacher only
    public GameObject playerAnswersPanel;

    private Color32 red;
    private Color32 green;

    public GameObject listContent;
    public GameObject studentListButton; //used by student, show QA A
    public GameObject teacherListButton; //used by teacher, show QA and player list
    //public GameObject playerListButton; //used by teacher, show student's A
    public StarRankingPanel starRankPanel_s; //student
    public GameObject starRankButton;
    public GameObject starRankPanel_t; //teacher
    public GameObject teacherOnly; //make star rank panels's parent
    public GameObject studentOnly;

    private GameReport gameReport;
    public StudentStats stuStats; //student only
    private PlayerGrouping playerGrouping;
    public static bool ended = false;

    // Start is called before the first frame update
    void Start()
    {
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        playerGrouping = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        red = new Color32(255, 138, 146, 255);
        green = new Color32(198, 255, 138, 255);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void endGameSetUp()
    {
        if (!ended)
        {
            ended = true;
            if (GameLiftManager.GetInstance().m_PeerId != 1)
            {
                teacherOnly.SetActive(false);
                stuStats = BoardGameManager.GetInstance().getGroupWorld(BoardGameManager.GetInstance().getPlayerGroup()).GetComponent<StudentStats>();
                studentStats.text = "Stats: " + stuStats.numCorrect + "/" + stuStats.numAnswered + "/" + GameReport.qPosted;
                starRankPanel_s.gameObject.SetActive(true);
                starRankPanel_s.groupRankSetUp(BoardGameManager.GetInstance().getPlayerGroup());
                starRankPanel_s.gameObject.SetActive(false);
                loadStudentButtons();
            } else
            {
                GameReport.setupUnpostedQuestionsList();
                studentOnly.SetActive(false);
                questionsPosted.text = "Questions Posted: " + GameReport.qPosted + " / " + GameReport.reportData.Count;
                starRankPanel_t.transform.SetParent(teacherOnly.transform);
                starRankPanel_t.transform.Find("StudentQuestionsPanel").GetComponent<StudentAnswersPanel>().close();
                starRankPanel_t.SetActive(false);
                loadTeacherButtons();
            }
            starRankButton.GetComponent<Button>().onClick.AddListener(rankButton);
        }
    }
    public void loadStudentButtons()
    {
        Debug.Log("Student End Game");
        int i = 0;
        foreach (KeyValuePair<int, GameReport.StudentData> kvp in GameReport.studentReportData)
        {
            GameObject newQ = GameObject.Instantiate(studentListButton);
            newQ.GetComponent<StudentAnswerButton>().setup(++i, kvp.Key, kvp.Value.question, kvp.Value.answer, kvp.Value.myAnswer, 
                kvp.Value.selfGrade, this);
            newQ.transform.SetParent(listContent.transform);
            newQ.SetActive(true);
        }
    }

    public void loadTeacherButtons()
    {
        Debug.Log("Teacher End Game");
        //Format Q1; list posted questions
        int num = 0;
        foreach (int index in GameReport.postedQuestions)
        {
            GameReport.TeacherData qData = GameReport.reportData[index];
            GameObject newQ = GameObject.Instantiate(teacherListButton);
            TeacherButton tb = newQ.GetComponent<TeacherButton>();
            tb.setUp(false, index);
            tb.setQA(qData.question, qData.answer, ++num, true);
            //tb.studentAnswersPanel = playerAnswersPanel;
            tb.published = true;
            newQ.transform.GetChild(1).gameObject.GetComponent<Text>().text = qData.numCorrect + "/" + qData.numAnswered + "/" + playerGrouping.playerCount;
            newQ.transform.parent = listContent.transform;
            newQ.SetActive(true);
        }
        num = 0;
        //add the unposted questions
        foreach (int index in GameReport.unpostedQuestions)
        {
            GameReport.TeacherData qData = GameReport.reportData[index];
            GameObject newQ = GameObject.Instantiate(teacherListButton);
            TeacherButton tb = newQ.GetComponent<TeacherButton>();
            tb.setUp(false, qData.questionIndex);
            tb.setQA(qData.question, qData.answer, ++num, false);
            //tb.studentAnswersPanel = playerAnswersPanel;
            tb.published = true;
            newQ.transform.GetChild(1).gameObject.GetComponent<Text>().text = qData.numCorrect + "/" + qData.numAnswered + "/" + playerGrouping.playerCount;
            newQ.transform.parent = listContent.transform;
            newQ.SetActive(true);
        }
    }

    public void rankButton()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            if (starRankPanel_s.gameObject.activeSelf)
            {
                starRankPanel_s.gameObject.SetActive(false);
            }
            else
            {
                starRankPanel_s.gameObject.SetActive(true);
            }
        } else
        {
            if (starRankPanel_t.activeSelf)
            {
                starRankPanel_t.SetActive(false);
            }
            else
            {
                starRankPanel_t.SetActive(true);
            }
        }
    }
    //public void teacherButtonBehavior()
    //{
    //    playerAnswersPanel.SetActive(true);
    //    playerAnswersPanel.GetComponent<StudentAnswersPanel>().loadPanel_noLabel(questionIndex);
    //}

}
