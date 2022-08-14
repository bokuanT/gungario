using TMPro;
using UnityEngine;

public class ControlPointScoreManager : MonoBehaviour
{
    public TMP_Text RedPoints;

    public TMP_Text BluePoints;

    public TMP_Text scoreboardRed;

    public TMP_Text scoreboardBlue;

    public ControlPoint controlPoint;

    private void Update()
    {
        if (controlPoint.spawned)
        {
            if (controlPoint.RedPoints == 0)
            {
                RedPoints.text = "0";
                scoreboardRed.text = "0";
            }
            else
            {
                string rp = controlPoint.RedPoints.ToString("#");
                RedPoints.text = rp;
                scoreboardRed.text = rp;
            }
            if (controlPoint.BluePoints == 0)
            {
                BluePoints.text = "0";
                scoreboardBlue.text = "0";
            }
            else
            {
                string bp = controlPoint.BluePoints.ToString("#");

                BluePoints.text = bp;

                scoreboardBlue.text = bp;
            }
            
            
        }
    }
}
