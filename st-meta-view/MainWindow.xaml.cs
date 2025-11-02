using st_meta_view.Properties;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace st_meta_view
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ContentControl? _lastSortClicked;
    private string? _lastSortOrder;

    public MainWindow()
    {
      InitializeComponent();
    }

    public void LoadMetadata(string filename, Dictionary<string, object?> metadata)
    {
      LblFilename.Content = filename;

      foreach (var element in metadata)
      {
        PopulateTree(RootGrid, element);
      }
    }

    private void PopulateTree(Grid parentGrid, KeyValuePair<string, object?> element)
    {
      var headerStyle = Application.Current.FindResource("CustomLabelStyle") as Style;
      var lfsize = GetFontSize();

      if (parentGrid.ColumnDefinitions.Count == 0)
      {
        parentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
        parentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });

        parentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        var lblHeaderKey = new Label { Content = "Key", Style = headerStyle, ToolTip = "Sort by this column", FontSize = lfsize };
        parentGrid.Children.Add(lblHeaderKey);
        Grid.SetColumn(lblHeaderKey, 0);
        Grid.SetRow(lblHeaderKey, parentGrid.RowDefinitions.Count - 1);
        lblHeaderKey.MouseDown += (sender, args) => { OnClickHeaderSort(lblHeaderKey, parentGrid); };

        var lblHeaderValue = new Label { Content = "Value", Style = headerStyle, ToolTip = "Sort by this column", FontSize = lfsize };
        parentGrid.Children.Add(lblHeaderValue);
        Grid.SetColumn(lblHeaderValue, 1);
        Grid.SetRow(lblHeaderValue, parentGrid.RowDefinitions.Count - 1);

        lblHeaderValue.MouseDown += (sender, args) => { OnClickHeaderSort(lblHeaderValue, parentGrid); };
      }

      parentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

      var d = element.Value as Dictionary<string, object?>;

      var lbl1 = new Label { Content = string.Format("{0}: ", element.Key), FontSize = lfsize };
      parentGrid.Children.Add(lbl1);
      Grid.SetColumn(lbl1, 0);
      Grid.SetRow(lbl1, parentGrid.RowDefinitions.Count - 1);

      if (d is not null)
      {
        lbl1.Style = headerStyle;
        lbl1.ToolTip = "Copy the values";
        lbl1.MouseDown += (sender, args) =>
        {
          var sb = new StringBuilder();

          foreach (var entry in d)
          {
            sb.AppendFormat("{0}: {1}", entry.Key,
              entry.Value is Dictionary<string, object?> ? "[dictionary]" : entry.Value);
            sb.AppendLine();
          }

          Clipboard.SetText(sb.ToString());
          SbMessage.Items.Add(new TextBlock { Text = "Structure copied to the clipboard." });
          ClearStatusBarDelayed();
        };

        var newGrid = new Grid { Tag = d };
        parentGrid.Children.Add(newGrid);
        Grid.SetColumn(newGrid, 1);
        Grid.SetRow(newGrid, parentGrid.RowDefinitions.Count - 1);

        foreach (var subelement in d)
        {
          PopulateTree(newGrid, subelement);
        }
      }
      else
      {
        var val = element.Value != null ? element.Value.ToString() : string.Empty;

        lbl1.Style = Application.Current.FindResource("ClickableLabelStyle") as Style;
        lbl1.ToolTip = "Copy the value";
        lbl1.MouseDown += (sender, args) =>
        {
          Clipboard.SetText(val);
          SbMessage.Items.Add(new TextBlock { Text = "Value copied to the clipboard." });
          ClearStatusBarDelayed();
        };

        var lbl2 = new Label { Content = val, FontSize = lfsize };
        parentGrid.Children.Add(lbl2);
        Grid.SetColumn(lbl2, 1);
        Grid.SetRow(lbl2, parentGrid.RowDefinitions.Count - 1);
      }
    }

    private void OnClickHeaderSort(ContentControl lblSender, Grid parentGrid)
    {
      if (lblSender.Parent is Grid grid
        && grid.Tag is Dictionary<string, object?> tmpDic)
      {
        IEnumerable<KeyValuePair<string, object?>> sortedData;

        if (lblSender.Content.ToString() == "Key")
        {
          if (!lblSender.Equals(_lastSortClicked) || _lastSortOrder == "D")
          {
            _lastSortOrder = "A";
            sortedData = from pair in tmpDic
                         orderby pair.Value is Dictionary<string, object?> ? 1 : 0, pair.Key
                         select pair;
          }
          else
          {
            _lastSortOrder = "D";
            sortedData = from pair in tmpDic
                         orderby pair.Value is Dictionary<string, object?> ? 1 : 0, pair.Key descending
                         select pair;
          }
        }
        else //"Value"
        {
          if (!lblSender.Equals(_lastSortClicked) || _lastSortOrder == "D")
          {
            _lastSortOrder = "A";
            sortedData = from pair in tmpDic
                         orderby pair.Value is Dictionary<string, object?> ? 1 : 0, pair.Value.ToString()
                         select pair;
          }
          else
          {
            _lastSortOrder = "D";
            sortedData = from pair in tmpDic
                         orderby pair.Value is Dictionary<string, object?> ? 1 : 0, pair.Value.ToString() descending
                         select pair;
          }
        }

        var toClear = from child in parentGrid.Children.OfType<UIElement>()
                      where Grid.GetRow(child) > 0
                      select child;

        var toClearArray = toClear.ToArray();

        foreach (var l in toClearArray)
          parentGrid.Children.Remove(l);

        while (parentGrid.RowDefinitions.Count > 1)
          parentGrid.RowDefinitions.RemoveAt(1);

        foreach (var data in sortedData)
        {
          PopulateTree(parentGrid, new KeyValuePair<string, object?>(data.Key, data.Value));
        }

      }
      _lastSortClicked = lblSender;
    }

    private async void ClearStatusBarDelayed()
    {
      await Task.Delay(TimeSpan.FromSeconds(2));
      SbMessage.Items.Clear();
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      if (Settings.Default.MainWindowHeight >= 100.0)
      {
        Height = Settings.Default.MainWindowHeight;
      }

      if (Settings.Default.MainWindowWidth >= 100.0)
      {
        Width = Settings.Default.MainWindowWidth;
      }

      if (Settings.Default.MainWindowTop >= 0.0)
      {
        Top = Settings.Default.MainWindowTop;
      }

      if (Settings.Default.MainWindowLeft >= 0.0)
      {
        Left = Settings.Default.MainWindowLeft;
      }
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      if (WindowState == WindowState.Normal)
      {
        Settings.Default.MainWindowHeight = Height;
        Settings.Default.MainWindowWidth = Width;
        Settings.Default.MainWindowTop = Top;
        Settings.Default.MainWindowLeft = Left;
      }
      Settings.Default.Save();

      base.OnClosing(e);
    }

    private static double GetFontSize()
    {
      return Settings.Default.FontSize > 0.0 ? Settings.Default.FontSize : 14.0;
    }

    private void UpdateFontSize(double size)
    {
      if (size < 6.0) size = 6.0;
      if (size > 128.0) size = 128.0;

      SetFontSizeRecursive(RootGrid, size);
      Settings.Default.FontSize = size;
      Settings.Default.Save();
    }

    private static void SetFontSizeRecursive(Grid parentGrid, double size)
    {
      foreach (var child in parentGrid.Children)
      {
        if (child is Label childLabel)
        {
          childLabel.FontSize = size;
        }
        else if (child is Grid childGrid)
        {
          SetFontSizeRecursive(childGrid, size);
        }
      }
    }

    private void BtnFontSmaller_Click(object sender, RoutedEventArgs e)
    {
      UpdateFontSize(GetFontSize() - 2);
    }

    private void BtnFontBigger_Click(object sender, RoutedEventArgs e)
    {
      UpdateFontSize(GetFontSize() + 2);
    }
  }
}