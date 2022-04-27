using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public int width, height;
    public GameObject block;
    GameObject head;
    public Material headMaterial, tailMaterial, borderMaterial, foodMaterial;
    List<GameObject> tail;
    Vector2 direction;
    float passedTime, timeBetweenMovements;
    public GameObject gameOverUI;
    bool top = true, down = true, left = false, right = true;
   
    void Start()
    {
        timeBetweenMovements = 0.5f;
        direction = Vector2.right;
        createGrid();
        newPlayer();
        spawnFood();
        block.SetActive(false);
        isAlive = true;
    }

    private Vector2 getRandomPosition()
    {
        //
        return new Vector2(Random.Range(-width/2+1, width/2), Random.Range(-height/2+1, height/2));
    }

    private bool isEaten(Vector2 spawnPosition)
    {
        bool isInHead = spawnPosition.x == head.transform.position.x && spawnPosition.y == head.transform.position.y;
        bool isInTail = false;
        foreach (var tailPart in tail)
        {
            if (tailPart.transform.position.x == spawnPosition.x && tailPart.transform.position.y == spawnPosition.y)
            {
                isInTail = true;
            }
        }
        //return true either food is in head (when tail is empty) or in tail
        return isInHead || isInTail;
    }
    GameObject food;
    bool isAlive;

    private void spawnFood()
    {
        Vector2 foodSpawnPos = getRandomPosition();
        while (isEaten(foodSpawnPos))
        {
            foodSpawnPos = getRandomPosition();
        }
        food = Instantiate(block);
        food.transform.position = new Vector3(foodSpawnPos.x, foodSpawnPos.y, 0);
        food.SetActive(true);
        food.GetComponent<MeshRenderer>().material = foodMaterial;
    }

    private void newPlayer()
    {
        head = Instantiate(block) as GameObject;
        head.GetComponent<MeshRenderer>().material = headMaterial;
        tail = new List<GameObject>();
    }

    private void createGrid()
    {
        for (int x = 0; x <= width; x++)
        {
            //creating top and bottom border from square blocks
            GameObject borderBottom = Instantiate(block) as GameObject;
            borderBottom.GetComponent<Transform>().position = new Vector3(x-(width/2), -height/2, 0);
            borderBottom.GetComponent<MeshRenderer>().material = borderMaterial;

            GameObject borderTop = Instantiate(block) as GameObject;
            borderTop.GetComponent<Transform>().position = new Vector3(x-(width/2), height-(height/2), 0);
            borderTop.GetComponent<MeshRenderer>().material = borderMaterial;
        }

        for (int y = 0; y <= height; y++)
        {
            //creating left and right border from squre blocks
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-width/2, y-(height/2), 0);
            borderRight.GetComponent<MeshRenderer>().material = borderMaterial;

            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(width-(width/2), y-(height/2), 0);
            borderLeft.GetComponent<MeshRenderer>().material = borderMaterial;
        }
    }

    

    private void gameOver()
    {
        isAlive = false;
        gameOverUI.SetActive(true);
    }

    public void restart()
    {
        //Restart game after hitting "restart" button
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement - boolean is added to prevent going in opposite direction (which would trigger instant game over)
        if (Input.GetKey(KeyCode.DownArrow) && down == true)
        {
            direction = Vector2.down;
            down = true; top = false; right = true; left = true;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && top == true)
        {
            direction = Vector2.up;
            down = false; top = true; right = true; left = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && right == true)
        {
            direction = Vector2.right;
            down = true; top = true; right = true; left = false;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && left == true)
        {
            direction = Vector2.left;
            down = true; top = true; right = false; left = true;
        }

        passedTime += Time.deltaTime;
        if (timeBetweenMovements < passedTime && isAlive)
        {
            passedTime = 0;
            // Move
            Vector3 newPosition = head.GetComponent<Transform>().position + new Vector3(direction.x, direction.y, 0);

            // Game ends if snake collides with border
            if (newPosition.x >= width/2 || newPosition.x <= -width/2 || newPosition.y >= height/2 || newPosition.y <= -height/2)
            {
                gameOver();
            }

            // Game ends if snake collides with itself
            foreach (var item in tail)
            {
                if (item.transform.position == newPosition)
                {
                    gameOver();
                }
            }
            if (newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y)
            {
                //Snake growing bigger after eating food
                GameObject newTile = Instantiate(block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                head.GetComponent<MeshRenderer>().material = tailMaterial;
                tail.Add(head);
                head = newTile;
                head.GetComponent<MeshRenderer>().material = headMaterial;
                spawnFood();
            }
            else
            {
                if (tail.Count == 0)
                {
                    head.transform.position = newPosition;
                }
                else
                { 
                    head.GetComponent<MeshRenderer>().material = tailMaterial;
                    tail.Add(head);
                    head = tail[0];
                    head.GetComponent<MeshRenderer>().material = headMaterial;
                    tail.RemoveAt(0);
                    head.transform.position = newPosition;
                }
            }

        }

    }
}