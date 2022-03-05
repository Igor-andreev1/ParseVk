using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WpfApp71
{
  [DataContract]
  class VkText
    {
    [DataMember]
    public string Text;
    [DataMember]
    public string Id;
  }

  [DataContract]
  class VkImages
  {
    [DataMember]
    public string Id;
    [DataMember]
    public string[] Images;
  }

  [DataContract]
  class VkHrefs
  {
    [DataMember]
    public string Id;
    [DataMember]
    public string[] Href;
  }

}
