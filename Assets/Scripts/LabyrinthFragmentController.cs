using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthFragmentController : MonoBehaviour
{
    static string ALL_DIRS = "NESW";
    public Transform northPoint;
    public Transform eastPoint;
    public Transform southPoint;
    public Transform westPoint;
    public bool isFinall;

    public GameObject northDoor;
    public GameObject eastDoor;
    public GameObject southDoor;
    public GameObject westDoor;

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

    static public string RandomDirs(
      string forbDirs,
      int suggestedMaxSize = -1 ,
      int suggestedMinSize = -1
    )
    {
      string dirs     = "";
      string randDirs = "";
      string tmpStr;

      char dir;

      int defMaxSize = ALL_DIRS.Length - forbDirs.Length;
      int maxSize    = suggestedMaxSize < 0 ? defMaxSize : Mathf.Min(defMaxSize, suggestedMaxSize);
      int minSize    = suggestedMinSize < 0 ? 0          : Mathf.Min(defMaxSize, suggestedMinSize);

      int size = UnityEngine.Random.Range( minSize, maxSize + 1);
                  

      foreach (char d in ALL_DIRS)
      {
        if ( ! forbDirs.Contains(d.ToString()) )
        {
          randDirs += d;
        }
      }


      for (int i = 0; i < size; i++)
      {
        dir = randDirs[UnityEngine.Random.Range(0, randDirs.Length)];
        dirs += dir;

        tmpStr = "";
        foreach(char d in randDirs)
        {
          if (dir != d)
          {
            tmpStr += d;
          }
        }
        randDirs = tmpStr;
      }

      return dirs;
    }
    
    public void Init()
    {
      bool thereIsADoor;
      foreach(char d in ALL_DIRS)
        if (DirIsAvailable(d))
        {
          thereIsADoor = ThereIsADoor(d);
          SetDoorState(d, !thereIsADoor, thereIsADoor);
        }
    }

    public void SetDoorState(char dir, bool open, bool thereIsADoor = true)
    {
      switch (dir)
      {
        case 'N':
          northPoint.gameObject.SetActive(open);
          northOpen  = open;
          if(thereIsADoor)
            northDoor.SetActive(!open);
          break;
        case 'S':
          southPoint.gameObject.SetActive(open);
          souhtOpen  = open;
          if(thereIsADoor)
            southDoor.SetActive(!open);
          break;
        case 'E':
          eastPoint.gameObject.SetActive(open);
          eastOpen  = open;
          if(thereIsADoor)
            eastDoor.SetActive(!open);
          break;
        case 'W':
          westPoint.gameObject.SetActive(open);
          westOpen  = open;
          if(thereIsADoor)
            westDoor.SetActive(!open);
          break;
        default:
          throw (new Exception("Direction no defined " + dir));
      }
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

    public bool ThereIsADoor(char dir)
    {
      switch (dir)
      {
        case 'N':
          return northDoor != null;
        case 'S':
          return southDoor != null;
        case 'E':
          return eastDoor != null;
        case 'W':
          return westDoor != null;
      }
      throw (new Exception("Direction no defined " + dir));
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

    public GameObject Point(char dir)
    {
      switch (dir)
      {
        case 'N':
          return northPoint ? northPoint.gameObject : null;
        case 'S':
          return southPoint ? southPoint.gameObject : null;
        case 'E':
          return eastPoint ? eastPoint.gameObject : null;
        case 'W':
          return westPoint ? westPoint.gameObject : null;
      }
      throw (new Exception("Direction no defined " + dir));
    }

    public string AvailableDirs()
    {
      string dirs = "";
      GameObject p;

      foreach(char dir in ALL_DIRS)
      {
        p = Point(dir);
        dirs += ( p && p.activeSelf) ? dir.ToString() : "X";
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
        + (northPoint != null && northPoint.gameObject.activeSelf ? "N" : "_")
        + (eastPoint != null  && eastPoint.gameObject.activeSelf  ? "E" : "_")
        + (southPoint != null && southPoint.gameObject.activeSelf ? "S" : "_")
        + (westPoint != null  && westPoint.gameObject.activeSelf  ? "W" : "_");
    }
}
