using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSide : MonoBehaviour
{
    public GameObject dice;
    private Vector3 diceVelocity;
    private Vector3 diceAngularVelocity;
    private bool stopInvoke;

    // Update is called once per frame
    void FixedUpdate()
    {
        diceVelocity = dice.GetComponent<Rigidbody>().velocity;
        diceAngularVelocity = dice.GetComponent<Rigidbody>().angularVelocity;
        stopInvoke = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (diceVelocity == Vector3.zero && diceAngularVelocity == Vector3.zero)
        {
            switch (other.gameObject.name)
            {
                case "Side1":
                    DiceRoll.DiceNumber = 6;
                    break;

                case "Side2":
                    DiceRoll.DiceNumber = 5;
                    break;

                case "Side3":
                    DiceRoll.DiceNumber = 4;
                    break;

                case "Side4":
                    DiceRoll.DiceNumber = 3;
                    break;

                case "Side5":
                    DiceRoll.DiceNumber = 2;
                    break;

                case "Side6":
                    DiceRoll.DiceNumber = 1;
                    break;
            }
        }
    }
}
