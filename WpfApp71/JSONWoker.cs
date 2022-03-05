using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

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
  }
}
