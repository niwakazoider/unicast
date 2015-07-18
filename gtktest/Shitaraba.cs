using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Gtk;

namespace gtktest
{
	public class Shitaraba
	{
		bool ready = false;
		bool stop = false;
		int no = 0;
		int nores = 0;
		string urlbase = "http://jbbs.shitaraba.net/bbs/rawmode.cgi/game/32713/1406136363/";
		private int reloadtime = 30;
		private Thread reloadthread;
		public List<ShitarabaInterface> listeners = new List<ShitarabaInterface> ();

		public Shitaraba (string url)
		{
			urlbase = url;
		}
		
		public void AddEventListener(ShitarabaInterface listener){
			listeners.Add(listener);
		}
		public void RemoveEventListener(ShitarabaInterface listener){
			listeners.Remove(listener);
		}

		public void SetReloadTime(int time)
		{
			if (10 <= time && time < 180) {
				reloadtime = time;
			}
		}

		private void Error(){
			stop = true;
			foreach (var listener in listeners) {
				listener.OnServerError ();
			}
		}

		public void Stop(){
			stop = true;
		}

		public void Start(){
			if (reloadthread == null) {
				reloadthread = new Thread (Run);
				reloadthread.Start ();
			}
		}

		public void Run(){
			if (!Check ()) {
				Error();
				Dialog.Error ("URL Error");
			}
			try {
				while (!stop) {
					string body = Get ();
					if(!Parse (body)){
						Error();
						Dialog.Error ("Invalid Thread");
						break;
					}
					if(no >= 1000){
						Error();
						Dialog.Info ("GoTo Next Thread");
						break;
					}
					if(nores>100){
						Error();
						Dialog.Info ("Reload");
						break;
					}
					int count = 0;
					for (int i = 0; i < 60; i++) {
						Thread.Sleep (1000);
						if(stop) break;
						if(count>reloadtime){
							break;
						}
						count++;
					}
				}
			} catch (Exception) {
				if (!stop) {
					Error();
					Dialog.Error ("Server Error");
				}
			}
		}
		private bool Check(){
			string[] p = urlbase.Split (new string[]{"/"}, StringSplitOptions.None);
			if (p.Length < 8)
				return false;
			if (p [0] != "http:")
				return false;
			if (p [1] != "")
				return false;
			if (p [2] != "jbbs.shitaraba.net" && p[2]!="jbbs.shitaraba.jp" && p[2]!="jbbs.livedoor.jp")
				return false;
			if (p [3] != "bbs")
				return false;
			if (p [4] != "read.cgi")
				return false;
			string category = p [5];
			string board = p[6];
			string thread = p[7];
			//http://jbbs.shitaraba.net/bbs/read.cgi/game/32713/1406136363/l50
			urlbase = "http://jbbs.shitaraba.net/bbs/rawmode.cgi/"+category+"/"+board+"/"+thread+"/";
			return true;
		}
		public string Get(){
			if (no >= 1000) {
				return "";
			}
			string url = urlbase+(no+1)+"-";
			System.Console.WriteLine (url);
			WebClient client = new WebClient();
			byte[] data = client.DownloadData(url);
			Encoding enc = Encoding.GetEncoding("EUC-JP");
			return enc.GetString(data);
		}
		private bool Parse(string data){
			List<string> list = new List<string>();
			string[] lines = data.Split ('\n');
			for (int i = 0; i < lines.Length; i++) {
				string line = lines [i];
				string[] p = line.Split(new string[] {"<>"}, StringSplitOptions.None);
				if (p.Length < 5) {
					break;
				}
				string num = p [0];
				//string name = p[1];
				//string mail = p[2];
				//string time = p[3];
				string message = p[4].Replace("<br>","\n");
				list.Add (message);
				//$message =  preg_replace('/<a.+>(.*)<\/a>?/','$1',$message);
				//$message =  preg_replace('/(&gt;&gt;)([0-9\-]+)/',"<a href=\"../test/read.cgi/text/$datnum/$2\" target=\"_blank\">&gt;&gt;$2</a>",$message);
				//$st = mb_ereg_replace('\(.*\)', '', $st);
				//System.Console.WriteLine (num);
				int n = 0;
				if (int.TryParse (num, out n)) {
					no = n;
					if (n >= 1000) {
						foreach (var listener in listeners) {
							listener.OnRes1000 ();
						}
					}
				}
			}
			if (!ready && list.Count == 0) {
				return false;
			}
			if (list.Count > 0) {
				if (!ready) {
					ready = true;
					string last = list [list.Count - 1];
					list.Clear ();
					list.Add (last);
					//return;
				}
				string[] res = list.ToArray ();
				foreach (var listener in listeners) {
					listener.OnNewRes (res);
				}
			}
			if (list.Count == 0) {
				nores++;
			} else {
				nores = 0;
			}
			return true;
		}
	}
}

