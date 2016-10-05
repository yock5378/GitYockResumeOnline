using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using YockResume.Utils;

namespace YockResume.Controllers
{
    public class DownloadPDFController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 執行此Url，下載PDF檔案
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePDF(string files)
        {
            WebClient wc = new WebClient();
            string htmlText;
            string filename;
            //從網址下載Html字串
            //string htmlText = ToBase64("http://yockresumeonline.apphb.com/html/PDFprint_resume.html");

            if (files == "resume")
            {
                htmlText = Server.MapPath("../html/PDFprint_resume.html");
                filename = "YockResume.pdf";
            }
            else if (files == "autobiography")
            {
                htmlText = Server.MapPath("../html/PDFprint_autobiography.html");
                filename = "YockAutobiography.pdf";
            }
            else
                return RedirectToAction("Index", "Home");
            htmlText = wc.DownloadString(htmlText);
            string fronturl = System.Configuration.ConfigurationManager.AppSettings["FrontendURL"];
            htmlText = htmlText.Replace("xxxfrontendxxx", fronturl);
            byte[] pdfFile = this.ConvertHtmlTextToPDF(htmlText);     //Html轉為PDF
            //byte[] pdfFile = this.CreateTextToPDF();                    //自己寫PDF

            return File(pdfFile, "application/pdf", filename);
        }

        private string ToBase64(string htmlText)
        {
            Regex urlRegex = new Regex("src\\s*=\\s*[\"']+(http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?)[\"']+");
            // Serch for SRCs.
            MatchCollection matchs = urlRegex.Matches(htmlText);
            foreach (Match match in matchs)
            {
                // Replace urls with embedded base64 images.
                htmlText = htmlText.Replace(match.Groups[1].Value, GetBase64(match.Groups[1].Value));
            }
            return htmlText;
        }

        private string GetBase64(string imageUrl)
        {
            string base64Data = "";

            try
            {
                // Prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imageUrl);
                request.Method = "GET";
                request.ContentType = "image/jpeg";
                request.UserAgent = "Mozilla/4.0+(compatible;+MSIE+5.01;+Windows+NT+5.0";

                // Execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //We will read data via the response stream
                Stream resStream = response.GetResponseStream();

                //Write content into the MemoryStream
                BinaryReader resReader = new BinaryReader(resStream);

                // Build base64 string.
                base64Data = string.Format("data:image/jpeg;base64,{0}",
                Convert.ToBase64String(resReader.ReadBytes((int)response.ContentLength)));
            }
            catch (Exception)
            {
            }

            return base64Data;
        }

        public virtual string SimpleAjaxImgFix(string xHtml)
        {
            return Regex.Replace(
                xHtml,
                "(?<image><img[^>]+)(?<=[^/])>",
                new MatchEvaluator(match => match.Groups["image"].Value + " />"),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
        }

        /// <summary>
        /// 將Html文字 輸出到PDF檔裡
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public byte[] ConvertHtmlTextToPDF(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return null;
            }

            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
            htmlText = "<p>" + htmlText + "</p>";

            //避免當htmlText有任何img image標籤時，轉PDF時會掛掉
            htmlText = SimpleAjaxImgFix(htmlText);

            MemoryStream outputStream = new MemoryStream();//要把PDF寫到哪個串流
            byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串轉成byte[]
            MemoryStream msInput = new MemoryStream(data);
            Document doc = new Document();//要寫PDF的文件，建構子沒填的話預設直式A4
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件預設開檔時的縮放為100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            doc.Open();
            //使用XMLWorkerHelper把Html parse到PDF檔裡
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
            //將pdfDest設定的資料寫到PDF檔
            PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            msInput.Close();
            outputStream.Close();
            //回傳PDF檔案 
            return outputStream.ToArray();

        }

