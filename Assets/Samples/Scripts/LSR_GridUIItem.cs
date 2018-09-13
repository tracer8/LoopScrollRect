using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSR_GridUIItem : MonoBehaviour
{
    public void Init(LSR_GridData data)
    {
        this.transform.Find("Name").GetComponent<Text>().text = "Index: " + data.index;
        this.transform.Find("Random").GetComponent<Text>().text = "Random: " + data.number;
        this.transform.GetComponent<Image>().color = data.color;
    }
}
