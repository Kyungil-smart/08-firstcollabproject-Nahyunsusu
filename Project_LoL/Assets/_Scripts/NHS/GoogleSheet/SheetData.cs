using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct SheetData
{
    [field: SerializeField] public string Url { get; private set; }

    // 읽고 있는 시트가 /t로 나눠져있는가 ','로 나눠져있는가
    [field: SerializeField] public SheetType Type { get; private set; }
    public char spliterSymbol => Type == SheetType.CSV ? ',' : '\t';

    public IEnumerator Load(Action<char, string[]> successCallback)
    {
        // 1. url 자르기

        // https://docs.google.com/spreadsheets/d/15LcRjBhDheyTDnpH5EAIoLbXNq-IAr8IwRnNzCeBzxk/edit?gid=2001857375#gid=2001857375

        string sheetId = Url.Split("d/")[1].Split('/')[0];
        string gid = Url.Split("gid=")[1].Split('&')[0].Split('#')[0];

        string format = Type == SheetType.CSV ? "csv" : "tsv";

        // 2. tsv로 받아오도록 하기 (접속할 url 재설정)
        string exportUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format={format}&gid={gid}";

        // 3. 웹 요청
        using (UnityWebRequest uwr = UnityWebRequest.Get(exportUrl))
        {
            // 3.1 요청보내고 응답받을때까지 대기하기
            yield return uwr.SendWebRequest();

            //  3.1 실패라면 에러 발생시키기
            if(uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(uwr.error);
                yield break;
            }

            //  3.2 성공적으로 받아왔으면 반영하기
            string sheetDataText = uwr.downloadHandler.text;
            string[] lines = sheetDataText.Replace("\r", "").Split('\n');

            successCallback?.Invoke(spliterSymbol, lines);
            Debug.Log("<color=green>Success Loaded Google Sheet Data</color>");
        }
    }
}

public enum SheetType
{
    CSV, TSV
}
