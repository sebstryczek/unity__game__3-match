using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform root;
    [SerializeField] private int amount = 20;
    [SerializeField] private bool expandable = true;
    private List<GameObject> pooledObjects;

    private void Start()
    {
        this.pooledObjects = new List<GameObject>();
        for (int i = 0; i < this.amount; i++)
        {
            GameObject obj = Instantiate(this.prefab, this.root);
            obj.name = prefab.name + "_" + i;
            obj.SetActive(false);
            this.pooledObjects.Add(obj);
        }
    }

    public GameObject GetObject()
    {
        for (int i = 0; i < this.pooledObjects.Count; i++)
        {
            if (!this.pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (this.expandable)
        {
            GameObject obj = GameObject.Instantiate(this.prefab, this.root);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}
