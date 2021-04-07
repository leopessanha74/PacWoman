using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PacWoman : MonoBehaviour
{
    [SerializeField] float speed;

    public Vector2 currentDirection = Vector2.zero;
    public Vector2 nextDirection;
    public TurningPoint currentTurningPoint, targetTurningPoint, previousTurningPoint, startingTurningPoint;
    public Mapa mapa;
    [SerializeField] Sprite pausedSprite;
    private SoundManager soundManager;
    public int score = 0;
    public Text scoreText;
    public int lives = 3;
    public Text livesText;
    bool facingWall;
    public bool canMove = true;
    public RuntimeAnimatorController pacman;
    public RuntimeAnimatorController pacManDeath;

    void Awake()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    private void Start()
    {
        if (GetTurningPointAtPosition(transform.position) != null)
        {
            currentTurningPoint = GetTurningPointAtPosition(transform.position);
        }
        startingTurningPoint = currentTurningPoint;
        previousTurningPoint = currentTurningPoint;
        currentDirection = Vector2.left;
        ChangePosition(currentDirection);
    }
    void FixedUpdate()
    {
        //scoreText.text = "Score:" + score.ToString();
        //livesText.text = "Lives:" + lives.ToString();
        if (canMove)
        {
            CheckInput();
            MovePacman();
            UpdateOrientation();
            UpdateEatingAnimation();
        }
    }

    public void Restart()
    {
        canMove = true;
        GetComponent<Animator>().runtimeAnimatorController = pacman;
        GetComponent<Animator>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true ;
        transform.position = startingTurningPoint.transform.position;
        currentTurningPoint = startingTurningPoint;
        previousTurningPoint = startingTurningPoint;
        currentDirection = Vector2.left;
        nextDirection = Vector2.left;
        ChangePosition(currentDirection);
    }
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Dots"))
        {
            score++;
            if (!soundManager.oneShotAS.isPlaying)
            {
                soundManager.PlayOneShot(soundManager.eatingDots);
            }
                         
            Destroy(_other.gameObject);
        }
        if (_other.CompareTag("Ghost"))
        {
            if (_other.GetComponent<Ghost>().currentMode == Ghost.Mode.Frightened)
            {
                soundManager.PlayOneShot(soundManager.eatingGhost);
                _other.GetComponent<Ghost>().HasBeenEaten();
                //animate ghost back to house
                //Keep score
            }
            else if(_other.GetComponent<Ghost>().currentMode != Ghost.Mode.Eaten)
            {
                mapa.StartDeath();
            }
        }
        if (_other.CompareTag("Pills"))
        {
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject go in ghosts)
            {
                go.GetComponent<Ghost>().StartFrightenedMode();
            }
            Destroy(_other.gameObject);
        }
    }
    void ChangePosition(Vector2 _direction)
    {
        //We wanna store a next direction only if it's not equal to the current direction
        if(_direction != currentDirection)
        {
            nextDirection = _direction;
        }

        if (currentTurningPoint != null)//If I am in a TurningPoint
        {
            //Check if I can move in the direction passed as an argument
            TurningPoint moveToTurningPoint = CanMove(_direction);
            if (moveToTurningPoint != null)//If I can
            {
                currentDirection = _direction;
                targetTurningPoint = moveToTurningPoint;
                previousTurningPoint = currentTurningPoint;
                currentTurningPoint = null; //I will be between nodes;
            }
        }
    }

    void MovePacman()
    {

        if(targetTurningPoint != null)
        {
            if(nextDirection == currentDirection*-1)
            {
                currentDirection *= -1;
                TurningPoint tempTurningPoint = targetTurningPoint;
                targetTurningPoint = previousTurningPoint;
                previousTurningPoint = tempTurningPoint;
            }
            


            if (ReachedTargetTurningPoint())
            {
                currentTurningPoint = targetTurningPoint;
                transform.position = currentTurningPoint.transform.position;

                GameObject destiny = GetPortal(currentTurningPoint.transform.position);
                if (destiny != null)
                {
                    transform.position = destiny.transform.position;
                    currentTurningPoint = destiny.GetComponent<TurningPoint>();
                }


                TurningPoint moveToTurningPoint = CanMove(nextDirection);
                if (moveToTurningPoint != null)//If I can go to the next direction
                {
                    //update current direction
                    currentDirection = nextDirection;
                }
                else //continue in the same direction
                {
                    moveToTurningPoint = CanMove(currentDirection);
                }

                if (moveToTurningPoint != null)//If I can go to the targetTurningPoint that is the neighbour in the next direction or the same direction
                {
                    targetTurningPoint = moveToTurningPoint;
                    previousTurningPoint = currentTurningPoint;
                    currentTurningPoint = null;
                }
                else //If I can't go
                {
                    //I will stop
                    currentDirection = Vector2.zero;
                }
            }
            else transform.position += (Vector3)currentDirection * speed * Time.deltaTime;
        }
    }
    float DistanceToPreviousTurningPoint(Vector2 _targetPosition)
    {
        Vector2 vector = _targetPosition - (Vector2)previousTurningPoint.transform.position;
        return vector.sqrMagnitude;
    }

    bool ReachedTargetTurningPoint()
    {
        float distanceToTarget = DistanceToPreviousTurningPoint(targetTurningPoint.transform.position);
        float travelledDistance = DistanceToPreviousTurningPoint(transform.position);
        return (travelledDistance > distanceToTarget);
    }
    void CheckInput()
    {
        if (Input.GetKeyDown("left"))
        {
            ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown("right"))
        {
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown("up"))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown("down"))
        {
            ChangePosition(Vector2.down);
        }
    }

    private void UpdateEatingAnimation()
    {
        if(currentDirection == Vector2.zero)
        {
            gameObject.GetComponent<Animator>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = pausedSprite;
        }else
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    TurningPoint GetTurningPointAtPosition(Vector2 _position)
    {
        GameObject gameObjectAtPosition = mapa.mapPoints[(int)_position.x, (int)_position.y];
        if (gameObjectAtPosition != null)
        {
            return mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>();
        }
        return null;
    }
    GameObject GetPortal(Vector2 _position)
    {
        TurningPoint currentTurningPoint = GetTurningPointAtPosition(_position);
        if (currentTurningPoint.isPortal)
        {
            return currentTurningPoint.destinationPortal;
        }
        return null;
    }

    void UpdateOrientation()
    {
        if (currentDirection == Vector2.left)
        {
            //left
            transform.localScale = Vector2.one;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else if (currentDirection == Vector2.right)
        {
            //right
            transform.localScale = new Vector2(-1, 1);
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else if (currentDirection == Vector2.up)
        {
            //up
            transform.localScale = new Vector2(1, -1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
        else if (currentDirection == Vector2.down)
        {
            //down
            transform.localScale = Vector2.one;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
    }

    TurningPoint CanMove(Vector2 _direction)
    {
        TurningPoint moveToTurningPoint = null;
        for (int i = 0; i < currentTurningPoint.neighborsTurningPoints.Length; i++)
        {
            if (currentTurningPoint.directionToNeighborTurningPoint[i] == _direction)
            {
                moveToTurningPoint = currentTurningPoint.neighborsTurningPoints[i];
                break;
            }
        }
        return moveToTurningPoint;
    }

}
