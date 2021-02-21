using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float speed = 3.8f;
    public TurningPoint startingPosition;
    public Mapa mapa;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;
    public int chaseModeTimer4 = 20;

    int modeChangeIteration = 1;
    float modeChangeTimer = 0;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened
    }

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    GameObject pacMan;

    public TurningPoint currentTurningPoint;
    public TurningPoint targetTurningPoint;
    public TurningPoint previousTurningPoint;
    public Vector2 direction;
    public Vector2 nextDirection;

    private void Start()
    {
        direction = Vector2.right;
        pacMan = GameObject.FindGameObjectWithTag("Pacman");
        TurningPoint turningPoint = GetTurningPointAtPosition(transform.position);
        if(turningPoint != null)
        {
            currentTurningPoint = turningPoint;
        }
        previousTurningPoint = currentTurningPoint;
        targetTurningPoint = GetTurningPointAtPosition(pacMan.transform.position);// GetTurningPointAtPosition(pacMan.transform.position);
        Debug.Log("Target TurningPoint:" + targetTurningPoint);
    }
    private void Update()
    {
        ModeUpdate();
        Move();
    }
    private void Move()
    {
        if(targetTurningPoint != null && targetTurningPoint != currentTurningPoint)
        {
            if (OverShotTarget())
            {
                Debug.Log("Overshot");
                currentTurningPoint = targetTurningPoint;
                transform.position = currentTurningPoint.transform.position;

                GameObject otherPortal = GetPortal(currentTurningPoint.transform.position);
                if (otherPortal != null)
                {
                    transform.position = otherPortal.transform.position;
                    currentTurningPoint = otherPortal.GetComponent<TurningPoint>();
                }

                targetTurningPoint = ChooseNextTurningPoint();
                previousTurningPoint = currentTurningPoint;
                currentTurningPoint = null;
            }
            else
            {
                //Debug.Log(direction);
                transform.position += (Vector3) direction * speed * Time.deltaTime;
            }
        }
    }

    TurningPoint ChooseNextTurningPoint()
    {
        TurningPoint moveToTurningPoint = null;
        Vector2 pacmanPosition = pacMan.transform.position;

        if (GetTurningPointAtPosition(pacmanPosition) == null)
        {
            targetTurningPoint = pacMan.GetComponent<PacWoman>().previousTurningPoint; 
        } else moveToTurningPoint = GetTurningPointAtPosition(pacmanPosition);

        //Vector2 targetTurningPoint = Vector2.zero;
        //new Vector2((int)pacmanPosition.x, (int)pacmanPosition.y);

        int totalPossibleTurningPoints = 0;
        TurningPoint[] possibleTurningPoints = new TurningPoint[4];
        Vector2[] possibleTurningPointsDirection = new Vector2[4];

        for (int i = 0; i < currentTurningPoint.neighborsTurningPoints.Length; i++)
        {
            if (currentTurningPoint.directionToNeighborTurningPoint[i] != (direction * -1))
            {
                possibleTurningPoints[i] = currentTurningPoint.neighborsTurningPoints[i];
                possibleTurningPointsDirection[i] = currentTurningPoint.directionToNeighborTurningPoint[i];
                totalPossibleTurningPoints++;
            }
        }

        moveToTurningPoint = possibleTurningPoints[0];
        direction = possibleTurningPointsDirection[0];

        float leastDistance = 100000000000000f;

        for (int i = 0; i < possibleTurningPoints.Length; i++)
        {
            if(possibleTurningPointsDirection[i] != Vector2.zero && targetTurningPoint != null)
            {
                float distance = Vector2.Distance(possibleTurningPoints[i].transform.position, targetTurningPoint.transform.position);
                Debug.Log("Distance:" + distance);

                if(distance < leastDistance)
                {
                    leastDistance = distance;
                    moveToTurningPoint = possibleTurningPoints[i];
                    direction = possibleTurningPointsDirection[i];
                }
            }
        }
        return moveToTurningPoint;  
    }
    TurningPoint GetTurningPointAtPosition(Vector2 _position)
    {
        if(mapa.mapPoints[(int)_position.x, (int)_position.y] != null)
        {
            if (mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>() != null)
            {
                return mapa.mapPoints[(int)_position.x, (int)_position.y].GetComponent<TurningPoint>();
            }
        }
        Debug.Log("Tried to find TurningPoint");
        return null;
    }
    void ChangeMode(Mode m)
    {
        currentMode = m;
    }
    void ModeUpdate()
    {
        if(currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;
            if(modeChangeIteration == 1)
            {
                if(currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if(currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                             
                }
            }
            //2
            else if (modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                    
                }
            }
            //3
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                    
                }
            }
            //4
            if (modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }


        }
        else if (currentMode == Mode.Frightened)
        {

        }
    }
    GameObject GetPortal(Vector2 _position)
    {
        TurningPoint currentTurningPoint = GetTurningPointAtPosition(transform.position);
        if (currentTurningPoint.isPortal)
        {
            return currentTurningPoint.destinationPortal;
        }
        return null;
    }
    float LengthFromNode(Vector2 _targetPosition)
    {
        Vector2 vector = _targetPosition - (Vector2)previousTurningPoint.transform.position;
        return vector.sqrMagnitude;
    }
    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetTurningPoint.transform.position);
        float nodeToSelf = LengthFromNode(transform.position);
        return (nodeToSelf > nodeToTarget);
    }
}

