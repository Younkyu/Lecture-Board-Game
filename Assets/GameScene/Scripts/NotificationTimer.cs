using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationTimer : MonoBehaviour
{
    public GameObject closeButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        StartCoroutine(timer());
    }

    IEnumerator timer()
    {
        //yield on a new YieldInstruction that waits for 4 seconds.
        yield return new WaitForSeconds(4);

        closeButton.SetActive(false);
        gameObject.SetActive(false);
        gameObject.GetComponent<NotificationTimer>().enabled = false;
    }
}
