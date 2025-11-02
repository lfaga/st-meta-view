using st_meta_view.Logic;
using System.Windows;

namespace st_meta_view
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

#if DEBUG
      //for now and to test, the arguments are hardcoded
      var path = @"E:\Tools\AI\LoRAs\SD1.5\Borg_Drone_v1\BorgDrone.safetensors";
      var isDump = false;
#else
      string path;
      var isDump = false;

      if (e.Args.Length == 1)
      {
        path = e.Args[0];
      }
      else if (e.Args.Length == 2 && e.Args[0] == "-d")
      {
        isDump = true;
        path = e.Args[1];
      }
      else
      {
        MessageBox.Show("You need to pass a valid full path to a valid .safetensors file\n"
          + "Usage: st-meta-view.exe [-d] <safetensors full path and filename>",
          "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        Shutdown();
        return;
      }
#endif

      var s = new SafetensorsParser();
      var metadataString = s.ReadSafetensorsMetadata(path);

      if (metadataString is not null)
      {
        if (isDump)
        {
          try
          {
            if (!s.WriteMetadataToJsonFile(path, metadataString))
            {
              MessageBox.Show(string.Format("{0}.json already exists.", path),
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          }
          catch (Exception ex)
          {
            MessageBox.Show(string.Format("An error ocurred writing the file.\n{0}", ex.Message),
              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
        else
        {
          var metadata = s.JsonStringToDictionary(metadataString);
          if (metadata is not null)
          {
            var w = new MainWindow();
            w.LoadMetadata(path, metadata);
            w.ShowDialog();
          }
          else
          {
            MessageBox.Show("The metadata in the file is malformed or corrupted.", "Metadata Viewer",
              MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }
      }
      else
      {
        MessageBox.Show("The safetensors file doesn't contain metadata.", "Metadata Viewer",
          MessageBoxButton.OK, MessageBoxImage.Exclamation);
      }
      Shutdown();
    }
  }
}
