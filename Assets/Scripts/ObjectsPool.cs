using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool
{
    public void PoolCells<T>(Vector3 position, List<T> poolObjects, T prefabCell,
        Transform containerCells, List<T> cellsText) where T : Component
    {
        if (poolObjects == null)
            return;

        bool freeObject = false;

        for (int k = 0; k < poolObjects.Count; k++)
        {
            if (!poolObjects[k].gameObject.activeInHierarchy)
            {
                poolObjects[k].transform.position = position;

                poolObjects[k].gameObject.SetActive(true);

                freeObject = true;
                break;
            }
        }
        if (!freeObject)
        {
            var card = GameObject.Instantiate(prefabCell, position, Quaternion.identity) as T;
            card.transform.SetParent(containerCells.GetComponent<RectTransform>().transform);
            cellsText.Add(card);
            poolObjects.Add(card);
        }
    }
}
