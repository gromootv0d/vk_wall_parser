using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace sumkin_Consoleapp
{
    class Program
    {
        //C:\Users\mark\source\repos\sumkin_Consoleapp\sumkin_Consoleapp\bin\Debug\netcoreapp3.1

        private static AutoResetEvent evt = new AutoResetEvent(false);
        public static void d1()
        {
            evt.WaitOne();
            string fileName = "out.txt";

            FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);
            sw.WriteLine("123");
            sw.Close();
        }

        public static void d2()
        {

            string fileName = "out.txt";
            FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);
            sw.WriteLine("abc");
            Thread.Sleep(600);
            sw.Close();
            evt.Set();
        }
        public static (List<string>, List<string>, List<string>, string[][]) start_all(IWebDriver driver)
        {


            string wall_text;
            string wall_id;
            string wall_link;
            var wall_image = new List<string>();

            //List<List<string>> wall_image_l = new List<List<string>>();
            var wall_image_l = new List<List<string>>();

            List<string> wall_id_l = new List<string>();
            List<string> wall_text_l = new List<string>();
            List<string> wall_link_l = new List<string>();

            driver.Navigate().Refresh();
            IReadOnlyList<IWebElement> posts = driver.FindElements(By.ClassName("feed_row"));
            string[][] images_mas = new string[10][];
            int coutner_for_photos = 0;
            foreach (var post in posts)
            {
                try
                {
                    var kek = post.FindElement(By.ClassName("feed_rows_next")); // фильтр (полные посты)                      
                }
                catch
                {
                    try
                    {
                        var kek = post.FindElement(By.ClassName("wall_text_name_explain_promoted_post")); //фильтр рекламы
                    }
                    catch
                    {

                        try
                        {

                            wall_id = post.FindElement(By.ClassName("_post")).GetAttribute("data-post-id").Remove(0, 1);
                            wall_id_l.Add(wall_id);

                            wall_link = "https://vk.com/wall-" + wall_id;
                            wall_link_l.Add(wall_link);

                            try
                            { //есть ли текст
                                wall_text = post.FindElement(By.ClassName("wall_post_text")).Text;
                                //Console.WriteLine("wall text = {0}", wall_text);
                            }
                            catch
                            {
                                //Console.WriteLine("wall text = пусто ");
                                wall_text = "";
                            }
                            wall_text_l.Add(wall_text);
                            try
                            { //есть ли картнки
                                IWebElement check = post.FindElement(By.ClassName("wall_text"));
                                var wall_photos = check.FindElements(By.ClassName("image_cover"));
                                //wall_image.Clear();
                                //делаем массив картинок
                                images_mas[coutner_for_photos] = new string[wall_photos.Count];
                                for (int e = 0; e < wall_photos.Count; e++)
                                {
                                    var background_image_property = wall_photos[e].GetCssValue("background-image");
                                    background_image_property = background_image_property.Remove(0, 5);
                                    background_image_property = background_image_property.Remove(background_image_property.Length - 1);
                                    background_image_property = background_image_property.Remove(background_image_property.Length - 1);
                                    images_mas[coutner_for_photos][e] = background_image_property;
                                    Console.WriteLine();
                                    //wall_image.Add(background_image_property);

                                }
                                wall_image_l.Add(wall_image);
                            }
                            catch
                            {

                                Console.WriteLine("Я НЕ ПЕРЕЗАПИСАЛ");
                            }
                            coutner_for_photos += 1;

                        }
                        catch (OpenQA.Selenium.NoSuchElementException)
                        {
                            Console.WriteLine("fuck");
                        }


                    }
                }


            }

            return (wall_id_l, wall_link_l, wall_text_l, images_mas);
        }

        public class Json_s
        {
            public string Id_post { get; set; }
            public string Link_post { get; set; }
            public string Text_post { get; set; }
            public string[] Images_post { get; set; }

        }
        public static void write_to_JSON((List<string>, List<string>, List<string>, string[][]) v1)
        //public static void write_to_JSON((System.Collections.Generic.List<string>, System.Collections.Generic.List<string>, System.Collections.Generic.List<string>, string[][]) v1)
        {
            List<Json_s> peoList = new List<Json_s>()
            {
                new  Json_s {Id_post = v1.Item1[0], Link_post = v1.Item2[0], Text_post = v1.Item3[0], Images_post = v1.Item4[0]},
                new  Json_s {Id_post = v1.Item1[1], Link_post = v1.Item2[1], Text_post = v1.Item3[1], Images_post = v1.Item4[1]},
                new  Json_s {Id_post = v1.Item1[2], Link_post = v1.Item2[2], Text_post = v1.Item3[2], Images_post = v1.Item4[2]},
                new  Json_s {Id_post = v1.Item1[3], Link_post = v1.Item2[3], Text_post = v1.Item3[3], Images_post = v1.Item4[3]},
                new  Json_s {Id_post = v1.Item1[4], Link_post = v1.Item2[4], Text_post = v1.Item3[4], Images_post = v1.Item4[4]},
                new  Json_s {Id_post = v1.Item1[5], Link_post = v1.Item2[5], Text_post = v1.Item3[5], Images_post = v1.Item4[5]},
                new  Json_s {Id_post = v1.Item1[6], Link_post = v1.Item2[6], Text_post = v1.Item3[6], Images_post = v1.Item4[6]},
                new  Json_s {Id_post = v1.Item1[7], Link_post = v1.Item2[7], Text_post = v1.Item3[7], Images_post = v1.Item4[7]},
                new  Json_s {Id_post = v1.Item1[8], Link_post = v1.Item2[8], Text_post = v1.Item3[8], Images_post = v1.Item4[8]},
                new  Json_s {Id_post = v1.Item1[9], Link_post = v1.Item2[9], Text_post = v1.Item3[9], Images_post = v1.Item4[9]},

            };
            //сериализация
            var json = JsonConvert.SerializeObject(peoList);
            StreamWriter file = new StreamWriter("user.json");
            file.WriteLine(json);
            file.Close();
            Console.WriteLine("SERIALIZATION DONE");


        }

        public static void deseriliziation()
        {
            //десериализация
            var jsonString = File.ReadAllText("user.json");
            List<Json_s> lista = new List<Json_s>(JsonConvert.DeserializeObject<List<Json_s>>(jsonString));
            foreach (var obj in lista)
            {
                Console.WriteLine("=========================");
                Console.WriteLine(obj.Id_post);
                Console.WriteLine(obj.Link_post);
                Console.WriteLine(obj.Text_post);
                Console.WriteLine(obj.Images_post);
                for (int i = 0; i < obj.Images_post.Length; i++)
                {
                    Console.WriteLine(obj.Images_post[i]);
                }
                Console.WriteLine("=========================");
            }
            Console.WriteLine("DESERIALIZATION DONE");
        }

        static void Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments(@"user-data-dir=C:\Users\mark\AppData\Local\Google\Chrome\User Data\Default");
            IWebDriver driver = new ChromeDriver(@"C:\Users\mark\source\repos\sumkin_Consoleapp\sumkin_Consoleapp\", options);

            driver.Navigate().GoToUrl("https://vk.com/feed");
            var v1 = start_all(driver);

            write_to_JSON(v1);
            deseriliziation();

            Thread t = new Thread(d1); Thread t2 = new Thread(d2); t.Start(); t2.Start();
            t.Join();
            t2.Join();


            driver.Close();

            //вывод 1го полученного кортежа
            Console.WriteLine("---------------------------");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("id = {0}", v1.Item1[i]);
                Console.WriteLine("link = {0}", v1.Item2[i]);
                Console.WriteLine("text = {0}", v1.Item3[i]);
                for (int j = 0; j < v1.Item4[i].Length; j++)
                    Console.WriteLine("images  = {0}", v1.Item4[i][j]);

            }

            Console.WriteLine("END");

        }



    }
}