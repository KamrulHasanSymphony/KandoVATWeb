using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymVATWebUI.Areas.VMS.Models
{
    public class GenericReport<TReport> : Controller where TReport : ReportClass
    {
        public FileStreamResult RenderReportAsPDF(TReport rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
    }
}