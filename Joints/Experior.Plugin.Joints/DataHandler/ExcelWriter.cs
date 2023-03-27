using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace Experior.Plugin.Joints.DataHandler
{
    public class ExcelWriter
    {
        #region Fields

        private Application _xlApp;
        private Workbook _workBook;
        private object _misValue;

        #endregion

        #region Constructor



        #endregion

        #region Public Methods

        public Workbook CreateWorkBook()
        {
            _xlApp = new Application();
            _misValue = Missing.Value;
            _workBook = _xlApp.Workbooks.Add(_misValue);

            return _workBook;
        }

        public void Generate(string name)
        {
            var fileName = name + ".xls";
            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);

            _workBook.SaveAs(path + @"\" + fileName, XlFileFormat.xlWorkbookNormal);
            _workBook.Close(true, _misValue, _misValue);
            _xlApp.Quit();

            Marshal.ReleaseComObject(_workBook);
            Marshal.ReleaseComObject(_xlApp);
        }

        #endregion
    }
}
