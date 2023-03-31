using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Experior.Interfaces;
using Microsoft.Office.Interop.Excel;

namespace Experior.Plugin.Joints.DataHandler
{
    public class ExcelWriter
    {
        #region Fields

        private Application _xlApp;
        private Workbook _workBook;
        private object _misValue;

        private readonly string _name;

        #endregion

        #region Constructor

        public ExcelWriter(string name)
        {
            _name = name;
           Init(); 
        }

        #endregion

        #region Public Methods

        public void CreateSheet(string name, List<List<string>> data)
        {
            if (_workBook.Worksheets.OfType<Worksheet>().Any(s => s.Name == name))
            {
                Log.Write($"Sheet: {name} already exists !", System.Windows.Media.Colors.Red, LogFilter.Error);
                return;
            }

            var sheet = (Worksheet)_workBook.Worksheets.Add();

            if (sheet == null)
            {
                return;
            }

            sheet.Name = name;

            var rowC = 1;
            var colC = 1;
            foreach (var row in data)
            {
                foreach (var value in row)
                {
                    sheet.Cells[rowC, colC] = value;
                    colC++;
                }

                rowC++;
                colC = 1;
            }
        }
        
        public void Generate()
        {
            try
            {
                var date = System.DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace('/', '_').Replace(':', '_').Replace(' ', '_');
                
                var fileName = _name + "_" + date + ".xls";
                var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

                _workBook.SaveAs(path + @"\" + fileName, XlFileFormat.xlWorkbookNormal);
            }
            catch (Exception e)
            {
                Log.Write("Error during exporting process", System.Windows.Media.Colors.Red, LogFilter.Error);
                Log.Write(e);
                throw;
            }
            finally
            {
                _workBook.Close(true, _misValue, _misValue);
                _xlApp.Quit();
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            // Start Excel:
            _xlApp = new Application();

            // Workbook:
            _misValue = Missing.Value;
            _workBook = _xlApp.Workbooks.Add(_misValue);
        }

        #endregion
    }
}
