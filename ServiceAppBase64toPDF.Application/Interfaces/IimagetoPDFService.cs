using ServiceAppBase64toPDF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppBase64toPDF.Application
{
    public interface IimagetoPDFService
    {
        Task<bool> ConvertImagesToPDFAsync(QF_Scanned_SI base64Image, string filename);
    }
}
