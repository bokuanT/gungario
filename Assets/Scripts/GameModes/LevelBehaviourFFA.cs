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

public class LevelBehaviourFFA : NetworkBehaviour
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

    [Networked]
    private NetworkBool roundStarted { get; set; }

    public bool activated = true;

    public override void Spawned()
    {
        if (activated)
        {
            StartLevel();
            GameObject tmp = GameObject.Find("Scoreboard_canvas/Scoreboard");
            if (tmp != null)
                scoreboard = tmp.GetComponent<Scoreboard>();
        }
    }

    public void ManualStart()
    {
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

        int position = 1;
        foreach (Player winner in results)
        {
            if (position < 5) //shows top 4 only
            {
                //TODO create entry (local)
                entryContainer.SpawnEntry(winner, position);
                // Assign rewards here
                if (winner.GetComponent<NetworkPlayer>().Equals(NetworkPlayer.Local))
                {
                    GameManager.Instance.AssignRewards(position);
                }
                position++;
            }
            else
            {
                break;
            }
        }

        entryContainer.StartVictorySound();
        entryContainer.StopVictorySoundInFiveSeconds();
        Invoke("ReturnToLobby", 5f);
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
        StartLevel();

    }

    private void ReturnToLobby()
    {
        Debug.Log("returning to lobby");
        GameLauncher.Instance.LeaveSession();
    }
}

