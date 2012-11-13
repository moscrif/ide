using System;
using System.Collections;
using System.Reflection;
using Gtk;
using Gdk;

namespace Moscrif.IDE
{
	public class StockIconsManager
	{
		// icons for path ./resources
		private static string[] stock_icon_names = {
			"application.png",
			"breakpoint.png",
			"bookmark.png",
			"bookmark-clear.png",
			"bookmark-next.png",
			"bookmark-previous.png",
			"close.png",
			"delete.png",
			"about.png",
			"find.png",
			"filter.png",
			"folder.png",
			"folder-new.png",
			"folder-open.png",
			"folder-properties.png",
			"folder-properties-16.png",
			"file-new.png",
			"file-open.png",
			"file-properties.png",
			"file-conditions.png",
			"file-exclude.png",
			"file-image.png",
			"file-database.png",
			"file-text.png",
			"file-ms.png",
			"file-html.png",
			"file.png",
			"preferences.png",
			"project.png",
			"project-new.png",
			"project-unload.png",
			"project-folder.png",
			"project-preferences.png",
			"workspace.png",
			"workspace-tree.png",
			"themes.png",
			"keyboard-shortcuts.png",
			"undo.png",
			"redo.png",
			"refresh.png",
			"rename.png",
			"save.png",
			"save-all.png",
			"save-as.png",
			"quit.png",
			"publish.png",
			"run.png",
			"run-in-console.png",
			"console.png",
			"task.png",
			"compile.png",
			"empty.png",
			"mobil.png",
			"stock-close.png",
			"stock-add.png",
			"device.png",
			"conditions.png",
			"apple.png",
			"android.png",
			"bada.png",
			"symbian.png",
			"windows.png",
			"macos.png",
			"win32.png",
			"error.png",
			"log.png",
			"home.png",
			"libs.png",
			"logo74.png",
			"logo96.png",
			"barier-add.png",
			"barier-delete.png",
			"barier-show.png",
			"barier-movie.png",
			"barier-delete-all.png",
			"zoom-in.png",
			"zoom-original.png",
			"zoom-out.png",
			"editor-image.png",
			"editor-text.png",
			"go-up.png",
			"drive-harddisk.png",
			"edit-copy.png",
			"edit-cut.png",
			"edit-paste.png",
			"emulator-skin.png",
			"workspace-right.png",
			"workspace-left.png",
			"workspace-bottom.png",
			"garbage-collector.png",
			"resolution.png",
			"emulator.png",
			"feedback.png",
			"actions24.png",
			"twitter24.png",
			"twitter12.png",
			"tutorial.png",
			"video.png",
			"showcase.png",
			"api.png",
			"content.png"
		};

		public static void Initialize()
		{
			IconFactory icon_factory = new IconFactory();
			icon_factory.AddDefault();
			
			Assembly entry_asm = System.Reflection.Assembly.GetEntryAssembly();
			
			foreach (string item_id in stock_icon_names) {
				StockItem item = new StockItem(item_id, null, 0, Gdk.ModifierType.ShiftMask, null);
				
				IconSet icon_set = null;
				
				Pixbuf default_pixbuf = null;
				
				//Tools tools = new Tools();
				string file = System.IO.Path.Combine(MainClass.Paths.ResDir, item_id);
				if (System.IO.File.Exists(file)){
					try{
						default_pixbuf = new Pixbuf(file);
					}catch(Exception ex){
						Tool.Logger.Error(ex.Message);
						//continue;
					}
				}
				
				icon_set = new IconSet();
				// (default_pixbuf);
				IconSource icoS = new IconSource();
				
				if (System.IO.File.Exists(file)){
					try{
						IconSource source = new IconSource();				
						source.Pixbuf = new Pixbuf(file);
						source.Size = IconSize.LargeToolbar;
						icon_set.AddSource(source);
					}catch(Exception ex){
						Tool.Logger.Error(ex.Message);
						//continue;
					}
				}

				if (icon_set == null) {
					continue;
				}
				
				icon_factory.Add(item.StockId, icon_set);
				StockManager.Add(item);
			}
		}
	}
}

