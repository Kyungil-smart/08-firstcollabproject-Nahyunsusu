using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace _Scripts.LYC.Utils
{
	public static class SheetFetcher
	{
		private static readonly HttpClient Client = new();

		/// <summary>
		/// Google Sheets CSV를 다운로드하여 raw string으로 반환합니다.
		/// </summary>
		/// <param name="sheetId">스프레드시트 ID</param>
		/// <param name="gid">시트 탭 GID (default: 0)</param>
		/// <returns>CSV 원문 string. 실패 시 null</returns>
		public static async Task<string> FetchCsvAsync(string sheetId, string gid = "0")
		{
			string url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={gid}";

			try
			{
				HttpResponseMessage response = await Client.GetAsync(url);
				response.EnsureSuccessStatusCode();

				string csv = await response.Content.ReadAsStringAsync();
				return csv;
			}
			catch (Exception e)
			{
				Debug.LogError($"[{nameof(SheetFetcher)}] 다운로드 실패: {e.Message}");
				return null;
			}
		}
	}
}