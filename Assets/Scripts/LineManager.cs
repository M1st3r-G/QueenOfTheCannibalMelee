using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LineManager : MonoBehaviour
{
    //ComponentReferences
    //Params
    [SerializeField] private int numberOfLines;
    public int NumberOfLines => numberOfLines;
    [SerializeField] private float heightOfLine;
    [SerializeField] private float firstLine;
    private float[] lineHeights;
    [SerializeField] private float displacementBetweenLines;
    public float[] LineHeights => lineHeights;

    //Temps
    private List<GameObject>[] objectsInLine;
    //Publics
    private static LineManager _instance;
    public static LineManager Instance => _instance;
    
    private void Awake()
    {
        if (_instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);

        objectsInLine = new List<GameObject>[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            objectsInLine[i] = new List<GameObject>();
        }

        lineHeights = new float[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            lineHeights[i] = firstLine + i * heightOfLine;
        }
    }
    
    private void OnDestroy()
    {
        _instance = null;
    }
    
    /// <summary>
    /// Changes the Line the Game Object is in
    /// </summary>
    /// <param name="g">The Object</param>
    /// <param name="newLine">The Index of the New Line</param>
    /// <exception cref="Exception">Raised, when the GameObject in no Line</exception>
    public void ChangeLine(GameObject g, int newLine)
    {
        
        int currentLine = GetLine(g);
        if (currentLine == -1) throw new Exception("Object not in Array");
        if (currentLine == newLine) return;
        objectsInLine[currentLine].Remove(g);
        InnerSetLine(g, newLine, currentLine);
        print($"Changed the line of {g.name} from {currentLine} to Line {newLine}");

    }

    /// <summary>
    /// Sets Object into the Line, if not already Present
    /// </summary>
    /// <param name="g">The Object to set</param>
    /// <param name="startLine">The LineIndex to set into</param>
    /// <exception cref="Exception">Raised if the Object is present</exception>
    public void SetToLine(GameObject g, int startLine)
    {
        if (GetLine(g) != -1) throw new Exception("Object already in Lines");
        InnerSetLine(g, startLine);
        print($"Set Line of {g.name} to {startLine}");
    }

    private void InnerSetLine(GameObject g, int l, int prev = -1)
    {
        objectsInLine[l].Add(g);
        float dis = prev == -1 ? 0 : displacementBetweenLines * (l - prev);
        Vector3 currentPos = g.transform.position;
        g.transform.position = new Vector3(
            g.transform.position.x + dis, // Add Displacement to Add depth (Except its first)
            lineHeights[l], 
            l);
        // Changes the Z Value, so Objects in Higher Lines appear Behind
    }
    
    /// <summary>
    /// Returns the LineIndex of a given GameObject. Returns -1 if the GameObject is not found.
    /// </summary>
    /// <param name="g">The GameObject to Search for</param>
    /// <returns>The Index of the Line the GameObjet is in</returns>
    public int GetLine(GameObject g)
    {
        int i = 0;
        foreach (List<GameObject> line in objectsInLine)
        {
            if (line.Contains(g)) return i;
            i++;
        }

        return -1;
    }
    
    public List<GameObject> GetObjectsInLine(int n)
    {
        if (0 > n || n >= numberOfLines) return null;
        return objectsInLine[n];
    }
}