        public byte[] CreateTextToPDF()
        {
            MemoryStream outputStream = new MemoryStream();//要把PDF寫到哪個串流
            Document doc = new Document();//要寫PDF的文件，建構子沒填的話預設直式A4
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件預設開檔時的縮放為100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            BaseFont bfChinese = BaseFont.CreateFont(@"C:\Windows\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font ChFont = new Font(bfChinese, 12);
            Font ChFont_title = new Font(bfChinese, 28);
            Font ChFont_blue = new Font(bfChinese, 16, Font.NORMAL, new BaseColor(255, 255, 255));

            Image image = Image.GetInstance(@"D:/160917_Resume_Yock/YockResume/images/YC_.jpg");  //讀取圖片來源
            image.ScalePercent(38);//'縮小到38%才放的進版面

            Paragraph title = new Paragraph("履歷表", ChFont_title);
            title.Alignment = Element.ALIGN_CENTER;  //內容水平置中
            title.SpacingAfter = 24;


            PdfPTable table = new PdfPTable(new float[] { 3f, 1f, 3f, 1f, 3f });
            PdfPCellHeader header = new PdfPCellHeader(new Phrase("基本資料", ChFont_blue));
            header.Colspan = 5;
            PdfPCell img1 = new PdfPCell(image);
            img1.Rowspan = 6;
            img1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            img1.VerticalAlignment = PdfPCell.ALIGN_CENTER;
            PdfPCellCenter c1 = new PdfPCellCenter(new Phrase("姓名", ChFont));
            PdfPCellLeft c2 = new PdfPCellLeft(new Phrase("張益誠", ChFont));
            PdfPCellCenter c3 = new PdfPCellCenter(new Phrase("性別", ChFont));
            PdfPCellLeft c4 = new PdfPCellLeft(new Phrase("男", ChFont));

            PdfPCellCenter c5 = new PdfPCellCenter(new Phrase("生日", ChFont));
            PdfPCellLeft c6 = new PdfPCellLeft(new Phrase("80年05月31日", ChFont));
            PdfPCellCenter c7 = new PdfPCellCenter(new Phrase("身高體重", ChFont));
            PdfPCellLeft c8 = new PdfPCellLeft(new Phrase("165公分、65公斤", ChFont));

            PdfPCellCenter c9 = new PdfPCellCenter(new Phrase("駕照", ChFont));
            PdfPCellLeft c10 = new PdfPCellLeft(new Phrase("汽車、機車，自備機車", ChFont));
            PdfPCellCenter c11 = new PdfPCellCenter(new Phrase("婚姻狀況", ChFont));
            PdfPCellLeft c12 = new PdfPCellLeft(new Phrase("未婚", ChFont));

            PdfPCellCenter c13 = new PdfPCellCenter(new Phrase("兵役狀況", ChFont));
            PdfPCellLeft c14 = new PdfPCellLeft(new Phrase("役畢 (退伍時間: 2014年5月)", ChFont));
            PdfPCellCenter c15 = new PdfPCellCenter(new Phrase("連絡電話", ChFont));
            PdfPCellLeft c16 = new PdfPCellLeft(new Phrase("0978901805", ChFont));

            PdfPCellCenter c17 = new PdfPCellCenter(new Phrase("聯絡地址", ChFont));
            PdfPCellLeft c18 = new PdfPCellLeft(new Phrase("新北市汐止區合順街84號2F-5", ChFont));
            PdfPCellCenter c19 = new PdfPCellCenter(new Phrase("電子郵件", ChFont));
            PdfPCellLeft c20 = new PdfPCellLeft(new Phrase("yock5378@gmail.com", ChFont));

            PdfPCellCenter c21 = new PdfPCellCenter(new Phrase("興趣", ChFont));
            PdfPCellLeft c22 = new PdfPCellLeft(new Phrase("平時喜歡聽音樂、看電影，假日時常常約朋友騎單車到較遠的地方看風景吹吹風，一方面可以增強體力，另外也能紓解平日累積的壓力。", ChFont));
            c22.Colspan = 3;

            table.AddCell(header);
            table.AddCell(img1);
            table.AddCell(c1);
            table.AddCell(c2);
            table.AddCell(c3);
            table.AddCell(c4);
            table.AddCell(c5);
            table.AddCell(c6);
            table.AddCell(c7);
            table.AddCell(c8);
            table.AddCell(c9);
            table.AddCell(c10);
            table.AddCell(c11);
            table.AddCell(c12);
            table.AddCell(c13);
            table.AddCell(c14);
            table.AddCell(c15);
            table.AddCell(c16);
            table.AddCell(c17);
            table.AddCell(c18);
            table.AddCell(c19);
            table.AddCell(c20);
            table.AddCell(c21);
            table.AddCell(c22);

            doc.Open();

            doc.Add(title);

            doc.Add(table);





            //將pdfDest設定的資料寫到PDF檔
            PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            writer.SetOpenAction(action);
            doc.Close();
            outputStream.Close();
            //回傳PDF檔案 
            return outputStream.ToArray();

        }

        public class PdfPCellHeader : PdfPCell
        {
            public PdfPCellHeader(Phrase phrase)
                : base(phrase)
            {
                this.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                this.VerticalAlignment = PdfPCell.ALIGN_CENTER;
                this.BackgroundColor = new BaseColor(32, 0, 144);
            }
        }

        public class PdfPCellCenter : PdfPCell
        {
            public PdfPCellCenter(Phrase phrase)
                : base(phrase)
            {
                this.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                this.VerticalAlignment = PdfPCell.ALIGN_CENTER;
            }
        }

        public class PdfPCellLeft : PdfPCell
        {
            public PdfPCellLeft(Phrase phrase)
                : base(phrase)
            {
                this.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                this.VerticalAlignment = PdfPCell.ALIGN_CENTER;
            }
        }
    }
}