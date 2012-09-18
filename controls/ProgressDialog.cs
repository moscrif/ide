using Gtk;
using GtkSharp;
using System;

using Mono.Unix;

namespace Moscrif.IDE.Controls
{
	public partial class ProgressDialog : Gtk.Dialog
	{
		private bool cancelled;

	private void HandleResponse (object me, ResponseArgs args)
	{
		cancelled = true;
	}

	public enum CancelButtonType {
		Cancel,
		Stop,
		None
	};

	private int totalCount;

	private ProgressBar progressBar;
	public ProgressBar Bar {
		get { return progressBar; }
	}

	private Label messageLabel;
	public Label Message {
		get { return messageLabel; }
	}

	//private DateTime start_time;

	private Gtk.Button button;
	public Gtk.Button Button {
		get {
			return button;
		}
	}
	public ProgressDialog()
	{
		this.Build();
	}

	public ProgressDialog (string title, CancelButtonType cancel_button_type, int totalCount, Gtk.Window parent_window)
	{
		this.WidthRequest = 350;
		Title = title;
		this.totalCount = totalCount;

		if (parent_window != null)
			this.TransientFor = parent_window;
		this.Modal =true;

		HasSeparator = false;
		BorderWidth = 6;
		SetDefaultSize (300, -1);

		messageLabel = new Label (String.Empty);
		VBox.PackStart (messageLabel, true, true, 12);

		progressBar = new ProgressBar ();
		VBox.PackStart (progressBar, true, true, 6);

		switch (cancel_button_type) {
		case CancelButtonType.Cancel:
			button = (Gtk.Button)AddButton (Gtk.Stock.Cancel, (int) ResponseType.Cancel);
			break;
		case CancelButtonType.Stop:
			button = (Gtk.Button)AddButton (Gtk.Stock.Stop, (int) ResponseType.Cancel);
			break;
		}

		Response += new ResponseHandler (HandleResponse);
		ShowAll ();
		while (Application.EventsPending ())
			Application.RunIteration ();
	}

	public ProgressDialog (string title, Gtk.Window parent_window)
	{
		this.WidthRequest = 350;
		Title = title;
		this.totalCount = 10;

		if (parent_window != null)
			this.TransientFor = parent_window;
		this.Modal =true;

		HasSeparator = false;
		BorderWidth = 6;
		SetDefaultSize (300, -1);

		messageLabel = new Label (String.Empty);
		VBox.PackStart (messageLabel, true, true, 12);

		progressBar = new ProgressBar ();
		VBox.PackStart (progressBar, true, true, 6);

		Response += new ResponseHandler (HandleResponse);
		ShowAll ();
		while (Application.EventsPending ())
			Application.RunIteration ();
	}

	public void Reset(int totalCount, string message){
		this.totalCount = totalCount;
		currentCount = 0;
		progressBar.Fraction = 0.0;

		if(message.Length> 50)
			message = message.Substring(message.Length-50,50);

		progressBar.Text = message;
		messageLabel.Text = message;
		
		while (Application.EventsPending ())
			Application.RunIteration ();
	}


	private int currentCount;

	public void SetLabel(string message){

		if(message.Length> 50)
			message = message.Substring(message.Length-50,50);

		messageLabel.Text = message;

		ShowAll ();

		while (Application.EventsPending ())
			Application.RunIteration ();
	}

	// Return true if the operation was cancelled by the user.
	public bool Update (string message)
	{
		currentCount ++;

		if(message.Length> 50)
			message = message.Substring(message.Length-50,50);

		messageLabel.Text = message;
		progressBar.Text = String.Format ("{0} of {1}", currentCount, totalCount);
		progressBar.Fraction = (double) currentCount / totalCount;

		ShowAll ();

		while (Application.EventsPending ())
			Application.RunIteration ();

		return cancelled;
	}

	public void AutomaticUpdate ()
	{
		/*currentCount ++;
		if (currentCount > 10)
			currentCount = 0;

		progressBar.Fraction = (double)currentCount/10;// / totalCount;
		ShowAll ();

		while (Application.EventsPending ())
			Application.RunIteration ();*/


		Gtk.Application.Invoke(delegate {
			currentCount ++;
			if (currentCount > 10)
				currentCount = 0;

			progressBar.Fraction = (double)currentCount/10;
			});

	}


}
}

