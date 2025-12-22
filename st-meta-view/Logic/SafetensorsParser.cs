using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace st_meta_view.Logic
{
  public class SafetensorsParser
  {
    private readonly JsonSerializerOptions options = new()
    {
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      WriteIndented = true
    };

    public string? ReadSafetensorsMetadata(string safetensorsFullPath)
    {
      using var file = File.Open(safetensorsFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      using var br = new BinaryReader(file);
      var header = br.ReadBytes(8);

      var firstByte = br.ReadByte();
      if (firstByte != 123)
        throw new Exception("Not a valid Safetensors file.");

      file.Position -= 1;
      var ullength = BitConverter.ToUInt64(header, 0);
      if (ullength > int.MaxValue)
        throw new Exception(string.Format("Error: The metadata is too long, longer than {0}.", int.MaxValue));

      var ilenght = Convert.ToInt32(ullength);
      var bytes = br.ReadBytes(ilenght);

      var jsonString = Encoding.UTF8.GetString(bytes);
      string? metadataString = null;

      using var jsonDoc = JsonDocument.Parse(jsonString);
      if (jsonDoc.RootElement.TryGetProperty("__metadata__", out JsonElement elementMetadata))
      {
        metadataString = elementMetadata.GetRawText();
      }

      return metadataString;
    }

    public Dictionary<string, object?>? JsonStringToDictionary(string metadataString)
    {
      using var jsonDoc = JsonDocument.Parse(metadataString);

      var metadata = ExpandElement(jsonDoc.RootElement, "metadata");

      return metadata;
    }

    private Dictionary<string, object?> ExpandElement(JsonElement element, string key)
    {
      var returnDict = new Dictionary<string, object?>();

      switch (element.ValueKind)
      {
        case JsonValueKind.Object:

          foreach (JsonProperty property in element.EnumerateObject())
          {
            returnDict.Add(property.Name, ExpandElement(property.Value, property.Name));
          }
          break;

        case JsonValueKind.Array:

          int idx = 0;

          foreach (JsonElement item in element.EnumerateArray())
          {
            var nk = string.Format("{0}-{1}", key, ++idx);
            returnDict.Add(nk, ExpandElement(item, nk));
          }
          break;

        case JsonValueKind.String:

          try
          {
            using (var doc = JsonDocument.Parse(element.GetString()!))
            {
              returnDict = ExpandElement(doc.RootElement, key);
            }
          }
          catch
          {
            returnDict.Add(key, element.GetString());
          }
          break;

        case JsonValueKind.Number:
          returnDict.Add(key, element.GetDouble());
          break;

        case JsonValueKind.True:
          returnDict.Add(key, true);
          break;
        case JsonValueKind.False:
          returnDict.Add(key, false);
          break;
        case JsonValueKind.Null:
        case JsonValueKind.Undefined:
          returnDict.Add(key, null);
          break;
        default:
          returnDict.Add(key, element.GetRawText());
          break;
      }

      return returnDict;
    }

    public bool WriteMetadataToJsonFile(string metadataFullPath, Dictionary<string, object?> metadata)
    {
      var fileOutPath = metadataFullPath + ".json";
      if (!File.Exists(fileOutPath))
      {
        string metadataString = JsonSerializer.Serialize(metadata, options);
        using var fileout = File.OpenWrite(fileOutPath);
        using var sw = new StreamWriter(fileout, Encoding.UTF8);
        sw.Write(metadataString);
        return true;
      }
      else
      {
        return false;
      }
    }
  }
}
