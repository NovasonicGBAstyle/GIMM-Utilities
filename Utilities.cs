using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
//This is for some of the cool photon stuff I build to deal with how photon uses network prefabs.
using UnityEditor;
#endif

/// <summary>
/// Simple class that Dr. Dan gave us to handle text input checking.  Pretty easy.
/// </summary>
public static class UtilityInputHandler
{

    public static string processText(string inS)
    {
        return inS;
    }
    public static int getAverageChars(string inS)
    {
        string[] words = inS.Split(' ');
        int sum = 0;
        foreach (var word in words)
        {
            sum += word.Length;
        }
        return sum / words.Length;
    }
    public static bool isNum(string inS)
    {
        try
        {
            double num = Double.Parse(inS);
            return true;
        }
        catch (FormatException e)
        {
            Debug.Log(e);
            return false;
        }
    }
    public static bool betterIsNum(string inS)
    {
        if (Double.TryParse(inS, out double j))
            return true;
        else
            return false;

    }
}

/// <summary>
/// This is a pretty ingenious class.  Basically, it works with a scene I made that has mutliple input fields and a couple
/// output fields.  Basically, it is created to take four categories of data.  Each category has a weight, and a set fo scores.
/// What it will do is take the scores you have provided, use the weight you gave for that category, and provide a total grade.
/// It's super smart as well.  If you don't have scores for a given category even if it has the score, it will simply ignore
/// that category.  Yay!
/// </summary>
public class UtilityUILogic : MonoBehaviour
{
    public Text text;
    public Text OutputGrade;
    public Text OutputWorry;
    public string processedText;
    public InputField hwWeight;
    public InputField hwScores;
    public InputField quizWeight;
    public InputField quizScores;
    public InputField testWeight;
    public InputField testScores;
    public InputField projWeight;
    public InputField projScores;

    private double dblHwWeight = 0.0;
    private List<double> arrHwScores = new List<double>();
    private double dblQuizWeight = 0.0;
    private List<double> arrQuizScores = new List<double>();
    private double dblTestWeight = 0.0;
    private List<double> arrTestScores = new List<double>();
    private double dblProjWeight = 0.0;
    private List<double> arrProjScores = new List<double>();
    private double dblGrade = 0.0;

    private string strOutputGrade = "";
    private string strOutputWorry = "";

    /// <summary>
    /// This gets the input and processes it using the processText handler.
    /// </summary>
    public void getInput()
    {
        cleanupValues();

        bool check = true;
        check = processHw();

        if (check)
        {
            check = processQuiz();
        }

        if (check)
        {
            check = processTest();
        }

        if (check)
        {
            check = processProj();
        }

        //Calculate the grade
        if (check)
        {
            calculateGrade();
        }

        //Display the output
        displayOutput();
    }

    /// <summary>
    /// So, we're just going to reset all the values before we start so we have clean values every time we run this.
    /// </summary>
    private void cleanupValues()
    {
        dblHwWeight = 0.0;
        arrHwScores = new List<double>();
        dblQuizWeight = 0.0;
        arrQuizScores = new List<double>();
        dblTestWeight = 0.0;
        arrTestScores = new List<double>();
        dblProjWeight = 0.0;
        arrProjScores = new List<double>();
        dblGrade = 0.0;
    }

    public void displayOutput()
    {
        //Debug.Log(processedText);
        //string typeOfInput = "none";
        //if (InputHandler.betterIsNum(processedText))
        //{
        //    typeOfInput = "number";
        //}
        //else
        //{
        //    typeOfInput = "string";
        //}
        //text.text = typeOfInput;

        OutputGrade.text = strOutputGrade;
        OutputWorry.text = strOutputWorry;

        //text.text = InputHandler.getAverageChars(processedText).ToString();
    }

