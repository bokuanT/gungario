using UnityEngine;
using Fusion;

public class ControlPoint : NetworkBehaviour
{
    [Networked]
    public int RedPlayers { get; set; }

    [Networked]
    public int BluePlayers { get; set; }

    public ProgressBar progressBar;

    public GameObject captureLocked;

    public override void FixedUpdateNetwork()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player.state == Player.State.Active)
                HandleEntry(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            Player player = other.gameObject.GetComponent<Player>();

            if (player.state == Player.State.Active)
                HandleExit(player);
        }
    }

    private void HandleEntry(Player player)
    {
        //update number of red/blue on plate
        if (player == null)
        {
            Debug.Log("plate null");
            return;
        }

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
}
