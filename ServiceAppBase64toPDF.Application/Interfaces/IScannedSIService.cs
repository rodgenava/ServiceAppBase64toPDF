using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppBase64toPDF.Application
{
    public interface IScannedSIService
    {
        Task<bool> GetScannedSIList();
    }
}
