using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    public float destructionTime = 1f;

    public float itemSpawnChance = 0.2f;
    public GameObject[] spawnableItems;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,  destructionTime);
    }
    
    private void OnDestroy()
    {
        if (spawnableItems.Length > 0 && Random.value < itemSpawnChance)
        {
            int randomIndex = Random.Range(0, spawnableItems.Length);
            GameObject item = Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
            ItemManager.Instance.AddItem(item);
        }
    }


}
