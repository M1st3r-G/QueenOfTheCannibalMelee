using UnityEngine;

public class LineManager : MonoBehaviour
{
    //ComponentReferences
    //Params
    public int NumberOfLines => numberOfLines;
    [SerializeField] [Range(1, 5)] private int numberOfLines;
    private float[] LineHeights { get; set; }
    [SerializeField] private float heightOfLine;
    [SerializeField] private float firstLine;
    [SerializeField] private float displacementBetweenLines;
    //Temps
    //Publics
    public static LineManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        
        LineHeights = new float[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            LineHeights[i] = firstLine + i * heightOfLine;
        }
    }
    
    public Vector3 ChangeLine(GameObject g, int newLine)
    {
        int currentLine = GetLine(g);
        if (currentLine == newLine) return g.transform.position;
        print($"Changed the line of {g.name} from {currentLine} to Line {newLine}");
        return InnerSetLine(g, newLine, currentLine);
    }
    
    public Vector3 SetToLine(GameObject g, int startLine)
    {
        print($"Set Line of {g.name} to {startLine}");
        return InnerSetLine(g, startLine);
    }
    
    private Vector3 InnerSetLine(GameObject g, int l, int prev = -1)
    {
        float dis = prev == -1 ? 0 : displacementBetweenLines * (l - prev);
        g.layer = LayerMask.NameToLayer($"Line{l + 1}");
        return new Vector3(g.transform.position.x + dis, LineHeights[l], l);
    }
    
    public static int GetLine(GameObject g) => LayerMask.LayerToName(g.layer)[^1] - '1';
    private void OnDestroy() => Instance = null;
}
