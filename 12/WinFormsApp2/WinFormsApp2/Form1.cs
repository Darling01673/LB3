using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        Dictionary<char, byte> iso8859_51 = new Dictionary<char, byte>
        {
            {'А', 0xB0}, {'Б', 0xB1}, {'В', 0xB2}, {'Г', 0xB3}, {'Д', 0xB4},
            {'Е', 0xB5}, {'Ж', 0xB6}, {'З', 0xB7}, {'И', 0xB8}, {'Й', 0xB9},
            {'К', 0xBA}, {'Л', 0xBB}, {'М', 0xBC}, {'Н', 0xBD}, {'О', 0xBE},
            {'П', 0xBF}, {'Р', 0xC0}, {'С', 0xC1}, {'Т', 0xC2}, {'У', 0xC3},
            {'Ф', 0xC4}, {'Х', 0xC5}, {'Ц', 0xC6}, {'Ч', 0xC7}, {'Ш', 0xC8},
            {'Щ', 0xC9}, {'Ъ', 0xCA}, {'Ы', 0xCB}, {'Ь', 0xCC}, {'Э', 0xCD},
            {'Ю', 0xCE}, {'Я', 0xCF}, {'а', 0xD0}, {'б', 0xD1}, {'в', 0xD2},
            {'г', 0xD3}, {'д', 0xD4}, {'е', 0xD5}, {'ж', 0xD6}, {'з', 0xD7},
            {'и', 0xD8}, {'й', 0xD9}, {'к', 0xDA}, {'л', 0xDB}, {'м', 0xDC},
            {'н', 0xDD}, {'о', 0xDE}, {'п', 0xDF}, {'р', 0xE0}, {'с', 0xE1},
            {'т', 0xE2}, {'у', 0xE3}, {'ф', 0xE4}, {'х', 0xE5}, {'ц', 0xE6},
            {'ч', 0xE7}, {'ш', 0xE8}, {'щ', 0xE9}, {'ъ', 0xEA}, {'ы', 0xEB},
            {'ь', 0xEC}, {'э', 0xED}, {'ю', 0xEE}, {'я', 0xEF}, {' ', 0xFF}
        };
        Dictionary<string, char> iso8859_52 = new Dictionary<string, char>
        {
                { "0xB0", 'А' }, { "0xB1", 'Б' }, { "0xB2", 'В' }, { "0xB3", 'Г' }, { "0xB4", 'Д' },
                { "0xB5", 'Е' }, { "0xB6", 'Ж' }, { "0xB7", 'З' }, { "0xB8", 'И' }, { "0xB9", 'Й' },
                { "0xBA", 'К' }, { "0xBB", 'Л' }, { "0xBC", 'М' }, { "0xBD", 'Н' }, { "0xBE", 'О' },
                { "0xBF", 'П' }, { "0xC0", 'Р' }, { "0xC1", 'С' }, { "0xC2", 'Т' }, { "0xC3", 'У' },
                { "0xC4", 'Ф' }, { "0xC5", 'Х' }, { "0xC6", 'Ц' }, { "0xC7", 'Ч' }, { "0xC8", 'Ш' },
                { "0xC9", 'Щ' }, { "0xCA", 'Ъ' }, { "0xCB", 'Ы' }, { "0xCC", 'Ь' }, { "0xCD", 'Э' },
                { "0xCE", 'Ю' }, { "0xCF", 'Я' }, { "0xD0", 'а' }, { "0xD1", 'б' }, { "0xD2", 'в' },
                { "0xD3", 'г' }, { "0xD4", 'д' }, { "0xD5", 'е' }, { "0xD6", 'ж' }, { "0xD7", 'з' },
                { "0xD8", 'и' }, { "0xD9", 'й' }, { "0xDA", 'к' }, { "0xDB", 'л' }, { "0xDC", 'м' },
                { "0xDD", 'н' }, { "0xDE", 'о' }, { "0xDF", 'п' }, { "0xE0", 'р' }, { "0xE1", 'с' },
                { "0xE2", 'т' }, { "0xE3", 'у' }, { "0xE4", 'ф' }, { "0xE5", 'х' }, { "0xE6", 'ц' },
                { "0xE7", 'ч' }, { "0xE8", 'ш' }, { "0xE9", 'щ' }, { "0xEA", 'ъ' }, { "0xEB", 'ы' },
                { "0xEC", 'ь' }, { "0xED", 'э' }, { "0xEE", 'ю' }, { "0xEF", 'я' }, { "0xFF", ' ' }
        };
        int port;
        string localIp;
        string remoteIp;
        int remoteport;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread sputnik = new Thread(new ThreadStart(ReceiveMessages));

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show(
            "Введите IP-адрессы и порты",
            "Сообщение",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                localIp = textBox1.Text;
                remoteIp = textBox2.Text;
                port = Int32.Parse(textBox3.Text);
                remoteport = Int32.Parse(textBox4.Text);
                sputnik.Start();
                button1.Enabled = false;
                button2.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }
        }
        private async void ReceiveMessages()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse(localIp), port);
            tcpListener.Start();

            while (true)
            {
                try
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    using (NetworkStream networkStream = tcpClient.GetStream())
                    {
                        byte[] data = new byte[1024];
                        int bytesRead = await networkStream.ReadAsync(data, 0, data.Length);
                        string receivedMessage = Encoding.UTF8.GetString(data, 0, bytesRead);
                        string[] str = receivedMessage.Split(new[] { ' ' });
                        string Text = "";
                        string binaryValue = "";
                        foreach (string bit in str)
                        {
                            if (iso8859_52.ContainsKey(bit))
                            {
                                Text += iso8859_52[bit];
                            }
                        }
                        string[] hexValues = receivedMessage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string hex in hexValues)
                        {
                            string cleanedHex = hex.StartsWith("0x") ? hex.Substring(2) : hex;

                            int decimalValue = Convert.ToInt32(cleanedHex, 16);


                            binaryValue += Convert.ToString(decimalValue, 2).PadLeft(8, '0') + " ";
                        }
                        this.Invoke((MethodInvoker)(() =>
                        {
                            textBox5.Text += remoteIp.ToString() + ": " + Text + " " + receivedMessage + "  " + binaryValue + Environment.NewLine;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }
        }

            private async void button2_Click(object sender, EventArgs e)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                tcpClient.Connect(IPAddress.Parse(remoteIp), remoteport);
                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    StringBuilder encodedMessage = new StringBuilder();
                    string predlog = textBox6.Text;

                    foreach (char character in predlog)
                    {
                        if (iso8859_51.ContainsKey(character))
                        {
                            string hexValue = "0x" + iso8859_51[character].ToString("X2") + ' ';
                            encodedMessage.Append(hexValue);
                        }
                    }

                    StringBuilder binaryMessage = new StringBuilder();
                    foreach (char character in predlog)
                    {
                        if (iso8859_51.ContainsKey(character))
                        {
                            string binaryValue = Convert.ToString(iso8859_51[character], 2).PadLeft(8, '0') + " ";
                            binaryMessage.Append(binaryValue);
                        }
                    }

                    string dvoich = binaryMessage.ToString();
                    string messageToSend = encodedMessage.ToString();
                    byte[] data = Encoding.UTF8.GetBytes(messageToSend);
                    networkStream.Write(data, 0, data.Length);

                    textBox5.Text += localIp.ToString() + ": " + encodedMessage.ToString() + " - " + dvoich + " - " + predlog + Environment.NewLine;

                    string fileName = DateTime.UtcNow.ToString("dd.MM.yyyy_HH-mm-ss") + ".txt";
                    using (StreamWriter writer = new StreamWriter(fileName, true))
                    {
                        writer.WriteLine("Отправлено: " + encodedMessage.ToString());
                        writer.WriteLine("Получено: " + dvoich);
                    }
                }
            }
        }
    }
}
