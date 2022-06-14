using UnityEngine;
using System.Collections;
using ASL;

public class DisplayName : MonoBehaviour
{
    // 1 - Create a 3Dtext prefab and add this script to it;
    // 2 - Put the prefab under the object who will have the name displayed

    private TextMesh textToDisplay;
    private PlayerGrouping pGroup;
    private BoardGameManager bgm;

    // Use this for initialization
    void Start()
    {
        textToDisplay = gameObject.GetComponent<TextMesh>();
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();

        foreach (int p in GameLiftManager.GetInstance().m_Players.Keys)
        {
            if (p != 1)
            {
                int playerNumber = 0;
                for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
                {
                    if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == p)
                    {
                        playerNumber = i;
                    }
                }

                GameObject playerWorld = bgm.getGroupWorld(bgm.getPlayerGroup(p));
                if (playerWorld == bgm.getGroupWorld(bgm.getPlayerGroup()))
                {
                    Debug.Log(GameLiftManager.GetInstance().m_Players[p] + " IS IN THIS GROUP");
                    playerWorld.transform.Find("Plane").Find("Player" + playerNumber + "Piece(Clone)").Find("NameDisplay").GetComponent<TextMesh>().text = GameLiftManager.GetInstance().m_Players[p];
                }
            }
        }
    }

    void Update()
    {
        // textToDisplay.text = ((string)transform.parent.name).Replace("Piece(Clone)", "");
    }

    void LateUpdate()
    {
        //Make the text allways face the camera
        transform.rotation = Camera.main.transform.rotation;
    }
}