    /// <summary>
    /// So, this is going to calculate a grade and assign it to our grade dealio.  It definitely makes assumptions about the specific input it is looking for.
    /// </summary>
    private void calculateGrade()
    {
        //So for weights, the calculation is about dividing by the total weight.  But we are only going to do division if we have scores.  For example, we don't
        //want to include quizes if there were no quizes.

        double dividend = 0.0;
        double divisor = 0.0;
        dblHwWeight = arrHwScores.Count > 0 ? dblHwWeight : 0.0;
        dblQuizWeight = arrQuizScores.Count > 0 ? dblQuizWeight : 0.0;
        dblTestWeight = arrTestScores.Count > 0 ? dblTestWeight : 0.0;
        dblProjWeight = arrProjScores.Count > 0 ? dblProjWeight : 0.0;

        dividend = dblHwWeight * arrayAverage(arrHwScores) + dblQuizWeight * arrayAverage(arrQuizScores) + dblTestWeight * arrayAverage(arrTestScores) + dblProjWeight * arrayAverage(arrProjScores);
        divisor = dblHwWeight + dblQuizWeight + dblTestWeight + dblProjWeight;

        if (divisor > 0)
        {
            dblGrade = dividend / divisor;
        }


        strOutputGrade = "Calculated Grade: " + dblGrade.ToString();
        strOutputWorry = dblGrade < 70 ? "Worry, worry!!!" : "No worries";

    }

    /// <summary>
    /// This takes an array of doubles and returns the average.
    /// </summary>
    /// <param name="arr">Array of doubles.</param>
    /// <returns>A double value that is the average of the array passed in.</returns>
    private double arrayAverage(List<double> ls)
    {
        //First, we'll have a couple variables.
        double sum = 0.0;
        double ret = 0.0;


        //We'll only calculate the array value if the array lenght is greater than zero.
        if (ls.Count > 0)
        {
            //Next, we'll loop through the array adding the value to our sum.
            for (int i = 0; i < ls.Count; i++)
            {
                sum += ls[i];
            }

            //Now, we do our division to get our average.
            ret = sum / ls.Count;
        }

        return ret;
    }

    /// <summary>
    /// This will process incoming data using the InputHandler class.
    /// </summary>
    /// <param name="inS"></param>
    public void processText(string inS)
    {
        Debug.Log("Before input handler: " + inS);
        //this.processedText = inS;
        this.processedText = InputHandler.processText(inS);
    }

    /// <summary>
    /// This will make sure the homework weight and scores are good to go.
    /// </summary>
    private bool processHw()
    {
        //Just a deal we'll use to see what we'll return.
        bool ret = true;

        //First, make sure the stuff weight is good to go.
        if (InputHandler.betterIsNum(hwWeight.text))
        {
            Debug.Log("We have a number for the homework weight.");
            Double.TryParse(hwWeight.text, out dblHwWeight);

            //Next, we'll try to get the homework scores.
            //First, make sure there is a string.
            if (hwScores.text.Length > 0)
            {
                //Debug.Log("hwScores length is: " + hwScores.text.Length.ToString());
                string[] temp = hwScores.text.Split(' ');

                foreach (string te in temp)
                {
                    if (InputHandler.betterIsNum(te))
                    {
                        Double.TryParse(te, out double dblHold);
                        arrHwScores.Add(dblHold);
                    }
                    else
                    {
                        ret = false;
                        strOutputGrade = "Cannot calculate grade.";
                        strOutputWorry = "Homework scores are invalid.";
                    }
                }
            }
        }
        else
        {
            Debug.Log("We do not have a number for the homework weight.");
            ret = false;
            strOutputGrade = "Cannot calculate grade.";
            strOutputWorry = "Homework weight is invalid.";
        }

        //Return our value.
        return ret;
    }

    /// <summary>
    /// This will make sure the homework weight and scores are good to go.
    /// </summary>
    private bool processQuiz()
    {
        //Just a deal we'll use to see what we'll return.
        bool ret = true;

        //First, make sure the stuff weight is good to go.
        if (InputHandler.betterIsNum(quizWeight.text))
        {
            Debug.Log("We have a number for the quiz weight.");
            Double.TryParse(quizWeight.text, out dblQuizWeight);

            //Next, we'll try to get the homework scores.
            //First, make sure there is a string.
            if (quizScores.text.Length > 0)
            {
                string[] temp = quizScores.text.Split(' ');

                foreach (string te in temp)
                {
                    if (InputHandler.betterIsNum(te))
                    {
                        Double.TryParse(te, out double dblHold);
                        arrQuizScores.Add(dblHold);
                    }
                    else
                    {
                        ret = false;
                        strOutputGrade = "Cannot calculate grade.";
                        strOutputWorry = "Quiz scores are invalid.";
                    }
                }
            }
        }
        else
        {
            Debug.Log("We do not have a number for the quiz weight.");
            ret = false;
            strOutputGrade = "Cannot calculate grade.";
            strOutputWorry = "Quiz weight is invalid.";
        }

        //Return our value.
        return ret;
    }

