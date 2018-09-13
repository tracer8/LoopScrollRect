using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class LSR_GridData
{
    public int index;
    public float number;
    public Color color;
}

public class LSR_GridManager : MonoBehaviour
{
    public List<LoopScrollRect> loopScrollRects;
    public GameObject prefab;
    public List<LSR_GridData> gridDatas = new List<LSR_GridData>();

    private void Start()
    {
        foreach(var item in loopScrollRects)
        {
            item.OnInstantiateNextItem = OnInstantiateNextItem;
        }

        for (int i = 0; i < 100; i++)
        {
            gridDatas.Add(new LSR_GridData()
            {
                index = i,
                number = Random.Range(0, 1000),
                color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f)
            });
        }

        foreach (var item in loopScrollRects)
        {
            item.prefabSource = new LoopScrollPrefabSource()
            {
                prefab = prefab,
                poolSize = 5
            };
            item.totalCount = gridDatas.Count;
            item.RefillCells();
        }

    }

    private void OnInstantiateNextItem(GameObject obj, int index)
    {
        Debug.Log("Index:" + index);

        obj.GetComponent<LSR_GridUIItem>().Init(this.gridDatas[index]);
    }
}
