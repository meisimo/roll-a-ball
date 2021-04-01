using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
  static LevelManager sharedInstance;

  public List<LabyrinthFragmentController> allLabyrinthFragments;
  public LabyrinthFragmentController mainLabyrinthFargment;
  public float finallFragmentProb;
  public int finallFragmentThreshold;
  public int maxMapWidth;
  public int maxMapHegiht;

  private List<LabyrinthFragmentController> labFragments;
  private List<LabyrinthFragmentController> finalLabFragments;
  private bool lastFragmentPlaced;
  private int centerMapHeigh;
  private int centerMapWidth;
  private int fragmentsPlaced;
  private int remainingOpenFragments;

  private void Awake()
  {
    if (sharedInstance == null)
    {
      sharedInstance = this;
      lastFragmentPlaced = false;
      fragmentsPlaced    = 0;
      remainingOpenFragments = maxMapHegiht * maxMapWidth - 1;

      finalLabFragments = new List<LabyrinthFragmentController>();
      labFragments      = new List<LabyrinthFragmentController>();

      centerMapHeigh    = maxMapHegiht >> 1;
      centerMapWidth    = maxMapWidth >> 1;

      ClassifyFragmentsByOuts();
      InitializeLabyrinth();
    }
  }

  private void ClassifyFragmentsByOuts()
  {
    for(int i = 0; i < sharedInstance.allLabyrinthFragments.Count; i++)
    {
      LabyrinthFragmentController fragment = sharedInstance.allLabyrinthFragments[i];
      if (fragment.IsFinall())  finalLabFragments.Add(fragment);
      else                      labFragments.Add(fragment);
    }
  }

  private bool PlaceFinallFragment(int placedFragments, bool forceClose)
  {
    if (placedFragments < sharedInstance.finallFragmentThreshold)
      return false;
    return forceClose || UnityEngine.Random.Range(0.0f, 1.0f) < sharedInstance.finallFragmentProb;
  }

  private bool ForceClose(int remainingOpenFragments)
  {
    // TODO: 
    return false;
  }

  private List<(int y, int x, char dir)> FindNeighboors(int y, int x)
    {
      List<(int y, int x, char dir)> neighboors = new List<(int, int, char)>();

      if ( y + 1 < sharedInstance.centerMapHeigh ) neighboors.Add( (y + 1, x    , 'N') );
      if ( x + 1 < sharedInstance.centerMapWidth ) neighboors.Add( (y    , x + 1, 'E') );
      if ( -sharedInstance.centerMapHeigh < y - 1) neighboors.Add( (y - 1, x    , 'S') );
      if ( -sharedInstance.centerMapWidth < x - 1) neighboors.Add( (y    , x - 1, 'W') );

      return neighboors;
    }

  private char OpositeDir(char dir)
  {
    switch (dir)
    {
      case 'N':
        return 'S';
      case 'S':
        return 'N';
      case 'E':
        return 'W';
      case 'W':
        return 'E';
    }
    throw (new Exception("Direction no defined " + dir));
  }

  private string FindRequiredDirections(LabyrinthFragmentController[,] map,
                                        int y,
                                        int x)
  {
    string dirs = "";
    foreach((int y, int x, char dir) p in FindNeighboors(y, x))
    {
      LabyrinthFragmentController fragment = GetFragmentInMapMatrix(map, p.y, p.x);
      if (fragment != null && fragment.DirIsOpen(OpositeDir(p.dir)))
      {
        dirs = dirs + p.dir;
      }
    }
    return dirs;
  }

  private string FindForbiddenDirections( LabyrinthFragmentController[,] map,
                                          int y,
                                          int x)
  {
    LabyrinthFragmentController fragment;
    string dirs = "";

    if ( y + 1 >= sharedInstance.centerMapHeigh ) dirs += 'N';
    if ( x + 1 >= sharedInstance.centerMapWidth ) dirs += 'E';
    if ( -sharedInstance.centerMapHeigh >= y - 1) dirs += 'S';
    if ( -sharedInstance.centerMapWidth >= x - 1) dirs += 'W';

    foreach((int y, int x, char dir) p in FindNeighboors(y, x))
    {
      
      if ( 
        (fragment = GetFragmentInMapMatrix(map, p.y, p.x) )!= null &&
        !fragment.DirIsOpen(OpositeDir(p.dir))
      )
      {
        dirs = dirs + p.dir;
      }
    }

    return dirs;
  }

  private void InsertFragmentInMapMatrix( LabyrinthFragmentController[,] map,
                                          int y,
                                          int x,
                                          LabyrinthFragmentController fragment)
  {
    map[sharedInstance.centerMapHeigh + y, sharedInstance.centerMapWidth + x] = fragment;
  }

  private LabyrinthFragmentController GetFragmentInMapMatrix(LabyrinthFragmentController[,] map,
                                      int y,
                                      int x)
  {
    return map[sharedInstance.centerMapHeigh + y, sharedInstance.centerMapWidth + x];
  }

  private LabyrinthFragmentController FindRandomFragment(
    string reqDirs,
    string forbDirs,
    List<LabyrinthFragmentController> fragments,
    int fixedValue = -1)
  {
    List<LabyrinthFragmentController> fragmentsMatch = new List<LabyrinthFragmentController>();
    string allDirs;

    foreach(LabyrinthFragmentController frag in fragments)
    {
      allDirs = frag.AllAvailableDirs();

      if ( 
        LabyrinthFragmentController.DirsIntersection(reqDirs, allDirs) == reqDirs    &&
        LabyrinthFragmentController.DirsIntersection(forbDirs, allDirs).Length == 0 && 
        ( ( fixedValue < 0 ) || ( reqDirs.Length + fixedValue == allDirs.Length ) )
      )
      {
        fragmentsMatch.Add(frag);
      }
    }

    return fragmentsMatch[UnityEngine.Random.Range(0, fragmentsMatch.Count)];
  }

  private LabyrinthFragmentController GenerateRandomFragment(
    LabyrinthFragmentController[,] map,
    int y,
    int x,
    int placedFragments,
    int remainingOpenFragments,
    bool finalFragmentPlaced)
  {
    bool forceClose      = ForceClose(remainingOpenFragments);
    string requiredDirs  = FindRequiredDirections(map, y, x);
    string forbiddenDirs = FindForbiddenDirections(map, y, x);

    LabyrinthFragmentController fragment;
    if (!finalFragmentPlaced && PlaceFinallFragment(placedFragments, forceClose))
    {
      fragment = FindRandomFragment(requiredDirs, forbiddenDirs, sharedInstance.finalLabFragments);
    }
    else if (forceClose)
    {
      fragment = FindRandomFragment(requiredDirs, forbiddenDirs, sharedInstance.labFragments, 0);
    }
    else
    {
      fragment = FindRandomFragment(requiredDirs, forbiddenDirs, sharedInstance.labFragments);
    }

    return Instantiate(fragment);
  }

  private void PlaceNewFragment(LabyrinthFragmentController newFragment,
                                LabyrinthFragmentController preFragment,
                                char dir)
  {
    newFragment.transform.position = preFragment.GetOutPoint(dir) - newFragment.GetOutPoint(OpositeDir(dir));
  }

  private LabyrinthFragmentController[,] CreatEmptyMapMatrix()
  {
    return new LabyrinthFragmentController[ sharedInstance.maxMapHegiht,
                                            sharedInstance.maxMapWidth];
  }

  private LabyrinthFragmentController FirstFragment()
  {

    LabyrinthFragmentController mainFragment = Instantiate(
      sharedInstance.mainLabyrinthFargment);

    mainFragment.transform.SetParent(sharedInstance.transform, false);

    return mainFragment;
  }

  private List<(int, int, char)> GetAvailablePoints(LabyrinthFragmentController[,] map,
                                              int y, 
                                              int x)
  {
    List<(int,int,char)> points = new List<(int, int, char)>();

    foreach((int y, int x, char dir) p in FindNeighboors(y, x))
    {
      if (GetFragmentInMapMatrix(map, p.y, p.x) == null)
      {
        points.Add(p);
      }
    }

    return points;
  }

  private int CloseConectedFragments(
    LabyrinthFragmentController newFragment,
    LabyrinthFragmentController[,] map,
    int y,
    int x
  )
  {
    int closedPoints = 0;

    LabyrinthFragmentController fragment = null;

    foreach (char dir in newFragment.AllDirs())
    {
      switch (dir)
      {
        case 'N':
          fragment = GetFragmentInMapMatrix(map, y + 1, x);
          break;
        case 'S':
          fragment = GetFragmentInMapMatrix(map, y - 1, x);
          break;
        case 'E':
          fragment = GetFragmentInMapMatrix(map, y    , x + 1);
          break;
        case 'W':
          fragment = GetFragmentInMapMatrix(map, y    , x - 1);
          break;
      }

      if (fragment != null)
      {
        fragment.CloseDir(OpositeDir(dir));
        newFragment.CloseDir(dir);
        closedPoints += 2;
      }
    }
    return closedPoints;
  }

  private void CompleteFragment(LabyrinthFragmentController fragment,
                                LabyrinthFragmentController[,] map,
                                int y,
                                int x)
  {
    Debug.Log("COMPLETE FRAGMENT " + fragment.toString() + " (" + x + ", " + y + ")");
    foreach( (int y, int x, char dir) p in GetAvailablePoints(map, y, x))
    {
      if (fragment.DirIsOpen(p.dir))
      {
        LabyrinthFragmentController newFragment = GenerateRandomFragment( map,
                                                                          p.y,
                                                                          p.x,
                                                                          sharedInstance.fragmentsPlaced,
                                                                          sharedInstance.remainingOpenFragments,
                                                                          sharedInstance.lastFragmentPlaced);

        PlaceNewFragment(newFragment, fragment, p.dir);
        InsertFragmentInMapMatrix(map, p.y, p.x, newFragment);

        sharedInstance.remainingOpenFragments += newFragment.AllDirs().Length;
        sharedInstance.remainingOpenFragments -= CloseConectedFragments(newFragment, map, p.y, p.x);
        sharedInstance.fragmentsPlaced++;

        if (newFragment.IsFinall())
        {
          sharedInstance.lastFragmentPlaced = true;
        }

        CompleteFragment(newFragment, map, p.y, p.x);
      }
    }
  }

  private void InitializeLabyrinth()
  {
    LabyrinthFragmentController mainFragment = FirstFragment();
    LabyrinthFragmentController[,] map       = CreatEmptyMapMatrix();

    sharedInstance.remainingOpenFragments--;
    InsertFragmentInMapMatrix(map, 0, 0, mainFragment);
    CompleteFragment(mainFragment, map, 0, 0);
  }
}
