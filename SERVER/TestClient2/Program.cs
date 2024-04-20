using Common.Network;
using Common.Proto.Player;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System;

namespace TestClient
{

    internal class Program
    {


        static async Task Main(string[] args)
        {
            TestNPOI();
        }




        static void ReleaseCOMObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        public static void TestNPOI()
        {
            string filePath = @"C:\project\MMORPG\SERVER\TestClient\test.xlsx";

            //// 创建 Excel 应用程序对象
            Application excelApp = new Application();
            excelApp.Visible = true; // 可见 Excel 程序界面

            // 打开 Excel 文件
            Workbook workbook = excelApp.Workbooks.Open(filePath);

            // 刷新共享工作簿
            workbook.RefreshAll();

            // 保存并关闭文件
            workbook.Save();
            workbook.Close();

            // 退出 Excel 应用程序
            excelApp.Quit();

            // 释放 COM 对象
            ReleaseCOMObject(workbook);
            ReleaseCOMObject(excelApp);

            Console.WriteLine("Excel文件已刷新。");


        }

    }


}