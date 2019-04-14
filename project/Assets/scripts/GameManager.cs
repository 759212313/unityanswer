using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using LitJson;
using UnityEngine.UI;


[System.Serializable]
public class answerdata
{
    public int id;
    public string question;
    public string answer;
}

public class item {
    public int idx;
    public string qStr;
    public string aStr;
}

public class GameManager : MonoBehaviour {

    public Text lb_question;
    public Text lb_answer;

    private Dictionary<string, answerdata> temp = new Dictionary<string, answerdata>();
    private int curPageIdx = 1;
    private answerdata curData = null;

    void Start () {
        InitAnswer();
    }

    public void InitAnswer() {
        if (File.Exists(Application.dataPath + "/Data/answerdata.json"))
        {
            string jsonString = File.ReadAllText(Application.dataPath + "/Data/answerdata.json");
            if (!string.IsNullOrEmpty(jsonString))
            {
                temp = JsonMapper.ToObject<Dictionary<string, answerdata>>(jsonString);
                RefreshCurQuestion();
            }
        }
    }

    public void AddPage()
    {
        curPageIdx += 1;
        int lengh = temp.Count;
        if (curPageIdx > lengh)
        {
            curPageIdx = lengh;
        }
        RefreshCurQuestion();
    }

    public void BackPage()
    {
        curPageIdx -= 1;
        if (curPageIdx < 1)
        {
            curPageIdx = 1;
        }
        RefreshCurQuestion();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RefreshData()
    {
        //删除文件
        //下载文件
        string PathURL = "http://192.168.43.1/data/answerdata.json";
        string name = "answerdata.json";
        StartCoroutine(DownloadAndSave(PathURL,name,(state,str,value)=> {
            Debug.Log(value);
            if (value == 1)
            {
                InitAnswer();
            }
        }));
    }

    public void RefreshCurQuestion()
    {
        curData = temp[curPageIdx.ToString()];
        lb_question.text = curData.question;
        lb_answer.text = curData.answer;
    }

    /// <summary>
    /// 下载并保存资源到本地
    /// </summary>
    /// <param name="url"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IEnumerator DownloadAndSave(string url, string name, Action<bool, string,int> Finish = null)
    {
        url = Uri.EscapeUriString(url);
        string Loading = string.Empty;
        bool b = false;
        WWW www = new WWW(url);
        if (www.error != null)
        {
            print("error:" + www.error);
        }
        while (!www.isDone)
        {

            Loading = (((int)(www.progress * 100)) % 100) + "%";
            if (Finish != null)
            {
                Finish(b, Loading,0);
            }
            yield return 1;
        }
        if (www.isDone)
        {
            Loading = "100%";
            byte[] bytes = www.bytes;
            b = SaveAssets(Application.dataPath + "/Data", name, bytes);
            if (Finish != null)
            {
                Finish(b, Loading,1);
            }
        }
    }

    /// <summary>
    /// 保存资源到本地
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <param name="length"></param>
    public static bool SaveAssets(string path, string name, byte[] bytes)
    {
        Stream sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (t.Exists)
        {
            File.Delete(path + "//" + name);
        }
        File.Delete(path + "//" + name);
        try
        {
            sw = t.Create();
            sw.Write(bytes, 0, bytes.Length);
            sw.Close();
            sw.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
