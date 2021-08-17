using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    private Vector2 move;

    public float screenWidth;
    public float screenHeight;
    private List<Transform> segments;
    public Transform snakeSegmentPrefab;
    public GameObject top, bottom, left, right;
    public Camera _camera;
    public GameObject Food;
    public TextMeshProUGUI scoreText;
    private int score = 0;
    public GameObject massGainerFood;
    public GameObject shield;
    public GameObject speedUp;
    public GameObject massBurner;

    public List<int> demo;


    private void Start()
    {
        segments = new List<Transform>();
        segments.Add(this.transform);
        OrthographicBound(_camera);
        GetFoodAtRandomPlace(Food);

        //InvokeRepeating("GetMassGainerFoodAtRandomPlace", spawnTime, spawnDelay);

        RefreshUI();
        GetMassGainerFoodAtRandomPlace();
        GetShieldAtRandomPlace();
        GetSpeedUpAtRandomPlace();
        GetMassBurnerAtRandomPlace();
    }
    //for MassBurner;
    private void GetMassBurnerAtRandomPlace()
    {
        StartCoroutine(GenerateMassBurner());
    }
    IEnumerator GenerateMassBurner()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(50,70));
        massBurner.transform.position = new Vector3(
        UnityEngine.Random.Range(left.transform.position.x + 1, right.transform.position.x - 1),
        UnityEngine.Random.Range(top.transform.position.y - 1, bottom.transform.position.y + 1),
        0.0f);
        Instantiate(massBurner, massBurner.transform.position, Quaternion.identity);
    }

    //for SpeedUp;
    private void GetSpeedUpAtRandomPlace()
    {
        StartCoroutine(GenerateSpeedUp());
    }

    IEnumerator GenerateSpeedUp()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0,40));
        speedUp.transform.position = new Vector3(
        UnityEngine.Random.Range(left.transform.position.x + 1, right.transform.position.x - 1),
        UnityEngine.Random.Range(top.transform.position.y - 1, bottom.transform.position.y + 1),
        0.0f);
        Instantiate(speedUp, speedUp.transform.position, Quaternion.identity);

    }

    //for shield;
    private void GetShieldAtRandomPlace()
    {
        StartCoroutine(GenerateShield());
    }

    IEnumerator GenerateShield()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(50,70));
        shield.transform.position = new Vector3(
        UnityEngine.Random.Range(left.transform.position.x + 1, right.transform.position.x - 1),
        UnityEngine.Random.Range(top.transform.position.y - 1, bottom.transform.position.y + 1),
        0.0f);
        Instantiate(shield, shield.transform.position, Quaternion.identity);

       
     }

    //for massgainer;
    private void GetMassGainerFoodAtRandomPlace()
    {
        StartCoroutine(GenerateMassFood());

    }

    IEnumerator GenerateMassFood()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(10,40));

        massGainerFood.transform.position = new Vector3(
         UnityEngine.Random.Range(left.transform.position.x + 1, right.transform.position.x - 1),
         UnityEngine.Random.Range(top.transform.position.y - 1, bottom.transform.position.y + 1),
         0.0f);
        Instantiate(massGainerFood, massGainerFood.transform.position, Quaternion.identity);



    }
    //for food;
    private void GetFoodAtRandomPlace(GameObject gameObject)
    {
        gameObject.transform.position = new Vector3(
            UnityEngine.Random.Range(left.transform.position.x+1, right.transform.position.x-1),
            UnityEngine.Random.Range(top.transform.position.y-1, bottom.transform.position.y+1),
            0.0f
            );
        Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            move = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            move = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move = Vector2.right;
        }

        // Debug.Log("Update x= " + gameObject.transform.position.x.ToString());
    }

   private void FixedUpdate()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            if (segments[i] != null)
            {
                segments[i].position = segments[i - 1].position;
            }
        }

        this.transform.position = new Vector3(
        Mathf.Round(this.transform.position.x + move.x),
        Mathf.Round(this.transform.position.y + move.y), 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTrigger x= " + this.transform.position.x.ToString());


        // For Food Grow;
        if (collision.CompareTag("Food"))
        {
            Grow();
            Destroy(collision.gameObject);
            GetFoodAtRandomPlace(Food);
            IncreaseScore(10);
        }

        else if (collision.CompareTag("wall"))
        {
            ChangeDirection();
        }

        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle hit");
            Restart();
        }
        //for 2xScore;
        if (collision.CompareTag("MassGainerFood"))
        {
            Debug.Log("MassGainerCollided");
            IncreaseScore(20);
            RefreshUI();
            Destroy(collision.gameObject);
            StartCoroutine(GenerateMassFood());

        }
        //for Shield;
        if (collision.CompareTag("Shield"))
        {
            Debug.Log("ShieldCollided");
            Destroy(collision.gameObject);
            StartCoroutine(Wait());
            
        }
        //for speedup;
        if (collision.CompareTag("SpeedUp"))
        {
            Debug.Log("SpeedUpCollided");
            Destroy(collision.gameObject);
            //StartCoroutine(Wait());
        }
        //for MassBurner;
        if (collision.CompareTag("MassBurner"))
        {
            Destroy(collision.gameObject);
            if(segments.Count>1)
            { 
                Destroy(segments[segments.Count - 1].gameObject);
                segments.RemoveAt(segments.Count - 1);

                Debug.Log("Count of segment is in MassBurner" + segments.Count);
                IncreaseScore(-10);
                RefreshUI();
                StartCoroutine(GenerateMassBurner());
            }
            
            
        }

    }

    IEnumerator Wait()
    {
        SegmentCollisionSwitch(true);
        yield return new WaitForSeconds(15);
        SegmentCollisionSwitch(false);
        StartCoroutine(GenerateShield());
    }

    private void SegmentCollisionSwitch(bool flag)
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Physics2D.IgnoreCollision(segments[i].GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), flag);
            
        }
    }

    public void IncreaseScore(int increment)
    {
        score = score + increment;
        RefreshUI();
    }

    private void RefreshUI()
    {
        scoreText.text = "Score: " + score;
    }

    private void Restart()
    {
        for (int i = 1;i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
        segments.Add(this.transform);
        score = 0;
        RefreshUI();
    }

    private void ChangeDirection()
    {
        if (move == Vector2.left)
        {
            Debug.Log("LeftWall");
            this.transform.position = new Vector3(
                Mathf.Round(right.transform.position.x) + move.x,
                this.transform.position.y,
                0.0f);
        }
        if (move == Vector2.right)
        {
            Debug.Log("RightWall");
            this.transform.position = new Vector3(
                 Mathf.Round(left.transform.position.x) + move.x,
                 this.transform.position.y,
                 0.0f);
        }
        if (move == Vector2.up)
        {
            Debug.Log("TopWall");
            this.transform.position = new Vector3(
                 this.transform.position.x,
                 Mathf.Round(bottom.transform.position.y) + move.y,
                 0.0f);
        }
        if (move == Vector2.down)
        {
            Debug.Log("BottomWall");
            this.transform.position = new Vector3(
                this.transform.position.x,
                Mathf.Round(top.transform.position.y) + move.y,
                0.0f);
        }
    }



    private void Grow()
    {
      
        {
            Debug.Log("Count of segment is in grow" + segments.Count );
            Transform newSnakeSegment = Instantiate(this.snakeSegmentPrefab);
            newSnakeSegment.position = segments[segments.Count - 1].position;
            segments.Add(newSnakeSegment);
        }
        
    }

    private void OrthographicBound(Camera _camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = _camera.orthographicSize * 2;
        Bounds bounds = new Bounds(_camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

        top.transform.position = new Vector3(0, bounds.max.y, 0);
        bottom.transform.position = new Vector3(0, bounds.min.y, 0);
        left.transform.position = new Vector3(bounds.min.x, 0, 0);
        right.transform.position = new Vector3(bounds.max.x, 0, 0);
    }

}
