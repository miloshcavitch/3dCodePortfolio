/*
this MonoBehaviour takes previously created bounds of an archipelago, draws an angled grid on them, cuts the grids up
into closed polygonal shapes and repeats the process on all the polygonal shapes/bounds to create an array of 'city blocks' i plan
on using to erect a 3d cityscape from. This part of the project is almost done, I have one minor bug and also need to write a function to reduce the size of 'exBounds'
for all the macroBounds which will increase the efficiency by quite a bit.

note: Hudson and Milo are both personal libraries I wrote for the project (I named Hudson after a friend of mine who studies architecture)
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class FractBlockGen : MonoBehaviour {
    public int gridSize, blockSize;
    private HexGen islandGenerator;
    private List<LineSegment> testLineBounds;
    private List<List<LineSegment>> macroBounds;
    private List<LineSegment> microLines;
    //below are for testing purposes within update
    private int index = 0;
    private int subIndex = 0;
    private LineSegment currentLine, nextLine;
    private int isEnd = 1;
    private int nextEnd;
    private List<LineSegment> tempBounds = new List<LineSegment>();
    private List<int> testUseList = new List<int>();
    private List<float> testAngleList = new List<float>();
    public List<List<LineSegment>> GenerateFractalBlocks(List<LineSegment> bounds, List<LineSegment> exBounds, int gSize, ref List<LineSegment> testLineBounds)
    {
        //function that does majority of work. takes the island bounds, draws lines over them then chops all lines up.
        //Then creates references for everyline, so every end of a line has a reference to all connected lines
        List<IntersectionIndex> FixRefList = new List<IntersectionIndex>();
        //turn bounds into continuous loop/polyline w/ func

        float angle = Random.Range(0, Mathf.PI * 2);
        float maxDist = Milo.determineMaxDist(Milo.linesToPoints(exBounds));
        List<List<LineSegment>> macroLineGrid = new List<List<LineSegment>>();
        for (int i = 0; i < 2; i++)
        {
            macroLineGrid.Add(new List<LineSegment>());
            bool cycleIsComplete = false;
            float multiplier = 0f;
            Vector3 currentPos, endPos;
            endPos = Milo.AngleProjectionFromPoint(bounds[0].points[0], Milo.GetPerp(angle), maxDist);
            while (!cycleIsComplete)
            {
                currentPos = Milo.AngleProjectionFromPoint(endPos, Milo.GetFlip(Milo.GetPerp(angle)), multiplier * gSize);
                multiplier += 1.0f;

                Debug.Log("test");
                Vector3 start = Milo.AngleProjectionFromPoint(currentPos, angle, maxDist);
                Vector3 finish = Milo.AngleProjectionFromPoint(currentPos, Milo.GetFlip(angle), maxDist);
                LineSegment thisLine = new LineSegment(start, finish);
                LinesOverLand(thisLine, ref bounds, ref macroLineGrid, i, ref FixRefList);



                if (Milo.Hypot(endPos, currentPos) > maxDist * 2)
                {
                    cycleIsComplete = true;
                }
            }
            angle = Milo.GetPerp(angle);//can be changed on a per level basis? macro at an angle with micro perp?
        }

        List<IntersectionIndex> intersections = new List<IntersectionIndex>();
        List<LineSegment> microGridLines = new List<LineSegment>();
        int nextIndex = 0;

        ChopLines(macroLineGrid[0], macroLineGrid[1], islandGenerator.islandBounds, ref microGridLines, ref intersections, ref nextIndex, 0);
        ChopLines(macroLineGrid[1], macroLineGrid[0], islandGenerator.islandBounds, ref microGridLines, ref intersections, ref nextIndex, 1);
        ChopLines(bounds, macroLineGrid[0], macroLineGrid[1], ref microGridLines, ref intersections, ref nextIndex, 2);
        Milo.VisualizeBounds(microGridLines, Color.grey, Vector3.zero);
        for (int i = 0; i < microGridLines.Count; i++)
        {
            microGridLines[i].testThisIndex = i;//test
            if (microGridLines[i].points[0] == microGridLines[i].points[1])
            Debug.Log("test");

            for (int j = 0; j < microGridLines.Count; j++)
            {
                if (i != j)
                {
                    if (microGridLines[j].points[0] == microGridLines[i].points[0] && microGridLines[j].points[1] == microGridLines[i].points[1])
                    Debug.Log("test");
                }

            }
        }
        Debug.Log("test");

        testLineBounds = microGridLines;
        microLines = microGridLines;
        List<List<LineSegment>> allBounds = GenerateInnerBounds(microGridLines);
        Debug.Log("test");

        return allBounds;
    }



    static void ChopLines(List<LineSegment> tooChop, List<LineSegment> chopOne, List<LineSegment> chopTwo, ref List<LineSegment> microGridLines, ref List<IntersectionIndex> previousIntersections, ref int nextIndex, int xybee)
    {//function chops lines and then creates references to the other line segments it is connected to. Probably most proud of this function :)
        if (xybee == 2)
        {
            Debug.Log("test");
        }
        for (int i = 0; i < tooChop.Count; i++)
        {

            LineSegment thisChop = tooChop[i];
            if (tooChop[i].xyb != 2)
            {
                //thisChop = Milo.ExtendLine(tooChop[i], 0.01f);
                } else
                {
                    Debug.Log("test");
                }
                List<Vector3> intersectingPoints = new List<Vector3>();
                if (tooChop[i].fixIntPos.Count != 0)
                {
                    for (int j = 0; j < tooChop[i].fixIntPos.Count; j++)
                    {
                        intersectingPoints.Add(tooChop[i].fixIntPos[j]);
                    }

                }
                for (int j = 0; j < chopOne.Count; j++)
                {
                    LineSegment oneChop = chopOne[j];
                    if (chopOne[j].xyb != 2)
                    {
                        oneChop = Milo.ExtendLine(chopOne[j], 0.01f);
                    }
                    Vector3 intersect = Milo.IntersectingPoint(thisChop, oneChop);
                    if (intersect.y == 0)
                    {
                        intersectingPoints.Add(intersect);
                    }
                }
                for (int j = 0; j < chopTwo.Count; j++)
                {
                    LineSegment twoChop = chopTwo[j];
                    if (chopTwo[j].xyb != 2)
                    {
                        twoChop = Milo.ExtendLine(chopTwo[j], 0.01f);
                    }
                    Vector3 intersect = Milo.IntersectingPoint(thisChop, twoChop);
                    if (intersect.y == 0)
                    {
                        intersectingPoints.Add(intersect);
                    }
                }

                Milo.LinePointsToSegments(intersectingPoints, tooChop[i], ref previousIntersections, ref nextIndex, ref microGridLines, xybee);

            }
        }

        static List<Vector3> LineExtensionIntersections(LineSegment tooChop, List<LineSegment> chopOne, List<LineSegment> chopTwo)
        {
            List<Vector3> intersectionReturn = new List<Vector3>();
            for (int i = 0; i < chopOne.Count; i++)
            {
                LineSegment elChop = Milo.ExtendLine(chopOne[i], 0.01f);
                Vector3 intersect = Milo.IntersectingPoint(tooChop, elChop);
                if (intersect.y == 0)
                {
                    intersectionReturn.Add(intersect);
                }
            }
            for (int i = 0; i < chopTwo.Count; i++)
            {
                LineSegment elChop = Milo.ExtendLine(chopTwo[i], 0.01f);
                Vector3 intersect = Milo.IntersectingPoint(tooChop, elChop);
                if (intersect.y == 0)
                {
                    intersectionReturn.Add(intersect);
                }
            }

            return intersectionReturn;
        }
        static List<LineSegment> LinesOverLand(LineSegment tooChop, ref List<LineSegment> bounds, ref List<List<LineSegment>> macroList, int xy, ref List<IntersectionIndex> FixRefList)
        {//chops lines by checking all points of intersection, orders them from distance of line origin and then determines which segments would be over land by only starting lines
            //at an odd index and ending them at the next even index (works if index 0 is not inside an island)
            List<Vector3> intersectionPositions = new List<Vector3>();
            for (int i = 0; i < bounds.Count; i++)
            {
                Vector3 intersect = Milo.IntersectingPoint(tooChop, bounds[i]);
                if (intersect.y == 0)
                {
                    intersectionPositions.Add(intersect);

                    FixRefList.Add(new IntersectionIndex(intersect, 2, i, 0));
                    bounds[i].fixIntPos.Add(intersect);
                    Debug.Log("test");
                }
            }

            intersectionPositions = intersectionPositions.Distinct().ToList();
            intersectionPositions.Sort((x, y) => -(Milo.Hypot(x, tooChop.points[0]).CompareTo(Milo.Hypot(y, tooChop.points[0]))));

            List<LineSegment> overLand = new List<LineSegment>();
            for (int i = 0; i < intersectionPositions.Count - 1; i += 2)
            {
                macroList[xy].Add(new LineSegment(intersectionPositions[i], intersectionPositions[i + 1]));
            }

            return overLand;
        }

        static LineSegment SplitLineSegment(ref LineSegment tooSplit, Vector3 intersection)
        {
            LineSegment newLine = new LineSegment(intersection, tooSplit.points[1]);
            tooSplit.points[1] = intersection;
            return newLine;
        }
        List<LineSegment> createInitBounds()
        {
            float width = GetComponent<HexGen>().width;
            float height = GetComponent<HexGen>().height;
            LineSegment left, right, top, bottom;
            List<LineSegment> iBounds = new List<LineSegment>();
            left = new LineSegment(new Vector3(0, 0, 0), new Vector3(0, 0, height));
            right = new LineSegment(new Vector3(width, 0, 0), new Vector3(width, 0, height));
            top = new LineSegment(new Vector3(0, 0, height), new Vector3(width, 0, height));
            bottom = new LineSegment(new Vector3(0, 0, 0), new Vector3(width, 0, 0));
            iBounds.Add(left);
            iBounds.Add(top);
            iBounds.Add(right);
            iBounds.Add(bottom);

            for (int i = 0; i < iBounds.Count; i++)
            {
                // Debug.DrawLine(iBounds[i].points[0], iBounds[i].points[1], Color.white, 20000, true);
            }

            return iBounds;
        }


        static List<List<LineSegment>> GenerateInnerBounds(List<LineSegment> microGridLines)
        {//uses the theory "if you only take right turns you will end up where you began"
        List<List<LineSegment>> allBounds = new List<List<LineSegment>>();
        for (int i = 0; i < microGridLines.Count; i++)
        {
            if (microGridLines[i].xyb != 2.0f && microGridLines[i].useCount < 2)
            {
                for (int u = 0; u < 2; u++)
                {
                    int newEnd, newStart;
                    newEnd = u;
                    newStart = Mathf.Abs(u - 1);
                    List<LineSegment> theseBounds = new List<LineSegment>();
                    Vector3 originPoint = microGridLines[i].points[newStart];
                    LineSegment currentLine = microGridLines[i];
                    List<int> thisUseList = new List<int>();
                    thisUseList.Add(i);
                    int testCount = 0;
                    while (currentLine.points[newEnd] != originPoint && testCount < 39)//
                    {
                        testCount++;
                        theseBounds.Add(currentLine);
                        LineSegment nextLine = new LineSegment(Vector3.zero, Vector3.zero);
                        int nextEnd = 20;
                        DetermineNextLine(microGridLines, currentLine, newEnd, ref nextLine, ref nextEnd, ref thisUseList);

                        currentLine = nextLine;
                        newEnd = nextEnd;

                        if (newEnd == 20)
                        {
                            Debug.Log("test");
                            Debug.DrawLine(currentLine.points[0] + Vector3.up, currentLine.points[1] + Vector3.up, Color.magenta, 2000, true);
                            Debug.DrawLine(currentLine.points[0], currentLine.points[1] + (Vector3.up * 5), Color.magenta, 2000, true);
                        }

                    }

                    bool alreadyExists = false;
                    for (int use = 0; use < thisUseList.Count; use++)
                    {
                        for (int subUse = 0; subUse < thisUseList.Count; subUse++)
                        {
                            if (microGridLines[thisUseList[use]].useList.Contains(thisUseList[subUse]))//needs to be tested
                            {
                                alreadyExists = true;
                            }
                        }
                    }

                    if (!alreadyExists)//add to allBounds
                    {
                        if (microGridLines[i].useCount > 0)
                        {
                            Debug.Log("test");
                        }

                        theseBounds.Add(currentLine);
                        allBounds.Add(theseBounds);
                        Vector3 thisCenter = Milo.MedianPoint(theseBounds);
                        Color thisColor = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);

                        //now add useCount to all
                        for (int us = 0; us < thisUseList.Count; us++)
                        {
                            microGridLines[thisUseList[us]].useCount++;
                            for (int sUs = 0; sUs < thisUseList.Count; sUs++)
                            {
                                if (us != sUs)
                                {
                                    microGridLines[thisUseList[us]].useList.Add(thisUseList[sUs]);
                                }
                            }
                        }

                        for (int b = 0; b < theseBounds.Count; b++)
                        {
                            //below is for temporary drawing of bounds
                            /*
                            Vector3 one, two;
                            one = Milo.AngleProjectionFromPoint(theseBounds[b].points[0], Milo.GetAngle(theseBounds[b].points[0], thisCenter), 0.2f);
                            two = Milo.AngleProjectionFromPoint(theseBounds[b].points[1], Milo.GetAngle(theseBounds[b].points[1], thisCenter), 0.2f);
                            Debug.DrawLine(one, two, thisColor, 20000, true);
                            */
                            //
                        }




                        } else if (testCount > 38)
                        {
                            Debug.Log("test");
                        }
                    }
                }
            }
            Debug.Log(allBounds.Count);
            return Milo.RefreshedBounds(allBounds);
        }


        static void DetermineNextLine(List<LineSegment> microGridLines, LineSegment currentLine, int isEnd, ref LineSegment nextLine, ref int nextEnd, ref List<int> useList)
        {
            //xyb == 2 == island borderline
            //xyb == 1 choplines, xyb == 0 are chopline perpendicular to xyb == 1
            //this works by knowing the type of turns that are possible depending on current location.
            //One example: If you're on an island border (xyb == 2) you can only go to the next border segment unless you have the option to connect to a non border segment, then you will DEFINITELY take that turn.

            switch (currentLine.connections[isEnd].Count)
            {
                case 0:
                //error;
                Debug.Log("test");
                break;
                case 1:
                //next line equals that connection
                nextLine = microGridLines[currentLine.connections[isEnd][0].index];
                nextEnd = Mathf.Abs(currentLine.connections[isEnd][0].isEnd - 1);
                if (currentLine.xyb != 2f)
                useList.Add(currentLine.connections[isEnd][0].index);
                break;
                case 2:
                //if currentLine.xyb == 2, go with the one that is 0 or 1
                if (currentLine.xyb == 2)
                {
                    if (currentLine.connections[isEnd][0].xyb == 0 || currentLine.connections[isEnd][0].xyb == 1)
                    {
                        nextLine = microGridLines[currentLine.connections[isEnd][0].index];
                        nextEnd = Mathf.Abs(currentLine.connections[isEnd][0].isEnd - 1);
                        if (currentLine.xyb != 2f)
                        useList.Add(currentLine.connections[isEnd][0].index);
                        } else
                        {
                            nextLine = microGridLines[currentLine.connections[isEnd][1].index];
                            nextEnd = Mathf.Abs(currentLine.connections[isEnd][1].isEnd - 1);
                            if (currentLine.xyb != 2f)
                            useList.Add(currentLine.connections[isEnd][1].index);

                        }
                        } else
                        {
                            IntFloat lineInfo = Milo.DetermineMostLeft(microGridLines, currentLine, isEnd);
                            nextLine = microGridLines[lineInfo.integer];
                            nextEnd = Mathf.Abs(lineInfo.isEnd - 1);
                            if (currentLine.xyb != 2f)
                            useList.Add(lineInfo.integer);

                        }
                        break;
                        case 3:
                        //all should not be xyb == 2, go with the leftmost one
                        IntFloat lineI = Milo.DetermineMostLeft(microGridLines, currentLine, isEnd);
                        if (microGridLines[lineI.integer].xyb == 2)
                        {
                            //nextLine = microGridLines[lineI.integer];
                            //nextEnd = Mathf.Abs(lineI.isEnd - 1);
                            lineI = Milo.MostLeftCaseThreeBug(microGridLines, currentLine, isEnd);
                            Debug.DrawLine(microGridLines[lineI.integer].points[0], microGridLines[lineI.integer].points[0] + (Vector3.up * 5), Color.green, 2000, true);
                        }
                        nextLine = microGridLines[lineI.integer];
                        nextEnd = Mathf.Abs(lineI.isEnd - 1);
                        if (currentLine.xyb != 2f)
                        useList.Add(lineI.integer);
                        break;
                        case 4:

                        Debug.Log("test");
                        break;
                        default:
                        Debug.Log("test");
                        break;
                    }
                }

                private int index = 0;
                private int subIndex = 0;
                private LineSegment currentLine, nextLine;
                private int isEnd = 1;
                private int nextEnd;
                private List<LineSegment> tempBounds = new List<LineSegment>();
                private List<int> testUseList = new List<int>();
                private List<float> testAngleList = new List<float>();
                void Update()
                {//for testing purposes
                    if (Input.GetKeyDown("0"))
                    {
                        index++;
                        if (index > macroBounds.Count)
                        index = 0;
                        Milo.VisualizeBounds(macroBounds[index], Color.green, Vector3.up, 5);
                        Milo.DrawNormalDirection(macroBounds[index], 5);
                        Milo.ShowSmallestMagnitude(macroBounds[index], 5);
                    }

                    if (Input.GetKeyDown("space"))
                    {
                        Milo.VisualizeBounds(macroBounds[index], Color.green, Vector3.up, 5);
                        Milo.DrawNormalDirection(macroBounds[index], 5);
                        Milo.ShowSmallestMagnitude(macroBounds[index], 5);
                    }
                }

                void Start()
                {
                    Hudson.OffsetPreTest();
                    islandGenerator = GetComponent<HexGen>();// original plan was to make the island with hex's but decided to go with squares
                    List<LineSegment> islandBounds = islandGenerator.islandBounds;
                    List<LineSegment> exBounds = createInitBounds();
                    macroBounds = GenerateFractalBlocks(islandBounds, exBounds, gridSize, ref testLineBounds);

                    List<List<List<LineSegment>>> microBounds = new List<List<List<LineSegment>>>();

                    for (int i = 0; i < macroBounds.Count; i++)
                    {
                        //List<LineSegment> eB = Milo.GenerateExBounds(macroBounds[i]);
                        //below should updated exBounds objects, just have to fix that function
                        microBounds.Add(GenerateFractalBlocks(macroBounds[i], exBounds, blockSize, ref testLineBounds));
                    }

                    Debug.Log("test");
                    currentLine = microLines[0];
                    macroBounds = Milo.ConcatenateBlocks(microBounds);
                    for (int i = 0; i < macroBounds.Count; i++)
                    {
                        macroBounds[i] = Milo.ReOrderLineList(macroBounds[i], Mathf.Sqrt(blockSize + blockSize));
                        float p = Milo.GetPerimeter(macroBounds[i]);
                        if (p < 2f)
                        {
                            Milo.VisualizeBounds(macroBounds[i], Color.magenta, Vector3.up);
                        }
                        if (p > 2f && p < 5f)
                        {
                            Milo.VisualizeBounds(macroBounds[i], Color.green, Vector3.up);

                        }
                    }
                    //Milo.VisualizeBlocks(macroBounds);
                }
            }
