using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class ControlPoint : NetworkBehaviour
{
    [Networked]
    public int RedPlayers { get; set; }

    [Networked]
    public int BluePlayers { get; set; }

    [Networked]
    public float RedPoints { get; set; }

    [Networked]
    public float BluePoints { get; set; }

    public bool spawned = false;

    public ProgressBar progressBar;

    public GameObject captureLocked;

    private HashSet<Player> _playersInside = new HashSet<Player>();

    public override void Spawned()
    {
        spawned = true;
        RedPoints = 0;
        BluePoints = 0;
    }
    public override void FixedUpdateNetwork()
    {
        UpdatePoints();
        //run or stop timers depending on players on plate, update scores
        if (progressBar.IsNeutral() && RedPlayers + BluePlayers > 0)
        {
            ProgressBar.Team team = RedPlayers > BluePlayers ? ProgressBar.Team.Red
                : ProgressBar.Team.Blue;
            progressBar.UpdatePercen(RedPlayers + BluePlayers, team);
        }
        else
        {
            if (RedPlayers > 0 && BluePlayers == 0)
            {
                progressBar.UpdatePercen(RedPlayers, ProgressBar.Team.Red);
            }
            if (BluePlayers > 0 && RedPlayers == 0)
            {
                progressBar.UpdatePercen(BluePlayers, ProgressBar.Team.Blue);
            }
            else
            {
                //captureLocked.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player.state == Player.State.Active && !_playersInside.Contains(player))
                HandleEntry(player);

            if (player.state == Player.State.Dead && _playersInside.Contains(player))
                HandleExit(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            Player player = other.gameObject.GetComponent<Player>();

            //if (player.state == Player.State.Active)
                HandleExit(player);
        }
    }

    private void UpdatePoints()
    {
        if (progressBar.team == ProgressBar.Team.Red)
            RedPoints += 0.02F;

        if (progressBar.team == ProgressBar.Team.Blue)
            BluePoints += 0.02F;
    }

    private void HandleEntry(Player player)
    {
        //update number of red/blue on plate
        if (player == null)
        {
            Debug.Log("plate null");
            return;
        }
        Debug.Log("HandleEntry");
        _playersInside.Add(player);
        if (player.team == Player.Team.Red)
            RedPlayers += 1;
        if (player.team == Player.Team.Blue)
            BluePlayers += 1;
    }

    private void HandleExit(Player player)
    {
        //update number of red/blue on plate
        if (player == null)
        {
            Debug.Log("plate null");
            return;
        }
        Debug.Log("Handle Exit");
        _playersInside.Remove(player);
        if (player.team == Player.Team.Red)
            RedPlayers -= 1;
        if (player.team == Player.Team.Blue)
            BluePlayers -= 1;
    }
    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void ResetPoints()
    {
        RedPoints = 0F;
        BluePoints = 0F;
        progressBar.ResetProgress();
    }
}
