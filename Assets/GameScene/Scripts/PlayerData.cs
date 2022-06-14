using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class PlayerData : MonoBehaviour
{
    private PlayerGrouping pGroup;
    private BoardGameManager bgm;
    private GameReport gameReport;
    private int playerNumber;

    private GameObject player1;
    public int p1Stars;
    public int p1MovePoints;

    private GameObject player2;
    public int p2Stars;
    public int p2MovePoints;

    private GameObject player3;
    public int p3Stars;
    public int p3MovePoints;

    private GameObject player4;
    public int p4Stars;
    public int p4MovePoints;

    // Start is called before the first frame update
    void Start()
    {
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        gameReport = GameObject.Find("GameReport").GetComponent<GameReport>();
        playerNumber = 0;

        gameObject.GetComponent<ASLObject>()._LocallySetFloatCallback(readData);
    }

    void Update()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            if (bgm.getGroupWorld(bgm.getPlayerGroup()) != null)
            {
                if (player1 == null)
                    player1 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player1").gameObject;
                if (player2 == null)
                    player2 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player2").gameObject;
                if (player3 == null)
                    player3 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player3").gameObject;
                if (player4 == null)
                    player4 = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Canvas").Find("PlayerPanels").Find("player4").gameObject;

                if (playerNumber == 0)
                {
                    for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
                    {
                        if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
                        {
                            playerNumber = i;
                        }
                    }
                }

                if (player1 != null)
                    player1.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p1Stars + "\nDices: " + p1MovePoints;
                if (player2 != null)
                    player2.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p2Stars + "\nDices: " + p2MovePoints;
                if (player3 != null)
                    player3.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p3Stars + "\nDices: " + p3MovePoints;
                if (player4 != null)
                    player4.transform.Find("playerPoints").GetComponent<Text>().text = "Stars: " + p4Stars + "\nDices: " + p4MovePoints;
            }
        }
    }

    public void readData(string _id, float[] _f)
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            if (_f[0] == bgm.getPlayerGroup())
            {
                switch (_f[1])
                {
                    case 1:
                        p1Stars = (int)_f[2];
                        p1MovePoints = (int)_f[3];
                        break;

                    case 2:
                        p2Stars = (int)_f[2];
                        p2MovePoints = (int)_f[3];
                        break;

                    case 3:
                        p3Stars = (int)_f[2];
                        p3MovePoints = (int)_f[3];
                        break;

                    case 4:
                        p4Stars = (int)_f[2];
                        p4MovePoints = (int)_f[3];
                        break;
                }
            }
        }
    }

    public void sendData()
    {
        gameObject.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[4];
            sendValue[0] = bgm.getPlayerGroup();
            sendValue[1] = playerNumber;
            sendValue[2] = DiceRoll.starCount;
            sendValue[3] = DiceRoll.movePoints;

            gameObject.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
        gameReport.GetComponent<ASLObject>().SendAndSetClaim(() =>
        {
            float[] sendValue = new float[3];
            sendValue[0] = 3;
            sendValue[1] = GameLiftManager.GetInstance().m_PeerId;
            sendValue[2] = DiceRoll.starCount;

            gameReport.GetComponent<ASLObject>().SendFloatArray(sendValue);
        });
    }
}
