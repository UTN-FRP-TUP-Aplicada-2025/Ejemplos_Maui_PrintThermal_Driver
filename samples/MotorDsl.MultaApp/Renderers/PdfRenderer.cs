using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace MotorDsl.MultaApp.Renderers
{
    public class PdfRenderer : IRenderer
    {
        public string Target => "pdf";

        public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
        {
            var result = new RenderResult("pdf");
            try
            {
#if ANDROID
                if (PdfSharpCore.Fonts.GlobalFontSettings.FontResolver == null || !(PdfSharpCore.Fonts.GlobalFontSettings.FontResolver is CustomFontResolver))
                {
                    PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new CustomFontResolver();
                }
#endif
                using var doc = new PdfDocument();
                var page = doc.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                using var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);

                var layoutInfos = document.NodeLayoutInfo.Values.OrderBy(n => n.LineNumber);
                double y = 40;
                foreach (var node in layoutInfos)
                {
                    if (!string.IsNullOrEmpty(node.WrappedText))
                    {
                        gfx.DrawString(node.WrappedText, font, XBrushes.Black, new XPoint(40, y));
                        y += font.Height + 4;
                    }
                }

                using var ms = new MemoryStream();
                doc.Save(ms, false);
                result.Output = ms.ToArray();
            }
            catch (Exception ex)
            {
                result.AddError($"PDF error: {ex.Message}");
            }
            return result;
        }

#if ANDROID
        public class CustomFontResolver : PdfSharpCore.Fonts.IFontResolver
        {
            public string DefaultFontName => "Arial";

            public byte[] GetFont(string faceName)
            {
                // Retornar una fuente embebida o null para usar default
                return null;
            }

            public PdfSharpCore.Fonts.FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                return new PdfSharpCore.Fonts.FontResolverInfo("Arial");
            }
        }
#endif
    }
}
