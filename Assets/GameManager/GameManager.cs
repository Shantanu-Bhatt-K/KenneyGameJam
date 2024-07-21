using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    MenuState,
    PlayState,
    EditState,
    GameOverState
}
public class GameManager : MonoBehaviour
{
    // Waves creator and manager
    public WaveManager _waveManager;
    private bool _hasActiveWave;
    // List of active enemies
    public static List<GameObject> _enemyInformationList = new List<GameObject>();
    // time in between each calculation of nodes and enemies
    private const float WORLD_CALCULATION_INTERVAL = 1f;
    private float _calculationTimer = WORLD_CALCULATION_INTERVAL;
    public GameObject MenuScreen;
    public GameObject EditScreen;
    public GameObject GameOverScreen;
    public GameObject PlayScreen;
    // This flag will be toggled every time an enemy chooses a side
    bool branchingFlag = false;
    System.Random rand = new System.Random();

    // UI GameObjects
    public GameObject _coinNumberUI;
    public GameObject _playcoinNumberUI;
    public GameObject _enemyKilledUI;
    public GameObject _highscoreUI;

    public GameObject _projectilePrefab;

    public PlacementManager placementManager = new PlacementManager();
    public GameState currentState = GameState.MenuState;
    public List<NodeData> nodeData;
    public List<NodeClass> nodeClasses = new List<NodeClass>();
    public List<Material> materials;
    public NodeClass parentNode;
    [HideInInspector]
    public ServerNode serverNode;
    Nodetype placementNode = Nodetype.Turret;
    public List<NodeClass> entryNodes = new List<NodeClass>();


    private int _enemiesKilled = 0;

    public int gameCoins = 10000;
    public int coinsPerTick = 1;
    // Start is called before the first frame update
    void Start()
    {
        AddEntryNode();
        serverNode = new ServerNode();
        serverNode.Init(entryNodes[0], Vector3.zero);
        nodeClasses.Add(serverNode);
        Camera.main.GetComponent<CamController>().SetTarget(serverNode.model.transform);
        placementManager.serverNode = serverNode;
        placementManager.gameManager = this;
        CheckHighScore(0);
        _waveManager.OnNewWave += (object sender, EventArgs e) => { NewWave(); };
        _waveManager.OnNewEntry += (object sender, EventArgs e) => { AddEntryNode(); };
        _enemyInformationList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCoins();
        Debug.Log(currentState.ToString());
        //if (Input.GetKeyDown(KeyCode.Q) && !isEditMode)
        //{
        //    Debug.Log("Entered Edit Mode");
        //    Debug.Log("Current node" + placementNode);
        //    isEditMode = true;
        //}
        //else if (Input.GetKeyDown(KeyCode.Q) && isEditMode)
        //{
        //    Debug.Log("Exit Edit Mode");
        //    parentNode = null;
        //    isEditMode = false;
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewWave();
        }

