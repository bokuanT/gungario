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
        else
        {
            Winner.color = Color.blue;
            Winner.text = "Blue wins";
        }
    }
}
