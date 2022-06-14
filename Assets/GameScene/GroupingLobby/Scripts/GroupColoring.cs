using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupColoring : MonoBehaviour
{
    public int groupLimit = 4;
    private Image image;
    private int currentIndex;
    private Color32 color1;
    private Color32 color2;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        currentIndex = transform.GetSiblingIndex();
        color1 = new Color32(181, 236, 255, 255); //light blue
        color2 = new Color32(255, 255, 255, 255); //white
        setColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex != transform.GetSiblingIndex())
        {
            currentIndex = transform.GetSiblingIndex();
            setColor();
        }
    }

    private void setColor()
    {
        if (((transform.GetSiblingIndex()) / groupLimit) % 2 == 0)
        {
            image.color = color1;
        } else
        {
            image.color = color2;
        }
    }
}
