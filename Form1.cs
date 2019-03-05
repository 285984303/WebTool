using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebTool
{
    public partial class Form1 : Form
    {
        HttpWebRequest httpReq;
        HttpWebResponse httpResp;

        string strBuff = "";
        char[] cbuffer = new char[256];
        int byteRead = 0;

        //string filename = @"e:\url_log.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            Uri httpURL = new Uri(this.txtUrl.Text);

            ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换 
            httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换

            httpResp = (HttpWebResponse)httpReq.GetResponse();
            ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容

            ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理 
            Stream respStream = httpResp.GetResponseStream();

            ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以

            //StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8） 
            StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);

            byteRead = respStreamReader.Read(cbuffer, 0, 256);

            while (byteRead != 0)
            {
                string strResp = new string(cbuffer, 0, byteRead);
                strBuff = strBuff + strResp;
                byteRead = respStreamReader.Read(cbuffer, 0, 256);
            }

            respStream.Close();
            //txtHTML.Text = strBuff;

            //Regex reg = new Regex(@"(?is)<a[^>]*?href=(['""\s]?)(?<href>[^'""\s]*)\1[^>]*?>");
            //MatchCollection match = reg.Matches(strBuff);


            //foreach (Match m in match)
            //{

            //    txtHTML.Text += m.Groups["href"].Value + "\r\n";
            //    Console.WriteLine(m.Groups["href"].Value + "\r\n");
            //}

            Regex reg1 = new Regex(@"(?is)<a[^>]*?href=(['""]?)(?<url>[^'""\s>]+)\1[^>]*>(?<text>(?:(?!</?a\b).)*)</a>");
            MatchCollection mc = reg1.Matches(strBuff);
            //foreach (Match m in mc)
            //{
            //    txtHTML.Text += m.Groups["url"].Value + "\n";
            //    txtHTML.Text += m.Groups["text"].Value + "\n";
            //}


            //string text = File.ReadAllText(Environment.CurrentDirectory + "//test.txt", Encoding.GetEncoding("gb2312"));
            //string prttern = "<a(\\s+(href=\"(?<url>([^\"])*)\"|'([^'])*'|\\w+=\"(([^\"])*)\"|'([^'])*'))+>(?<text>(.*?))</a>";
            //var maths = Regex.Matches(strBuff, prttern, RegexOptions.Multiline);
            //抓取出来写入的文件
            //txtHTML.Text = "";
            using (FileStream w = new FileStream(Environment.CurrentDirectory + "//url_list.txt", FileMode.Create))
            {

                //for (int i = 0; i < maths.Count; i++)
                //{
                //    byte[] bs = Encoding.UTF8.GetBytes(string.Format("链接地址:{0}, innerhtml:{1}", maths[i].Groups["url"].Value,maths[i].Groups["text"].Value) + "\r\n");
                //    w.Write(bs, 0, bs.Length);
                //    //Console.WriteLine();
                //    txtHTML.Text += maths[i].Groups["text"].Value +" "+ maths[i].Groups["url"].Value + "\r\n";
                //}

                foreach (Match m in mc)
                {
                    byte[] bs = Encoding.UTF8.GetBytes(string.Format("链接地址:{0}, 名称:{1}", m.Groups["url"].Value, m.Groups["text"].Value) + "\r\n");
                    w.Write(bs, 0, bs.Length);
                    if (m.Groups["url"].Value.Contains("9game.cn"))
                        txtHTML.Text += m.Groups["text"].Value + " " + m.Groups["url"].Value + "\r\n";
                    else
                    {
                        txtHTML.Text += m.Groups["text"].Value + " " + "http://bbs.9game.cn/" + m.Groups["url"].Value + "\r\n";
                    }
                }
            }
            //Console.ReadKey();

        }
    }
}
