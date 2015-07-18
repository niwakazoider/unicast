using System;
using Gtk;
using Gdk;

namespace gtktest
{
	public class ImageWindow:Gtk.Window
	{
		private Pixbuf pixbuf;
		private Cairo.Context cr;
		public ImageWindow(string src) : base(src)
		{
			Decorated = false;
			KeepAbove = true;
			Resize (640, 240);
			Move (100, 100);
			//Opacity = 1.0;

			pixbuf = new Pixbuf (src);

			Gtk.Image image = new Gtk.Image ();
			image.Pixbuf = pixbuf;

			EventBox box = new EventBox ();
			box.Add (image);
			box.ButtonPressEvent += new ButtonPressEventHandler (WindowController.OnButtonDragPress);
			box.ExposeEvent += HandleMyWinExposeEvent;

			Add (box);
			
			WindowController.HandleMyWinScreenChanged(this, null); 
			WindowController.SetWindowShapeFromPixbuf (this, image.Pixbuf);

			DestroyEvent += new DestroyEventHandler (delegate(object o, DestroyEventArgs args) {
				if(cr!=null){
					((IDisposable)cr).Dispose();
				}
			});

			ShowAll();
		}

		void HandleMyWinExposeEvent (object o, ExposeEventArgs args)
		{
			GLib.Idle.Add (new GLib.IdleHandler (delegate() {
				if (cr == null) {
					Gtk.Widget widget = ((Gtk.Widget)o);
					cr = Gdk.CairoHelper.Create (widget.GdkWindow);
				}
				WindowController.DrawPixbuf (cr, pixbuf);
				return false;
			}));
		}

	}
}

