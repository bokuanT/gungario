using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataAccess : MonoBehaviour
{
    public static void Save(GameDetails gameDetails)
    {
        string dataPath = string.Format("{0}/GameDetails.dat", Application.persistentDataPath);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream;

        if (File.Exists(dataPath))
        {
            File.WriteAllText(dataPath, string.Empty);
            fileStream = File.Open(dataPath, FileMode.Open);
        }
        else
        {
            fileStream = File.Create(dataPath);
        }

        binaryFormatter.Serialize(fileStream, gameDetails);
        fileStream.Close();

    }

    public static GameDetails Load()
    {
        GameDetails gameDetails = null;
        string dataPath = string.Format("{0}/GameDetails.dat", Application.persistentDataPath);

        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(dataPath, FileMode.Open);

            gameDetails = (GameDetails)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
        }


        return gameDetails;
    }

}

[Serializable]
public class GameDetails
{
    public string GUID;

    public GameDetails(string ID)
    {
        GUID = ID;
    }
}