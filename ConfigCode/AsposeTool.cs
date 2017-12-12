using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Aspose.Cells;

namespace ConfigCode
{
    public class AsposeTool
    {
        #region 将Excel转换为DataTable
        /// <summary>
        /// 将Excel转换为DataTable
        /// </summary>
        /// <param name="strFileName">文件路径</param>
        /// <returns>DataTable</returns>
        public static List<String> ReadExcel(String strFileName)
        {
            List<string> Lst = new List<string>();
            Workbook book = new Workbook(strFileName);

            //book.Open(strFileName);
            for (int i = 0; i < book.Worksheets.Count; i++)
            {
                if (book.Worksheets[i] != null)
                {
                    Worksheet sheet = book.Worksheets[i];
                    Cells cells = sheet.Cells;
                    DataTable dd = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                    for (int j = 0; j < dd.Rows.Count; j++)
                    {
                        Lst.Add(Convert.ToString(dd.Rows[j][3]));
                    }

                }
            }
            return Lst;
        }
        #endregion
        #region 将Excel转换为DataTable
        /// <summary>
        /// 将Excel转换为DataTable
        /// </summary>
        /// <param name="strFileName">文件路径</param>
        /// <returns>DataTable</returns>
        public static List<DataTable> ReadExcell(String strFileName)
        {
            List<DataTable> dt = new List<DataTable>();
            Workbook book = new Workbook(strFileName);
            //book.Open(strFileName);

            for (int i = 0; i < book.Worksheets.Count; i++)
            {
                if (book.Worksheets[i] != null)
                {
                    Worksheet sheet = book.Worksheets[i];
                    Cells cells = sheet.Cells;
                    DataTable dd = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
                    dt.Add(dd);
                }
            }

            return dt;
        }
        #endregion
        #region 将DataTable转换为Excel
        public static Boolean DatatableToExcel(DataTable dt, string outFileName)
        {
            Workbook book = new Workbook();
            Worksheet sheet = book.Worksheets[0];
            Boolean yn = false;
            try
            {
                //sheet.Name = sheetName;

                //AddTitle(title, dt.Columns.Count);
                //AddHeader(dt);
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        sheet.Cells[r + 1, c].PutValue(dt.Rows[r][c].ToString());
                    }
                }

                sheet.AutoFitColumns();
                //sheet.AutoFitRows();

                book.Save(outFileName);
                yn = true;
                return yn;
            }
            catch (Exception)
            {
                return yn;
                // throw e;
            }
        }
        #endregion

        public DataTable Excel2DataTable(String strFileName)
        {

            Workbook book = new Workbook(strFileName);
            //book.Open(strFileName);
            Worksheet sheet = book.Worksheets[0];
            Cells cells = sheet.Cells;
            DataTable dt= cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            return dt;













            //Workbook book = new Workbook();
            //DataTable dd;
            //book.Open(strFileName);
            ////for (int i = 0; i < book.Worksheets.Count; i++)
            ////{
            //if (book.Worksheets[0] != null)
            //{
            //    Worksheet sheet = book.Worksheets[0];
            //    Cells cells = sheet.Cells;
            //    dd = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, true);
            //    DataModel.dt.Columns.Clear();
            //    DataModel.dt = dd;
            //    //for (int j = 0; j < dd.Rows.Count; j++)
            //    //{
            //    //    Lst.Add(Convert.ToString(dd.Rows[j][3]));
            //    //}

            //    //}
            //}
        }



    }
}
