using System;
using System.Collections.Generic;
using Gtk;
using Moscrif.IDE.Iface.Entities;

namespace Moscrif.IDE.Editors
{
	public interface IEditor
	{
		/// <summary>
		/// Vrati meno suboru (moze byt null ak je novy subor)
		/// </summary>
		string FileName { get; }

		/// <summary>
		/// Caption
		/// </summary>
		string Caption { get; }

		/// <summary>
		/// Priznak pozmeneneho subora. Ak je modified editor nemoze byt len tak zatvoreny (upozornenie a/alebo vyzva na ulozenie) 
		/// </summary>
		bool Modified { get; }
		
		/// <summary>
		/// Ulozit zmeny (ak su). 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podarilo ulozit subor, inac false.
		/// </returns>
		bool Save();

		/// <summary>
		/// Presuni sa v dokumente na position .
		/// </summary>
		/// <returns>
		/// </returns>
		void GoToPosition(object position);

		/// <summary>
		/// Ulozit Ako .
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podarilo ulozit subor, inac false.
		/// </returns>
		bool SaveAs(string newPath);

		/// <summary>
		/// metoda volana pred zavretim.
		/// </summary>
		void Close();

		/// <summary>
		/// get selectet object .
		/// </summary>
		/// <returns>
		/// return selected object
		/// </returns>
		object GetSelected();

		/// <summary>
		/// Naspet.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podari spet, inac false.
		/// </returns>
		bool Undo();

		/// <summary>
		/// Dopredu.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary dopredu, inac false.
		/// </returns>
		bool Redo();

		/// <summary>
		/// Hladanie.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary  inac false.
		/// </returns>
		bool SearchExpression(SearchPattern expresion);

		/// <summary>
		/// Hladanie dopredu.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary  inac false.
		/// </returns>
		bool SearchNext(SearchPattern expresion);

		/// <summary>
		/// Hladanie dozadu.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary  inac false.
		/// </returns>
		bool SearchPreviu(SearchPattern expresion);

		/// <summary>
		/// Prepisanie.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary  inac false.
		/// </returns>
		bool Replace(SearchPattern expresion);

		/// <summary>
		/// Prepisanie vsetko.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary  inac false.
		/// </returns>
		bool ReplaceAll(SearchPattern expresion);

		/// <summary>
		/// prepise, resp vyhlada vsetko a vraty vysledok
		/// </summary>
		/// <returns>
		/// A <see cref="System.Object"/>, vraty dictionary object,object, prve je pozicia, druhe najdeny objekt.
		/// </returns>
		List<FindResult> FindReplaceAll(SearchPattern expresion);

		/// <summary>
		/// Premenovanie suboru
		/// </summary>
		void Rename(string newName);

		/// <summary>
		/// PRefresh settingu editora
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>, True ak sa podary  inac false.
		/// </returns>
		bool RefreshSettings();

		/// <summary>
		/// UI prvok reprezentujuci editor.
		/// </summary>
		Gtk.Widget Control { get; }

		Gtk.ActionGroup EditorAction  { get; }

		/// <summary>
		/// Activate edtor.
		/// </summary>
		/// <returns>
		/// </returns>
		void ActivateEditor(bool updateStatus);

		/// <summary>
		/// Vyvola sa pri zmene stavu dokumentu (Bez Zmeny na Zo zmenou a naopak).
		/// </summary>
		event EventHandler<ModifiedChangedEventArgs> ModifiedChanged;

		/// <summary>
		/// Zapise do statu baru .
		/// </summary>
		event EventHandler<WriteStatusEventArgs> WriteStatusChange;

	}

	public class ModifiedChangedEventArgs : EventArgs
	{
		public bool State {
			get;
			set;
		}

		public ModifiedChangedEventArgs(bool state){
			this.State = state;
		}

	}

	public class WriteStatusEventArgs : EventArgs
	{
		public string Message {
			get;
			set;
		}

		public WriteStatusEventArgs(string message){
			this.Message = message;
		}

	}
}

