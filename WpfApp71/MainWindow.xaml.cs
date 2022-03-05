using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using WpfApp71;

namespace WpfApp71
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void setTextInWebElement(ChromeDriver chromeDriver, string id, string value)
        {
            List<IWebElement> webElements = chromeDriver.FindElements(By.Id(id)).ToList();
            foreach (var item in webElements)
            {
                if (!item.Displayed)
                    continue;
                item.SendKeys(value);
            }
        }
        private void setClickInDate(ChromeDriver chromeDriver, string XPath, string id)
        {
            List<IWebElement> webElements = chromeDriver.FindElements(By.XPath(XPath)).ToList();
            foreach (var item in webElements)
            {
                if (!item.Displayed)
                    continue;
                item.Click();
                break;
            }
            //string pc = chromeDriver.PageSource;
            webElements = chromeDriver.FindElements(By.Id(id)).ToList();
            foreach (var item in webElements)
            {
                if (!item.Displayed)
                    continue;
                item.Click();
                break;
            }
        }
        private string[] getHrefs(IWebElement item)
    {
        List<IWebElement> webElementsHref = item.FindElements(By.TagName("a")).ToList(); // Ищем все теги ссылок
        List<string> Hrefs = new List<string>();

        foreach(var hrefs in webElementsHref) // Оставляем только нужные
        {
          if (hrefs == null)
            continue;
          if (hrefs.GetAttribute("target") != "_blank")
            continue;
          if (hrefs.GetAttribute("rel") != "nofollow noopener")
            continue;
        Hrefs.Add(hrefs.GetAttribute("href"));
        }

      return Hrefs.ToArray();
    }
        private string[] getImages(IWebElement item)
    {

      List<IWebElement> webElementsImages = item.FindElements(By.TagName("img")).ToList();
      List<string> images = new List<string>();
      List<IWebElement> webElementsImagesPack = item.FindElements(By.TagName("a")).ToList();
      List<string> imagesStyle = new List<string>();

      foreach (var temp in webElementsImagesPack)
      {
        if (temp.GetAttribute("aria-label") != "фотография")
          continue;
        imagesStyle.Add(temp.GetAttribute("style"));
      }

      if (imagesStyle.Count() == 1)
        imagesStyle.Clear();

      for(int i=0;i<imagesStyle.Count();i++)
      {
        int start = imagesStyle[i].IndexOf('(');
        int end = imagesStyle[i].IndexOf(')');

        imagesStyle[i] = imagesStyle[i].Substring(start + 1, end - start);
      }

      foreach(var tempimage in webElementsImages)
      {
        if (tempimage.GetAttribute("src") == null)
          continue;
       if (tempimage.GetAttribute("class").Trim() != "PagePostLimitedThumb")
          continue;
        images.Add(tempimage.GetAttribute("src"));
      }
      
      foreach(var tempimage in imagesStyle)
      {
        images.Add(tempimage);
      }

      return images.ToArray();

    }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument(@"user-data-dir=C:\Users\student\AppData\Local\Google\Chrome\User Data");
            ChromeDriver chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Navigate().GoToUrl("https://vk.com/feed");
            List<IWebElement> webElements = chromeDriver.FindElements(By.TagName("div")).ToList();

            List<VkText> texts = new List<VkText>();
            List<VkImages> images = new List<VkImages>();
            List<VkHrefs> hrefs = new List<VkHrefs>();

            foreach (var item in webElements)
            {
                try
                {
                    if (!item.Displayed)
                        continue;
                }
                catch (Exception)
                {
                    continue;
                }
                if (item.GetAttribute("class")==null)
                    continue;
                if (!item.GetAttribute("class").Trim().Equals("feed_row"))
                    continue;
                IWebElement webElementId = item.FindElement(By.TagName("div"));
                if (webElementId == null)
                    continue;
                if (webElementId.GetAttribute("id") == null)
                    continue;

                VkText vkText = new VkText()
                {
                    Text = item.Text,
                    Id = webElementId.GetAttribute("id")
               };
                VkImages vkImages = new VkImages()
                {
                  Id = webElementId.GetAttribute("id"),
                  Images = getImages(item)
                };
                VkHrefs vkHrefs = new VkHrefs()
                {
                  Id = webElementId.GetAttribute("id"),
                  Href = getHrefs(item)
                };

                texts.Add(vkText);
                images.Add(vkImages);
                hrefs.Add(vkHrefs);
            }

              JSONWorker.setTextInJson(texts);
              JSONWorker.setImagesInJson(images);
              JSONWorker.setHrefsInJson(hrefs);
    }
    }
}
