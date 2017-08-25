using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace Misora
{
	public static class MisoraCore
	{
		private static DiscordSocketClient _client = new DiscordSocketClient();
		private static string _nowPlayingFile = @"C:\Users\Karuta\Documents\Misc\nowPlaying.txt";
		private static Timer _timer;

		internal static async Task<bool> Login(string token)
		{
			try
			{
				_client.Ready += MonitorPlayback;
				await _client.LoginAsync(TokenType.User, token);
				await _client.StartAsync();
				SaveToken(token);
				return true;
			}catch(Exception)
			{
				return false;
			}
		}


		internal static async Task Logout()
		{
			await _client.LogoutAsync();
			await _client.StopAsync();
		}

		internal static void SaveToken(string token)
			=> File.WriteAllText("token", token);

		internal static string LoadToken()
		{
			try
			{
				return File.ReadAllText("token");
			}catch(IOException)
			{
				return null;
			}
		}


		internal static Task MonitorPlayback()
		{
			if(!File.Exists(_nowPlayingFile))
			{
				MessageBox.Show($"Now playing file does not exist: \"{_nowPlayingFile}\"", "File Not Found");
				return Task.CompletedTask;
			}
			string lastSong = "";
			_timer = new Timer(async (x) =>
			{
				string nowPlaying = File.ReadAllText(_nowPlayingFile);
				Debug.WriteLine(nowPlaying);
				if(nowPlaying != "stopped")
				{
					if(nowPlaying.StartsWith("paused:"))
					{
						if (lastSong == null)
							return;
						lastSong = null;
						await _client.SetGameAsync(null);
					}else
					{
						string songName = nowPlaying.Remove(0, 9);
						if (lastSong == songName)
							return;
						lastSong = songName;
						await _client.SetGameAsync(songName);
					}
					var u = _client.CurrentUser;
					Debug.WriteLine((u.Game.HasValue) ? u.Game.Value.Name : "");
				}
			}, null, 0, 500);
			return Task.CompletedTask;
		}
	}
}
