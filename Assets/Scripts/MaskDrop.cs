using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class MaskDrop : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private int typeOfMask;
    //Params
    //Temps
    //Publics
    
    private void Awake()
    {
        List<int> options = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if(!MaskUIController.Instance.Unlocked[i]) options.Add(i);
        }
        typeOfMask = options[Random.Range(0, options.Count)];
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = MaskUIController.Instance.GetMaskSprite(typeOfMask);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        MaskUIController.Instance.UnlockMask(typeOfMask);
        MaskUIController.Instance.masksFound++;
        Destroy(gameObject);
    }
}