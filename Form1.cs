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


        //string filename = @"e:\url_log.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            HttpWebRequest httpReq;
            HttpWebResponse httpResp;

            string strBuff = "";
            char[] cbuffer = new char[512];
            int byteRead = 0;

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

            byteRead = respStreamReader.Read(cbuffer, 0, 512);

            while (byteRead != 0)
            {
                string strResp = new string(cbuffer, 0, byteRead);
                strBuff = strBuff + strResp;
                byteRead = respStreamReader.Read(cbuffer, 0, 512);
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
            this.txtHTML.Clear();
            this.txtHTML.Rtf = "";
            this.txtHTML.Text = "";
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
                    
                    string text = m.Groups["text"].Value.Replace("<img src=\"static/image/common/pin_1.gif\" alt=\"本版置顶\" style=\"width:18px;\"/>", "");
                    string url = m.Groups["url"].Value;
                    //string title = "";
                    if (!url.Contains("9game.cn"))
                        url = "http://bbs.9game.cn/" + url;

                    txtHTML.Text += text + " " + url + "\r\n";


                    byte[] bs = Encoding.UTF8.GetBytes(string.Format("{0}, {1}", text, url) + "\r\n");
                    w.Write(bs, 0, bs.Length);
                }
            }
            //Console.ReadKey();

        }
        /// <summary>
        /// postDataStr
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData">name=test&pass=123456</param>
        /// <param name="dataEncode"></param>
        /// <returns></returns>
        private string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataStr.Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(postDataStr);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
            string retString = reader.ReadToEnd();
            return retString;
        }

        /// <summary>
        /// GET请求与获取结果
        /// </summary>
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        static void Main(string[] args)
        {
            string url = "http://www.mystudy.cn/LoginHandler.aspx";
            string data = "UserName=admin&Password=123";
            string result = HttpPost(url, data);
            Console.WriteLine(result);
            Console.ReadLine();
        }

    }
}
