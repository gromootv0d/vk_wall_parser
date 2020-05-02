using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;



namespace sumkin_app2
{

    class Program
    {

        //C:\Users\mark\source\repos\sumkin_Consoleapp\sumkin_Consoleapp\bin\Debug\netcoreapp3.1
        public static (List<string>, List<string>, List<string>, string[][]) start_all(IWebDriver driver)
        {
            string wall_text;
            string wall_id;
            string wall_link;

            List<string> wall_id_l = new List<string>();
            List<string> wall_text_l = new List<string>();
            List<string> wall_link_l = new List<string>();

            driver.Navigate().Refresh();
            IReadOnlyList<IWebElement> posts = driver.FindElements(By.ClassName("feed_row"));
            string[][] images_mas = new string[10][];//2мерный массив чтобы хранить картинки с 10 постой (зубчатый)
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
                            //получаем id
                            wall_id = post.FindElement(By.ClassName("_post")).GetAttribute("data-post-id").Remove(0, 1);
                            wall_id_l.Add(wall_id);
                            //получаем ссылку
                            wall_link = "https://vk.com/wall-" + wall_id;
                            wall_link_l.Add(wall_link);
                            //получаем текст
                            try
                            { //есть ли текст
                                wall_text = post.FindElement(By.ClassName("wall_post_text")).Text;
                            }
                            catch
                            {

                                wall_text = "";
                            }
                            wall_text_l.Add(wall_text);
                            //картиночки
                            try
                            { //есть ли картнки
                                IWebElement check = post.FindElement(By.ClassName("wall_text"));
                                var wall_photos = check.FindElements(By.ClassName("image_cover"));

                                //делаем массив картинок
                                images_mas[coutner_for_photos] = new string[wall_photos.Count];
                                for (int e = 0; e < wall_photos.Count; e++)
                                {
                                    var background_image_property = wall_photos[e].GetCssValue("background-image");
                                    background_image_property = background_image_property.Remove(0, 5);
                                    background_image_property = background_image_property.Remove(background_image_property.Length - 1);
                                    background_image_property = background_image_property.Remove(background_image_property.Length - 1);
                                    images_mas[coutner_for_photos][e] = background_image_property;

                                }
                            }
                            catch { }
                            coutner_for_photos += 1;
                        }
                        catch (OpenQA.Selenium.NoSuchElementException) { }
                    }
                }
            }
            return (wall_id_l, wall_link_l, wall_text_l, images_mas);
        }

        public class Json_s //класс для картежа информации с постов
        {
            public string Id_post { get; set; }
            public string Link_post { get; set; }
            public string Text_post { get; set; }
            public string[] Images_post { get; set; }

        }

        public static void write_to_JSON((List<string>, List<string>, List<string>, string[][]) v1, int file_counter) //запись в файлы
        {
            Console.WriteLine(String.Format("* * поток TA0-{0} начал работу", file_counter));
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
            //StreamWriter file = new StreamWriter("user.json");
            StreamWriter file = new StreamWriter(String.Format("user{0}.json", file_counter));
            file.WriteLine(json);
            file.Close();
            Console.WriteLine(String.Format("* * поток TA0-{0} закончил работу", file_counter));
        }

        public static void deseriliziation(string connStr, int counter_posts_for_bd) //десериализация и запись в бд
        {
            //десериализация
            Console.WriteLine(String.Format("* * поток TB{0} начал работу", counter_posts_for_bd));
            var jsonString = File.ReadAllText(String.Format("user{0}.json", counter_posts_for_bd));
            List<Json_s> lista = new List<Json_s>(JsonConvert.DeserializeObject<List<Json_s>>(jsonString));
            int counter_id = 1; //id в бд
            foreach (var obj in lista)
            {
                string pic_string = ""; //строка со всеми картинками в посте для бд
                for (int i = 0; i < obj.Images_post.Length; i++)
                {
                    pic_string += obj.Images_post[i] + ", ";
                }
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
                string sql = String.Format("INSERT INTO dbpost{0} (id, vk_id, link, text, images) VALUES ({1}, '{2}', '{3}', '{4}', '{5}')", counter_posts_for_bd, counter_id, obj.Id_post, obj.Link_post, obj.Text_post, pic_string);
                MySqlCommand command = new MySqlCommand(sql, conn);
                command.ExecuteNonQuery();
                conn.Close();
                counter_id += 1;
            }
            Console.WriteLine(String.Format("* * поток TB{0} закончил работу", counter_posts_for_bd));
        }

        private static AutoResetEvent evt_TA = new AutoResetEvent(false); //event ожидания конца работы потока TA (ожидает TB)
        private static AutoResetEvent evt_TA0 = new AutoResetEvent(false); //event ожидания конца работы потока TA0 (ожидает TA4)

        public static void TA0((List<string>, List<string>, List<string>, string[][]) v1, (List<string>, List<string>, List<string>, string[][]) v2, (List<string>, List<string>, List<string>, string[][]) v3)
        {
            Console.WriteLine("* * запустился поток ТА0");
            //тут происходит многопоточная запись в json файлы
            var myThread1 = new Thread(() => write_to_JSON(v1, 1)) { IsBackground = true }; myThread1.Start();
            var myThread2 = new Thread(() => write_to_JSON(v2, 2)) { IsBackground = true }; myThread2.Start();
            var myThread3 = new Thread(() => write_to_JSON(v3, 3)) { IsBackground = true }; myThread3.Start();

            myThread1.Join();
            myThread2.Join();
            myThread3.Join();
            Console.WriteLine("* * Поток ТА0 закончил работу");
            //evt_TA0.Set();
        }

        public static void TA4()
        {
            //дожидается окончания потока ТА0 и проверяет наличие информации в файлах (многопоточно)
            //evt_TA0.WaitOne();
            Console.WriteLine("* * поток ТА4 начал работу");
            var TA4_1 = new Thread(() => {
                StreamReader sr = new StreamReader("user1.json");
                string check = sr.ReadToEnd();
                if (check == "") { Console.WriteLine("Файл user1.json пуст"); }
                Thread.Sleep(500);
                sr.Close();
                Console.WriteLine("* * * * поток ТА4 проверил 1 файл");
            })
            { IsBackground = true }; TA4_1.Start();

            var TA4_2 = new Thread(() => {
                StreamReader sr = new StreamReader("user2.json");
                string check = sr.ReadToEnd();
                if (check == "") { Console.WriteLine("Файл user2.json пуст"); }
                Thread.Sleep(500);
                sr.Close();
                Console.WriteLine("* * * * поток ТА4 проверил 2 файл");
            })
            { IsBackground = true }; TA4_2.Start();

            var TA4_3 = new Thread(() => {
                StreamReader sr = new StreamReader("user3.json");
                string check = sr.ReadToEnd();
                if (check == "") { Console.WriteLine("Файл user3.json пуст"); }
                Thread.Sleep(500);
                sr.Close();
                Console.WriteLine("* * * * поток ТА4 проверил 3 файл");
            })
            { IsBackground = true }; TA4_3.Start();

            TA4_1.Join();
            TA4_2.Join();
            TA4_3.Join();
            Console.WriteLine("* * Поток ТА4 закончил работу");

        }
        public static void TA((List<string>, List<string>, List<string>, string[][]) v1, (List<string>, List<string>, List<string>, string[][]) v2, (List<string>, List<string>, List<string>, string[][]) v3)
        {
            Console.WriteLine("Запустился поток ТА");
            var myThread_TA0 = new Thread(() => TA0(v1, v2, v3)) { IsBackground = true }; myThread_TA0.Start();
            var myThread_TA4 = new Thread(() => TA4()) { IsBackground = true }; myThread_TA4.Start();

            myThread_TA0.Join(); //можно убрать ибо поток TA4 начинается только после потока ТА0
            myThread_TA4.Join();
            Console.WriteLine("Поток ТА закончил работу");
            //evt_TA.Set(); //разрешаем потоку TB запуститься
        }

        public static void TB()
        {
            //evt_TA.WaitOne(); //ждет пока ТА разрешит ему запуститься
            Console.WriteLine("Поток ТВ начал работу");
            string connStr = "server=localhost; user=root;database=dbpost;password=MysqlPass33";
            var myThread1_db = new Thread(() => deseriliziation(connStr, 1)) { IsBackground = true }; myThread1_db.Start();
            var myThread2_db = new Thread(() => deseriliziation(connStr, 2)) { IsBackground = true }; myThread2_db.Start();
            var myThread3_db = new Thread(() => deseriliziation(connStr, 3)) { IsBackground = true }; myThread3_db.Start();

            myThread1_db.Join();
            myThread2_db.Join();
            myThread3_db.Join();
            Console.WriteLine("Поток ТВ закончил работу");
        }
        static void Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments(@"user-data-dir=C:\Users\mark\AppData\Local\Google\Chrome\User Data\Default");
            IWebDriver driver = new ChromeDriver(@"C:\Users\mark\source\repos\sumkin_app2\sumkin_app2", options);
            driver.Navigate().GoToUrl("https://vk.com/feed");
            Console.WriteLine("--------------PROGRAM START-------------");
            var v1 = start_all(driver);
            Thread.Sleep(1);
            var v2 = start_all(driver);
            Thread.Sleep(1);
            var v3 = start_all(driver);
            Console.WriteLine("Информация с постов получена");

            var myThread_TA = new Thread(() => TA(v1, v2, v3)) { IsBackground = true }; myThread_TA.Start();

            var myThread_TB = new Thread(() => TB()) { IsBackground = true }; myThread_TB.Start();
            myThread_TA.Join();
            myThread_TB.Join();

            driver.Close();

            Console.WriteLine("--------------PROGRAM END-------------");

        }
    }
}