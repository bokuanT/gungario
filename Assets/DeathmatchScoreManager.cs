using UnityEngine;
using TMPro;
using System;
public class DeathmatchScoreManager : MonoBehaviour
{
    public TMP_Text RedPoints;

    public TMP_Text BluePoints;

    public TMP_Text scoreboardRed;

    public TMP_Text scoreboardBlue;

    public Scoreboard scoreboard;
    // Update is called once per frame
    void Update()
    {
        try
        {
            string rp = scoreboard.redScore.ToString();
            string bp = scoreboard.blueScore.ToString();
            RedPoints.text = rp;
            scoreboardRed.text = rp;
            BluePoints.text = bp;
            scoreboardBlue.text = bp;
        }
        catch (InvalidOperationException e)
        {
            string rp = "0";
            string bp = "0";
            RedPoints.text = rp;
            scoreboardRed.text = rp;
            BluePoints.text = bp;
            scoreboardBlue.text = bp;
        }
    }
}
