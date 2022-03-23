using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;

namespace WpfApp71
{

  class JSONWorker
  {
    public static void setTextInJson(List<VkText> text)
    {
      DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(List<VkText>));
      using (FileStream fileStream = new FileStream("text.json", FileMode.Create))
      {
        dataContractJsonSerializer.WriteObject(fileStream, text);
      }
    }
    public static void setImagesInJson(List<VkImages> images)
    {
      DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(List<VkImages>));
      using (FileStream fileStream = new FileStream("images.json", FileMode.Create))
      {
        dataContractJsonSerializer.WriteObject(fileStream, images);
      }
    }
    public static void setHrefsInJson(List<VkHrefs> hrefs)
    {
      DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(List<VkHrefs>));
      using (FileStream fileStream = new FileStream("hrefs.json", FileMode.Create))
      {
        dataContractJsonSerializer.WriteObject(fileStream, hrefs);
      }
    }
    public static string readText()
    {
      File.Copy("text.json", "textTemp.json");
      string fileName = "textTemp.json";
      string jsonString = File.ReadAllText(fileName);
      var vkText = JsonConvert.DeserializeObject<List<VkText>>(jsonString);
      string sumString = "";
      foreach(var item in vkText)
      {
        sumString += item.Id + ": " + item.Text;
        sumString += "\n";
      }
      File.Delete("textTemp.json");

      return sumString;
    }
    public static string readImages()
    {
      File.Copy("images.json", "imagesTemp.json");
      string fileName = "imagesTemp.json";
      string jsonString = File.ReadAllText(fileName);
      var vkImages = JsonConvert.DeserializeObject<List<VkImages>>(jsonString);
      string sumString = "";
      foreach (var item in vkImages)
      {
        sumString += item.Id + ": \n";
        foreach(var image in item.Images)
        {
          sumString += image + "\n";
        }
        sumString += "\n";
      }
      File.Delete("imagesTemp.json");

      return sumString;
    }
    public static string readHrefs()
    {
      File.Copy("hrefs.json", "hrefsTemp.json");
      string fileName = "hrefsTemp.json";
      string jsonString = File.ReadAllText(fileName);
      var vkHrefs = JsonConvert.DeserializeObject<List<VkHrefs>>(jsonString);
      string sumString = "";
      foreach (var item in vkHrefs)
      {
        sumString += item.Id + ": \n";
        foreach (var href in item.Href)
        {
          sumString += href + "\n";
        }
        sumString += "\n";
      }
      File.Delete("hrefsTemp.json");

      return sumString;
    }
  }
}
