using Hopper;
using Weland;
using Gtk;
using System;
using System.Text;
using System.Collections.Generic;

public class Plugin {
  public static bool Compatible() { 
    return true;
  }

  public static string Name() {
    return "5-D Space Intersections";
  }

  public static void GtkRun(Editor editor) {
    Window[] all_top = Window.ListToplevels();
    Window frontmost = all_top[1]; // is this right for non-Mac?
    if (frontmost == null) {
      frontmost = all_top[0];
    }
    
    try {
      List<Tuple<int, int>> coll = Hop5DSpace.Intersections(editor.Level);
    
      string msg = String.Format("{0} intersections occur in 5-D space.", coll.Count);
      StringBuilder dmsg = new StringBuilder();
      for (int i = 0; i < coll.Count; ++i) {
        dmsg.AppendFormat("Polygons {0} and {1}", coll[i].Item1, coll[i].Item2);
        dmsg.AppendLine();
      }
    
      MessageDialog m = new MessageDialog(frontmost,
                              DialogFlags.DestroyWithParent,
                              MessageType.Info,
                              ButtonsType.Close,
                              msg);
      m.Title = "5-D Space Intersections";
      m.SecondaryText = dmsg.ToString();
      m.Run();
      m.Destroy();
    } catch (Exception e) {
      MessageDialog m = new MessageDialog(frontmost,
                              DialogFlags.DestroyWithParent,
                              MessageType.Error,
                              ButtonsType.Close,
                              "An error occured while running this plugin.");
      m.Title = "Plugin error";
      m.SecondaryText = string.Concat(e.Message, e.StackTrace);
      m.Run();
      m.Destroy();
    }
  }      
}
