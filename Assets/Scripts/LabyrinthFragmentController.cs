using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthFragmentController : MonoBehaviour
{
    static string ALL_DIRS = "NESW";
    public Transform northPoint;
    public Transform southPoint;
    public Transform eastPoint;
    public Transform westPoint;
    public bool isFinall;

    private List<Transform> outPoints;
    private bool northOpen;
    private bool souhtOpen;
    private bool eastOpen;
    private bool westOpen;

    static public string DirsIntersection(string dirs1, string dirs2)
    {
      string intersection = "";

      foreach(char d1 in dirs1)
      {
        if(dirs2.Contains(d1.ToString()))
        {
          intersection += d1;
        }
      }

      return intersection;
    }
    
    private void Awake()
    {
        outPoints = new List<Transform>();
        if (northPoint != null)
        {
          outPoints.Add(northPoint);
          northOpen = true;
        }
        if (southPoint != null)
        {
          outPoints.Add(southPoint);
          souhtOpen = true;
        }
        if (eastPoint != null)
        {
          outPoints.Add(eastPoint);
          eastOpen = true;
        }
        if (westPoint != null)
        {
          outPoints.Add(westPoint);
          westOpen = true;
        }
        Debug.Log("AWAKE " + toString());
    }

    public int CountOutPoints()
    {
        return outPoints.Count;
    }

    public List<Vector3> GetOutPoints()
    {
        List<Vector3> outVecs = new List<Vector3>();

        for(int i = 0; i < CountOutPoints(); i++)
        {
            outVecs.Add(outPoints[i].transform.position);
        }

        return outVecs;
    }

    public bool IsFinall()
    {
        return isFinall;
    }

    public bool DirIsOpen(char dir)
    {
      bool isOpen;
      switch (dir)
      {
        case 'N':
          isOpen = northOpen;
          break;
        case 'S':
          isOpen = souhtOpen;
          break;
        case 'E':
          isOpen = eastOpen;
          break;
        case 'W':
          isOpen = westOpen;
          break;
        default:
          throw (new Exception("Direction no defined " + dir));
      }
      return isOpen;
    }

    public bool DirIsAvailable(char dir)
    {
      switch (dir)
      {
        case 'N':
          return northPoint != null;
        case 'S':
          return southPoint != null;
        case 'E':
          return eastPoint != null;
        case 'W':
          return westPoint != null;
      }
      throw (new Exception("Direction no defined " + dir));
    }

    public Vector3 GetOutPoint(char dir)
    {
      switch (dir)
      {
        case 'N':
          return northPoint.position;
        case 'S':
          return southPoint.position;
        case 'E':
          return eastPoint.position;
        case 'W':
          return westPoint.position;
      }
      throw (new Exception("Direction no defined " + dir));
    }

    public string AllAvailableDirs()
    {
      string dirs = "";

      foreach(char dir in ALL_DIRS)
      {
        if (DirIsAvailable(dir))
        {
          dirs = dirs + dir;
        }
      }

      return dirs;
    }

    public string AllDirs()
    {
      string dirs = "";

      foreach(char dir in ALL_DIRS)
      {
        if (DirIsOpen(dir))
        {
          dirs = dirs + dir;
        }
      }

      return dirs;
    }

    public void CloseDir(char dir)
    {
      switch (dir)
      {
        case 'N':
          if (!northOpen)
            throw (new Exception("DIRECCION YA CERRADA " + dir));
          northOpen = false;
          break;
        case 'S':
          if (!souhtOpen) throw (new Exception("DIRECCION YA CERRADA " + dir));
          souhtOpen = false;
          break;
        case 'E':
          if (!eastOpen) throw (new Exception("DIRECCION YA CERRADA " + dir));
          eastOpen = false;
          break;
        case 'W':
          if (!westOpen) throw (new Exception("DIRECCION YA CERRADA " + dir));
          westOpen = false;
          break;
      }
    }

    public string toString()
    {
      return "Fragment: " 
        + (northPoint == null ? "_" : "N")
        + (eastPoint == null ?  "_" : "E")
        + (southPoint == null ? "_" : "S")
        + (westPoint == null ?  "_" : "W");
    }
}
