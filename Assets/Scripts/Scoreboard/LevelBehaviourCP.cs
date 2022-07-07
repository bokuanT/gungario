using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Linq;
using System.Threading.Tasks;
using System;
using TMPro;

//using FusionUtilsEvents;

public class LevelBehaviourCP : NetworkBehaviour
{
    [SerializeField] private float _levelTime = 300f;
    [SerializeField] private float _startTime = 7f;

    [Networked]
    private TickTimer StartTimer { get; set; }

    [Networked]
    private TickTimer LevelTimer { get; set; }

    [SerializeField] private TMP_Text _startTimerText;
    [SerializeField] private TMP_Text _levelTimerText;

    private Scoreboard scoreboard;

    public GameObject controlPoint;

    [Networked]
    public NetworkBool roundStarted { get; set; }


    [Networked]
    private NetworkBool initiated { get; set; }

    public bool activated = true;

    public override void Spawned()
    {
        if (activated)
        {
            initiated = false;
            Debug.Log("spawned");
            controlPoint.SetActive(true);
            StartLevel();
            GameObject tmp = GameObject.Find("Scoreboard_canvas/Scoreboard");
            if (tmp != null)
                scoreboard = tmp.GetComponent<Scoreboard>();
        }
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void ManualStart()
    {
        Debug.Log("manual start");
        controlPoint.SetActive(true);
        StartLevel();
        GameObject tmp = GameObject.Find("Scoreboard_canvas/Scoreboard");
        if (tmp != null)
            scoreboard = tmp.GetComponent<Scoreboard>();
    }

    public override void FixedUpdateNetwork()
    {

        if (StartTimer.IsRunning && _startTimerText.gameObject.activeInHierarchy)
        {
            _startTimerText.text = "Round starts in: " + ((int?)StartTimer.RemainingTime(Runner)).ToString();
        }
        if (StartTimer.Expired(Runner) && !roundStarted)
        {
            roundStarted = true;
            scoreboard.ResetScore();
            _startTimerText.gameObject.SetActive(false);
            _levelTimerText.gameObject.SetActive(true);
            LevelTimer = TickTimer.CreateFromSeconds(Runner, _levelTime);
        }
        if (LevelTimer.IsRunning)
        {
            if (Object.HasStateAuthority && LevelTimer.Expired(Runner))
            {

                RPC_FinishLevel();
                LevelTimer = TickTimer.None;
            }

            //for players who join late
            if (_startTimerText.gameObject.activeSelf)
            {
                _startTimerText.gameObject.SetActive(false);
                _levelTimerText.gameObject.SetActive(true);
            }
            _levelTimerText.text = "Round ends in: " + ((int?)LevelTimer.RemainingTime(Runner)).ToString();
        }
    }

    public void StartLevel()
    {
        _startTimerText.gameObject.SetActive(true);
        StartTimer = TickTimer.CreateFromSeconds(Runner, _startTime);
        roundStarted = false;
        initiated = true;
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_FinishLevel()
    {
        _levelTimerText.gameObject.SetActive(false);

        Player[] results = scoreboard.GetAllScoreboardPlayers();
        IPlayerComparer playerComparer = new IPlayerComparer();
        Array.Sort(results, playerComparer); //index 0 is largest

        GameObject resultsScreen = GameObject.Find("Scoreboard_canvas")
            .transform.Find("GameOverScreen").gameObject;
        resultsScreen.SetActive(true);
        KillLeaderEntryContainer entryContainer = resultsScreen
            .GetComponentInChildren<KillLeaderEntryContainer>();
        
        //Display team winner
        TeamWinnerUI t_w_UI = resultsScreen.GetComponentInChildren<TeamWinnerUI>();
        ControlPoint cp = controlPoint.GetComponent<ControlPoint>();
        t_w_UI.DisplayInfo(cp);
        
        int position = 1;
        foreach (Player winner in results)
        {
            if (position < 5) //shows top 4 only
            {
                //TODO create entry (local)
                entryContainer.SpawnEntry(winner, position);
                position++;
            }
            else
            {
                break;
            }
        }

        Invoke("NewGame", 5f);
    }

    private void NewGame()
    {
        GameObject resultsScreen = GameObject.Find("Scoreboard_canvas")
            .transform.Find("GameOverScreen").gameObject;
        resultsScreen.SetActive(false);

        scoreboard.ResetScore();
        KillLeaderEntryContainer entryContainer = resultsScreen
            .GetComponentInChildren<KillLeaderEntryContainer>();
        entryContainer.ResetEntries();

        ControlPoint cp = controlPoint.GetComponent<ControlPoint>();
        cp.ResetPoints();

        StartLevel();

    }

    public void TurnOff()
    {
        StartTimer = TickTimer.None;
        LevelTimer = TickTimer.None;
        initiated = false;
        roundStarted = false;
    }
}


