using ServiceAppBase64toPDF.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppBase64toPDF.Application
{
    public class ScannedSIService : IScannedSIService
    {
        private readonly IQF_Scanned_SIRepository _qF_Scanned_SIRepository;
        private readonly IimagetoPDFService _imagetoPDFService;

        public ScannedSIService(IQF_Scanned_SIRepository qF_Scanned_SIRepository, IimagetoPDFService imagetoPDFService)
        {
            _qF_Scanned_SIRepository = qF_Scanned_SIRepository;
            _imagetoPDFService = imagetoPDFService;
        }


        public async Task<bool> GetScannedSIList()
        {
            try
            {
                QF_Scanned_SI ScannedSI = new QF_Scanned_SI();
                ScannedSI = await _qF_Scanned_SIRepository.GetScannedSIList();

                if (ScannedSI.ImageScanned != null)
                {
                    string filename = ScannedSI.RCRNumber + ".pdf";
                    bool isdone = await _imagetoPDFService.ConvertImagesToPDFAsync(ScannedSI, filename);
                    //bool isdone = await _imagetoPDFService.ConvertBase64ImageToPdf(ScannedSI, filename);
                    if (isdone)
                    {
                        bool isdoneupdate = await SetPDFtoScanned_SI(filename, ScannedSI.RCRNumber);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log or handle the exception from the calling method
                Console.WriteLine($"Error during PDF conversion: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SetPDFtoScanned_SI(string filename,string RCRNumber)
        {
            try
            {
                string SIpdf = string.Format("<iframe src=uploads/SIscanned/{0} width=600 height=500></iframe>", filename);

                bool isdone = await _qF_Scanned_SIRepository.SetPDFtoScanned_SI(SIpdf, RCRNumber);
                if (isdone)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception from the calling method
                Console.WriteLine($"Error during PDF conversion: {ex.Message}");
                return false;
            }
        }

    }
}
