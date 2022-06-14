using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL
{
    public class ASLpanel : MonoBehaviour
    {
        RectTransform panel;
        // Start is called before the first frame update
        void Start()
        {
            panel = GetComponent<RectTransform>();
            Debug.Assert(panel != null);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void increaseScale(Vector2 newScale)
        {
            /*TheEcho.text = v.ToString("0");
            if (mCallBack != null)
                mCallBack(v);
            TheSlider.value = v;*/
            panel.sizeDelta += newScale;
        }
    }
}
