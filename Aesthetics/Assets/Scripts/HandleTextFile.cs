using System.IO;
using UnityEditor;
using UnityEngine;

public class HandleTextFile : MonoBehaviour
{
    [MenuItem ("Tools/Write file")]
    static void WriteString (string text)
    {
        string path = "Assets/Resources/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter (path, true);
        writer.WriteLine (text);
        writer.Close ();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset (path);
        TextAsset asset = (TextAsset) Resources.Load ("test");

        //Print the text from the file
        Debug.Log (asset.text);
    }

    [MenuItem ("Tools/Read file")]
    static void ReadString ()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader (path);
        Debug.Log (reader.ReadToEnd ());
        reader.Close ();
    }

    private void Start ()
    {

        //WriteString ("ganon");

        //ReadString ();
    }

}