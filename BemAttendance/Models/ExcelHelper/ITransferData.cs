using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEMAttendance.Models
{
    public interface ITransferData
    {
        Stream GetStream(DataTable table);
        DataTable GetData(Stream stream);
    }
    public enum DataFileType
    {
        CSV,
        XLS,
        XLSX
    }
}
