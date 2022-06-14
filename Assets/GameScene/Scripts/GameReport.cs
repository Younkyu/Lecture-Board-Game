using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleFileBrowser;
using ASL;
using System.Linq;

public class GameReport : MonoBehaviour
{
    private PlayerGrouping playerGrouping;
    private EndGameUI endGameBehavior;
    private StarRankingPanel overallRankingPanel;
    public StarRankingPanel groupRanking; //student use
    private static bool isHost = false;
    public static List<TeacherData> reportData; //teacher use
    public static Dictionary<int, StudentData> studentReportData; //Key=questionIndex; student use 
    public static Dictionary<int, StudentStat> studentStats = new Dictionary<int, StudentStat>(); //Key=id
    public static int qPosted = 0;
    public static List<int> postedQuestions; //list of questionIndex; student and teacher both use

    //Only up to date at end game
    private List<KeyValuePair<int, int>> overallRankingList;
    private List<List<KeyValuePair<int, int>>> groupRankingList;
    public static List<int> unpostedQuestions; //list of questionIndex

    public class StudentStat
    {
        public int numCorrect = 0;
        public int numAnswered = 0;
        public int stars = 0;
        //public int groupRank = 1;
        //public int overallRank = 1;
    }
    //Store one data for one line in student csv
    public class StudentData
    {
        public string myAnswer = "";
        public int selfGrade = 0; //Correct=1 or Incorrect=-1
        //For student report
        public string question;
        public string answer;
        public int postedNumber = 0; //first posted question is 1
        //For teacher report
        public int questionIndex;
        public int peerID;
        public string username;
    }
    //Store one data for one line in teacher csv
    public class TeacherData
    {
        public string question;
        public string answer;
        public int numAnswered = 0;
        public int notAnswered = 0;
        public int numCorrect = 0;
        public int numIncorrect = 0;
        public int postedNumber = 0; //Not a key; 0 if question has not been posted
        public int questionIndex = -1;
        //Key=student's peer id, val=student data for this qa
        public Dictionary<int, StudentData> studentAnswers = new Dictionary<int, StudentData>();

