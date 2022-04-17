using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerTextAnim : MonoBehaviour
{
    public bool IsScaled = false;
    public RectTransform startRect;
    // Start is called before the first frame update
    void Start()
    {
        startRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.active == true)
        {
            if (!IsScaled)
            {
                gameObject.GetComponent<RectTransform>().localScale = Vector3.Slerp(gameObject.GetComponent<RectTransform>().localScale, new Vector3(8, 8, 8), 3f * Time.deltaTime);
                if (gameObject.GetComponent<RectTransform>().localScale.x >= 7.8f)
                {
                    IsScaled = true;
                }
            }
            else
            {
                gameObject.GetComponent<RectTransform>().localScale = Vector3.Slerp(gameObject.GetComponent<RectTransform>().localScale, new Vector3(6, 6, 6), 3f * Time.deltaTime);
                if (gameObject.GetComponent<RectTransform>().localScale.x <= 6.2)
                {
                    IsScaled = false;
                }
            }
        }
    }
}
