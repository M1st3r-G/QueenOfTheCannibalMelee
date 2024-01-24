using System.Collections.Generic;
using UI.Parallax;
using UnityEngine;

public class SpiceParallaxController : MonoBehaviour, IParallaxLayer
{
    // Param
    [SerializeField] private float parallaxFactor;
    [SerializeField] private float spiceDistance;
    [SerializeField] private float yPos;
    [SerializeField] private Vector2 randomOffsetRange;
    [SerializeField] private GameObject spiceDefault;
    private List<GameObject> spices;
    private float currentMove;
    private int sortingLayer;
    
    private void Awake()
    {
        spices = new List<GameObject>();
        currentMove = 0f;
        for (int i = 0; i < 1 + 19.2 / spiceDistance; i++)
        {
            Sprite spice = SceneController.Instance.GetCurrentLevel().GetRandomSpiceSprite;
            GameObject tmp = CreateSpice(spice);
            float xPos = -9 + i * spiceDistance + Random.Range(0f, randomOffsetRange.x) - randomOffsetRange.x / 2;
            tmp.transform.position = new Vector3(xPos, yPos + Random.Range(0f, randomOffsetRange.y) - randomOffsetRange.y / 2, 0);
            
            spices.Add(tmp);
        }
    }

    private GameObject CreateSpice(Sprite s)
    {
        GameObject tmp =  Instantiate(spiceDefault, transform);
        SpriteRenderer sr = tmp.GetComponent<SpriteRenderer>();
        sr.sprite = s;
        sr.sortingOrder = sortingLayer;
        return tmp;
    }
    
    public void Move(float delta)
    {
        transform.localPosition += (1 - parallaxFactor) * delta * Vector3.left;
        currentMove += (1 - parallaxFactor) * delta;
        if (!(currentMove > spiceDistance)) return;
        
        currentMove -= spiceDistance;
        Destroy(spices[0]);
        spices.RemoveAt(0);
            
        Sprite spice = SceneController.Instance.GetCurrentLevel().GetRandomSpiceSprite;
        GameObject tmp = CreateSpice(spice);
        float xPos = spices[^1].transform.position.x + spiceDistance + Random.Range(0f, randomOffsetRange.x) - randomOffsetRange.x / 2;
        tmp.transform.position = new Vector3(xPos, yPos + Random.Range(0f, randomOffsetRange.y) - randomOffsetRange.y / 2, 0);

        spices.Add(tmp);
    }
    

    public void FixOrder(int layer) => sortingLayer = layer;
}