    /// <summary>
    /// This will make sure the homework weight and scores are good to go.
    /// </summary>
    private bool processTest()
    {
        //Just a deal we'll use to see what we'll return.
        bool ret = true;

        //First, make sure the stuff weight is good to go.
        if (InputHandler.betterIsNum(testWeight.text))
        {
            Debug.Log("We have a number for the test weight.");
            Double.TryParse(testWeight.text, out dblTestWeight);

            //Next, we'll try to get the homework scores.
            //First, make sure there is a string.
            if (testScores.text.Length > 0)
            {
                string[] temp = testScores.text.Split(' ');

                foreach (string te in temp)
                {
                    if (InputHandler.betterIsNum(te))
                    {
                        Double.TryParse(te, out double dblHold);
                        arrTestScores.Add(dblHold);
                    }
                    else
                    {
                        ret = false;
                        strOutputGrade = "Cannot calculate grade.";
                        strOutputWorry = "Test scores are invalid.";
                    }
                }
            }
        }
        else
        {
            Debug.Log("We do not have a number for the test weight.");
            ret = false;
            strOutputGrade = "Cannot calculate grade.";
            strOutputWorry = "Test weight is invalid.";
        }

        //Return our value.
        return ret;
    }

    /// <summary>
    /// This will make sure the homework weight and scores are good to go.
    /// </summary>
    private bool processProj()
    {
        //Just a deal we'll use to see what we'll return.
        bool ret = true;

        //First, make sure the stuff weight is good to go.
        if (InputHandler.betterIsNum(projWeight.text))
        {
            Debug.Log("We have a number for the proj weight.");
            Double.TryParse(projWeight.text, out dblProjWeight);

            //Next, we'll try to get the homework scores.
            //First, make sure there is a string.
            if (projScores.text.Length > 0)
            {
                string[] temp = projScores.text.Split(' ');

                foreach (string te in temp)
                {
                    if (InputHandler.betterIsNum(te))
                    {
                        Double.TryParse(te, out double dblHold);
                        arrProjScores.Add(dblHold);
                    }
                    else
                    {
                        ret = false;
                        strOutputGrade = "Cannot calculate grade.";
                        strOutputWorry = "Proj scores are invalid.";
                    }
                }
            }
        }
        else
        {
            Debug.Log("We do not have a number for the proj weight.");
            ret = false;
            strOutputGrade = "Cannot calculate grade.";
            strOutputWorry = "Proj weight is invalid.";
        }

        //Return our value.
        return ret;
    }
}

/// <summary>
/// With Photon, you can't instantiate a prefab the way you would normally in Unity passing in a game object reference.
/// You have to instantiate using the game object name and the path in the resources folder.  Clearly this is bad
/// because it can cause issues with typos, and everytime you move something, it has to be changed everywhere.
/// This will take care of that for us.  It will automatically build the paths for us, and we will just be able
/// to instantiate normally just passing in the prefab and it will pull the stored paths.
/// </summary>
[System.Serializable]
public class UtilityNetworkedPrefab
{
    public GameObject Prefab;
    public string Path;

    public UtilityNetworkedPrefab(GameObject obj, string path)
    {
        Prefab = obj;
        Path = ReturnPrefabPathModified(path);
        //When this is called in MasterManager, it doesn't give us the path we need.  we get the first path below.  But we wan
        //Assets/Resources/File.prefab
        //Resources/File
    }

    /// <summary>
    /// When this is called in MasterManager, it doesn't give us the path we need.  This will give us just what we actually need.
    /// </summary>
    /// <param name="path">i.e. Assets/Resources/File.prefab</param>
    /// <returns>i.e. Resources/File.  Upon error, it returns an empty string.</returns>
    private string ReturnPrefabPathModified(string path)
    {
        //First, get the lenth of the extension (.prefab)
        int extensionLength = System.IO.Path.GetExtension(path).Length;

        //We will now find the part of the path we want to start from since we don't want the "Resources" part of the path.
        string pathSearch = "resources";

        //Get the start of our resources deal.
        int startIndex = path.ToLower().IndexOf(pathSearch);

        //We are adding 1 because it will have a slash that we want to ignore.  But depending on the platform, it could be either forward or backslash.
        //We don't care, just ignore it.
        int additionalLength = pathSearch.Length + 1;

        //Shouldn't happen, but if we didn't find the path, return an empty string.  Deal with that later.
        if (startIndex == -1)
        {
            //Return an empty string as we have reached an error.
            return string.Empty;
        }
        else
        {
            //Return the part of the string between the parts we don't want, from "Resources" to extension. 
            return path.Substring(startIndex + additionalLength, path.Length - (additionalLength + startIndex + extensionLength));
        }
    }
}

