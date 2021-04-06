using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
  static LevelManager sharedInstance;

  public GameObject ramps;
  public List<LabyrinthFragmentController> allLabyrinthFragments;
  public LabyrinthFragmentController mainLabyrinthFargment;
  public string nextLevelName;
  public float collectableProb;
  public int timePerCollectableReward;
  public int maxMapWidth;
  public int maxMapHegiht;
  public int hFinallLowerBound;
  public int hFinallUpperBound;
  public int vFinallLowerBound;
  public int vFinallUpperBound;
  public bool isTheFinalLevel;
  public Timer timer;
  public Collectables collectables;
  public AudioClip collectableSound;
  public AudioClip leavingRampsSound;

  private AudioSource audioSource;
  private List<LabyrinthFragmentController> labFragments;
  private List<LabyrinthFragmentController> finalLabFragments;
  private bool lastFragmentPlaced;
  private int centerMapHeigh;
  private int centerMapWidth;
  private int fragmentsPlaced;
  private int remainingOpenFragments;
  private int allCollectables;

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
      audioSource = GetComponent<AudioSource>();
    }
    timer.Init();
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
                                      int x,
                                      bool debug = false)
  {
    if (debug)
    {
      Debug.Log("DEBUG");
      Debug.Log( y + ", " + x);
      Debug.Log( (sharedInstance.centerMapHeigh + y) + ", " + sharedInstance.centerMapWidth + x);
      return null;
    }
    return map[sharedInstance.centerMapHeigh + y, sharedInstance.centerMapWidth + x];
  }

  private LabyrinthFragmentController FindRandomFragment(
    string reqDirs,
    string forbDirs,
    List<LabyrinthFragmentController> fragments,
    int minOpenDoors = -1)
  {
    LabyrinthFragmentController fragment = Instantiate(fragments[UnityEngine.Random.Range(0, fragments.Count)]);
    fragment.Init();

    string dirsOpen = reqDirs + LabyrinthFragmentController.RandomDirs(reqDirs + forbDirs, -1, minOpenDoors);

    foreach(char d in dirsOpen)
    {
      fragment.SetDoorState(d, true);
    }

    foreach (char d in forbDirs)
    {
      fragment.SetDoorState(d, false);
    }

    fragment.transform.SetParent(this.transform);
    return fragment;
  }

  private int CalculateMinOpenDoors(int y, int x)
  {
    float xP   = ((float)x)/ sharedInstance.centerMapWidth;
    float yP   = ((float)y)/ sharedInstance.centerMapHeigh;
    float minP = Mathf.Min(xP, yP);
    int minOpenDoor;

    if ( minP < 0.6)
      minOpenDoor = 1;
    else  
      minOpenDoor = 0;

    return minOpenDoor;
  }

  private LabyrinthFragmentController GenerateRandomFragment(
    LabyrinthFragmentController[,] map,
    int y,
    int x,
    int placedFragments,
    int remainingOpenFragments,
    bool finalFragmentPlaced)
  {
    string requiredDirs  = FindRequiredDirections(map, y, x);
    string forbiddenDirs = FindForbiddenDirections(map, y, x);
    int minOpenDoors     = CalculateMinOpenDoors(y, x);

    LabyrinthFragmentController fragment =  FindRandomFragment(requiredDirs, forbiddenDirs, sharedInstance.labFragments, minOpenDoors);

    if ( UnityEngine.Random.Range(0.0f, 1.0f) < sharedInstance.collectableProb)
    {
      fragment.ActivateCollectables();
    }

    if (fragment.AllDirs().Length == 0)
      throw new Exception("ALL DIRECTION EMPTY");

    return fragment;
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

    mainFragment.Init();
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

  private int X;
  private void CompleteFragment(LabyrinthFragmentController fragment,
                                LabyrinthFragmentController[,] map,
                                int y,
                                int x)
  {
    Debug.Log("COMPLETE FRAGMENT " + fragment.ToString() + " IN (" + x + ", " + y + ")");

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

  private void PlaceFinalFragment(LabyrinthFragmentController[,] map)
  {
    LabyrinthFragmentController fragmentToRemove = null;
    int x = 0, y = 0;

    while(!fragmentToRemove)
    {
      x = UnityEngine.Random.Range(sharedInstance.hFinallLowerBound, sharedInstance.hFinallUpperBound + 1);
      y = UnityEngine.Random.Range(sharedInstance.vFinallLowerBound, sharedInstance.vFinallUpperBound + 1);

      x *= UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
      y *= UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;

      fragmentToRemove = GetFragmentInMapMatrix(map, y, x);
    }

    Vector3 position     = fragmentToRemove.transform.position;
    string availableDirs = fragmentToRemove.AvailableDirs();
    string forbDirs      = "";

    Destroy(fragmentToRemove.gameObject);

    foreach(char d in LabyrinthFragmentController.ALL_DIRS)
    {
      if(!availableDirs.Contains(d.ToString()))
      {
        forbDirs += d;
      }
    }

    LabyrinthFragmentController finalFragment = FindRandomFragment(availableDirs, forbDirs, sharedInstance.finalLabFragments, 0);
    finalFragment.transform.position = fragmentToRemove.transform.position;

    InsertFragmentInMapMatrix(map, y, x, finalFragment);
  }

  private (LabyrinthFragmentController, (int, int, char)) FindBorderFragment(LabyrinthFragmentController[,] map)
  {
    LabyrinthFragmentController borderFragment = null, fragment;
    (int y, int x, char dir) borderPoint       = (0, 0, 'X');
    (int y, int x, char dir) emptyPoint        = (0, 0, 'X');
    int i, j;


    for ( i = -sharedInstance.centerMapHeigh + 1; i < sharedInstance.centerMapHeigh; i++)
      for ( j = -sharedInstance.centerMapWidth + 1; j < sharedInstance.centerMapWidth; j++)
      {
        if ( borderFragment == null && GetFragmentInMapMatrix(map, i, j) == null )
        {
          foreach((int y, int x, char dir) p in FindNeighboors(i, j))
          {
            fragment = GetFragmentInMapMatrix(map, p.y, p.x);
            if(
              fragment && 
              fragment.ThereIsADoor(OpositeDir(p.dir))
            )
            {
              borderFragment = fragment;
              borderPoint    = p;
              emptyPoint     = (i, j, p.dir);
              break;
            }
          }
        } 
      }
    
    return (borderFragment, borderPoint);
  }

  private void FillLabyrinth(LabyrinthFragmentController[,] map)
  {
    int flag = 5;
    LabyrinthFragmentController borderFragment;
    (int y, int x, char dir) borderPoint;

    (borderFragment, borderPoint) = FindBorderFragment(map);

    while( borderFragment != null)
    {
      Debug.Log(flag + " COUNT BORDER POINT AT " + borderPoint.ToString());

      sharedInstance.remainingOpenFragments++;
      borderFragment.SetDoorState(OpositeDir(borderPoint.dir), true);

      CompleteFragment(borderFragment, map, borderPoint.y, borderPoint.x);
      (borderFragment, borderPoint) = FindBorderFragment(map);
    }
  }

  private void InitializeLabyrinth()
  {
    LabyrinthFragmentController mainFragment = FirstFragment();
    LabyrinthFragmentController[,] map       = CreatEmptyMapMatrix();

    sharedInstance.remainingOpenFragments--;
    InsertFragmentInMapMatrix(map, 0, 0, mainFragment);
    CompleteFragment(mainFragment, map, 0, 0);
    FillLabyrinth(map);
    PlaceFinalFragment(map);

    collectables.Init();
    collectables.SetTotalCollectables(CountCollectables(map));
  }

  private int CountCollectables(LabyrinthFragmentController[,] map)
  {
    int count = 0;

    LabyrinthFragmentController f;
    for(int i = 0; i < sharedInstance.maxMapHegiht; i++)
      for(int j = 0; j < sharedInstance.maxMapWidth; j++)
      {
        f = map[i, j];
        if (f)
        {
          count += f.CountCollectables();
        }
      }
    
    return count;
  }

  private void print(LabyrinthFragmentController[,] map)
  {
    LabyrinthFragmentController f;
    int i, j;
    string str = "MAP!\n";

    for( i = sharedInstance.maxMapHegiht - 1; 0 <= i ; i--)
    {
      for ( j = 0; j < sharedInstance.maxMapWidth; j++)
      {
        f = map[i,j];
        if ( i == sharedInstance.centerMapHeigh && j == sharedInstance.centerMapWidth)
          str = str + " XXXX ";
        else
          str = str + (( f == null ) ? " XXXX " :  " " + f.AvailableDirs() +  " ");
      }
      str = str + "\n";
    }

    Debug.Log(str);
  }

  private void ChangeToNextLevel()
  {
    SceneManager.LoadScene(nextLevelName);
  }

  private void FinalOfTheGame()
  {
    Debug.Log("FINAL!!");
  }

  public void FinalReached()
  {
    if(isTheFinalLevel)
    {
      FinalOfTheGame();
    }
    else {
      ChangeToNextLevel();
    }
  }

  public bool HasRamps()
  {
    return ramps != null;
  }

  public void RemoveRamps()
  {
    ramps.SetActive(false);
    audioSource.PlayOneShot(leavingRampsSound, 1f);
  }

  public void Collected(GameObject collectable)
  {
    timer.IncreaseTimeOffset(-timePerCollectableReward);
    collectables.IncrementCollected();
    collectable.SetActive(false);
    audioSource.PlayOneShot(collectableSound, 1f);
  }

}
