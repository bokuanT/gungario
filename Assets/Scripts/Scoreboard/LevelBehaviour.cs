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

public class LevelBehaviour : NetworkBehaviour
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

    public override void Spawned()
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
        StartLevel();

    }
}

public class IPlayerComparer : IComparer<Player>
{
    public int Compare(Player first, Player second)
    {
        return first.kills.CompareTo(second.kills) * -1;
    }
}
