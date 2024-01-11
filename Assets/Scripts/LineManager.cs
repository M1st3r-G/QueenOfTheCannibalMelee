using System;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    //ComponentReferences
    //Params
    public int NumberOfLines => numberOfLines;
    [SerializeField] [Range(1, 5)] private int numberOfLines;
    public float[] LineHeights { get; private set; }
    [SerializeField] private float heightOfLine;
    [SerializeField] private float firstLine;
    [SerializeField] private float displacementBetweenLines;

    //Temps
    //Publics
    public static LineManager Instance => _instance;
    private static LineManager _instance;
    
    private void Awake()
    {
        if (_instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
        
        LineHeights = new float[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            LineHeights[i] = firstLine + i * heightOfLine;
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

    /// <summary>
    /// Not for Use Outside this Class, Sets the Gameobject g to Line l and adjusts the Position. A previous line of -1 Will be interpretet as the Object not being in a Line before
    /// </summary>
    /// <param name="g">The GameObjetc to set</param>
    /// <param name="l">The LineIndex to set to</param>
    /// <param name="prev">The Line the Object was first in (-1 if left empty)</param>
    private void InnerSetLine(GameObject g, int l, int prev = -1)
    {
        float dis = prev == -1 ? 0 : displacementBetweenLines * (l - prev);
        g.transform.position = new Vector3(
            g.transform.position.x + dis, // Add Displacement to Add depth (Except its first)
            LineHeights[l], 
            l); // Changes the Z Value, so Objects in Higher Lines appear Behind
        g.layer = LayerMask.NameToLayer($"Line{l + 1}");
    }
    
    /// <summary>
    /// Returns the LineIndex of a given GameObject.
    /// </summary>
    /// <param name="g">The GameObject to Search for</param>
    /// <returns>The Index of the Line the GameObjet is in</returns>
    /// <exception cref="IndexOutOfRangeException">Raised if Object not in a Right Layer</exception>
    public static int GetLine(GameObject g) => LayerMask.LayerToName(g.layer)[^1] - '0';
}