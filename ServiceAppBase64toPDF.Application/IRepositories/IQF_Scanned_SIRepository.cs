using ServiceAppBase64toPDF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppBase64toPDF.Application
{
    public interface IQF_Scanned_SIRepository
    {
        //Task<QF_Scanned_SI> GetScannedSIList();
        Task<QF_Scanned_SI> GetScannedSIList();
        Task<bool> SetPDFtoScanned_SI(string SIpdf, string RCRNumber);
    }
}
