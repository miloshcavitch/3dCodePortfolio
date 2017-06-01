//this file contains public classes and functions to be used for all aspects of the project.
//functions are mostly geometrical for manipulation and creation of line segments and are contained in the 'Milo' class
//please ignore the fact that alot of this could have been done with rays, I began this project before I knew about them and stuck with what I had already made
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class IntFloat
{
    public int integer;
    public float floating;
    public int isEnd;
    public IntFloat(int i, float f, int e)
    {
        integer = i;
        floating = f;
        isEnd = e;
    }
}
public class IntersectionIndex
{
    public Vector3 position;
    public int xyb;
    public int index;
    public int isEnd;
    public IntersectionIndex(Vector3 p, int x, int i, int e)
    {
        position = p;
        xyb = x;
        isEnd = e;
        index = i;
    }

}
public class Connection
{
    public int index;
    public int xyb;
    public int isEnd;

    public Connection(int i, int x, int e)
    {
        index = i;
        xyb = x;
        isEnd = e;
    }
}
public class LineSegment
{
    public Vector3[] points = new Vector3[2];
    public List<List<Connection>> connections = new List<List<Connection>>();
    bool isBorder = false;
    public int xyb;
    public List<int> useList = new List<int>();
    public int useCount = 0;
    public int testThisIndex;
    public List<Vector3> fixIntPos = new List<Vector3>();

    public LineSegment(Vector3 start, Vector3 end)
    {
        points[0] = start;
        points[1] = end;
        connections.Add(new List<Connection>());
        connections.Add(new List<Connection>());

    }
    public LineSegment(Vector3 start, Vector3 end, int x)
    {
        points[0] = start;
        points[1] = end;
        xyb = x;
        connections.Add(new List<Connection>());
        connections.Add(new List<Connection>());

    }


}

public class Milo
{

    static public float Hypot(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(((a.x - b.x) * (a.x - b.x)) + ((a.z - b.z) * (a.z - b.z)));
    }
    static public float GetAngle(Vector3 start, Vector3 end)
    {
        float unfixedSlope = (start.z - end.z) / (start.x - end.x);
        float angle = Mathf.Abs(Mathf.Atan(unfixedSlope));

        if (Mathf.Approximately(angle, Mathf.PI / 2) || Mathf.Approximately(angle, 0))
        {
            if (start.x == end.x)
            {
                if (start.z > end.z)
                {
                    angle = Mathf.PI * 1.5f;
                }
                else
                {
                    angle = Mathf.PI / 2;
                }
            }
            else if (start.z == end.z)
            {
                if (start.x < end.x)
                {
                    angle = 0;
                }
                else
                {
                    angle = Mathf.PI;
                }
            }
        }
        else
        {
            if ((start.x > end.x && start.z < end.z))
            {

                angle += Mathf.PI / 2;
            }
            else if ((start.x > end.x && start.z > end.z))
            {

                angle += Mathf.PI;
            }
            else if ((start.x < end.x && start.z > end.z))
            {

                angle += Mathf.PI * 1.5f;
            }
            else
            {

            }
        }
        //Debug.Log (angle * 57.2958f);

        return angle;
    }

