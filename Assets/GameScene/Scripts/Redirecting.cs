using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class Redirecting : MonoBehaviour
{
    public GameObject background;
    public Button split;
    public Button straight;
    private PlayerGrouping pGroup;
    private BoardGameManager bgm;
    private int playerNumber;
    private GameObject player;
    public Text stepsLeft;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = split.GetComponent<Button>();
        btn.onClick.AddListener(selectedSplit);
        Button btn2 = straight.GetComponent<Button>();
        btn2.onClick.AddListener(selectedStraight);
        playerNumber = 0;
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
    }

    private void selectedSplit()
    {
        player.GetComponent<PlayerMovement>().SplitPath();
    }

    private void selectedStraight()
    {
        player.GetComponent<PlayerMovement>().StraightPath();
    }


    // Update is called once per frame
    void Update()
    {
        if (bgm.getGroupWorld(bgm.getPlayerGroup()) != null)
        {
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
            if (player == null)
            {
                player = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player" + playerNumber + "Piece(Clone)").gameObject;
            }
        }
    }
}
