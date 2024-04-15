using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MoleListGenerator : MonoBehaviour
{
    [SerializeField]
    int numberOfMoles;

    [SerializeField]
    int wallWidth;

    [SerializeField]
    int wallHeight;

    [SerializeField]
    int moleLifetime;

    [SerializeField]
    bool allowMoleReuse;

    [SerializeField]
    bool allowCenterMole;

    [SerializeField]
    [Tooltip("Offset is counted left, up, right and down, not diagonal")]
    int newMoleMinimumOffset;

    [SerializeField]
    Text textBox;

    [SerializeField][InspectorButton("OnButtonClickGenerateText")]
    bool generateText;

    void Start()
    {
        if(textBox != null)
        {
            GenerateList();
        }
    }

    private void OnButtonClickGenerateText()
    {
        if (textBox != null)
        {
            GenerateList();
        }
    }

    //Pretty much just making this a coroutine so it doesn't freeze unity if it fails to resolve
    private void GenerateList()
    {
        textBox.text = "";

        List<string> linesGenerated= new List<string>();
        List<(int, int)> coordPairs = new List<(int, int)>();

        int loopLimit = 1000;
        int loopCount = 0;

        while(coordPairs.Count < numberOfMoles)
        {
            loopCount++;
            if(loopCount >= loopLimit)
            {
                Debug.LogError("TOO MANY LOOP ITERATIONS! CANCELLING TEXT GENERATION");
                return;
            }

            int xCoord = Random.Range(1, wallWidth + 1);
            int yCoord = Random.Range(1, wallHeight + 1);

            bool isValid = true;
            if(xCoord == 1 && yCoord == 1 
                || xCoord == 1 && yCoord == wallHeight
                || xCoord == wallWidth && yCoord == 1
                || xCoord == wallWidth && yCoord == wallHeight)
            {
                isValid = false;
            }
            else if (!allowCenterMole && xCoord == (wallWidth+1) / 2 && yCoord == (wallHeight+1) / 2)
            {
                isValid = false;
            }

            //Test if unique
            bool isUnique = true;
            if (!allowMoleReuse)
            {
                foreach ((int, int) coordPair in coordPairs)
                {
                    if (xCoord == coordPair.Item1 && yCoord == coordPair.Item2)
                    {
                        isUnique = false;
                    }
                }
            }

            bool isFarEnoughAway = true;
            if (coordPairs.Count > 0)
            {
                //Test if too close to last pair
                (int, int) lastPair = coordPairs.LastOrDefault();
                int xDifference = Mathf.Abs(lastPair.Item1 - xCoord);
                int yDifference = Mathf.Abs(lastPair.Item2 - yCoord);

                if (xDifference + yDifference < newMoleMinimumOffset)
                {
                    isFarEnoughAway = false;
                }
            }

            //Generate the text line to add
            if(isValid && isUnique && isFarEnoughAway)
            {
                coordPairs.Add((xCoord, yCoord));
            }
        }

        foreach((int, int) molePosition in coordPairs)
        {
            string newLine = "MOLE:(X = " + molePosition.Item1 + ", Y = " + molePosition.Item2 + 
                ", LIFETIME = " + moleLifetime + ") //" + ((linesGenerated.Count + 2) / 2) + "\n";
            linesGenerated.Add(newLine);
            string waitLine = "WAIT:(HIT)\n";
            linesGenerated.Add(waitLine);
        }

        foreach(string line in linesGenerated)
        {
            textBox.text = textBox.text + line;
        }
    }

}
