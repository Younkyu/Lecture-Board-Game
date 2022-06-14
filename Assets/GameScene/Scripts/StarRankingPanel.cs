using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class StarRankingPanel : MonoBehaviour
{
    public PlayerGrouping pGroup;
    private GameReport gameReport;

    public Text title;
    public GameObject rankingContent;
    public GameObject starRankButton;
    //teacherUI only
    public GameObject playerButton;
    public GameObject studentQPanel;
    public GameObject playerListContent;
    public StarRankingButton selectedPlayer; //selected from player list (overall ranking)

    // Start is called before the first frame update
    void Start()
    {
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        if (pGroup == null)
            pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //should be called once
    public void teacherUISetUp() //fill player list
    {
        foreach (KeyValuePair<int, int> player in pGroup.m_players)
        {
            if (player.Key > BoardGameManager.hostID)
            {
                addPlayerButton(GameLiftManager.GetInstance().m_Players[player.Key], player.Key, false);
            }
        }
        //select first player on the list
        if (playerListContent.transform.childCount > 0)
            playerListContent.transform.GetChild(0).GetComponent<StarRankingButton>().selectPlayer();
    }

    public void groupRankSetUp(int groupNum)
    {
        //if (selectedPlayer != null && selectedPlayer.groupNum != groupNum) {
        if (groupNum <= 0)
        {
            Debug.Log("StarRankingPanel groupRankSetUp: ERROR groupNum <= 0");
            return;
        }
        if (rankingContent.transform.childCount > 0)
        { //clear it
            for (int i = 0; i < rankingContent.transform.childCount; i++)
            {
                Destroy(rankingContent.transform.GetChild(i).gameObject);
            }
        }
        title.text = "Group " + groupNum + " Star Ranking";
        List<int> group = pGroup.m_playerGroups[groupNum - 1];
        foreach (int playerID in group)
        {
            if (playerID > 0)
                addPlayerButton(GameLiftManager.GetInstance().m_Players[playerID], playerID, true);
        }
    }

    public void addPlayerButton(string username, int id, bool forGroupRanking)
    {
        if (forGroupRanking)
        {
            GameObject newStudent = GameObject.Instantiate(starRankButton);
            newStudent.GetComponent<StarRankingButton>().setup(username, id, forGroupRanking, this);
            newStudent.transform.parent = rankingContent.transform;
            newStudent.SetActive(true);
            rankPlayer(newStudent, forGroupRanking);
        }
        else
        {
            GameObject newStudent = GameObject.Instantiate(playerButton);
            newStudent.GetComponent<StarRankingButton>().setup(username, id, forGroupRanking, this);
            newStudent.GetComponent<StarRankingButton>().setRank(1);
            newStudent.transform.parent = playerListContent.transform;
            newStudent.SetActive(true);
        }
    }

    public void rankPlayer(GameObject player, bool forGroupRanking)
    {
        GameObject listContent;
        if (forGroupRanking)
        {
            listContent = rankingContent;
        }
        else
        {
            listContent = playerListContent;
        }
        int stars = GameReport.studentStats[player.GetComponent<StarRankingButton>().peerId].stars;
        for (int i = 0; i < listContent.transform.childCount; i++)
        {
            GameObject curr = listContent.transform.GetChild(i).gameObject;
            int currStars = GameReport.studentStats[curr.GetComponent<StarRankingButton>().peerId].stars;
            if (stars >= currStars || (i == listContent.transform.childCount - 1))
            {
                player.transform.SetSiblingIndex(i);
                break;
            }
        }
        int prevStars = -1;
        int prevRank = 0;
        //re-rank players after the order change
        for (int i = 0; i < listContent.transform.childCount; i++)
        {
            StarRankingButton curr = listContent.transform.GetChild(i).gameObject.GetComponent<StarRankingButton>();
            int currStars = GameReport.studentStats[curr.peerId].stars;
            if (currStars == prevStars)
            {
                curr.setRank(prevRank);
            }
            else
            {
                curr.setRank(i + 1);
                prevRank = i + 1;
                prevStars = currStars;
            }
        }
    }

    public StarRankingButton GetStarRankButton(int id, bool forRankingList)
    {
        GameObject listContent;
        if (forRankingList)
        {
            listContent = rankingContent;
        }
        else
        {
            listContent = playerListContent;
        }
        for (int i = 0; i < listContent.transform.childCount; i++)
        {
            StarRankingButton player = listContent.transform.GetChild(i).gameObject.GetComponent<StarRankingButton>();
            if (player.peerId == id)
            {
                return player;
            }
        }
        return null;
    }

    public void updatePlayerStar(int id, int stars)
    {
        StarRankingButton player = GetStarRankButton(id, false);
        if (player != null)
        {
            player.setStars();
            rankPlayer(player.gameObject, false);
            if (selectedPlayer != null && selectedPlayer.groupNum == BoardGameManager.GetInstance().getPlayerGroup(id))
            { //update on group ranking
                player = GetStarRankButton(id, true);
                if (player != null)
                {
                    player.setStars();
                    rankPlayer(player.gameObject, true);
                }
            }
        }
    }
    //match GameReport studentStats
    public void updatePlayerStats(int id)
    {
        StarRankingButton player = GetStarRankButton(id, false);
        if (player != null)
        {
            player.setStat();
        }
    }

    public void openQuestionList() //teacher UI
    {
        if (selectedPlayer != null)
        {
            studentQPanel.SetActive(true);
            studentQPanel.GetComponent<StudentAnswersPanel>().loadPanel(selectedPlayer.peerId, selectedPlayer.username);
        }
    }

}
