using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YockResume.Controllers
{
    public class DownloadExcelController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateExcel()
        {
            MemoryStream file = Export();
            return this.File(file.ToArray(), "application/vnd.ms-excel", "MyExcel.xlsx");
        }


        public MemoryStream Export()
        {

            int pictureIndex = 0;

            // 建立工作表
            var workbook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("sheet1");
            XSSFDrawing patriarch = (XSSFDrawing)sheet.CreateDrawingPatriarch();

            // 宣告儲存格樣式
            XSSFCell cell = null;
            XSSFCellStyle titleStyle = null;
            XSSFCellStyle headerStyle = null;
            XSSFCellStyle centerStyle = null;
            XSSFCellStyle leftStyle = null;

            // 字體尺寸
            XSSFFont font12 = (XSSFFont)workbook.CreateFont();
            font12.FontHeightInPoints = 12;
            XSSFFont font18 = (XSSFFont)workbook.CreateFont();
            font18.FontHeightInPoints = 18;
            XSSFFont font36 = (XSSFFont)workbook.CreateFont();
            font36.FontHeightInPoints = 36;

            // 設定字體顏色
            var color = new XSSFColor(Color.White);
            font18.SetColor(color);

            // 設定儲存格樣式
            titleStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.VerticalAlignment = VerticalAlignment.Center;
            titleStyle.WrapText = true;
            titleStyle.SetFont(font36);

            headerStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.VerticalAlignment = VerticalAlignment.Center;
            headerStyle.WrapText = true;
            headerStyle.SetFont(font18);
            headerStyle.SetFillForegroundColor(new XSSFColor(new byte[] { 32, 0, 144 }));
            headerStyle.FillPattern = FillPattern.SolidForeground;

            centerStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            centerStyle.BorderTop = BorderStyle.Thin;
            centerStyle.BorderLeft = BorderStyle.Thin;
            centerStyle.BorderBottom = BorderStyle.Thin;
            centerStyle.BorderRight = BorderStyle.Thin;
            centerStyle.Alignment = HorizontalAlignment.Center;
            centerStyle.VerticalAlignment = VerticalAlignment.Center;
            centerStyle.WrapText = true;
            centerStyle.SetFont(font12);

            leftStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            leftStyle.BorderTop = BorderStyle.Thin;
            leftStyle.BorderLeft = BorderStyle.Thin;
            leftStyle.BorderBottom = BorderStyle.Thin;
            leftStyle.BorderRight = BorderStyle.Thin;
            leftStyle.Alignment = HorizontalAlignment.Left;             // 會跟框線衝突，設定了也沒作用
            leftStyle.VerticalAlignment = VerticalAlignment.Center;
            leftStyle.WrapText = true;
            leftStyle.SetFont(font12);


            #region 畫表格
                // 建立第一行設定表頭資料
                sheet.CreateRow(0);

                // 合併儲存格
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));

                // 設定表格寬度
                sheet.SetColumnWidth(0, 23 * 256);
                sheet.SetColumnWidth(1, 12 * 256);
                sheet.SetColumnWidth(2, 23 * 256);
                sheet.SetColumnWidth(3, 12 * 256);
                sheet.SetColumnWidth(4, 23 * 256);

                // 此行每一格都設定Style
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(0).CreateCell(i);
                    cell.CellStyle = titleStyle;
                    if (i == 0)
                        cell.SetCellValue("履歷表");
                }

                sheet.CreateRow(1);
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(1).CreateCell(i);
                    cell.CellStyle = headerStyle;
                    if (i == 0)
                        cell.SetCellValue("基本資料");
                }

                var row2 = sheet.CreateRow(2);
                sheet.AddMergedRegion(new CellRangeAddress(2, 7, 0, 0));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(2).CreateCell(i);
                    if (i == 0 || i == 1 || i == 3)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;
                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("姓名");
                            break;
                        case 2:
                            cell.SetCellValue("張益誠");
                            break;
                        case 3:
                            cell.SetCellValue("性別");
                            break;
                        case 4:
                            cell.SetCellValue("男");
                            break;
                        default:
                            break;
                    }
                }


                sheet.CreateRow(3);
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(3).CreateCell(i);
                    if (i == 0 || i == 1 || i == 3)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;
                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("生日");
                            break;
                        case 2:
                            cell.SetCellValue("80年05月31日");
                            break;
                        case 3:
                            cell.SetCellValue("身高體重");
                            break;
                        case 4:
                            cell.SetCellValue("165公分、65公斤");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(4);
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(4).CreateCell(i);
                    if (i == 0 || i == 1 || i == 3)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;
                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("駕照");
                            break;
                        case 2:
                            cell.SetCellValue("汽車、機車，自備機車");
                            break;
                        case 3:
                            cell.SetCellValue("婚姻狀況");
                            break;
                        case 4:
                            cell.SetCellValue("未婚");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(5);
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(5).CreateCell(i);
                    if (i == 0 || i == 1 || i == 3)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;
                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("兵役狀況");
                            break;
                        case 2:
                            cell.SetCellValue("役畢 (退伍時間: 2014年5月)");
                            break;
                        case 3:
                            cell.SetCellValue("連絡電話");
                            break;
                        case 4:
                            cell.SetCellValue("0978901805");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(6);
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(6).CreateCell(i);
                    if (i == 0 || i == 1 || i == 3)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;
                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("聯絡地址");
                            break;
                        case 2:
                            cell.SetCellValue("新北市汐止區合順街84號2F-5");
                            break;
                        case 3:
                            cell.SetCellValue("電子郵件");
                            break;
                        case 4:
                            cell.SetCellValue("yock5378@gmail.com");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(7);
                sheet.AddMergedRegion(new CellRangeAddress(7, 7, 2, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(7).CreateCell(i);
                    if (i == 0 || i == 1 || i == 3)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;
                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("興趣");
                            break;
                        case 2:
                            cell.SetCellValue("平時喜歡聽音樂、看電影，假日時常常約朋友騎單車到較遠的地方看風景吹吹風，一方面可以增強體力，另外也能紓解平日累積的壓力。");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(8);
                sheet.AddMergedRegion(new CellRangeAddress(8, 8, 0, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(8).CreateCell(i);
                    cell.CellStyle = headerStyle;
                    if (i == 0)
                        cell.SetCellValue("學歷");
                }

                sheet.CreateRow(9);
                sheet.AddMergedRegion(new CellRangeAddress(9, 9, 3, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(9).CreateCell(i);
                    cell.CellStyle = centerStyle;

                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("學校名稱");
                            break;
                        case 2:
                            cell.SetCellValue("科系名稱");
                            break;
                        case 3:
                            cell.SetCellValue("就讀期間");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(10);
                sheet.AddMergedRegion(new CellRangeAddress(10, 10, 3, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(10).CreateCell(i);
                    cell.CellStyle = centerStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("最高學歷");
                            break;
                        case 1:
                            cell.SetCellValue("台中市 靜宜大學");
                            break;
                        case 2:
                            cell.SetCellValue("資訊工程學系");
                            break;
                        case 3:
                            cell.SetCellValue("2009/07 ~ 2013/05");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(11);
                sheet.AddMergedRegion(new CellRangeAddress(11, 11, 0, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(11).CreateCell(i);
                    cell.CellStyle = headerStyle;
                    if (i == 0)
                        cell.SetCellValue("專長與技能");
                }

                sheet.CreateRow(12);
                sheet.AddMergedRegion(new CellRangeAddress(12, 12, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(12).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("程式語言");
                            break;
                        case 1:
                            cell.SetCellValue("C、ASP.NET MVC、HTML");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(13);
                sheet.AddMergedRegion(new CellRangeAddress(13, 13, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(13).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("資料庫");
                            break;
                        case 1:
                            cell.SetCellValue("MS SQL Server");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(14);
                sheet.AddMergedRegion(new CellRangeAddress(14, 14, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(14).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("軟體操作");
                            break;
                        case 1:
                            cell.SetCellValue("Windows、Office、Photoshop");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(15);
                sheet.AddMergedRegion(new CellRangeAddress(15, 15, 0, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(15).CreateCell(i);
                    cell.CellStyle = headerStyle;
                    if (i == 0)
                        cell.SetCellValue("工作經歷");
                }

                sheet.CreateRow(16);
                sheet.AddMergedRegion(new CellRangeAddress(16, 16, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(16, 16, 3, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(16).CreateCell(i);
                    cell.CellStyle = centerStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("服務單位");
                            break;
                        case 1:
                            cell.SetCellValue("職稱及服務時間");
                            break;
                        case 3:
                            cell.SetCellValue("主要工作");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(17);
                sheet.AddMergedRegion(new CellRangeAddress(17, 17, 1, 2));
                sheet.AddMergedRegion(new CellRangeAddress(17, 17, 3, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(17).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("研通科技股份有限公司");
                            break;
                        case 1:
                            cell.SetCellValue("工程師(RD) 2014/08 ~ 2016/08");
                            break;
                        case 3:
                            cell.SetCellValue("擔任MCU產品研發工程師，主要是依客戶提供的規格表設計一套程式流程進而實作之。");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(18);
                sheet.AddMergedRegion(new CellRangeAddress(18, 18, 0, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(18).CreateCell(i);
                    cell.CellStyle = headerStyle;
                    if (i == 0)
                        cell.SetCellValue("個人優缺點");
                }

                sheet.CreateRow(19);
                sheet.AddMergedRegion(new CellRangeAddress(19, 21, 0, 0));
                sheet.AddMergedRegion(new CellRangeAddress(19, 19, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(19).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("優點");
                            break;
                        case 1:
                            cell.SetCellValue("充滿好奇心 -喜歡學習新事物及新技術，看到有趣或實用的技術時，會自己嘗試做做看並且學習起來。");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(20);
                sheet.AddMergedRegion(new CellRangeAddress(20, 20, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(20).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("個性好相處 -個性較溫和，不太會與人吵架結怨。");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(21);
                sheet.AddMergedRegion(new CellRangeAddress(21, 21, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(21).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("富有責任感 -分配到的任務會盡力去完成，如有閒暇之餘也喜歡幫助別人。");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(22);
                sheet.AddMergedRegion(new CellRangeAddress(22, 23, 0, 0));
                sheet.AddMergedRegion(new CellRangeAddress(22, 22, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(22).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 0:
                            cell.SetCellValue("缺點");
                            break;
                        case 1:
                            cell.SetCellValue("英文程度稍弱 -利用APP一天學習一部影片，看懂內容並且背下其中單字，增強英文能力。");
                            break;
                        default:
                            break;
                    }
                }

                sheet.CreateRow(23);
                sheet.AddMergedRegion(new CellRangeAddress(23, 23, 1, 4));
                for (int i = 0; i <= 4; i++)
                {
                    cell = (XSSFCell)sheet.GetRow(23).CreateCell(i);
                    if (i == 0)
                        cell.CellStyle = centerStyle;
                    else
                        cell.CellStyle = leftStyle;

                    switch (i)
                    {
                        case 1:
                            cell.SetCellValue("記憶力稍差 -勤作筆記，列出Todolist幫助記憶，讓自己永遠知道下一步要做什麼。");
                            break;
                        default:
                            break;
                    }
                }
            #endregion


            // 讀取圖片 必須使用絕對路徑
            System.Drawing.Image image = System.Drawing.Image.FromFile("D:/160917_Resume_Yock/YockResume/images/YC.jpg");
            ImageFormat thisFormat = image.RawFormat;

            // 產生縮圖
            decimal sizeRatio = ((decimal)image.Height / image.Width);
            int thumbWidth = 160;
            int thumbHeight = decimal.ToInt32(sizeRatio * thumbWidth);
            var thumbStream = image.GetThumbnailImage(thumbWidth, thumbHeight, () => false, IntPtr.Zero);
            var memoryStream = new MemoryStream();
            thumbStream.Save(memoryStream, ImageFormat.Jpeg);

            // 將縮圖加入到 workbook 中
            pictureIndex = workbook.AddPicture(memoryStream.ToArray(), NPOI.SS.UserModel.PictureType.JPEG);

            // 將圖定位到 worksheet 中
            XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, 2, 0, 0);
            XSSFPicture picture = (XSSFPicture)patriarch.CreatePicture(anchor, pictureIndex);
            var size = picture.GetImageDimension();
            //row2.HeightInPoints = thumbHeight;
            anchor.Dx1 = 10;
            anchor.Dy1 = 10;
            anchor.Dx2 = 10;
            anchor.Dy2 = 10;

            picture.Resize();

            memoryStream.Close();


            // 將資料寫入串流
            MemoryStream file = new MemoryStream();
            workbook.Write(file);
            file.Close();
            return file;
        }
    }
}