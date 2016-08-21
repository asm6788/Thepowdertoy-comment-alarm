using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 파우더토이_댓글알림
{
    public partial class Form1 : Form
    {
        int CommentCount = 0;
        int totalpage = 0;
        string 제목 = null;
        string[] 전에 = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public void Int()
        {
            textBox3.Clear();
            HttpWebRequest wReq;
            HttpWebResponse wRes;
            int count = CommentCount + 20;
            Uri uri = new Uri("http://powdertoy.co.uk/Browse/Comments.json?ID=" + textBox1.Text + "& Start =" + CommentCount + "&Count=" + count); // string 을 Uri 로 형변환
            wReq = (HttpWebRequest)WebRequest.Create(uri); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
            wReq.Method = "GET"; // 전송 방법 "GET" or "POST"
            wReq.ServicePoint.Expect100Continue = false;
            wReq.CookieContainer = new CookieContainer();
            string res = null;

            using (wRes = (HttpWebResponse)wReq.GetResponse())
            {
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                res = readerPost.ReadToEnd();
            }
            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(res);
            JsonArrayCollection col = (JsonArrayCollection)obj;

            string[] Username = new string[22];
            string[] CommentText = new string[22];
            string[] Date = new string[22];
            int i = 0;
            foreach (JsonObjectCollection joc in col)
            {
                i++;
                Username[i] = (string)joc["Username"].GetValue();
                CommentText[i] = (string)joc["Text"].GetValue();
                Date[i] = (string)joc["Timestamp"].GetValue();
                Console.WriteLine(Username[i] + CommentText[i] + Date[i]);
                TimeSpan t = TimeSpan.FromSeconds(Convert.ToInt32(Date[i]));
                int hour = t.Hours + 9;
                if (hour > 24)
                {
                    hour = hour - 24;
                    if (hour >= 12)
                        hour = hour + 12;
                }
                textBox3.AppendText("닉네임: " + Username[i] + "\r\n" + "날짜: " + hour + "시" + t.Minutes + "분" + t.Seconds + "초" + " 댓글: " + CommentText[i] + "\r\n\r\n");
            }
            전에 = CommentText;
        }

     

        private void button1_Click(object sender, EventArgs e)
        {
            Int();
            HttpWebRequest wReq;
            HttpWebResponse wRes;
            textBox2.Clear();

            WebRequest requestPic = WebRequest.Create("http://static.powdertoy.co.uk/" + textBox1.Text + ".png");

            WebResponse responsePic = requestPic.GetResponse();

            Image webImage = Image.FromStream(responsePic.GetResponseStream());

            pictureBox1.Image = webImage;
            Uri uri = new Uri("http://powdertoy.co.uk/Browse/View.json?ID=" + textBox1.Text); // string 을 Uri 로 형변환
            wReq = (HttpWebRequest)WebRequest.Create(uri); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
            wReq.Method = "GET"; // 전송 방법 "GET" or "POST"
            wReq.ServicePoint.Expect100Continue = false;
            wReq.CookieContainer = new CookieContainer();
            string res = null;

            using (wRes = (HttpWebResponse)wReq.GetResponse())
            {
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                res = readerPost.ReadToEnd();
            }

            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(res);

            JsonUtility.GenerateIndentedJsonText = false;

            Console.WriteLine();
            Console.WriteLine("Parsed data without indentation in JSON data format:");
            Console.WriteLine(obj.ToString());
            Console.WriteLine();


            // enumerate values in json object
            Console.WriteLine();
            int i = 0;
            String[] View = null;
            View = new string[17];
            foreach (JsonObject field in obj as JsonObjectCollection)
            {
                i++;
                string name = field.Name;
                string value = string.Empty;
                string type = field.GetValue().GetType().Name;

                // try to get value.
                switch (type)
                {
                    case "String":
                        value = (string)field.GetValue();
                        break;

                    case "Double":
                        value = field.GetValue().ToString();
                        break;

                    case "Boolean":
                        value = field.GetValue().ToString();
                        break;

                }
                View[i] = value;
                Console.WriteLine("{0} {1}",
                    name, value);
            }

            textBox2.AppendText("요청하신 ID:" + View[1] + "\r\n");
            textBox2.AppendText("총점수:" + View[3] + "\r\n");
            textBox2.AppendText("보트업:" + View[4] + "\r\n");
            textBox2.AppendText("보트다운:" + View[5] + "\r\n");
            textBox2.AppendText("조회수:" + View[6] + "\r\n");
            textBox2.AppendText("제목:" + View[8] + "\r\n");
            제목 = View[8];
            textBox2.AppendText("설명:" + View[9] + "\r\n");
            TimeSpan t = TimeSpan.FromSeconds(Convert.ToInt32(View[11]));
            int hour = t.Hours + 9;
            if (hour > 24)
            {
                hour = hour - 24;
                if (hour >= 12)
                    hour = hour + 12;
            }
           
            textBox2.AppendText("업로드날짜:" + hour + "시" + t.Minutes + "분" + t.Seconds + "초" + "\r\n");
            textBox2.AppendText("제작자:" + View[12] + "\r\n");
            textBox2.AppendText("댓글개수:" + View[13] + "\r\n");
            if (Convert.ToInt32(View[13]) / 20 == 0)
            {
                totalpage = Convert.ToInt32(View[13]);
                textBox2.AppendText("댓글 페이지 수:" + "1" + "\r\n");
            }
            else if (Convert.ToInt32(View[13]) / 20 == Convert.ToInt32(Convert.ToInt32(View[13]) / 20))
            {
                int one = Convert.ToInt32(View[13]) / 20 + 1;
                totalpage = one;
                textBox2.AppendText("댓글 페이지 수:" + one + "\r\n");
            }
            else
            {
                totalpage = Convert.ToInt32(View[13]) / 20;
                textBox2.AppendText("댓글 페이지 수:" + Convert.ToInt32(View[13]) / 20 + "\r\n");
            }
            textBox2.AppendText("공개세이브:" + View[14] + "\r\n");
            //if(View[16] == "")
            //    textBox2.AppendText("태그:" + "없음" + "\r\n");
            //else
            //    textBox2.AppendText("태그:" + View[16] + "\r\n");
        }

        
           

        private void button4_Click(object sender, EventArgs e)
        {
            CommentCount -= 20;
            HttpWebRequest wReq;
            HttpWebResponse wRes;
            int count = CommentCount;
            int startcount = CommentCount - 20;
            Uri uri = new Uri("http://powdertoy.co.uk/Browse/Comments.json?ID=" + textBox1.Text + "&Start=" + startcount + "&Count=" + count); // string 을 Uri 로 형변환
            wReq = (HttpWebRequest)WebRequest.Create(uri); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
            wReq.Method = "GET"; // 전송 방법 "GET" or "POST"
            wReq.ServicePoint.Expect100Continue = false;
            wReq.CookieContainer = new CookieContainer();
            string res = null;

            using (wRes = (HttpWebResponse)wReq.GetResponse())
            {
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                res = readerPost.ReadToEnd();
            }
            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(res);
            JsonArrayCollection col = (JsonArrayCollection)obj;

            string[] Username = new string[22];
            string[] CommentText = new string[22];
            string[] Date = new string[22];
            int i = 0;
            foreach (JsonObjectCollection joc in col)
            {
                i++;
                Username[i] = (string)joc["Username"].GetValue();
                CommentText[i] = (string)joc["Text"].GetValue();
                Date[i] = (string)joc["Timestamp"].GetValue();
                Console.WriteLine(Username[i] + CommentText[i] + Date[i]);
                TimeSpan t = TimeSpan.FromSeconds(Convert.ToInt32(Date[i]));
                int hour = t.Hours + 9;
                if (hour > 24)
                {
                    hour = hour - 24;
                    if (hour >= 12)
                        hour = hour + 12;
                }
                textBox3.AppendText("닉네임: " + Username[i] + "\r\n" + "날짜: " + hour + "시" + t.Minutes + "분" + t.Seconds + "초" + " 댓글: " + CommentText[i] + "\r\n\r\n");
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            CommentCount = 0;
            progressBar1.Value = 0;
            int i = 0;
            double temp = 0;
            List<string> Username = new List<string>();
            List<string> CommentText = new List<string>();
            List<string> Date = new List<string>();
            if (totalpage == 0)
                MessageBox.Show("불러오기 먼저 하십시요.");
            else
                while (true)
                {
                    CommentCount += 20;
                    HttpWebRequest wReq;
                    HttpWebResponse wRes;
                    int startcount = CommentCount - 20;
                    Uri uri = new Uri("http://powdertoy.co.uk/Browse/Comments.json?ID=" + textBox1.Text + "&Start=" + startcount + "&Count=20"); // string 을 Uri 로 형변환
                    wReq = (HttpWebRequest)WebRequest.Create(uri); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
                    wReq.Method = "GET"; // 전송 방법 "GET" or "POST"
                    wReq.ServicePoint.Expect100Continue = false;
                    wReq.CookieContainer = new CookieContainer();
                    string res = null;

                    if((i / ((double)totalpage * 20))+0.01 == temp)
                    {
                        progressBar1.Value = 100;
                        goto Out;
                    }
                    else
                    {
                        temp = (i / ((double)totalpage * 20)) + 0.01;
                    }
                    
                    using (wRes = (HttpWebResponse)wReq.GetResponse())
                    {
                        Stream respPostStream = wRes.GetResponseStream();
                        StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                        res = readerPost.ReadToEnd();
                    }
                    JsonTextParser parser = new JsonTextParser();
                    JsonObject obj = parser.Parse(res);
                    JsonArrayCollection col = (JsonArrayCollection)obj;



                    foreach (JsonObjectCollection joc in col)
                    {
                        i++;

                        Console.WriteLine(Convert.ToInt32((i / ((double)totalpage * 20)) * 100));
                        progressBar1.Value = Convert.ToInt32((i / ((double)totalpage * 20)) * 100);
                        Username.Add((string)joc["Username"].GetValue());
                        CommentText.Add((string)joc["Text"].GetValue());
                        Date.Add((string)joc["Timestamp"].GetValue());
                        Console.WriteLine(Username[Username.Count-1] + CommentText[CommentText.Count - 1] + Date[Date.Count - 1]);
                        TimeSpan t = TimeSpan.FromSeconds(Convert.ToInt32(Date[Date.Count - 1]));
                        int hour = t.Hours + 9;
                        if (hour > 24)
                        {
                            hour = hour - 24;
                            if (hour >= 12)
                                hour = hour + 12;
                        }
                        if (progressBar1.Value == 100)
                        {
                            goto Out;
                        }
                        textBox3.AppendText("닉네임: " + Username[Username.Count - 1] + "\r\n" + "날짜: " + hour + "시" + t.Minutes + "분" + t.Seconds + "초" + " 댓글: " + CommentText[CommentText.Count - 1] + "\r\n\r\n");
                    }
                }
            Out:;
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (totalpage == 0)
                MessageBox.Show("불러오기 먼저 하십시요.");
            else
            {
                MessageBox.Show("설정완료");
                timer1.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(textBox4.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            HttpWebRequest wReq;
            HttpWebResponse wRes;
            int count = CommentCount + 20;
            Uri uri = new Uri("http://powdertoy.co.uk/Browse/Comments.json?ID=" + textBox1.Text + "& Start =" + CommentCount + "&Count=" + count); // string 을 Uri 로 형변환
            wReq = (HttpWebRequest)WebRequest.Create(uri); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
            wReq.Method = "GET"; // 전송 방법 "GET" or "POST"
            wReq.ServicePoint.Expect100Continue = false;
            wReq.CookieContainer = new CookieContainer();
            string res = null;

            using (wRes = (HttpWebResponse)wReq.GetResponse())
            {
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                res = readerPost.ReadToEnd();
            }
            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(res);
            JsonArrayCollection col = (JsonArrayCollection)obj;

            string[] Username = new string[22];
            string[] CommentText = new string[22];
            string[] Date = new string[22];
            int i = 0;
            foreach (JsonObjectCollection joc in col)
            {
                i++;
                Username[i] = (string)joc["Username"].GetValue();
                CommentText[i] = (string)joc["Text"].GetValue();
                Date[i] = (string)joc["Timestamp"].GetValue();
                Console.WriteLine(Username[i] + CommentText[i] + Date[i]);
                TimeSpan t = TimeSpan.FromSeconds(Convert.ToInt32(Date[i]));
                int hour = t.Hours + 9;
                if (hour > 24)
                {
                    hour = hour - 24;
                    if (hour >= 12)
                        hour = hour + 12;
                }
                textBox3.AppendText("닉네임: " + Username[i] + "\r\n" + "날짜: " + hour + "시" + t.Minutes + "분" + t.Seconds + "초" + " 댓글: " + CommentText[i] + "\r\n\r\n");
            }

            if (전에[1] != CommentText[1])
            {              
                notifyIcon1.Visible = true; // 트레이의 아이콘을 보이게 한다.
                notifyIcon1.BalloonTipText = CommentText[1];
                notifyIcon1.ShowBalloonTip(500);
                전에 = CommentText;
                timer1.Start();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            timer1.Stop();
            MessageBox.Show("댓글알림 정지");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