/// <summary>
/// We are going to create a singleton that can exist in multiple scenes without any issues because it is handled
/// in the inspector.  Cool story.  We get the benefits of referencing the singleton without having to have a direct
/// reference to the object.  Sweet!
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class UtilityScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] results = Resources.FindObjectsOfTypeAll<T>();

                if (results.Length == 0)
                {
                    Debug.LogError("SingletonScriptableObject -> Instance -> results lenght is 0 for type " + typeof(T).ToString() + ".");
                    return null;
                }

                if (results.Length > 1)
                {
                    Debug.LogError("SingletonScriptableObject -> Instance -> results length is greater than 1 for type " + typeof(T).ToString() + ".");
                    return null;
                }

                _instance = results[0];
            }

            return _instance;
        }
    }
}

/// <summary>
/// Here, we will create a scriptable object that does two things.  First, it is a scriptable object so we will be able
/// to use it in all our code without having to create a direct reference to it.  We will just have to access it using the
/// class name.
/// For example, if we want to get the GameVersion, we just type GameSettings.GameVersion wherever we need it!
/// Even cooler, this is an asset menu item.  We can right click in Unity and create a new one of these just like we
/// would add a sphere or a canvas.
/// </summary>
[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class UtilityGameSettings : ScriptableObject
{
    [SerializeField]
    private string _gameVersion = "0.0.0";
    public string GameVersion { get { return _gameVersion; } }
    [SerializeField]
    private string _nickName = "Punfish";

    public string NickName
    {
        get
        {
            int value = UnityEngine.Random.Range(0, 9999);
            return _nickName + value.ToString();
        }
    }
}

/// <summary>
/// Pulled from First Gear Games on YouTube.
/// This code utilizes Photon for Unity 2.  Make sure to include that from the asset store.
/// PUN 2 - Free by Exit Games
/// </summary>
[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class UtilityMasterManager : UtilityScriptableObjectSingleton<UtilityMasterManager>
{

    [SerializeField]
    private UtilityGameSettings _gameSettings;
    public static UtilityGameSettings GameSettings { get { return Instance._gameSettings; } }

    //This will be a list of network prefabs because Photon has weird networking instantiation that's rough.  See NetworkedPrefab.cs
    [SerializeField]
    private List<UtilityNetworkedPrefab> _networkedPrefabs = new List<UtilityNetworkedPrefab>();

    /// <summary>
    /// This will take an object, make sure it's one of our NetworkedPrefab objects, and instantiate it through Photon if it is.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns>The game object instantiated.</returns>
    public static GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        foreach (UtilityNetworkedPrefab networkedPrefab in Instance._networkedPrefabs)
        {
            //Check for the object to make sure it matches our prefab variable.  If so, instantiate it.
            if (networkedPrefab.Prefab == obj)
            {
                //Check for an empty path.
                if (networkedPrefab.Path != string.Empty)
                {
                    //Instantiate and return the object.
                    GameObject result = PhotonNetwork.Instantiate(networkedPrefab.Path, position, rotation);
                    return result;
                }
                else
                {
                    //There was no path for some reason, so return nothing.
                    Debug.LogError("Path is empty for GameObject: " + networkedPrefab.Prefab);
                    return null;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// This will build our network prefabs list.  We are going to do it before the scene loads.  It will require editor code, so
    /// it should only run in the editor itself.
    /// IMPORTANT
    /// IMPORTANT
    /// This has to be run once in the editor after everything has be built so that this will be built.  Otherwise, it won't build
    /// and won't have all the data it needs.  This includes needing to be done anything something changes or moves.
    /// IMPORTANT
    /// IMPORTANT
    /// TODO:
    /// See if you can get it to do this when it runs instead of on editor.  Just as a thought.  That way you won't have to do this
    /// everytime you make a change.  So, if you build and run before running in the editor, things won't work as this is.  This works
    /// because classes are serializable.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void PopulateNetworkPrefabs()
    {
        //Make sure this only runs in the editor.  Otherwise, exit.
#if UNITY_EDITOR

        //Clear out the prefab list to start.
        Instance._networkedPrefabs.Clear();

        //Get a list of all resources from the entire resources folder.
        GameObject[] results = Resources.LoadAll<GameObject>("");

        //Loop through the list.
        for (int i = 0; i < results.Length; i++)
        {
            //Check to see if this is a Photon network object.
            if (results[i].GetComponent<PhotonView>() != null)
            {
                //This is a photon network object, so get the path and add it to the list.
                string path = AssetDatabase.GetAssetPath(results[i]);
                Instance._networkedPrefabs.Add(new UtilityNetworkedPrefab(results[i], path));
            }
        }

        for (int i = 0; i < Instance._networkedPrefabs.Count; i++)
        {
            UnityEngine.Debug.Log("Newtork Prefab " + Instance._networkedPrefabs[i].Prefab.name + ": " + Instance._networkedPrefabs[i].Path);
        }
#endif
    }
}

/// <summary>
/// Posted in class by Nicolai Cascio
/// This is a script I made to create an infinite road in a driving game.I have another function which returns random road
/// 'pieces' aka 3d models.The basics of what are going on here is that index 0 of the array which holds the road pieces is
/// destroyed, all the road pieces are moved down 1 place in the array, and then the new piece of road is instantiated at
/// the end of the array.Effectively this makes an infinite road that gets deleted over time (as you drive past it). Okay
/// thank you for coming to my TED talk.
/// </summary>
public class UtilityInfiniteRoad : MonoBehaviour
{
    //void CreateRoad()
    //{

    //    RoadExit = JustSpawned.transform.Find("ExitPoint");
    //    SpawnPosition.transform.position = RoadExit.transform.position;
    //    JustSpawned = Instantiate<GameObject>(ReturnRoad());
    //    JustSpawned.transform.position = SpawnPosition.transform.position;

    //    Destroy(Roads[0]);
    //    Roads[0] = null;
    //    for (int i = 0; i < (Roads.Length - 1); i++)
    //    {
    //        if (Roads[i] == null && Roads[i + 1] != null)
    //        {
    //            Roads[i] = Roads[i + 1];
    //            Roads[i + 1] = null;
    //        }
    //    }
    //    Roads[Roads.Length - 1] = JustSpawned;
    //}
}

/// <summary>
/// Pull from class by Cody Fitch
/// The PHP script connects to the SQL server and acts as the middleman between the server and Unity.Unity can't connect to the
/// SQL server directly due to security concerns. It takes the data on the table and echos it (prints it). The C# script takes
/// that echoed data and assigns it into an array which each array key can then be used for something. This isn't the most secure
/// way of sending data since you can see the data on the PHP page but it works for things like scores and data that doesn't
/// necessarily need to be secret.
/// </summary>
public class UtilityReadSQLThroughPhp : MonoBehaviour
{
    ////Import data from SQL server through PHP script
    //UnityWebRequest messUpdate = UnityWebRequest.Get(messageData);
    //yield return messUpdate.SendWebRequest();

    //        if (messUpdate.error != null)
    //        {
    //            Debug.Log(messUpdate.error);
    //        }
    //        else
    //        {
    //            //Split incoming string into message categories
    //            incomingText.text = messUpdate.downloadHandler.text;
    //            string allData = incomingText.text;
    //string[] splitStr = allData.Split(" "[0]);
    //int[] value = new int[splitStr.Length - 1];
    //string[] splitArr = allData.Split(char.Parse(";"));
    //a.text = splitArr[0];
    //            b.text = splitArr[1];

    //PHP script:

    //php
    //    // Configuration
    //    $hostname = 'localhost';
    //    $username = '******'; //username of SQL server
    //    $password = '******'; //password of SQL server
    //    $database = 'database1'; //name of the database containing the table
    //    try {
    //        $dbh = new PDO('mysql:host='. $hostname.';dbname='. $database, $username, $password);
    //    } catch(PDOException $e) {
    //        echo 'an error has occured';

    //        <pre>', $e->getMessage() ,' pre>';
    //    }
    //    $sth = $dbh->query('SELECT * FROM table1'); //select which table in the database you want to pull from, this is a SQL script
    //    $sth->setFetchMode(PDO::FETCH_ASSOC);
    //    $result = $sth->fetchAll();
    //    if(count($result) > 0) {
    //        foreach($result as $r) {
    //            echo $r['a'], ";";
    //            echo $r['b'], ";";

    //     }

    //   }

    //?>
}