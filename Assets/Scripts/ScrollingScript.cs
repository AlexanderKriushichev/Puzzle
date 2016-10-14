using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Parallax scrolling script that should be assigned to a layer
/// </summary>
public class ScrollingScript : MonoBehaviour
{
  /// <summary>
  /// Scrolling speed
  /// </summary>
  public Vector2 speed = new Vector2(10, 10);

  /// <summary>
  /// Moving direction
  /// </summary>
  public Vector2 direction = new Vector2(-1, 0);

  /// <summary>
  /// Movement should be applied to camera
  /// </summary>
  public bool isLinkedToCamera = false;

  /// <summary>
  /// Background is inifnite
  /// </summary>
  public bool isLooping = false;

  private List<Transform> backgroundPart;
  private Vector2 repeatableSize;
  private Vector3 gap = Vector3.zero;

  void Start()
  {
    // For infinite background only
    if (isLooping)
    {
      // Get all part of the layer
      backgroundPart = new List<Transform>();

      for (int i = 0; i < transform.childCount; i++)
      {
        Transform child = transform.GetChild(i);

        // Only visible children
        if (child.GetComponent<Renderer>() != null)
        {
          backgroundPart.Add(child);
          
          // First element
          if (backgroundPart.Count == 1)
          {
            // Gap is the space between zero and the first element. 
            // We need it when we loop.
            gap = child.transform.position;
            
            if (direction.x == 0) gap.x = 0;
            if (direction.y == 0) gap.y = 0;
          }
        }
      }

      if (backgroundPart.Count == 0)
      {
        Debug.LogError("Nothing to scroll!");
      }

      // Sort by position 
      // -- Depends on the scrolling direction
      backgroundPart = backgroundPart.OrderBy(t => t.position.x * (-1 * direction.x)).ThenBy(t => t.position.y * (-1 * direction.y)).ToList();

      // Get the size of the repeatable parts
      var first = backgroundPart.First();
      var last = backgroundPart.Last();

      repeatableSize = new Vector2(
        Mathf.Abs(last.position.x - first.position.x),
        Mathf.Abs(last.position.y - first.position.y)
        );
    }
  }

  void Update()
  {
    // Movement
    Vector3 movement = new Vector3(
      speed.x * direction.x,
      speed.y * direction.y,
      0);

    movement *= Time.deltaTime;
    transform.Translate(movement);

    // Move the camera
    if (isLinkedToCamera)
    {
      Camera.main.transform.Translate(movement);
    }

    // Loop
    if (isLooping)
    {
      // Camera borders
      var dist = (transform.position - Camera.main.transform.position).z;
      float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
      float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
//      float width = Mathf.Abs(rightBorder - leftBorder);

      var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
      var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
//      float height = Mathf.Abs(topBorder - bottomBorder);

      // Determine entry and exit border using direction
      Vector3 exitBorder = Vector3.zero;
      Vector3 entryBorder = Vector3.zero;

      if (direction.x < 0)
      {
        exitBorder.x = leftBorder;
        entryBorder.x = rightBorder;
      }
      else if (direction.x > 0)
      {
        exitBorder.x = rightBorder;
        entryBorder.x = leftBorder;
      }

      if (direction.y < 0)
      {
        exitBorder.y = bottomBorder;
        entryBorder.y = topBorder;
      }
      else if (direction.y > 0)
      {
        exitBorder.y = topBorder;
        entryBorder.y = bottomBorder;
      }

      // Get the first object
      Transform firstChild = backgroundPart.FirstOrDefault();

      if (firstChild != null)
      {
        bool checkVisible = false;
        if (direction.x != 0)
        {
          if ((direction.x < 0 && (firstChild.position.x < exitBorder.x))
          || (direction.x > 0 && (firstChild.position.x > exitBorder.x)))
          {
            checkVisible = true;
          }
        }
        if (direction.y != 0)
        {
          if ((direction.y < 0 && (firstChild.position.y < exitBorder.y))
          || (direction.y > 0 && (firstChild.position.y > exitBorder.y)))
          {
            checkVisible = true;
          }
        }

        // Check if the sprite is really visible on the camera or not
        if (checkVisible)
        {
          if (firstChild.GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false)
          {
            // Set position in the end
            firstChild.position = gap; 
            // The first part become the last one
            backgroundPart.Remove(firstChild);
            backgroundPart.Add(firstChild);
          }
        }
      }

    }
  }
}
