using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL;

public class CoinFlip : MonoBehaviour
{
    private Vector3 originalPos;
    private static Rigidbody rb;
    public static GameObject CoinView;
    public static GameObject CoinDetector;
    public static bool good;
    private BoardGameManager bgm;
    private PlayerGrouping pGroup;
    private PlayerMovement pm;
    public static bool canFlip;
    private static int passByPlayerNum;
    public static CoinFlip instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        CoinView = GameObject.Find("CoinView");
        CoinView.SetActive(false);
        CoinDetector = GameObject.Find("CoinDetector");
        CoinDetector.SetActive(false);
        originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        good = true;
        pGroup = GameObject.Find("GameManager").GetComponent<PlayerGrouping>();
        bgm = GameObject.Find("GameManager").GetComponent<BoardGameManager>();
        canFlip = false;
        Invoke("findPlayer", 1);
    }

    void findPlayer()
    {
        if (GameLiftManager.GetInstance().m_PeerId != 1)
        {
            int playerNumber = 0;
            for (int i = 1; i <= pGroup.m_playerGroups[bgm.getPlayerGroup() - 1].Count; i++)
            {
                if (pGroup.m_playerGroups[bgm.getPlayerGroup() - 1][i - 1] == GameLiftManager.GetInstance().m_PeerId)
                {
                    playerNumber = i;
                }
            }
            pm = bgm.getGroupWorld(bgm.getPlayerGroup()).transform.Find("Plane").Find("Player" + playerNumber + "Piece(Clone)").GetComponent<PlayerMovement>();
        }
    }
    //passByPlayer is -1 if not pass-by steal; otherwise, it is the player number of the other player
    public static void flipWhenReady(int passByPlayer)
    {
        CoinFlip.instance.StartCoroutine(CoinFlip.instance.flipWhenReadyHelper(passByPlayer));
    }
    
    IEnumerator flipWhenReadyHelper(int passByPlayer)
    {
        while (canFlip)
        {
            yield return null;
        }
        canFlip = true;
        passByPlayerNum = passByPlayer;
        CoinView.SetActive(true);
        CoinDetector.SetActive(true);
        Flip();
    }

    void Flip()
    {
        CoinView.SetActive(true);
        rb.velocity = Vector3.zero;
        float dirX = Random.Range(2000, 10000);
        float dirY = Random.Range(2000, 10000);
        float dirZ = Random.Range(2000, 10000);
        float upForce = Random.Range(500, 1000);
        transform.position = originalPos;
        transform.rotation = Quaternion.identity;
        rb.AddForce(transform.up * upForce);
        rb.AddTorque(dirX, dirY, dirZ);
        Invoke("coinOff", 3.5f);
    }

    private void Update()
    {
    }

    void coinOff()
    {
        CoinView.SetActive(false);
        canFlip = false;
        pm.fight(good, passByPlayerNum);
    }
}
