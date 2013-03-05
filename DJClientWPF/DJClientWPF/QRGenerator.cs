using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Controls;
using PdfSharp;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace DJClientWPF
{
    class QRGenerator
    {
        const int  WIDTH = 500;
        const int HEIGHT = 500;
        const string IMAGE_NAME = "TempQRCode.png";

        public static void GenerateQR(string encodeString, string venueName, string branding)
        {
            //Ask the user for the path where the QR pdf file will be stored.  If canceled, return and do nothing.
            string filePath = GetSaveFilePath();
            if (filePath.Equals(""))
                return;

            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode code = encoder.Encode(encodeString);

            Renderer renderer = new Renderer(25, System.Drawing.Brushes.Black, System.Drawing.Brushes.White);
            renderer.CreateImageFile(code.Matrix, IMAGE_NAME, ImageFormat.Png);

            PrintToPDF(venueName, branding, filePath);
        }

        //Ask the user for the path where the QR pdf file will be stored.  If the user cancels in the dialog, returns the empty string
        private static string GetSaveFilePath()
        {
            string path = "";

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PDF File(*.pdf)|*.pdf";
            dialog.Title = "Save QR Venue Code";
            dialog.AddExtension = true;
            DialogResult result = STAShowDialog(dialog);
            if (result == DialogResult.OK)
            {
                path = dialog.FileName;
            }

            return path;
        }

        private static DialogResult STAShowDialog(SaveFileDialog dialog)
        {
            DialogState state = new DialogState(dialog);
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.Result;
        }

        //Prints the QR code and text to a PDF files and saves it at the path selected by the user.
        private static void PrintToPDF(string venueName, string branding, string saveFilePath)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "QR Code";
            document.Info.CreationDate = DateTime.Now;
            document.Info.Author = "Karaoke Suite";

            // Create an empty page
            PdfPage page = document.AddPage();

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            //Draw the QR code
            XImage image = XImage.FromFile(IMAGE_NAME);
            double dx = (page.Width - WIDTH) / 2;
            gfx.DrawImage(image, dx, 0, 500, 500);

            // Create a font
            XFont boldFont = new XFont("Arial", 30, XFontStyle.Bold);
            XFont regFont = new XFont("Arial", 20);

            // Draw the text
            gfx.DrawString(venueName, boldFont, XBrushes.Black,
              new XRect(0, HEIGHT - 20, page.Width, page.Height),
              XStringFormats.TopCenter);

            gfx.DrawString("Scan this QR Code with the Mobioke app on your", regFont, XBrushes.Black,
                          new XRect(0, HEIGHT +20, page.Width, page.Height),
                          XStringFormats.TopCenter);

            gfx.DrawString("smartphone to log in and be able to make song requests.", regFont, XBrushes.Black,
                          new XRect(0, HEIGHT +40, page.Width, page.Height),
                          XStringFormats.TopCenter);

            // Save the document...
            document.Save(saveFilePath);
            // ...and start a viewer.
            Process.Start(saveFilePath);
        }

    }

    public class DialogState
    {
        public DialogResult Result {get; private set;}
        public SaveFileDialog Dialog {get; private set;}

        public DialogState(SaveFileDialog dialog)
        {
            this.Dialog = dialog;
        }

        public void ThreadProcShowDialog()
        {
            Result = Dialog.ShowDialog();
        }
    }
}