        if (currentState == GameState.MenuState)
        {

        }
        else if (currentState == GameState.EditState)
        {
            Editmode();
            EditScreen.SetActive(true);
            PlayScreen.SetActive(false);
        }
        else if (currentState == GameState.PlayState)
        {
            Playmode();
            EditScreen.SetActive(false);
            PlayScreen.SetActive(true);
        }


    }

    IEnumerator DelayWave()
    {
        yield return new WaitForSeconds(1);

    }

    void NewWave()
    {
        _waveManager.GetComponent<WaveManager>().StartWaves(entryNodes);
        _hasActiveWave = true;
        currentState = GameState.PlayState;
    }
    void Editmode()
    {

        placementManager.GetInput();
    }

    void Playmode()
    {

        if (_calculationTimer < 0)
        {
            CalculateInteractions();
            _calculationTimer = WORLD_CALCULATION_INTERVAL;

            foreach (NodeClass node in nodeClasses)
                node.Update();
        }
        _calculationTimer -= Time.deltaTime;
    }
    void CalculateInteractions()
    {
        // List of nodes under attack with the total firepower against
        Dictionary<NodeClass, float> nodesUnderAttack = new Dictionary<NodeClass, float>();
        // List of nodes under attack with the enemy on the front
        Dictionary<NodeClass, GameObject> nodesAndEnemies = new Dictionary<NodeClass, GameObject>();
        foreach (var enemy in _enemyInformationList)
        {
            EnemyInformation enemyInformation = enemy.GetComponent<EnemyInformation>();
            // Finding target Node
            NodeClass targetNode = null;
            if (enemyInformation.GetTarget() == null)
            {
                // First upcoming node is target node
                enemyInformation.SetTarget(enemyInformation._nextNodes[0]);
            }
            else if (enemyInformation.GetTarget().data.isHacked)
            {
                enemyInformation._currentNode = enemyInformation.GetTarget();
                if (enemyInformation.GetTarget().data.type == Nodetype.Branch)
                {
                    int randomNum = rand.Next(0, 2);
                    enemyInformation.SetTarget(enemyInformation.GetTarget().children[randomNum]);
                }
                else
                {
                    enemyInformation.SetTarget(enemyInformation.GetTarget().children[0]);
                }
            }
            else
            {
                GenerateProjectiles(enemyInformation.GetTarget(), enemyInformation._currentNode);
            }

            enemy.transform.position = enemyInformation._currentNode.model.transform.position + 
                2 * enemyInformation._currentNode.model.transform.forward;
            // Rotate enemy to look at the target
            enemy.transform.rotation = Quaternion.LookRotation(enemyInformation.GetTarget().model.transform.position - enemy.transform.position);


            targetNode = enemyInformation.GetTarget();
            if (targetNode != null)
            {
                if (nodesUnderAttack.ContainsKey(targetNode))
                    nodesUnderAttack[targetNode] += enemyInformation._damagePerHit * enemyInformation._hitPerSecond;
                else
                    nodesUnderAttack.Add(targetNode, enemyInformation._damagePerHit * enemyInformation._hitPerSecond);
                // 
                if (nodesAndEnemies.ContainsKey(targetNode))
                {
                    if (nodesAndEnemies[targetNode].GetComponent<EnemyInformation>()._health < 0)
                    {
                        nodesAndEnemies[targetNode] = enemy;
                    }
                }
                else
                    nodesAndEnemies.Add(targetNode, enemy);
            }
        }

        // For each Node under attack
        foreach (var attackedNode in nodesUnderAttack)
        {
            NodeClass node = attackedNode.Key;
            float nodeAttackPower = node.data.damagePerHit * node.data.hitsPerSecond;
            float enemyAttackPower = attackedNode.Value;

            // Apply damages
            node.data.health -= enemyAttackPower;
            nodesAndEnemies[node].GetComponent<EnemyInformation>()._health -= nodeAttackPower;

            // Destroy the front attacker
            if (nodesAndEnemies[node].GetComponent<EnemyInformation>()._health < 0)
            {
                gameCoins += nodesAndEnemies[node].GetComponent<EnemyInformation>()._value;
                _enemyInformationList.Remove(nodesAndEnemies[node]);
                GameObject.Destroy(nodesAndEnemies[node]);
                // Remove the key from dictionary (will be replaced in the next calculation)
                nodesAndEnemies.Remove(node);
                _enemiesKilled++;
                if (_enemyKilledUI != null)
                    _enemyKilledUI.GetComponent<TextMeshProUGUI>().text = "Enemies Killed: " + _enemiesKilled.ToString();
                CheckHighScore(_enemiesKilled);

                //  One wave finished
                if (_enemyInformationList.Count == 0)
                {
                    _waveManager.ResetTimer();
                    _hasActiveWave = false;
                    currentState = GameState.EditState;

                    foreach (NodeClass resetNode in nodeClasses)
                    {
                        resetNode.ResetNode();
                    }
                    // TODO: add the continue button here
                    // Set the waveManagerTimer
                    currentState = GameState.EditState;
                    EditScreen.SetActive(true);
                    return;
                }
            }

            // Destroy the node
            if (node.data.health < 0)
            {
                node.data.isHacked = true;
                if (node.data.type == Nodetype.Turret)
                {
                    Debug.Log("Turret down!");
                }
                if (node.data.type == Nodetype.Branch)
                {
                    Debug.Log("Branch down!");
                }
                if (node.data.type == Nodetype.Farm)
                {
                    Debug.Log("Farm Down");
                }
                if (node.data.type == Nodetype.Server)
                {
                    Debug.Log("Server down! You lost!!");
                    currentState = GameState.GameOverState;
                    GameOverScreen.SetActive(true);
                }
            }
        }
    }

    void UpdateCoins()
    {
        // Update coin UI

        _coinNumberUI.GetComponent<TextMeshProUGUI>().text = gameCoins.ToString();
        _playcoinNumberUI.GetComponent<TextMeshProUGUI>().text = gameCoins.ToString();
    }
    void CheckHighScore(int killed)
    {
        if (killed > PlayerPrefs.GetInt("Highscore", 0))
        {
            PlayerPrefs.SetInt("Highscore", killed);
        }
        if (_enemyKilledUI != null)
            _highscoreUI.GetComponent<TextMeshProUGUI>().text = "HighScore: " + PlayerPrefs.GetInt("Highscore", 0).ToString();
    }
    public void AddEntryNode()
    {
        EntryNode entryNode = new EntryNode();
        entryNode.Init(UnityEngine.Random.insideUnitSphere.normalized * UnityEngine.Random.Range(8f, 15f));
        nodeClasses.Add(entryNode);
        entryNodes.Add(entryNode);
        serverNode?.AddParentNode(entryNode);
    }
    private void GenerateProjectiles(NodeClass endingNode, NodeClass startingNode)
    {
        Vector3 distance = endingNode.model.transform.position - startingNode.model.transform.position;
        Vector3 direction = Vector3.Normalize(distance);
        GameObject projectile = Instantiate(_projectilePrefab, startingNode.model.transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = 10 * direction;
        //projectile.GetComponent<Projectile>().SetDestinaion(endingNode.model.transform.position);
        projectile.GetComponent<Projectile>().SetDistance(Vector3.Magnitude(distance));
    }

    public void PlaceFarm()
    {

        placementManager.PlaceFarm();

    }
    public void PlaceTurret()
    {

        placementManager.PlaceTurret();

    }
    public void PlaceBranch()
    {

        placementManager.PlaceBranch();

    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        MenuScreen.SetActive(false);
        currentState = GameState.EditState;
        EditScreen.SetActive(true);
    }
    public void goMain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
