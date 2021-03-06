﻿using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace BEMAttendance.Models
{
    public class XlsTransferData:ExcelTransferData
    {
        public override Stream GetStream(DataTable table)
        {
            base._workBook = new HSSFWorkbook();
            return base.GetStream(table);
        }

        public override DataTable GetData(Stream stream)
        {
            base._workBook = new HSSFWorkbook(stream);
            return base.GetData(stream);
        }

    }
}