/**
    // Red: 2, Pink: 5, Blue: 10, Orange: 15 
    public float startWaitTime = 0;

    // Seconds to wait after Ghost is eaten
    public float waitTimeAfterEaten = 4.0f;


    //Define starting x & y position
    // All Ys 15.5 RedX: 11.5, 12.5, 13.5, 14.5
    public float cellXPos = 0;
    public float cellYPos = 0;
    public bool isGhostBlue = false;

    // 5. Ms. Pac-Mans position
    public GameObject pacmanGO = null;
    [SerializeField] float speed;
    private Rigidbody2D rB;
    [SerializeField] Sprite left;
    [SerializeField] Sprite right;
    [SerializeField] Sprite up;
    [SerializeField] Sprite down;
    [SerializeField] Sprite blueGhost1;
    [SerializeField] Sprite blueGhost2;
    public int destinationIndex;
    Vector2 moveVector;
    SpriteRenderer sR;
    Vector2[] destinations = new Vector2[]
    {
        new Vector2(1,29),
        new Vector2(26,29),
        new Vector2(26,1),
        new Vector2(1,1),
        new Vector2(6,22)
    };
    private void Awake()
    {
        rB = gameObject.GetComponent<Rigidbody2D>();
        sR = gameObject.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartMoving", startWaitTime);

    }

    void StartMoving()
    {
        // Move Ghost from cell to starting position
        transform.position = new Vector2(14f, 19f);
        float xDestination = destinations[destinationIndex].x;
        if (xDestination < transform.position.x)
        {
            rB.velocity = Vector2.left * speed;
            this.GetComponent<SpriteRenderer>().sprite = left;
        }
        else
        {
            rB.velocity = Vector2.right * speed;
            this.GetComponent<SpriteRenderer>().sprite = right;
        }

    }

    public void ResetGhostAfterEaten(GameObject pacman)
    {
        // Move Ghosts to their cell position
        transform.position = new Vector2(cellXPos, cellYPos);
        // Stop the Ghost
        rB.velocity = Vector2.zero;
        // 5. 
        pacmanGO = pacman;
        // Starts moving Ghost after defined seconds
        Invoke("StartMoving", waitTimeAfterEaten);
    }
    
    bool CanMoveInDirection(Vector2 _direction, Vector2 _pointVect)
    {
        Vector2 ghostPoisition = transform.position;
        Transform point = GameObject.Find("Mapa").GetComponent<Mapa>().mapPoints[(int)_pointVect.x, (int)_pointVect.y];
        if (point != null)
        {
            GameObject pointGameObject = point.gameObject;
            Vector2[] vectorToNextPoint = pointGameObject.GetComponent<TurningPoint>().vectorToNextPoint;
            for (int i = 0; i < vectorToNextPoint.Length; i++)
            {
                if (vectorToNextPoint[i] == _direction)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TurningPoints"))
        {
            moveVector = GetNewDirection(collision.transform.position);
            transform.position = new Vector2((int)collision.transform.position.x, (int)collision.transform.position.y);
            // Change the sprites when turning
            if (moveVector.x != 2)
            {

                if (moveVector == Vector2.right)
                {

                    // Changes the direction of the Ghost
                    rB.velocity = moveVector * speed;

                    // Change the sprite
                    // Change if Sprite isn't blue

                    if (!isGhostBlue)
                    {
                        sR.sprite = right;
                    }

                }
                else if (moveVector == Vector2.left)
                {

                    // Changes the direction of the Ghost
                    rB.velocity = moveVector * speed;

                    // Change the sprite
                    // Change if Sprite isn't blue

                    if (!isGhostBlue)
                    {
                        sR.sprite = left;
                    }

                }
                else if (moveVector == Vector2.up)
                {

                    // Changes the direction of the Ghost
                    rB.velocity = moveVector * speed;

                    // Change the sprite
                    // Change if Sprite isn't blue

                    if (!isGhostBlue)
                    {
                        sR.sprite = up;
                    }

                }
                else if (moveVector == Vector2.down)
                {

                    // Changes the direction of the Ghost
                    rB.velocity = moveVector * speed;

                    // Change the sprite
                    // Change if Sprite isn't blue

                    if (!isGhostBlue)
                    {
                        sR.sprite = down;
                    }

                }
            }

        }

        Vector2 ghostMoveVector = new Vector2(0, 0);
        if (this.transform.position == new Vector3(26, 16, 0))
        {
            this.transform.position = new Vector3(2, 16, 0);
            rB.velocity = Vector2.right * speed;

        }
        else if (this.transform.position == new Vector3(1, 16, 0))
        {
            this.transform.position = new Vector3(25, 16, 0);
            rB.velocity = Vector2.left * speed;
        }
    }

    private Vector2 GetNewDirection(Vector2 _pointVector)
    {
        float xPosition = (float)Math.Floor(Convert.ToDouble(transform.position.x));
        float yPosition = (float)Math.Floor(Convert.ToDouble(transform.position.y));
        _pointVector.x =  (float)Math.Floor(Convert.ToDouble(_pointVector.x));
        _pointVector.y =  (float)Math.Floor(Convert.ToDouble(_pointVector.y));
        Vector2 destination = destinations[destinationIndex];
        if (pacmanGO != null)
        {
            destination = pacmanGO.transform.position;
        }
        if (_pointVector.x == destination.x && _pointVector.y == destination.y)
        {
           destinationIndex = (destinationIndex == 4) ? 0 : destinationIndex + 1;
        }
        destination = destinations[destinationIndex];
 
        Vector2 newDirection =  new Vector2(2,0);
        Vector2 previousDirection = rB.velocity.normalized;
        float destinationXDistance = destination.x - xPosition;
        float destinationYDistance = destination.y - yPosition;
        //Quadrante Superior Esquerdo
        //Se Eu Estou no Quadrante Superior Esquerdo, Distância Y Positiva e Distância X Negativa
        if (destinationYDistance > 0 && destinationXDistance < 0)
        {
            if (xPosition == 6 && yPosition == 16)
            {
                if (CanMoveInDirection(Vector2.up,_pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
            } else if (destinationYDistance > destinationXDistance )
            {
                if (CanMoveInDirection(Vector2.left,_pointVector ) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;

                }else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;

                }else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;
                }
                else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }
            } else if (destinationXDistance > destinationYDistance)
            {
                if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;
                }else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;
                }
   


            }
        }
        //Quadrante Superior Direito
        //Se Eu Estou no Quadrante Superior Direito, Distância Y Positiva e Distância X Positiva
        if (destinationYDistance > 0 && destinationXDistance > 0)
        {
            if (xPosition == 21 && yPosition == 16)
            {
                if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
            }
            else if (destinationYDistance > destinationXDistance)
            {
                if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;

                }
                else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;

                }
                else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;

                }
                else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }
            }
            else if (destinationXDistance > destinationYDistance)
            {
                if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
                else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;
                }
                else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }
                else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;
                }
            }
        }
        //Quadrante Inferior Direito
        //Se Eu Estou no Quadrante Inferior Direito, Distância Y Negativa e Distância X Positiva
        if (destinationYDistance < 0 && destinationXDistance > 0)
        {
            if (destinationYDistance > destinationXDistance)
            {
                if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;

                }
                else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;

                }
                else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;

                }
                else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
            }
            else if (destinationXDistance > destinationYDistance)
            {
                if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }
                else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;
                }
                else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
                else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;
                }
            }
        
    }
        //Quadrante Inferior Esquerdo
        //Se Eu Estou no Quadrante Inferior Esquerdo, Distância Y Negativa e Distância X Negativa
        if (destinationYDistance < 0 && destinationXDistance < 0)
        {
            if (destinationYDistance > destinationXDistance)
            {
                if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;
                }
                else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }
                else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;
                }
                else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
            }
            else if (destinationXDistance > destinationYDistance)
            {
                if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
                {
                    newDirection = Vector2.down;
                }
                else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
                {
                    newDirection = Vector2.left;
                }
                else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
                {
                    newDirection = Vector2.up;
                }
                else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
                {
                    newDirection = Vector2.right;
                }
            }
        }
        if((int)destination.y == (int)yPosition && destinationXDistance > 0)
        {
            if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
            {
                newDirection = Vector2.right;

            }
            else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
            {
                newDirection = Vector2.up;

            }
            else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
            {
                newDirection = Vector2.down;

            }
            else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
            {
                newDirection = Vector2.left;
            }
        }
        if ((int)destination.y == (int)yPosition && destinationXDistance < 0)
        {
            if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
            {
                newDirection = Vector2.left;

            }
            else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
            {
                newDirection = Vector2.up;

            }
            else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
            {
                newDirection = Vector2.down;

            }
            else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
            {
                newDirection = Vector2.right;
            }
        }
        if ((int)destination.x == (int)xPosition && destinationYDistance > 0)
        {
            if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
            {
                newDirection = Vector2.up;
            }
            else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
            {
                newDirection = Vector2.right;
            }
            else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
            {
                newDirection = Vector2.left;
            }
            else if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
            {
                newDirection = Vector2.down;
            }
        }
        if((int)destination.x == (int)xPosition && destinationYDistance < 0)
        {
            if (CanMoveInDirection(Vector2.down, _pointVector) && Vector2.down != previousDirection * -1)
            {
                newDirection = Vector2.down;
            }
            else if (CanMoveInDirection(Vector2.right, _pointVector) && Vector2.right != previousDirection * -1)
            {
                newDirection = Vector2.right;
            }
            else if (CanMoveInDirection(Vector2.left, _pointVector) && Vector2.left != previousDirection * -1)
            {
                newDirection = Vector2.left;
            }
            else if (CanMoveInDirection(Vector2.up, _pointVector) && Vector2.up != previousDirection * -1)
            {
                newDirection = Vector2.up;
            }
        }
        return newDirection;
    }

    public void TurnGhostBlue()
    {

        StartCoroutine(TurnGhostBlueAndBack());

    }

    private IEnumerator TurnGhostBlueAndBack()
    {
        // Set so that the Ghost isn't animated while blue
        isGhostBlue = true;
        // Change to the blueGhost (SET IN INSPECTOR)
        sR.sprite = blueGhost1;
        // Wait 6 seconds before changing back
        yield return new WaitForSeconds(10f);
        // Allow for the Ghost to be animated again
        isGhostBlue = false;
    }
    **/