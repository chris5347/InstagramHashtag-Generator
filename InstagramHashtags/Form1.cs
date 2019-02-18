using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstagramHashtags
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ArrayList hashtags = new ArrayList();
        ArrayList hashtagsUsed = new ArrayList();
        ArrayList hashtagPostCount = new ArrayList();
        int foundHashTagCount = 0;
        public string memberName;
        public int rank;
        public int userID;
        public string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        ChromeDriver driver;
        string profileIconXPath = "//*[@id=\"react-root\"]/section/nav/div[2]/div/div/div[3]/div/div[3]/a";


        async private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                button2_Click(null,null); //clear
                //texbox 1 filled out only
                if (textBox1.Text.Length > 0 && textBox3.Text.Length == 0)
                {
                    await scrape(textBox1.Text);
                }//texboxs both filled out
                else if (textBox1.Text.Length > 0 && textBox3.Text.Length > 0)
                {
                    await scrape(textBox1.Text);
                    await scrape(textBox3.Text);
                }//textbox 3 only filled out
                else if (textBox1.Text.Length == 0 && textBox3.Text.Length > 0)
                {
                    await scrape(textBox3.Text);
                }//no textboxs filled
                else if (textBox1.Text.Length == 0 && textBox3.Text.Length == 0)
                {
                    MessageBox.Show("Please input your niche characteristics");
                    return;
                }



                button2.BackColor = button1.BackColor;
                button2.Text = "Dont Like Your Hashtags? Clear";
                button2.Enabled = true;
                button1.Enabled = true;
                label2.Text = (textBox2.Lines.Length - 1).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(hashtags.Count.ToString() + ":" + ex.ToString());
                button2.BackColor = button1.BackColor;
                button2.Text = "Dont Like Your Hashtags? Clear";
                button2.Enabled = true;
                button1.Enabled = true;
                label2.Text = (textBox2.Lines.Length - 1).ToString();
            }
        }

        async Task waitForElement(string xPath)
        {
            try
            {
                await Task.Delay(500);
                int trys = 0;
                int count = 0;
                do
                {
                    if (xPath.Contains("/"))
                    {
                        var links = driver.FindElementsByXPath(xPath);
                        count = links.Count;
                    }
                    else
                    {
                        var links = driver.FindElementsByClassName(xPath);
                        count = links.Count;
                    }

                    trys++;
                    await Task.Delay(1000);
                } while (count == 0 && trys < 10);
            }
            catch (Exception)
            {

            }
        }

        async private Task scrape(string hashT)
        {
            try
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button2.Text = "Gathering Best Hashtags..";
                button2.BackColor = Color.DarkSeaGreen;
                //strip the text
                ArrayList audience = new ArrayList();
                string temp = "";
                string x = hashT;
                if (x.Contains("#") || x.Contains(" "))
                {
                    MessageBox.Show("Dont use #");
                    return;
                }
                for (int i = 0; i < x.Length; i++)
                {
                    string letter = x.Substring(i, 1);
                    if (letter.Equals(""))
                    {
                        continue;
                    }
                    if (!letter.Equals(","))
                    {
                        temp += letter;
                    }
                    else
                    {
                        audience.Add(temp);
                        temp = "";
                    }
                    if (i == x.Length - 1)
                    {
                        audience.Add(temp);
                    }
                }
                //done stripping text
                Random rnd = new Random();
                ArrayList hashtag = new ArrayList();
                foreach (string searchItem in audience)
                {
                    await search("https://www.instagram.com/bob", profileIconXPath);
                    driver.FindElementByClassName("TqC_a").Click(); //click on search bar to start typing
                    await Task.Delay(300);
                    IWebElement searchBar = driver.FindElementByClassName("XTCLo");
                    searchBar.Clear();
                    searchBar.SendKeys("#" + searchItem); //send niche to searchbar
                    await waitForElement("_2_M76");
                    var tagNames = driver.FindElementsByClassName("Ap253"); //each tag that appears after typing in
                    if (tagNames.Count == 0)
                    {
                        textBox2.Text += "Bad tag to find hashtag: " + searchItem + searchItem + Environment.NewLine;
                        continue;
                    }
                    string randomHash = tagNames[rnd.Next(0, tagNames.Count / 2)].Text;
                    for (int i = 0; i < 15; i++)
                    {
                        randomHash = tagNames[rnd.Next(0, tagNames.Count)].Text;
                        string xx = randomHash.Substring(1);
                        if(!hashtag.Contains(xx)){
                            hashtag.Add(xx);
                        }
                        else
                        {
                            i--;
                        }
                        await Task.Delay(500);
                    }
                    
                }
                textBox4.Text = "";
                foreach (string item in hashtag)
                {
                    textBox2.Text += "#"+item + Environment.NewLine;
                    await Task.Delay(500);
                    label2.Text = textBox2.Lines.Count().ToString();
                }

                button1.Enabled = true;
                button2.Enabled = true;
                button2.Text = "Clear";
                button2.BackColor = Color.Silver;

            }
            catch (Exception ex)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button2.Text = "Clear";
                button2.BackColor = Color.Silver;
                return;
            }
        }
       
        
        public static void doSelectionSort(ArrayList array, ArrayList array2)
        {
            string tmp = "";
            int min_key;

            for (int j = 0; j < array.Count - 1; j++)
            {
                min_key = j;

                for (int k = j + 1; k < array.Count; k++)
                {
                    if (Convert.ToInt32(array[k].ToString()) > Convert.ToInt32(array[min_key].ToString()))
                    {
                        min_key = k;
                    }
                }

                tmp = array[min_key].ToString();
                array[min_key] = array[j];
                array[j] = tmp;

                tmp = array2[min_key].ToString();
                array2[min_key] = array2[j];
                array2[j] = tmp;

            }
        }

        private void addHashtagsToList()
        {
            for (int i = 0; i < 30; i++)
            {
                if (hashtags.Count <= 0)
                {
                    break;
                }
                Random rnd = new Random();
                int randNum = rnd.Next(hashtags.Count);

                if (Regex.IsMatch(hashtags[randNum].ToString().Substring(1), @"^[a-zA-Z0-9_]+$"))
                {
                    textBox2.Text += hashtags[randNum];
                    hashtagsUsed.Add(hashtags[randNum]);
                    hashtags.RemoveAt(randNum);
                    hashtagPostCount.RemoveAt(randNum);
                    textBox2.Text += Environment.NewLine;
                }
                else
                {
                    hashtagsUsed.Add(hashtags[randNum]);
                    hashtags.RemoveAt(randNum);
                    hashtagPostCount.RemoveAt(randNum);
                }
            }
        }

       
        private int NthIndexOf(string target, string value, int n)
        {
            Match m = Regex.Match(target, "((" + Regex.Escape(value) + ").*?){" + n + "}");

            if (m.Success)
                return m.Groups[2].Captures[n - 1].Index;
            else
                return -1;
        }

        async public Task search(string url, string xPathElementToWaitFor)
        {
            int trys = 0;
            driver.Navigate().GoToUrl(url);
            if (xPathElementToWaitFor.Length > 0)
            {

                await waitForElement(xPathElementToWaitFor);
            }
            else
            {
                if (trys >= 10)
                {
                    return;
                }
                trys++;
                await Task.Delay(1000);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Registry.SetValue("HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\Explorer\\Navigating\\.Current", "", "NULL");
            textBox1.Focus();
           
            
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");
            driver = new ChromeDriver(driverService, options);
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox2.Text.Length == 0){
                return;
            }
            string textforClipboard = textBox2.Text;
            Clipboard.Clear();
            Clipboard.SetText(textforClipboard);
            MessageBox.Show("Hashtags Copied To Clipboard");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(!button1.Enabled){
                return;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                button1_Click(null, null);
                // these last two lines will stop the beep sound
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://ogusers.com/MrRobot");
        }


       
       
        private string fixMessage(string txt)
        {
            string x = "";
            for (int i = 0; i < txt.Length;i++ )
            {
                string letter = txt.Substring(i,1);
                if(letter.Equals("#")){
                    x += " #";
                }
                else
                {
                    x += letter;
                }
            }
            return x;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string message = fixMessage(textBox2.Text);
            sendMessage(textBox4.Text,message);
        }

        public String getCarrier(int x)
        {
            switch (x)
            {
                case 0:
                    return "@txt.att.net"; //ATT
                case 1:
                    return "@vtext.com"; //VERIZON
                case 2:
                    return "@messaging.sprintpcs.com"; //SPRINT
                case 3:
                    return "@tmomail.net"; //T-MOBILE  
                default:
                    return "ERROR";
            }
        }

        public void sendMessage(String number, String message)
        {
            string carrier = getCarrier(comboBox1.SelectedIndex);
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("ddos7003@gmail.com");
                mail.Sender = new MailAddress("ddos7003@gmail.com");
                mail.To.Add(number + carrier);
                mail.IsBodyHtml = true;
                mail.Subject = "Hashtags for: "+textBox1.Text+","+textBox3.Text;
                mail.Body = message;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new System.Net.NetworkCredential("ddos7003@gmail.com", "chris5347");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;

                smtp.Send(mail);
                MessageBox.Show("Text Sent: "+number+carrier);
            }
            catch (Exception e)
            {
                MessageBox.Show(("ERROR: " + e.ToString()));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            hashtags.Clear();
            hashtagsUsed.Clear();
            hashtagPostCount.Clear();
            label2.Text = "0";
        }
        




    }
}
