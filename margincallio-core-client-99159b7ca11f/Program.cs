using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace TcpClient1
{
    class Program
    {
        static UdpClient udp = new UdpClient(15000);
        static void StartListening()
        {
            udp.BeginReceive(Receive, new object());
        }
        static void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = null;
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("Received trades: " + message);
            StartListening();
        }

        static void Main(string[] args)
        {
            //StartListening();            
            //Console.WriteLine("Listening for casts");
            //Console.ReadLine();
            //return;

            Stopwatch sw = new Stopwatch();
            Random gen = new Random();

            int cmd = 0;

            TcpClient client;

            try
            {
                client = new TcpClient("104.214.213.157", 1330); //184.168.134.144      127.0.0.1
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                return;
            }

            //while (true)
            //{
            //    string received = ReadResponse(client);
            //    Console.WriteLine(received);
            //}

            while (cmd != -1)
            {
                sw.Reset();
                Console.Write("Command to send: ");
                cmd = int.Parse(Console.ReadLine());

                if (cmd == 100 || cmd == 200 || cmd == 300 || cmd == 400 || cmd == 2500) //на вход подаётся User ID
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }                
                else if (cmd == 5100 || cmd == 5200 || cmd == 8800 || cmd == 8900 || cmd == 9000 || cmd == 9100 || cmd == 9500) //параметры отсутствуют
                {
                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                else if (cmd == 500 || cmd == 600) //пополнение или вывод
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Currency: ");
                    string currency = Console.ReadLine();
                    Console.Write("Amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WritePropertyName("2");
                        writer.WriteValue(currency);
                        writer.WritePropertyName("3");
                        writer.WriteValue(amount);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                else if (cmd == 700) //лимитная заявка
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Derived currency: ");
                    string derived_currency = Console.ReadLine();                       
                    Console.Write("Side (0 / 1): ");
                    int side = int.Parse(Console.ReadLine());
                    Console.Write("Amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());
                    Console.Write("Rate: ");
                    decimal rate = decimal.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WritePropertyName("2");
                        writer.WriteValue(derived_currency);
                        writer.WritePropertyName("3");
                        writer.WriteValue(side);                        
                        writer.WritePropertyName("4");
                        writer.WriteValue(amount);
                        writer.WritePropertyName("5");
                        writer.WriteValue(rate);
                        writer.WritePropertyName("6");
                        writer.WriteValue(0m);
                        writer.WritePropertyName("7");
                        writer.WriteValue(0m);
                        writer.WritePropertyName("8");
                        writer.WriteValue(0m);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                else if (cmd == 800) //рыночная заявка
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Derived currency: ");
                    string derived_currency = Console.ReadLine();
                    Console.Write("Side (0 / 1): ");
                    int side = int.Parse(Console.ReadLine());
                    Console.Write("Base amount (0 / 1): ");
                    int base_amount = int.Parse(Console.ReadLine());
                    Console.Write("Amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WritePropertyName("2");
                        writer.WriteValue(derived_currency);
                        writer.WritePropertyName("3");
                        writer.WriteValue(side);
                        writer.WritePropertyName("4");
                        writer.WriteValue(base_amount);
                        writer.WritePropertyName("5");
                        writer.WriteValue(amount);
                        writer.WritePropertyName("6");
                        writer.WriteValue(0m);
                        writer.WritePropertyName("7");
                        writer.WriteValue(0m);
                        writer.WritePropertyName("8");
                        writer.WriteValue(0m);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                if (cmd == 900 || cmd == 2800) //закрытие заявки
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Order ID: ");
                    long order_id = long.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WritePropertyName("2");
                        writer.WriteValue(order_id);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                if (cmd == 2400 || cmd == 2700) //GetAccountBalance / GetOpenOrders
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Currency (leave blank to view all balances): ");
                    string currency = Console.ReadLine();

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);

                        if (!String.IsNullOrEmpty(currency))
                        {
                            writer.WritePropertyName("2");
                            writer.WriteValue(currency);
                        }

                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                if (cmd == 2600) //GetAccountFee
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Derived currency: ");
                    string derived_currency = Console.ReadLine();

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WritePropertyName("2");
                        writer.WriteValue(derived_currency);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                if (cmd == 1000) //SetAccountFee
                {
                    Console.Write("User ID: ");
                    int user_id = int.Parse(Console.ReadLine());
                    Console.Write("Derived currency: ");
                    string derived_currency = Console.ReadLine();
                    Console.Write("Fee (%): ");
                    decimal fee_in_perc = decimal.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(user_id);
                        writer.WritePropertyName("2");
                        writer.WriteValue(derived_currency);
                        writer.WritePropertyName("3");
                        writer.WriteValue(fee_in_perc);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                else if (cmd == 5000 || cmd == 7000) //на вход подаётся derived_currency (CreateCurrencyPair / GetTicker)
                {
                    Console.Write("Derived currency: ");
                    string derived_currency = Console.ReadLine();

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(derived_currency);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }
                else if (cmd == 7100) //на вход подаётся derived_currency и limit (GetDepth)
                {
                    Console.Write("Derived currency: ");
                    string derived_currency = Console.ReadLine();
                    Console.Write("Limit: ");
                    int limit = int.Parse(Console.ReadLine());

                    //запись команды в JSON
                    StringBuilder sb = new StringBuilder();
                    StringWriter strwrt = new StringWriter(sb);

                    using (JsonWriter writer = new JsonTextWriter(strwrt))
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("0");
                        writer.WriteValue(cmd);
                        writer.WritePropertyName("1");
                        writer.WriteValue(derived_currency);
                        writer.WritePropertyName("2");
                        writer.WriteValue(limit);
                        writer.WriteEndObject();
                    }

                    sw.Start();
                    SendMessage(client, sb.ToString());
                    Console.WriteLine(ReadResponse(client));
                    Console.WriteLine(ReadResponse(client));
                    sw.Stop();
                }

                Console.WriteLine(sw.ElapsedMilliseconds.ToString() + " ms");
            }

            client.Close();
        }

        private static void SendMessage(TcpClient client, string message)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                client.GetStream().Write(bytes, 0, bytes.Length);
            }
            catch
            {
                //произошла ошибка сокета
                return;
            }
        }

        private static string ReadResponse(TcpClient client)
        {
            byte[] buffer = new byte[4096];
            int totalRead = 0;

            try
            {
                //читаем байты, пока ни одного не останется
                do
                {
                    int read = client.GetStream().Read(buffer, totalRead, buffer.Length - totalRead);
                    totalRead += read;
                } while (client.GetStream().DataAvailable);

                if (totalRead == 0) return "dc";
                return Encoding.ASCII.GetString(buffer, 0, totalRead);
            }
            catch
            {
                if (totalRead == 0) return "dc";
                return "err";
            }
        }

    }
}
