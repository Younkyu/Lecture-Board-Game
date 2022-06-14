using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class StarRankingButton : MonoBehaviour
{
    public bool forGroupRanking; //false--for overal ranking List
    private StarRankingPanel rankPanel;

    public string username;
    public int peerId;
    public int groupNum;
    public int ranking;
    public int stars;

    private bool makeYellow = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        while (makeYellow)
        {
            StarRankingButton player = rankPanel.GetStarRankButton(peerId, true);
            if (player != null)
            {
                Debug.Log("selectPlayer no color ERROR");
                player.GetComponent<Image>().color = Color.yellow;
            }
            makeYellow = false;
        }
    }

    public void setup(string username, int id, bool forGroupRanking, StarRankingPanel starRankPanel)
    {
        this.username = username;
        peerId = id;
        groupNum = BoardGameManager.GetInstance().getPlayerGroup(id);
        this.forGroupRanking = forGroupRanking;
        rankPanel = starRankPanel;
        setStars();
        transform.GetChild(0).gameObject.GetComponent<Text>().text = username;
        if (GameLiftManager.GetInstance().m_PeerId == id)
        {
            GetComponent<Image>().color = Color.yellow;
        }
    }

    public void setRank(int rank) //star ranking button
    {
        ranking = rank;
        transform.GetChild(0).gameObject.GetComponent<Text>().text = rank + ". " + username;
        //if (forGroupRanking)
        //{
        //    GameReport.updatePlayerGroupRank(peerId, rank);
        //} else
        //{
        //    GameReport.updatePlayerOverallRank(peerId, rank);
        //}
    }

    public void setStars() //star ranking button
    {
        stars = GameReport.studentStats[peerId].stars;
        if (forGroupRanking)
        {
            transform.GetChild(1).gameObject.GetComponent<Text>().text = stars.ToString();
        }
    }

    public void setStat() //player list button
    {
        transform.GetChild(1).gameObject.GetComponent<Text>().text = GameReport.studentStats[peerId].numCorrect + "/" +
            GameReport.studentStats[peerId].numAnswered + "/" + GameReport.qPosted;
    }

    public void selectPlayer() //teacher UI
    {
        if (forGroupRanking)
        {
            StarRankingButton player = rankPanel.GetStarRankButton(peerId, false);
            if (player != null)
            {
                if (rankPanel.selectedPlayer != null)
                {
                    rankPanel.selectedPlayer.GetComponent<Image>().color = Color.white;
                    rankPanel.GetStarRankButton(rankPanel.selectedPlayer.peerId, true).GetComponent<Image>().color = Color.white;
                }
                rankPanel.selectedPlayer = player;
                player.GetComponent<Image>().color = Color.yellow;
                GetComponent<Image>().color = Color.yellow;
            }
            rankPanel.openQuestionList();
        }
        else
        {
            if (rankPanel.selectedPlayer != null)
            {
                rankPanel.selectedPlayer.GetComponent<Image>().color = Color.white;
            }
            if (groupNum <= 0)
            {
                groupNum = BoardGameManager.GetInstance().getPlayerGroup(peerId);
            }
            rankPanel.groupRankSetUp(groupNum);
            rankPanel.selectedPlayer = this;
            GetComponent<Image>().color = Color.yellow;
            makeYellow = true;
            StarRankingButton player = rankPanel.GetStarRankButton(peerId, true);
            if (player != null)
            {
                Debug.Log("selectPlayer no color ERROR");
                player.GetComponent<Image>().color = Color.yellow;
            }
        }
    }
}
