namespace st_meta_view.Logic
{
  public class AdaptiveComparer : IComparer<object?>
  {
    public int Compare(object? x, object? y)
    {
      if (x is null && y is null)
        return 0;
      if (x is null)
        return 1;
      if (y is null)
        return -1;

      if (float.TryParse(x.ToString(), out float fx) && float.TryParse(y.ToString(), out float fy))
        return fx.CompareTo(fy);

      if (x is Dictionary<string, object?> && y is Dictionary<string, object?>)
        return 0;

      if (x is Dictionary<string, object?>)
        return 1;

      if (y is Dictionary<string, object?>)
        return -1;

      return x.ToString()!.CompareTo(y.ToString());
    }
  }
}
