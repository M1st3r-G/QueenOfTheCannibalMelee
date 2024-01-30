using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class MaskDrop : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private MaskManager.MaskType typeOfMask;
    //Params
    //Temps
    //Publics
    
    private void Awake()
    {
        List<int> options = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if(!MaskManager.Instance.Unlocked[i]) options.Add(i);
        }
        typeOfMask = (MaskManager.MaskType) options[Random.Range(0, options.Count)];
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = MaskManager.Instance.GetMaskSprite(typeOfMask);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        MaskManager.Instance.UnlockMask(typeOfMask);
        MaskManager.Instance.IncreaseMaskFound();
        Destroy(gameObject);
        MaskTutorial.Instance.Activate(typeOfMask);
    }
}