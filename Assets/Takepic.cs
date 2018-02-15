using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Takepic : MonoBehaviour {

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 300, 200), "Share Screenshot"))
        {
            StartCoroutine(ShareScreenshot());
        }

    }

    public IEnumerator ShareScreenshot()
    {

        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // prepare texture with Screen and save it
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // save to persistentDataPath File
        byte[] data = texture.EncodeToPNG();
        string destination = Path.Combine(Application.persistentDataPath,
                                          System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        File.WriteAllBytes(destination, data);

        if (!Application.isEditor)
        {
#if UNITY_ANDROID
            string body = "Body of message to be shared";
            string subject = "Subject of message";

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // run intent from the current Activity
            currentActivity.Call("startActivity", intentObject);
#endif
        }
    }
}