    static public Vector3 AngleProjectionFromPoint(Vector3 origin, float angle, float dist)
    {//takes a point, distance and direction and returns the point in that direction at that distance
        float rise = Mathf.Sin(angle) * dist;
        float run = Mathf.Cos(angle) * dist;
        Vector3 newPos = new Vector3(run, 0, rise) + origin;
        return newPos;
    }
    static public float GetPerp(float angle)
    {
        float perp = angle + Mathf.PI / 2;
        if (perp > Mathf.PI * 2)
        perp -= Mathf.PI * 2;
        return perp;
    }
    static public float GetFlip(float angle)
    {
        float flip = angle + Mathf.PI;
        if (flip > Mathf.PI * 2)
        flip -= Mathf.PI * 2;
        return flip;
    }
    static public LineSegment LineFromCenter(Vector3 origin, float angle, float maxDist)
    {
        Vector3 Start, Finish;
        Start = AngleProjectionFromPoint(origin, angle, maxDist);
        Finish = AngleProjectionFromPoint(origin, GetFlip(angle), maxDist);

        return new LineSegment(Start, Finish);
    }
    static public Vector3[] linesToPoints(List<LineSegment> lines)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < lines.Count; i++)
        {
            points.Add(lines[i].points[0]);
            points.Add(lines[i].points[1]);
        }
        points = points.Distinct().ToList();
        Vector3[] returnable = points.ToArray();

        return returnable;
    }
    static public float determineMaxDist(Vector3[] points)
    {
        float dist = 0f;
        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                float a = points[i].x - points[j].x;
                float b = points[i].z - points[j].z;
                float thisDist = Mathf.Sqrt((a * a) + (b * b));
                if (thisDist > dist)
                {
                    dist = thisDist;
                }
            }
        }
        return dist;
    }

    static public bool ApproximateVector(Vector3 a, Vector3 b)
    {
        if (Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.z, b.z))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    static public Vector3 MedianPoint(List<LineSegment> theseBounds)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < theseBounds.Count; i++)
        {
            points.Add(theseBounds[i].points[0]);
            points.Add(theseBounds[i].points[1]);
        }
        points.Distinct().ToList();

        points.Sort((x, y) => x.x.CompareTo(y.x));
        float ex = points[0].x + ( (points[points.Count - 1].x - points[0].x) / 2 );

        points.Sort((x, y) => x.z.CompareTo(y.z));
        float zee = points[0].z + ((points[points.Count - 1].z - points[0].z) / 2);
        Debug.Log("test");

        return new Vector3(ex, 0, zee);
    }
    static public Vector3 MedianPoint(LineSegment theseBounds)
    {
        Vector3 point = (theseBounds.points[0] + theseBounds.points[1]) / 2;
        return point;
    }
    static public void OffsetBounds(List<LineSegment> theseBounds)
    {
        List<LineSegment> offsetBounds = new List<LineSegment>();

    }
    static public IntFloat DetermineMostLeft(List<LineSegment> microGridLines, LineSegment currentLine, int isEnd)
    {
        List<IntFloat> positions = new List<IntFloat>();
        float xOne, yOne, xTwo, yTwo;
        Vector3 currentPoint = currentLine.points[isEnd];
        Vector3 otherPoint = currentLine.points[Mathf.Abs(isEnd - 1)];
        xOne = currentPoint.x;
        yOne = currentPoint.z;
        xTwo = otherPoint.x;
        yTwo = otherPoint.z;

        for (int i = 0; i < currentLine.connections[isEnd].Count; i++)
        {//d=(x−x1)(y2−y1)−(y−y1)(x2−x1)
            float x, y;
            x = microGridLines[currentLine.connections[isEnd][i].index].points[ Mathf.Abs(currentLine.connections[isEnd][i].isEnd - 1) ].x;
            y = microGridLines[currentLine.connections[isEnd][i].index].points[ Mathf.Abs(currentLine.connections[isEnd][i].isEnd - 1) ].z;

            float d = (x - xOne ) * ( yTwo - yOne ) - ( y - yOne ) * ( xTwo - xOne );//if negative its on left side
            positions.Add(new IntFloat(currentLine.connections[isEnd][i].index, d, currentLine.connections[isEnd][i].isEnd));
        }
        positions.Sort((x, y) => x.floating.CompareTo(y.floating));
        Debug.Log("test");
        return positions[0];
    }

    static public IntFloat MostLeftCaseThreeBug(List<LineSegment> microGridLines, LineSegment currentLine, int isEnd)
    {
        List<Connection> leftoverChoices = new List<Connection>();
        for (int i = 0; i < currentLine.connections[isEnd].Count; i++)
        {
            int ray = currentLine.connections[isEnd][i].isEnd;
            if (microGridLines[currentLine.connections[isEnd][i].index].connections[ray] == microGridLines[currentLine.connections[isEnd][i].index].connections[Mathf.Abs(ray - 1)] || microGridLines[currentLine.connections[isEnd][i].index].points[0] == microGridLines[currentLine.connections[isEnd][i].index].points[1])
            {
                Debug.Log("test");

                } else
                {
                    leftoverChoices.Add(currentLine.connections[isEnd][i]);
                }
            }
            Debug.Log("test");
            return DetermineMostLeft(microGridLines, currentLine, isEnd);
        }
        static public Vector3 IntersectingPoint(LineSegment lineOne, LineSegment lineTwo)
        {//returns 1 if segments paralel, 2 if dont intersect
            float denominator, a, b, numerator1, numerator2;
            denominator = ((lineTwo.points[1].z - lineTwo.points[0].z) * (lineOne.points[1].x - lineOne.points[0].x)) - ((lineTwo.points[1].x - lineTwo.points[0].x) * (lineOne.points[1].z - lineOne.points[0].z));
            if (denominator == 0)
            {
                return new Vector3(0, 1, 0);//parallel
            }
            a = lineOne.points[0].z - lineTwo.points[0].z;
            b = lineOne.points[0].x - lineTwo.points[0].x;
            numerator1 = ((lineTwo.points[1].x - lineTwo.points[0].x) * a) - ((lineTwo.points[1].z - lineTwo.points[0].z) * b);
            numerator2 = ((lineOne.points[1].x - lineOne.points[0].x) * a) - ((lineOne.points[1].z - lineOne.points[0].z) * b);
            a = numerator1 / denominator;
            b = numerator2 / denominator;


            float x = lineOne.points[0].x + (a * (lineOne.points[1].x - lineOne.points[0].x));
            float y = lineOne.points[0].z + (a * (lineOne.points[1].z - lineOne.points[0].z));

            if (a >= 0 && a <= 1 && b >= 0 && b <= 1)
            {
                return new Vector3(x, 0, y);
            }
            else
            {
                return new Vector3(0, 2, 0);
            }
        }

        static public void LinePointsToSegments(List<Vector3> points, LineSegment fullLine, ref List<IntersectionIndex> previousIntersections, ref int nextIndex, ref List<LineSegment> listAdd, int xyb)
        {
            int lineCount = 0;
            Color[] colorRay = new Color[5];
            colorRay[0] = Color.green;
            colorRay[1] = Color.red;
            colorRay[2] = Color.black;
            colorRay[3] = Color.yellow;
            colorRay[4] = Color.magenta;
            int colorIndex = 0;
            points.Add(fullLine.points[0]);
            points.Add(fullLine.points[1]);
            points = points.Distinct().ToList();
            points.Sort((x, y) => -(Hypot(x, fullLine.points[0]).CompareTo(Hypot(y, fullLine.points[0]))));
            List<LineSegment> segments = new List<LineSegment>();
            for (int i = 0; i < points.Count - 1; i++)
            {

                LineSegment newLine = new LineSegment(points[i], points[i + 1], xyb);
                if (newLine.points[0] != newLine.points[1])
                {

                    for (int s = 0; s < segments.Count; s++)
                    {
                        if ( (segments[s].points[0] == newLine.points[0] && segments[s].points[1] == newLine.points[1]) || (segments[s].points[0] == newLine.points[1] && segments[s].points[1] == newLine.points[0]))
                        {
                            Debug.Log("test");
                        }
                    }

                    newLine = AddConnections(ref listAdd, newLine, ref previousIntersections, nextIndex);
                    nextIndex++;
                    listAdd.Add(newLine);
                    segments.Add(newLine);


                    //add to list
                    //increase index
                }

                lineCount++;
                //Debug.DrawLine(points[i], points[i + 1], colorRay[colorIndex % colorRay.Length], 2000, true);
                //Debug.DrawLine(newLine.points[0], newLine.points[1], colorRay[colorIndex % colorRay.Length], 20000, true);
                colorIndex++;
            }
            //Debug.Log(lineCount);
        }

        static public LineSegment AddConnections(ref List<LineSegment> lines, LineSegment thisLine, ref List<IntersectionIndex> intersections, int thisIndex)
        {
            int interCount = intersections.Count;
            intersections.Add(new IntersectionIndex(thisLine.points[0], thisLine.xyb, thisIndex, 0));
            intersections.Add(new IntersectionIndex(thisLine.points[1], thisLine.xyb, thisIndex, 1));
            for (int i = 0; i < interCount; i++)
            {
                if (thisLine.points[0] == intersections[i].position && thisIndex != intersections[i].index)
                {
                    thisLine.connections[0].Add(new Connection(intersections[i].index, intersections[i].xyb, intersections[i].isEnd));
                    lines[intersections[i].index].connections[intersections[i].isEnd].Add(new Connection(thisIndex, thisLine.xyb, 0));
                }
                if (thisLine.points[1] == intersections[i].position && thisIndex != intersections[i].index)
                {
                    thisLine.connections[1].Add(new Connection(intersections[i].index, intersections[i].xyb, intersections[i].isEnd));
                    lines[intersections[i].index].connections[intersections[i].isEnd].Add(new Connection(thisIndex, thisLine.xyb, 1));
                }
            }
            return thisLine;
        }

        static public void RemoveConnections(ref List<LineSegment> lines, int removeIndex, List<Connection> connections)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                Debug.Log("test");
                lines[connections[i].index].connections[connections[i].isEnd] = lines[connections[i].index].connections[connections[i].isEnd].Where(x => x.index != removeIndex).ToList();
                Debug.Log("test");
            }
        }
        static public LineSegment ExtendLine(LineSegment line, float dist)
        {

            LineSegment newLine = line;

            LineSegment bugLine = newLine;

            // Debug.DrawLine(bugLine.points[0] - Vector3.up, bugLine.points[1] - Vector3.up, Color.green, 20000, true);

            Vector3 bb = bugLine.points[0] - bugLine.points[1];
            bb = bb.normalized;
            float mag = bugLine.points[0].magnitude - bugLine.points[1].magnitude;
            Vector3 first, second;
            first = bugLine.points[0] + (bb * 0.1f);
            second = bugLine.points[1] - ( bb * 0.1f);
            //Debug.DrawLine(first - Vector3.up, second - Vector3.up, Color.green, 20000, true);
            //bugLine.points[0] = first;
            //bugLine.points[1] = second;
            return newLine;
        }

        static public List<LineSegment> GenerateExBounds(List<LineSegment> bounds)//MUST FIX THIS FUNCTION!!
        {
            List<LineSegment> exBounds = new List<LineSegment>();
            float leftMost, rightMost, topMost, bottomMost;
            leftMost = rightMost = bounds[0].points[0].x;
            topMost = bottomMost = bounds[0].points[0].z;
            for (int i = 0; i < bounds.Count; i++)
            {
                if (bounds[i].points[0].x < leftMost)
                leftMost = bounds[i].points[0].x;
                if (bounds[i].points[1].x < leftMost)
                leftMost = bounds[i].points[0].x;

                if (bounds[i].points[0].x > rightMost)
                leftMost = bounds[i].points[0].x;
                if (bounds[i].points[1].x > rightMost)
                leftMost = bounds[i].points[0].x;

                if (bounds[i].points[0].z < bottomMost)
                leftMost = bounds[i].points[0].z;
                if (bounds[i].points[1].z < bottomMost)
                leftMost = bounds[i].points[0].z;

                if (bounds[i].points[0].z > topMost)
                leftMost = bounds[i].points[0].z;
                if (bounds[i].points[1].z > topMost)
                leftMost = bounds[i].points[0].z;
            }

            exBounds.Add(new LineSegment(new Vector3(leftMost, 0, bottomMost), new Vector3(leftMost, 0, topMost)));
            exBounds.Add(new LineSegment(new Vector3(rightMost, 0, bottomMost), new Vector3(rightMost, 0, topMost)));
            exBounds.Add(new LineSegment(new Vector3(leftMost, 0, topMost), new Vector3(rightMost, 0, topMost)));
            exBounds.Add(new LineSegment(new Vector3(leftMost, 0, bottomMost), new Vector3(rightMost, 0, topMost)));
            //test
            for (int i = 0; i < exBounds.Count; i++)
            {
                Debug.DrawLine(exBounds[i].points[0], exBounds[i].points[1], Color.green, 20000, true);
            }
            return exBounds;
        }

        static public void VisualizeBounds(List<LineSegment> bounds, Color thisColor, Vector3 vOffset)
        {

            for (int i = 0; i < bounds.Count; i++)
            {
                Debug.DrawLine(bounds[i].points[0] + vOffset, bounds[i].points[1] + vOffset, thisColor, 20000, true);
            }
        }

        static public void VisualizeBounds(List<LineSegment> bounds, Color thisColor, Vector3 vOffset, int secs)
        {

            for (int i = 0; i < bounds.Count; i++)
            {
                Debug.DrawLine(bounds[i].points[0] + vOffset, bounds[i].points[1] + vOffset, thisColor, secs, true);
            }
        }

        static public List<List<LineSegment>> RefreshedBounds(List<List<LineSegment>> roughBounds)
        {
            /*
            * how fresh do i need this to be? will xyb == 2 and the point info be enough?
            * should i create a continuous line? maybe later
            * for the sake of speed i could consilidate lines
            */
            List<List<LineSegment>> freshBounds = new List<List<LineSegment>>();

            for (int i = 0; i < roughBounds.Count; i++)
            {
                List<LineSegment> fBound = new List<LineSegment>();
                for (int j = 0; j < roughBounds[i].Count; j++)
                {
                    if (roughBounds[i][j].points[0] != roughBounds[i][j].points[1])
                    {
                        fBound.Add(new LineSegment(roughBounds[i][j].points[0], roughBounds[i][j].points[1], 2));
                    }

                }
                freshBounds.Add(fBound);
            }
            return freshBounds;
        }

        static public List<LineSegment> ReOrderLineList(List<LineSegment> unList, float maxDist)
        {
            //first reorder the list
            LineSegment startLine = unList[0];
            LineSegment finishLine = unList[unList.Count - 1];
            bool mustFlipStart = false;
            if (startLine.points[1] == finishLine.points[0] || startLine.points[1] == finishLine.points[1])
            {
                mustFlipStart = true;
            }
            if (mustFlipStart)
            {
                Vector3 newEnd = unList[0].points[0];
                unList[0].points[0] = unList[0].points[1];
                unList[0].points[1] = newEnd;
            }
            for (int i = 0; i < unList.Count - 1; i++)
            {
                if (unList[i].points[1] != unList[i + 1].points[0])
                {
                    Vector3 newEnd = unList[i + 1].points[0];
                    unList[i + 1].points[0] = unList[i + 1].points[1];
                    unList[i + 1].points[1] = newEnd;
                }
            }

            Debug.Log("test");

            //now determine bottom right most point
            IntFloat smallestMag = new IntFloat(0, 0f, 0);
            for (int i = 0; i < unList.Count; i++)
            {
                if (unList[i].points[0].magnitude < unList[smallestMag.integer].points[smallestMag.isEnd].magnitude)
                {
                    smallestMag = new IntFloat(i, 0f, 0);
                }

                if (unList[i].points[1].magnitude < unList[smallestMag.integer].points[smallestMag.isEnd].magnitude)
                {
                    smallestMag = new IntFloat(i, 0f, 1);
                }
            }

            //and find the line that shares that point too
            IntFloat sharedMag;
            for ( int i = 0; i < unList.Count; i++)
            {
                if (i != smallestMag.integer)
                {
                    if (unList[smallestMag.integer].points[smallestMag.isEnd] == unList[i].points[0])
                    {
                        sharedMag = new IntFloat(i, 0f, 0);
                    }
                    if (unList[smallestMag.integer].points[smallestMag.isEnd] == unList[i].points[1])
                    {
                        sharedMag = new IntFloat(i, 0f, 1);
                    }
                }
            }
            Debug.Log("test");
            //now determine which way is "out"


            return unList;
        }
        static public List<LineSegment> FlipLineList(List<LineSegment> bounds)
        {
            for ( int i = 0; i < bounds.Count; i++)
            {
                Vector3 newEnd = bounds[i].points[0];
                bounds[i].points[0] = bounds[i].points[1];
                bounds[i].points[1] = newEnd;
            }

            return bounds;
        }
        static public void DrawNormalDirection(List<LineSegment> bounds, int sex)
        {
            for (int i = 0; i < bounds.Count; i++)
            {
                Vector3 lineMedian = Milo.MedianPoint(bounds[i]);
                Vector3 one, two;
                one = bounds[i].points[0] - lineMedian;
                two = bounds[i].points[1] - lineMedian;
                Vector3 three = two;
                three.x = two.z * -1;
                three.z = two.x;
                three.Normalize();
                Debug.DrawLine(lineMedian + Vector3.up, lineMedian + three + Vector3.up, Color.white, sex, true);
            }
        }
        static public void VisualizeBlocks(List<List<LineSegment>> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                Vector3 median = MedianPoint(blocks[i]);
                Color thisColor = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
                for (int j = 0; j < blocks[i].Count; j++)
                {
                    Vector3 one, two;
                    one = Milo.AngleProjectionFromPoint(blocks[i][j].points[0], Milo.GetAngle(blocks[i][j].points[0], median), 0.2f);
                    two = Milo.AngleProjectionFromPoint(blocks[i][j].points[1], Milo.GetAngle(blocks[i][j].points[1], median), 0.2f);
                    Debug.DrawLine(one, two, thisColor, 20000, true);
                }
            }
        }
        static public List<List<LineSegment>> ConcatenateBlocks(List<List<List<LineSegment>>> blocks)
        {
            List<List<LineSegment>> newList = new List<List<LineSegment>>();
            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks[i].Count; j++)
                {
                    newList.Add(blocks[i][j]);
                }
            }
            return newList;
        }
        static public void ShowSmallestMagnitude(List<LineSegment> bounds, int sex)
        {
            Vector3 smallest = bounds[0].points[0];
            for (int i = 0; i < bounds.Count; i++)
            {
                if (bounds[i].points[0].magnitude < smallest.magnitude)
                smallest = bounds[i].points[0];
                if (bounds[i].points[1].magnitude < smallest.magnitude)
                smallest = bounds[i].points[1];

            }
            Debug.DrawLine(smallest, smallest + (Vector3.up * 5), Color.magenta, sex, true);
        }
        static public float GetPerimeter(List<LineSegment> bounds)
        {
            float count = 0f;
            for (int i = 0; i < bounds.Count; i++)
            {
                count += (bounds[i].points[0] - bounds[i].points[1]).magnitude;
            }
            return count;
        }
    }
