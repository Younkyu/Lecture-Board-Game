using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCoinSide : MonoBehaviour
{
    public GameObject coin;
    private Vector3 coinVelocity;
    private Vector3 coinAngularVelocity;
    private bool stopInvoke;

    // Update is called once per frame
    void FixedUpdate()
    {
        coinVelocity = coin.GetComponent<Rigidbody>().velocity;
        coinAngularVelocity = coin.GetComponent<Rigidbody>().angularVelocity;
        stopInvoke = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (coinVelocity == Vector3.zero && coinAngularVelocity == Vector3.zero)
        {
            switch (other.gameObject.name)
            {
                case "Side1":
                    CoinFlip.good = false;
                    break;

                case "Side2":
                    CoinFlip.good = true;
                    break;
            }
        }
    }
}