        public void updateStats(int numAnswered, int notAnswered, int numCorrect, int numIncorrect)
        {
            this.numAnswered = numAnswered;
            this.notAnswered = notAnswered;
            this.numCorrect = numCorrect;
            this.numIncorrect = numIncorrect;
        }
    }
    void Awake()
    {
        endGameBehavior = GameObject.Find("EndGameUI").GetComponent<EndGameUI>();
    }
    /// <summary>Initialize values</summary>
    void Start()
    {
        playerGrouping = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        GetComponent<ASL.ASLObject>()._LocallySetFloatCallback(MyFloatFunction);
        isHost = GameLiftManager.GetInstance().m_PeerId == BoardGameManager.hostID;
        overallRankingPanel = BoardGameManager.GetInstance().starRanking;
        postedQuestions = new List<int>();
        //Initialize studentStats
        foreach (var player in GameLiftManager.GetInstance().m_Players)
        {
            if (player.Key > 1)
            {
                studentStats.Add(player.Key, new StudentStat());
            }
        }
        if (isHost)
        {
            reportData = new List<TeacherData>();
        }
        else
        {
            studentReportData = new Dictionary<int, StudentData>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void downloadReport()
    {
        SimpleFileBrowser.FileBrowser.ShowLoadDialog((filepath) => { downloadReportHelper(filepath[0] + "\\"); }, null, FileBrowser.PickMode.Folders,
            false, null, null, "Select Folder", "Select"); //select folder from file explorer, then call downloadReportHelper
    }

    private void downloadReportHelper(string filepath)
    {
        if (!isHost)
        {
            string username = GameLiftManager.GetInstance().m_Username;
            string filename = username + "_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + ".csv";
            int i = 0;
            while (File.Exists(filepath + filename))
            {
                filename = username + "_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + " (" + (++i) + ").csv";
            }
            studentReport(filepath + filename);
        }
        else
        {
            string filename = "Teacher_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + ".csv";
            int i = 0;
            while (File.Exists(filepath + filename))
            {
                filename = "Teacher_GameReport_" + DateTime.Today.ToString("MM-dd-yy") + " (" + (++i) + ").csv";
            }
            teacherReport(filepath + filename);
        }
    }

    private void studentReport(string filepath)
    {
        int numColumns = 5; //for QA section
        //only add if positive
        int addedCommas = groupRanking.rankingContent.transform.childCount + 1 - numColumns;
        //Star ranking section
        string tableHeader = "Group " + playerGrouping.getPlayerGroup(GameLiftManager.GetInstance().m_PeerId) + " Ranking,";
        string line = "Stars,";
        for (int i = 0; i < groupRanking.rankingContent.transform.childCount; i++)
        {
            StarRankingButton player = groupRanking.rankingContent.transform.GetChild(i).GetComponent<StarRankingButton>();
            tableHeader += player.ranking + ": " + csvFormatString(player.username) + ",";
            line += player.stars + ",";
        }
        if (addedCommas < 0)
        {
            for (int i = 0; i > addedCommas; i--)
            {
                tableHeader += ",";
                line += ",";
            }
        }
        addRecord(tableHeader, filepath);
        addRecord(line, filepath);
        line = ",,,,,";
        if (addedCommas > 0)
        {
            for (int i = 0; i < addedCommas; i++) { line += ","; }
        }
        addRecord(line, filepath);
        //Statistics section
        tableHeader = "Correct,Answered,Questions,";
        addRecord(tableHeader, filepath);
        line = endGameBehavior.stuStats.numCorrect + "," + endGameBehavior.stuStats.numAnswered + "," + qPosted + ",";
        if (addedCommas > 0)
        {
            for (int i = 0; i < addedCommas; i++) { line += ","; }
        }
        addRecord(line, filepath);
        line = ",,,,,";
        if (addedCommas > 0)
        {
            for (int i = 0; i < addedCommas; i++) { line += ","; }
        }
        addRecord(line, filepath);
        //QA section
        tableHeader = "Q Number,Question,Answer,MyAnswer,SelfGrade";
        addRecord(tableHeader, filepath);
        foreach (var kvp in studentReportData)
        {
            line = "Q" + kvp.Value.postedNumber + "," + csvFormatString(kvp.Value.question) + "," + csvFormatString(kvp.Value.answer) + "," +
                csvFormatString(kvp.Value.myAnswer) + ",";
            if (kvp.Value.selfGrade == 1)
                line += "Correct";
            else if (kvp.Value.selfGrade == -1)
                line += "Incorrect";
            else
                line += "Ungraded";
            if (addedCommas > 0)
            {
                for (int i = 0; i < addedCommas; i++) { line += ","; }
            }
            addRecord(line, filepath);
        }

    }
//old teacher report (flipped row and column)
    private void teacherReport2(string filepath) 
    {
        int numColumns = 7 + playerGrouping.playerCount;
        string tableHeader = "";
        string line = "";
        starReport(filepath, numColumns);
        tableHeader = "Q Number,Question,Answer,Answered,NoAnswered,Correct,Incorrect";
        foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
        {
            tableHeader += "," + csvFormatString(getUsername(player.Key)) + ": : " + studentStats[player.Key].numCorrect + "/" + studentStats[player.Key].numAnswered + "/" + qPosted;
        }
        addRecord(tableHeader, filepath);
        int questionIndex = 0;
        foreach (TeacherData qaData in reportData)
        {
            line = (++questionIndex) + "," + csvFormatString(qaData.question) + "," + csvFormatString(qaData.answer) + "," +
                qaData.numAnswered + "," + qaData.notAnswered + "," + qaData.numCorrect + "," + qaData.numIncorrect;
            foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
            { //KeyValuePair<int, StudentData> kvp in qaData.studentAnswers
                string selfGrade = "";
                if (qaData.studentAnswers[player.Key].selfGrade == 1)
                    selfGrade += "Correct: : ";
                else if (qaData.studentAnswers[player.Key].selfGrade == -1)
                    selfGrade += "Incorrect: : ";
                else
                    selfGrade += "Ungraded: : ";
                line += "," + csvFormatString(selfGrade + qaData.studentAnswers[player.Key].myAnswer);
            }
            addRecord(line, filepath);
        }
    }

    private string getCommas(int numCommas)
    {
        string str = "";
        for (int i = 0; i < numCommas; i++)
        {
            str += ",";
        }
        return str;
    }

    private void teacherReport(string filepath)
    {
        int numColumns = 4 + qPosted; //7 + playerGrouping.playerCount;

        //star ranking section
        starReport(filepath, numColumns);
        addRecord(getCommas(numColumns), filepath);
        addRecord(getCommas(numColumns), filepath);

        //QA data section
        string question = ",,,Question";
        string answer = ",,,Answer";
        string answered = ",,,Answered";
        string noAnswered = ",,,NoAnswered";
        string correct = ",,,Correct";
        string incorrect = ",,,Incorrect";
        string line = "Student,Correct,Answered,Questions";
        foreach (int index in postedQuestions)
        {
            question += "," + csvFormatString(reportData[index].question);
            answer += "," + csvFormatString(reportData[index].answer);
            answered += "," + reportData[index].numAnswered;
            noAnswered += "," + reportData[index].notAnswered;
            correct += "," + reportData[index].numCorrect;
            incorrect += "," + reportData[index].numIncorrect;
            line += ",Q" + reportData[index].postedNumber;
        }
        addRecord(question, filepath);
        addRecord(answer, filepath);
        addRecord(answered, filepath);
        addRecord(noAnswered, filepath);
        addRecord(correct, filepath);
        addRecord(incorrect, filepath);
        addRecord(line, filepath);
        foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
        {
            if (player.Key <= 1)
            {
                continue;
            }
            line = csvFormatString(getUsername(player.Key)) + "," + studentStats[player.Key].numCorrect + "," + studentStats[player.Key].numAnswered + "," + qPosted;
            foreach (int index in postedQuestions)
            {
                TeacherData qaData = reportData[index];
                string selfGrade = "";
                if (qaData.studentAnswers[player.Key].selfGrade == 1)
                    selfGrade += "Correct: : ";
                else if (qaData.studentAnswers[player.Key].selfGrade == -1)
                    selfGrade += "Incorrect: : ";
                else
                    selfGrade += "Ungraded: : ";
                line += "," + selfGrade + csvFormatString(qaData.studentAnswers[player.Key].myAnswer);
            }
            addRecord(line, filepath);
        }
        addRecord(getCommas(numColumns), filepath);
        addRecord(getCommas(numColumns), filepath);

        //Unaswered question data section
        string commas = getCommas(numColumns - 2);
        addRecord("Unposted Question,Answer" + commas, filepath);
        foreach (int index in unpostedQuestions)
        {
            line = csvFormatString(reportData[index].question) + "," + csvFormatString(reportData[index].answer) + commas;
            addRecord(line, filepath);
        }
    }

    private void starReport(string filePath, int numColumns) //teacher only
    {
        setUpGroupRankingList();
        setUpOverallRankingList();
        for (int i = 0; i < playerGrouping.m_playerGroups.Count; i++)
        {
            starReportGroup(filePath, numColumns, i + 1);
            addRecord(getCommas(numColumns), filePath);
        }
        if (playerGrouping.m_playerGroups.Count > 1)
            starReportOverall(filePath, numColumns);
    }

    private void starReportGroup(string filepath, int numColumns, int groupNum) //teacher only
    {
        if (groupNum < 1)
            return;
        int addedCommas = groupRankingList.Count + 1 - numColumns;
        //Star ranking section
        string tableHeader = ",Group " + groupNum + " Ranking,";
        string line = ",Stars,";
        foreach (var kvp in groupRankingList[groupNum - 1])
        {
            tableHeader += kvp.Value + ". " + csvFormatString(getUsername(kvp.Key)) + ",";
            line += studentStats[kvp.Key].stars + ",";
        }
        if (addedCommas < 0)
        {
            for (int i = 0; i > addedCommas; i--)
            {
                tableHeader += ",";
                line += ",";
            }
        }
        addRecord(tableHeader, filepath);
        addRecord(line, filepath);
    }

    private void starReportOverall(string filepath, int numColumns) //teacher only
    {
        int addedCommas = overallRankingList.Count + 1 - numColumns;
        //Star ranking section
        string tableHeader = ",Overall Ranking,";
        string line = ",Stars,";
        //string statsLine = "Correct|Answered|Questions,";
        foreach (var kvp in overallRankingList)
        {
            tableHeader += kvp.Value + ". " + csvFormatString(getUsername(kvp.Key)) + ",";
            line += studentStats[kvp.Key].stars + ",";
            //statsLine += studentStats[kvp.Key].numCorrect + "|" + studentStats[kvp.Key].numAnswered + "|" + qPosted + ",";
        }
        if (addedCommas < 0)
        {
            for (int i = 0; i > addedCommas; i--)
            {
                tableHeader += ",";
                line += ",";
                //statsLine += ",";
            }
        }
        addRecord(tableHeader, filepath);
        addRecord(line, filepath);
        //addRecord(statsLine, filepath);
    }

    public static void addRecord(string csvLine, string filepath)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
        {
            file.WriteLine(csvLine);
        }
    }
    //Format the string if needed so it can be added to the csv comma list
    public static string csvFormatString(string arg)
    {
        bool formattingNeeded = false;
        for (int i = 0; i < arg.Length; i++)
        {   //Quotation & comma need special formating for csv
            if (arg[i] == '"' || arg[i] == ',')
            {
                formattingNeeded = true;
                break;
            }
        }
        if (formattingNeeded)
        {
            arg = "\"" + arg;
            //if there a " make it ""
            for (int i = 1; i < arg.Length; i++)
            {
                if (arg[i] == '"')
                {   //Add ", skip next i
                    arg = arg.Insert(i++, "\"");
                }
            }
            arg += "\"";
        }
        return arg;
    }

    public static string getUsername(int id)
    {
        if (GameLiftManager.GetInstance().m_Players[id] != null)
        {
            return GameLiftManager.GetInstance().m_Players[id];
        }
        if (GameReport.reportData.Count > 0)
        {
            return GameReport.reportData[0].studentAnswers[id].username;
        }
        string username = "";
        foreach (Transform child in GameObject.Find("GameManager").GetComponent<PlayerGrouping>().playersGrid.transform)
        {
            if (child.name == id.ToString())
            {
                username = child.GetChild(0).GetComponent<Text>().text;
            }
        }
        return username;
    }

    //only teacher should call this
    public int createTeacherData(string question, string answer)
    {
        TeacherData newQA = new TeacherData();
        newQA.question = question;
        newQA.answer = answer;
        newQA.questionIndex = reportData.Count;
        var players = playerGrouping.m_players;
        foreach (var player in players)
        {
            if (player.Key > BoardGameManager.hostID)
            {
                StudentData studentData = new StudentData();
                studentData.peerID = player.Key;
                studentData.username = getUsername(player.Key);
                //studentData.question = question;
                //studentData.answer = answer;
                studentData.questionIndex = reportData.Count;
                newQA.studentAnswers.Add(player.Key, studentData);
            }
        }
        reportData.Add(newQA);
        return reportData.Count - 1;
    }
    public static void setTeacherQA(string question, string answer, int questionIndex) //existing TeacherData
    {
        reportData[questionIndex].question = question;
        reportData[questionIndex].answer = answer;
    }
    public static void updateQuestionPostedNumber(int questionIndex, int postedNumber)
    {
        reportData[questionIndex].postedNumber = postedNumber;
    }

    public void updateStats(int questionIndex, int numAnswers, int numCorrect, int numIncorrect) //question stats
    {
        if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
        reportData[questionIndex].updateStats(numAnswers, (playerGrouping.playerCount - numAnswers),
                numCorrect, numIncorrect);
    }
    //only student should call this
    public void createStudentData(int peerID, string username, string question, string answer, int questionIndex)
    {
        StudentData newQA = new StudentData();
        newQA.question = question;
        newQA.answer = answer;
        newQA.peerID = peerID;
        newQA.username = username;
        newQA.questionIndex = questionIndex;
        newQA.postedNumber = qPosted;
        studentReportData.Add(questionIndex, newQA);
    }
    //teacher only
    private int findQuestionIndex(string q, string a)
    {
        for (int i = 0; i < reportData.Count; i++)
        {
            if (reportData[i].question == q && reportData[i].answer == a)
            {
                return i;
            }
        }
        return -1;
    }
    //teacher only
    public static int findQuestionIndex(int postedNum)
    {
        for (int i = 0; i < reportData.Count; i++)
        {
            if (reportData[i].postedNumber == postedNum)
            {
                return i;
            }
        }
        return -1;
    }

    public static void setupUnpostedQuestionsList()
    {
        unpostedQuestions = new List<int>();
        foreach (TeacherData data in reportData)
        {
            if (data.postedNumber < 1)
            {
                unpostedQuestions.Add(data.questionIndex);
            }
        }
    }

    public void setUpOverallRankingList()
    {
        overallRankingList = new List<KeyValuePair<int, int>>();
        foreach (KeyValuePair<int, int> player in playerGrouping.m_players)
        {
            if (player.Key > BoardGameManager.hostID)
            {
                rankingListInsert(ref overallRankingList, player.Key);
            }
        }
        rerankList(ref overallRankingList);
    }

    public void setUpGroupRankingList()
    {
        groupRankingList = new List<List<KeyValuePair<int, int>>>();
        foreach (List<int> group in playerGrouping.m_playerGroups)
        {
            List<KeyValuePair<int, int>> newGroup = new List<KeyValuePair<int, int>>();
            foreach (int playerID in group)
            {
                if (playerID > BoardGameManager.hostID)
                {
                    rankingListInsert(ref newGroup, playerID);
                }
            }
            rerankList(ref newGroup);
            groupRankingList.Add(newGroup);
        }

    }

    private void rerankList(ref List<KeyValuePair<int, int>> players)
    {
        int prevStars = -1;
        int prevRank = 0;
        for (int i = 0; i < players.Count; i++)
        {
            int currStars = studentStats[players[i].Key].stars;
            if (currStars == prevStars)
            {
                players[i] = new KeyValuePair<int, int>(players[i].Key, prevRank);
            }
            else
            {
                players[i] = new KeyValuePair<int, int>(players[i].Key, prevRank = i + 1);
            }
            prevStars = currStars;
        }
    }

    private void rankingListInsert(ref List<KeyValuePair<int, int>> players, int playerID)
    {
        if (players == null)
            return;
        for (int i = 0; i < players.Count; i++)
        {
            if (studentStats[playerID].stars >= studentStats[players[i].Key].stars)
            {
                players.Insert(i, new KeyValuePair<int, int>(playerID, 0));
                return;
            }
        }
        players.Add(new KeyValuePair<int, int>(playerID, 0));
    }

    public static void updateStudentAnswer(int id, int questionIndex, string studentResponse)
    {
        if (isHost)
        {
            if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
            reportData[questionIndex].studentAnswers[id].myAnswer = studentResponse;
            studentStats[id].numAnswered++;
            GameObject.Find("GameReport").GetComponent<GameReport>().overallRankingPanel.updatePlayerStats(id);
        }
        else if (GameLiftManager.GetInstance().m_PeerId == id)
        {
            studentReportData[questionIndex].myAnswer = studentResponse;
            studentStats[id].numAnswered++;
        }
    }
    public static void updateStudentGrade(int id, int questionIndex, int selfGrade)
    {
        if (isHost)
        {
            if (questionIndex >= reportData.Count || questionIndex < 0) { return; }
            reportData[questionIndex].studentAnswers[id].selfGrade = selfGrade;
            if (selfGrade == 1)
            {
                studentStats[id].numCorrect++;
                GameObject.Find("GameReport").GetComponent<GameReport>().overallRankingPanel.updatePlayerStats(id);
            }
        }
        else if (GameLiftManager.GetInstance().m_PeerId == id)
        {
            studentReportData[questionIndex].selfGrade = selfGrade;
            if (selfGrade == 1)
                studentStats[id].numCorrect++;
        }
    }

    //public static void updatePlayerGroupRank(int id, int rank)
    //{
    //    ASLObject thisASL = GameObject.Find("GameReport").GetComponent<ASLObject>();
    //    float[] m_MyFloats = new float[3];
    //    m_MyFloats[0] = 4;
    //    m_MyFloats[1] = id;
    //    m_MyFloats[2] = rank;
    //    thisASL.SendAndSetClaim(() =>
    //    {
    //        string floats = "BoardGameManager Floats sent: ";
    //        for (int i = 0; i < m_MyFloats.Length; i++)
    //        {
    //            floats += m_MyFloats[i].ToString();
    //            if (m_MyFloats.Length - 1 != i)
    //            {
    //                floats += ", ";
    //            }
    //        }
    //        Debug.Log(floats);
    //        thisASL.SendFloatArray(m_MyFloats);
    //    });
    //}

    //public static void updatePlayerOverallRank(int id, int rank)
    //{
    //    ASLObject thisASL = GameObject.Find("GameReport").GetComponent<ASLObject>();
    //    float[] m_MyFloats = new float[3];
    //    m_MyFloats[0] = 5;
    //    m_MyFloats[1] = id;
    //    m_MyFloats[2] = rank;
    //    thisASL.SendAndSetClaim(() =>
    //    {
    //        string floats = "BoardGameManager Floats sent: ";
    //        for (int i = 0; i < m_MyFloats.Length; i++)
    //        {
    //            floats += m_MyFloats[i].ToString();
    //            if (m_MyFloats.Length - 1 != i)
    //            {
    //                floats += ", ";
    //            }
    //        }
    //        Debug.Log(floats);
    //        thisASL.SendFloatArray(m_MyFloats);
    //    });
    //}

    public static void MyFloatFunction(string _id, float[] _myFloats)
    {
        string floats = "GradeReport - Floats received: ";
        for (int i = 0; i < _myFloats.Length; i++)
        {
            floats += _myFloats[i].ToString();
            if (_myFloats.Length - 1 != i)
            {
                floats += ", ";
            }
        }
        Debug.Log(floats);
        switch (_myFloats[0])
        {
            case 1: //send student response: 1, id, questionIndex, response
                string response = "";
                //covert int to char to get response
                for (int i = 3; i < _myFloats.Length; i++)
                {
                    response += System.Convert.ToChar((int)_myFloats[i]);
                }
                updateStudentAnswer((int)_myFloats[1], (int)_myFloats[2], response);
                break;
            case 2: //send student self grade: 2, id, questionIndex, correct/incorrect
                updateStudentGrade((int)_myFloats[1], (int)_myFloats[2], (int)_myFloats[3]);
                break;
            case 3: //send player stars: 3, id, stars
                studentStats[(int)_myFloats[1]].stars = (int)_myFloats[2];
                if (isHost)
                    GameObject.Find("GameReport").GetComponent<GameReport>().overallRankingPanel.updatePlayerStar((int)_myFloats[1], (int)_myFloats[2]);
                break;
            //case 4: //send player group rank: 4, id, rank
            //    studentStats[(int)_myFloats[1]].groupRank = (int)_myFloats[2];
            //    break;
            //case 5: //send player overall rank: 5, id, rank
            //    studentStats[(int)_myFloats[1]].overallRank = (int)_myFloats[2];
            //    break;
            default:
                Debug.Log("DownloadableReport Float function Default");
                break;
        }
    }
}
