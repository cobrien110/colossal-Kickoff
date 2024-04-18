using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    public bool isPlaying = false;
    [SerializeField] private UIManager UM = null;
    [SerializeField] private GameObject Ball = null;
    [SerializeField] private List<WarriorController> warriorList;
    [SerializeField] private MonsterController MC = null;
    [SerializeField] private MultipleTargetCamera MTC = null;
    private PlayerInputManager PIM = null;
    private GameObject BallSpawner = null;
    private GameObject WarriorSpawner = null;

    Vector3 WarSpawnPos;
    private List<PlayerInput> allPlayers = new List<PlayerInput>();

    // Start is called before the first frame update
    void Start()
    {
        BallSpawner = GameObject.Find("BallSpawner");
        WarriorSpawner = GameObject.Find("WarriorSpawner");
        WarSpawnPos = WarriorSpawner.transform.position;
        PIM = GameObject.Find("Warrior Manager").GetComponent<PlayerInputManager>();
        StartCoroutine(Kickoff());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlaying()
    {
        isPlaying = true;
        UM.StartTimer();
    }

    public void StopPlaying()
    {
        isPlaying = false;
        UM.StopTimer();
    }

    private IEnumerator Kickoff()
    {
        StartCoroutine(UM.Countdown());
        yield return new WaitForSeconds(3f);
        StartPlaying();
    }

    public void Reset()
    {
        StopPlaying();
        StartCoroutine(Kickoff());
        GameObject newBall = Instantiate(Ball, BallSpawner.transform.position, Quaternion.identity);
        Ball = newBall;
        for (int i = 0; i < warriorList.Count; i++)
        {
            warriorList[i].Ball = newBall;
            warriorList[i].BP = Ball.GetComponent<BallProperties>();
            warriorList[i].ResetPlayer();
        }
        MC.Ball = newBall;
        MC.BP = Ball.GetComponent<BallProperties>();
        MC.ResetPlayer();
        MultipleTargetCamera MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        MTC.targets[0] = newBall.transform;
    }

    //isPlaying getter and setter
    public bool IsPlayingGet()
    {
        return isPlaying;
    }

    public void IsPlayingSet(bool set)
    {
        isPlaying = set;
    }

    public void AddPlayer(PlayerInput player)
    {
        allPlayers.Add(player);
        MTC.AddTarget(player.transform);
        player.transform.position = WarSpawnPos;
        WarSpawnPos.z -= 1;
    }
}
