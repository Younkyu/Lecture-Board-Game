using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ASL;

public class PlayerPanel : MonoBehaviour
{
    public int ownerID;
    public int stars = 0;
    public int movePts = 0;
    public ASLObject m_ASLObject;
    private Text playerName;
    private Text playerPoints;

    void Awake()
    {
        m_ASLObject = gameObject.GetComponent<ASLObject>();
        Debug.Assert(m_ASLObject != null);
        playerName = gameObject.transform.Find("playerName").GetComponent<Text>();
        playerPoints = gameObject.transform.Find("playerPoints").GetComponent<Text>();
        m_ASLObject._LocallySetFloatCallback(floatFunction);
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setPlayer(int id)
    {
        float[] m_MyFloats = new float[4];
        m_MyFloats[0] = 1;
        m_MyFloats[1] = id;
        m_ASLObject.SendAndSetClaim(() =>
        {
            string floats = "PlayerGrouping Floats sent: ";
            for (int i = 0; i < m_MyFloats.Length; i++)
            {
                floats += m_MyFloats[i].ToString();
                if (m_MyFloats.Length - 1 != i)
                {
                    floats += ", ";
                }
            }
            Debug.Log(floats);
            m_ASLObject.SendFloatArray(m_MyFloats);
        });
    }

    public void updatePoints(int addedStars, int addedMovePts)
    {
        float[] m_MyFloats = new float[4];
        m_MyFloats[0] = 2;
        m_MyFloats[1] = addedStars;
        m_MyFloats[2] = addedMovePts;
        m_ASLObject.SendAndSetClaim(() =>
        {
            string floats = "PlayerGrouping Floats sent: ";
            for (int i = 0; i < m_MyFloats.Length; i++)
            {
                floats += m_MyFloats[i].ToString();
                if (m_MyFloats.Length - 1 != i)
                {
                    floats += ", ";
                }
            }
            Debug.Log(floats);
            m_ASLObject.SendFloatArray(m_MyFloats);
        });
    }

    private void updatePointsText()
    {
        playerPoints.text = "Stars: " + stars + "\nMove Pts: " + movePts;
    }

    /// <summary>
    /// Sets the owner of this object equal to peerID.
    /// </summary>
    /// <param name="_f">If _f[0] is equal to 1 then the owner is set to _f[1].
    /// If _f[0] is equal to 2, then add player points where stars is _f[1] and
    /// move pts is _f[2] before updating the playerPanel.</param>
    public void floatFunction(string _id, float[] _f)
    {
        Debug.Log("userObject float function");
        if (_f[0] == 1)
        {
            ownerID = (int)_f[1];
            playerName.text = GameLiftManager.GetInstance().m_Players[(int)_f[1]] + "";
        }
        else if (_f[0] == 2)
        {
            stars += (int)_f[1];
            movePts += (int)_f[2];
            updatePointsText();
        }
    }
}