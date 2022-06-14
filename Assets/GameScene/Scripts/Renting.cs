using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;


public class Renting : MonoBehaviour
{
    public GameObject background;
    public Button renting;
    public Button notRenting;
    private PlayerGrouping pGroup;
    private BoardGameManager bgm;
    private int playerNumber;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = renting.GetComponent<Button>();
        btn.onClick.AddListener(selectedRent);
        Button btn2 = notRenting.GetComponent<Button>();
        btn2.onClick.AddListener(selectedPass);
        playerNumber = 0;
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
    }

    private void selectedRent()
    {
        player.GetComponent<PlayerMovement>().renting();
    }

    private void selectedPass()
    {
        
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
