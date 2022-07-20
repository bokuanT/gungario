using UnityEngine;
using TMPro;

public class TeamWinnerUI : MonoBehaviour
{
    public TMP_Text Winner;
    public TMP_Text RedScoreFinal;
    public TMP_Text BlueScoreFinal;

    public void DisplayInfo(ControlPoint cp)
    {
        string rsf = cp.RedPoints == 0F ? "0"
            : cp.RedPoints.ToString("#.##");
        string bsf = cp.BluePoints == 0F ? "0"
            : cp.BluePoints.ToString("#.##");

        RedScoreFinal.text = rsf;
        BlueScoreFinal.text = bsf;

        if (cp.RedPoints > cp.BluePoints)
        {
            Winner.color = Color.red;
            Winner.text = "Red wins";
        }
        else if (cp.RedPoints < cp.BluePoints)
        {
            Winner.color = Color.blue;
            Winner.text = "Blue wins";
        }
        else
        {
            Winner.color = Color.white;
            Winner.text = "Draw";
        }
    }

    public void DisplayInfoTDM(Player[] players)
    {
        int redScore = 0;
        int blueScore = 0;

        foreach (Player player in players)
        {
            if (player.team == Player.Team.Red)
                redScore += player.kills;

            if (player.team == Player.Team.Blue)
                blueScore += player.kills;
        }
        RedScoreFinal.text = redScore.ToString();
        BlueScoreFinal.text = blueScore.ToString();

        if (redScore > blueScore)
        {
            Winner.color = Color.red;
            Winner.text = "Red wins";
        }
        else if (redScore < blueScore)
        {
            Winner.color = Color.blue;
            Winner.text = "Blue wins";
        }
        else
        {
            Winner.color = Color.white;
            Winner.text = "Draw";
        }

    }
}
