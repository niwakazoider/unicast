using System;
using Gtk;

namespace gtktest
{
	public class Dialog
	{
		
		public static void Error(string msg){
			GLib.Idle.Add (new GLib.IdleHandler (delegate() {
				MessageDialog md = new MessageDialog (null, DialogFlags.DestroyWithParent, 
					MessageType.Error, ButtonsType.Ok, msg); 
				md.DeleteEvent += delegate(object o, DeleteEventArgs args) {
					md.Destroy();
				};
				md.Response += delegate(object o, ResponseArgs args) {
					md.Hide();
					md.Destroy();
				};
				md.Show (); 
				return false;
			}));
		}

		public static void Info(string msg){
			GLib.Idle.Add (new GLib.IdleHandler (delegate() {
				MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, 
					MessageType.Info, ButtonsType.Ok, msg); 
				md.DeleteEvent += delegate(object o, DeleteEventArgs args) {
					md.Destroy();
				};
				md.Response += delegate(object o, ResponseArgs args) {
					md.Hide();
					md.Destroy();
				};
				md.Show (); 
				return false;
			}));
		}

	}
}

