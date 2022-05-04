using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.IO;

namespace WpfApp71
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      File.Delete("text.json");
      File.Delete("images.json");
      File.Delete("hrefs.json");
      File.Delete("textTemp.json");
      File.Delete("hrefsTemp.json");
      File.Delete("imagesTemp.json");

      InitializeComponent();
    }
    private string[] getHrefs(IWebElement item)
    {
      List<IWebElement> webElementsHref = item.FindElements(By.TagName("a")).ToList(); // Ищем все теги ссылок
      List<string> Hrefs = new List<string>();

      foreach (var hrefs in webElementsHref) // Оставляем только нужные
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

      for (int i = 0; i < imagesStyle.Count(); i++)
      {
        int start = imagesStyle[i].IndexOf('(');
        int end = imagesStyle[i].IndexOf(')');

        imagesStyle[i] = imagesStyle[i].Substring(start + 1, end - start);
      }

      foreach (var tempimage in webElementsImages)
      {
        if (tempimage.GetAttribute("src") == null)
          continue;
        if (tempimage.GetAttribute("class").Trim() != "PagePostLimitedThumb")
          continue;
        images.Add(tempimage.GetAttribute("src"));
      }

      foreach (var tempimage in imagesStyle)
      {
        images.Add(tempimage);
      }

      return images.ToArray();

    }
    private string getText(IWebElement item)
    {
      try
      {
        return item.FindElement(By.ClassName("wall_post_text")).Text;
      }
      catch
      {
        return null;
      }

    }

    /// <summary>
    /// Читатет информацию из файла
    /// </summary>
    /// <param name="option"> 
    /// images 
    /// text
    /// hrefs
    /// </param>
    private void setDataInLabel(string option)
    {
      if (option == "text")
        MessageBox.Show(JSONWorker.readText());
      if (option == "hrefs")
        MessageBox.Show(JSONWorker.readHrefs());
      if (option == "images")
        MessageBox.Show(JSONWorker.readImages());
    }

    public static List<string> uniqId = new List<string>();

    private void parseFeedRow(ChromeDriver chromeDriver, List<VkText> texts, List<VkImages> images, List<VkHrefs> hrefs)
    {

      List<IWebElement> webElements = chromeDriver.FindElements(By.TagName("div")).ToList();

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
        if (item.GetAttribute("class") == null)
          continue;
        if (!item.GetAttribute("class").Trim().Equals("feed_row"))
          continue;
        IWebElement webElementId = item.FindElement(By.TagName("div"));
        if (webElementId == null)
          continue;
        if (webElementId.GetAttribute("id") == null)
          continue;

        if (!uniqId.Exists(x => x == webElementId.GetAttribute("id").ToString()))
        {
          uniqId.Add(webElementId.GetAttribute("id").ToString());
        }
        else continue;

        VkText vkText = new VkText()
        {
          Text = getText(item),
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
    }

    /// <summary>
    /// Обёртка для parametrizedThreadStart
    /// </summary>
    /// <param name="i">
    /// text
    /// images
    /// hrefs
    /// </param>
    private void setDataInLabelObj(object i)
    {
      Console.WriteLine("Работает поток 4");

      setDataInLabel((string)i);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      ChromeOptions chromeOptions = new ChromeOptions();
      chromeOptions.AddArgument(@"user-data-dir=C:\Users\man21\AppData\Local\Google\Chrome\User Data");
      ChromeDriver chromeDriver = new ChromeDriver(chromeOptions);
      chromeDriver.Navigate().GoToUrl("https://vk.com/feed");

      List<VkText> texts = new List<VkText>();
      List<VkImages> images = new List<VkImages>();
      List<VkHrefs> hrefs = new List<VkHrefs>();

      Thread thread1 = new Thread(() => JSONWorker.setTextInJson(texts));
      Thread thread2 = new Thread(() => JSONWorker.setImagesInJson(images));
      Thread thread3 = new Thread(() => JSONWorker.setHrefsInJson(hrefs));
      Thread thread4 = new Thread(new ParameterizedThreadStart(setDataInLabelObj));

      int j = 0; // Итератор для работы thread4
      int key = 0; // Блокировка потоков при работе thread4

      for (int i = 0; i < 10; i ++)
      {
        Console.WriteLine($"\n\nИтерация номер - {i + 1}\n\n");

        if ((i != 0) && (i % 4 != 0))
        {
          if (i == 1)
            thread4.Start("text");
          else
          {
            thread4 = new Thread(new ParameterizedThreadStart(setDataInLabelObj));
          }


          if (j == 0)
          {
            key = 1;
            if (i != 1)
            {
              thread4.Start("text");
            }
          }
          else if (j == 1)
          {
            key = 2;
            thread4.Start("images");
          }
          else
          {
            key = 3;
            thread4.Start("hrefs");
          }
          j++;
          if (j == 3)
            j = 0;

          thread4.Join();
          thread4.Interrupt();
        }
        else
        {
          key = 0;
        }


        parseFeedRow(chromeDriver, texts, images, hrefs);


        if (i == 0)
        {
          thread1.Start();
        }
        else if (key != 1)
        {
          thread1 = new Thread(() => JSONWorker.setTextInJson(texts));
          thread1.Start();
        }

        if (i == 0)
        {
          thread2.Start();
        }
        else if (key != 2)
        {
          thread2 = new Thread(() => JSONWorker.setImagesInJson(images));
          thread2.Start();
        }

        if (i == 0)
        {
          thread3.Start();
        }
        else if (key != 3)
        {
          thread3 = new Thread(() => JSONWorker.setHrefsInJson(hrefs));
          thread3.Start();
        }


        thread1.Join();
        thread2.Join();
        thread3.Join();
        thread1.Interrupt();
        thread2.Interrupt();
        thread3.Interrupt();

        chromeDriver.Navigate().Refresh();
      }
    }
  }
}
