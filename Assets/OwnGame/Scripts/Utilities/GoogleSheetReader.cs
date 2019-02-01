using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GoogleSheetReader
{

	public bool isError;
	public GoogleSheetError error;
	public string rawResult;

	public string spreadsheetId { get; private set; }

	public string apiKey { get; private set; }

	public GoogleSheetReader (string apiKey, string spreadsheetId)
	{
		this.spreadsheetId = spreadsheetId;
		this.apiKey = apiKey;
	}

	public IEnumerator Load (string fromCell, string toCell, string sheetId)
	{
		return Load (fromCell, toCell, sheetId, false);
	}

	public IEnumerator Load (string fromCell, string toCell, string sheetId, bool columnsDimension)
	{
		isError = false;
		error = null;
		rawResult = null;

		var format = "https://sheets.googleapis.com/v4/spreadsheets/{0}/values/'{1}'!{2}:{3}?key={4}&majorDimension={5}";
		var url = string.Format (format, spreadsheetId, sheetId, fromCell, toCell, apiKey, columnsDimension ? GoogleSheetDimension.COLUMNS : GoogleSheetDimension.ROWS);

		var www = UnityWebRequest.Get (url);
		yield return www.Send ();

		isError = www.isNetworkError;
		if (isError) {
			error = new GoogleSheetError ();
			error.message = www.error;
		} else {
			rawResult = www.downloadHandler.text;
			var json = JSON.Parse (rawResult);
			var jsonErr = json ["error"];
			if (jsonErr != null) {
				isError = true;
				error = JsonUtility.FromJson<GoogleSheetError> (json ["error"].ToString ());
			}
		}
	}

	public JSONArray GetValues ()
	{
		var json = JSON.Parse (rawResult);
		var values = json ["values"].AsArray;
		return values;
	}

}

public class GoogleSheetError
{
	public int code;
	public string message;
	public string status;

	public override string ToString ()
	{
		return string.Format ("Error {0} - {1}: {2}", code, status, message);
	}
}

public enum GoogleSheetDimension
{
	DIMENSION_UNSPECIFIED,
	ROWS,
	COLUMNS,
}