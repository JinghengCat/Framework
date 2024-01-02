using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CfgGenerator
{
    public abstract class AReader
    {
        public abstract int RowCount { get; }
        public abstract int ColumnCount { get; }
        public abstract void Open(string filePath);
        public abstract void Close();

        public abstract int ReadInt(int rowNum, int columnNum);
        public abstract float ReadFloat(int rowNum, int columnNum);
        public abstract string ReadString(int rowNum, int columnNum);
        public abstract bool ReadBoolean(int rowNum, int columnNum);
    }
    
    public class ExcelReader : AReader
    {
        private FileStream _fileStream;
        private IWorkbook _workBook;
        private ISheet _sheet;

        private int _rowCount;
        private int _columnCount;

        public int SheetCount { get => _workBook.NumberOfSheets; }
        public override int RowCount { get => _rowCount; }
        public override int ColumnCount { get => _columnCount; }

        public string SheetName { get => _sheet.SheetName; }
        
        public override void Open(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not exist");
            }
            
            _fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            
            if (Path.GetExtension(filePath) == ".xls")
            {
                _workBook = new HSSFWorkbook(_fileStream);
            }
            else if (Path.GetExtension(filePath) == ".xlsx")
            {
                _workBook = new XSSFWorkbook(_fileStream);
            }
        }
        
        public void OpenSheet(int sheetIndex)
        {
            _sheet = _workBook.GetSheetAt(sheetIndex);
            // Row获取的是index
            _rowCount = _sheet.LastRowNum + 1;
            // Column获取的是index+1
            _columnCount = _sheet.GetRow(0).LastCellNum;
        }
        
        public override void Close()
        {
            _workBook.Close();
            _workBook.Dispose();
            _fileStream.Close();
            _fileStream.Dispose();

            _workBook = null;
            _fileStream = null;
        }

        private ICell GetCell(int rowNum, int columnNum)
        {
            IRow row = _sheet.GetRow(rowNum);
            ICell cell = row.GetCell(columnNum);
            return cell;
        }
        
        public override int ReadInt(int rowNum, int columnNum)
        {
            ICell cell = GetCell(rowNum, columnNum);
            if (cell == null)
            {
                return 0;
            }
            
            if (cell.CellType == CellType.String)
            {
                return int.Parse(cell.StringCellValue);
            }
            else
            {
                return (int)cell.NumericCellValue;
            }
        }

        public override float ReadFloat(int rowNum, int columnNum)
        {
            ICell cell = GetCell(rowNum, columnNum);
            if (cell == null)
            {
                return 0f;
            }
            
            if (cell.CellType == CellType.String)
            {
                return float.Parse(cell.StringCellValue);
            }
            else
            {
                return (float)cell.NumericCellValue;
            }
        }

        public override string ReadString(int rowNum, int columnNum)
        {
            ICell cell = GetCell(rowNum, columnNum);
            if (cell == null)
            {
                return String.Empty;
            }
            
            return cell.StringCellValue;
        }

        public override bool ReadBoolean(int rowNum, int columnNum)
        {
            ICell cell = GetCell(rowNum, columnNum);
            if (cell == null)
            {
                return false;
            }
            return cell.BooleanCellValue;
        }
    }
}