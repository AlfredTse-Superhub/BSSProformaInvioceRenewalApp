using BSSProformaInvioceRenewalApp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;

namespace BSSProformaInvioceRenewalApp
{
    public static class InvoiceHandling
    {
        private static Font titleFont = FontFactory.GetFont("Arial", 14.0f, BaseColor.DARK_GRAY);
        private static Font contentFont = FontFactory.GetFont("Arial", 7.0f, BaseColor.DARK_GRAY);
        private static Font contentBoldFont = FontFactory.GetFont("Arial", 7.0f, Font.BOLD, BaseColor.BLACK);
        private static Font contentUnderlineFont = FontFactory.GetFont("Arial", 7.0f, Font.UNDERLINE, BaseColor.BLACK);
        private static Font tableHeaderFont = FontFactory.GetFont("Arial", 7.0f, Font.BOLD, BaseColor.WHITE);
        private static Font tableContentFont = FontFactory.GetFont("Arial", 7.0f, BaseColor.DARK_GRAY);
        private static BaseColor themeColor = new(System.Drawing.ColorTranslator.FromHtml("#ed157c"));
        private static BaseColor tableBorderColor = new(System.Drawing.ColorTranslator.FromHtml("#f0f0f0"));
        private static float cellBorderWidth = 1.5f;
        private static float cellHeaderPadding = 3;
        private static float cellPadding = 3.5f;

        public static void GeneratePDF(List<Subscription> subGroup, string invoiceID)
        {
            try
            {
                string logoPath = Environment.CurrentDirectory + "/Images/logo.png";
                string outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                Subscription firstSub = subGroup.FirstOrDefault();
                string contactName = (firstSub.BillToContact != null)
                    ? firstSub.BillToContact?.FirstName + " " + firstSub.BillToContact.LastName
                    : "N/A";
                string billToTel = string.IsNullOrEmpty(firstSub.Customer.Phone)
                    ? "N/A"
                    : firstSub.Customer.Phone;
                string billToEmail = string.IsNullOrEmpty(firstSub.Customer.BillToEmail)
                    ? "N/A"
                    : firstSub.Customer.BillToEmail;
                string billToName = firstSub.BillingTo.Name ?? "N/A";
                string billToAddress = firstSub.Customer.Address ?? "";
                string accountName = firstSub.Account.Name ?? "N/A";
                decimal totalPrice = 0;

                FileStream fs = new(Environment.CurrentDirectory + "/PDF/Prof_invoice_" + invoiceID + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);
                // MemoryStream ms = new MemoryStream();
                Document doc = new(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                FontSelector selector = new FontSelector();
                selector.AddFont(FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14));
                selector.AddFont(FontFactory.GetFont("MSung-Light", "UniCNS-UCS2-H", BaseFont.NOT_EMBEDDED));
                Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleAbsoluteWidth(70);
                logo.ScaleAbsoluteHeight(20);

                doc.Open();

                // Logo
                Paragraph p1 = new Paragraph();
                p1.Add(logo);

                // Title
                Paragraph p2 = new Paragraph();
                p2.Font = titleFont;
                p2.Add("INVOICE");

                // Customer Info
                Paragraph p3 = new();
                PdfPTable CustomerInfo = new(3);
                int[] p3TableWidth = { 40, 25, 35 };
                CustomerInfo.WidthPercentage = 100;
                CustomerInfo.SetWidths(p3TableWidth);
                CustomerInfo.DefaultCell.Border = Rectangle.NO_BORDER;
                Phrase phrase1 = new();
                phrase1.Add(new Chunk("BILL TO: \n", contentBoldFont));
                phrase1.Add(new Chunk(billToName + "\n\nAttention: " + contactName + "\n" + billToAddress + "\n\nTel: " + billToTel + "\nEmail: " + billToEmail, contentFont));
                Phrase phrase2 = new();
                phrase2.Add(new Chunk("REF NUMBER \n", contentBoldFont));
                phrase2.Add(new Chunk(invoiceID, contentFont));
                phrase2.Add(new Chunk("\n\nDATE \n", contentBoldFont));
                phrase2.Add(new Chunk(DateTime.Now.ToString("yyyy-MM-dd"), contentFont));
                phrase2.Add(new Chunk("\n\nCUSTOMER REF \n", contentBoldFont));
                phrase2.Add(new Chunk(accountName, contentFont));
                Phrase phrase3 = new Phrase();
                phrase3.Add(new Chunk("Superhub Limited \n", contentBoldFont));
                phrase3.Add(new Chunk("Accounting Dept. \n12 / F, \nWong Tze Buliding, \nKwun Tong \nHong Kong \nTel: 852 - 23531445 \nFax: 852 - 23531105 \nEmail: css@superhub.com.hk", contentFont));
                CustomerInfo.AddCell(phrase1);
                CustomerInfo.AddCell(phrase2);
                CustomerInfo.AddCell(phrase3);
                p3.Add(CustomerInfo);

                // Sub-title
                Paragraph p4 = new();
                p4.Font = contentBoldFont;
                p4.Add("SERVICE DETAILS\n\n");

                // Subscriptions Item Table
                Paragraph p5 = new();
                PdfPTable p5TblHeader = new(7);
                int[] p5TableWidth = { 12, 75, 15, 20, 38, 30, 30 };
                p5TblHeader.WidthPercentage = 100;
                p5TblHeader.SetWidths(p5TableWidth);
                string[] p5Headers = { "ITEM", "DESCRIPTION", "QTY", "UNIT", "BILLING PERIOD", "UNIT PRICE HK$", "AMOUNT HK$" };
                foreach (string header in p5Headers)
                {
                    PdfPCell pdfPCell = new(new Phrase(header, tableHeaderFont));
                    pdfPCell.BackgroundColor = themeColor;
                    pdfPCell.BorderColor = tableBorderColor;
                    pdfPCell.BorderWidth = cellBorderWidth;
                    pdfPCell.Padding = cellHeaderPadding;
                    pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pdfPCell.VerticalAlignment = Element.ALIGN_CENTER;
                    if (header == "ITEM")
                    {
                        pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    }
                    if (header == "DESCRIPTION")
                    {
                        pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    }
                    p5TblHeader.AddCell(pdfPCell);
                }
                PdfPTable p5TblContent = new(7);
                p5TblContent.WidthPercentage = 100;
                p5TblContent.SetWidths(p5TableWidth);
                p5TblContent.DefaultCell.BorderColor = tableBorderColor;
                p5TblContent.DefaultCell.BorderWidth = cellBorderWidth;
                p5TblContent.DefaultCell.Padding = cellPadding;
                p5TblContent.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                p5TblContent.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                var ItemNo = 1;
                foreach (var item in subGroup)
                {
                    string billingStartDate = DateTime.Parse(item.EndDate.Substring(0, 10)).AddDays(1).ToString("yyyy-MM-dd");
                    string billingEndDate = DateTime.Parse(item.EndDate.Substring(0, 10)).AddYears(1).ToString("yyyy-MM-dd");
                    var itemTotal = decimal.Parse(item.Quantity) * decimal.Parse(item.Product.FinalUnitPrice);
                    PdfPCell cell1 = new(new Phrase(ItemNo.ToString(), tableContentFont));
                    PdfPCell cell2 = new(new Phrase(item.Name, tableContentFont));
                    cell1.BorderColor = cell2.BorderColor = tableBorderColor;
                    cell1.BorderWidth = cell2.BorderWidth = cellBorderWidth;
                    cell1.Padding = cell2.Padding = cellPadding;
                    cell1.VerticalAlignment = cell2.VerticalAlignment = Element.ALIGN_CENTER;
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    p5TblContent.AddCell(cell1);
                    p5TblContent.AddCell(cell2);
                    p5TblContent.AddCell(new Phrase(item.Quantity, tableContentFont));
                    p5TblContent.AddCell(new Phrase(item.Unit.Name, tableContentFont));
                    // p5TblContent.AddCell(new Phrase(item.EndDate.Substring(0, 10), tableContentFont));
                    p5TblContent.AddCell(new Phrase(billingStartDate + " - " + billingEndDate, tableContentFont));
                    p5TblContent.AddCell(new Phrase("$" + String.Format("{0:0.00}", decimal.Parse(item.Product.FinalUnitPrice)), tableContentFont));
                    p5TblContent.AddCell(new Phrase("$" + String.Format("{0:0.00}", itemTotal), tableContentFont));
                    ItemNo += 1;
                    totalPrice += itemTotal;
                    if (item.Addons.Count > 0)
                    {
                        foreach (var addon in item.Addons)
                        {
                            decimal addonTotal = decimal.Parse(addon.Quantity) * decimal.Parse(addon.PriceInfo.UnitPrice.Value);
                            PdfPCell addonCell1 = new(new Phrase(ItemNo.ToString(), tableContentFont));
                            PdfPCell addonCell2 = new(new Phrase(addon.Product.Name, tableContentFont));
                            addonCell1.BorderColor = addonCell2.BorderColor = tableBorderColor;
                            addonCell1.BorderWidth = addonCell2.BorderWidth = cellBorderWidth;
                            addonCell1.Padding = addonCell2.Padding = cellPadding;
                            addonCell1.VerticalAlignment = addonCell2.VerticalAlignment = Element.ALIGN_CENTER;
                            addonCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            addonCell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            p5TblContent.AddCell(addonCell1);
                            p5TblContent.AddCell(addonCell2);
                            p5TblContent.AddCell(new Phrase(((int)double.Parse(addon.Quantity)).ToString(), tableContentFont));
                            p5TblContent.AddCell(new Phrase(addon.PriceInfo.Unit.Name, tableContentFont));
                            p5TblContent.AddCell(new Phrase(item.EndDate.Substring(0, 10), tableContentFont));
                            p5TblContent.AddCell(new Phrase("$" + String.Format("{0:0.00}", decimal.Parse(addon.PriceInfo.UnitPrice.Value)), tableContentFont));
                            p5TblContent.AddCell(new Phrase("$" + String.Format("{0:0.00}", addonTotal), tableContentFont));
                            totalPrice += addonTotal;
                        }
                    }
                }
                p5.Add(p5TblHeader);
                p5.Add(p5TblContent);

                // Total Expense Table
                Paragraph p6 = new();
                PdfPTable p6TblHeader = new(2);
                int[] p6TableWidth = { 63, 27 };
                p6TblHeader.WidthPercentage = 50;
                p6TblHeader.SetWidths(p6TableWidth);
                p6TblHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                string[] p6tableHeaders = { "Payable Items", "Amount" };
                foreach (string header in p6tableHeaders)
                {
                    PdfPCell pdfPCell = new(new Phrase(header, tableHeaderFont));
                    pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pdfPCell.BackgroundColor = themeColor;
                    pdfPCell.BorderColor = tableBorderColor;
                    pdfPCell.BorderWidth = cellBorderWidth;
                    pdfPCell.Padding = cellHeaderPadding;
                    if (header == "Payable Items")
                    {
                        pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    }
                    p6TblHeader.AddCell(pdfPCell);
                }
                PdfPTable p6TblContent = new(2);
                p6TblContent.WidthPercentage = 50;
                p6TblContent.SetWidths(p6TableWidth);
                p6TblContent.HorizontalAlignment = Element.ALIGN_LEFT;
                p6TblContent.DefaultCell.BorderColor = tableBorderColor;
                p6TblContent.DefaultCell.BorderWidth = cellBorderWidth;
                p6TblContent.DefaultCell.Padding = cellPadding;
                PdfPCell p6ContentCell = new(new Phrase("Total:", tableContentFont));
                p6ContentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                p6ContentCell.BorderColor = tableBorderColor;
                p6ContentCell.BorderWidth = cellBorderWidth;
                p6ContentCell.Padding = cellPadding;
                p6TblContent.AddCell(p6ContentCell);
                p6TblContent.AddCell(new Phrase("$" + String.Format("{0:0.00}", totalPrice), tableContentFont));
                p6.Add(p6TblHeader);
                p6.Add(p6TblContent);

                // Sub-title
                Paragraph p7 = new();
                p7.Font = contentBoldFont;
                p7.Add("TERMS AND CONDITIONS\n");

                // Terms & Conditions
                Paragraph p8 = new();
                p8.Font = contentFont;
                p8.SetLeading(0f, 2f);
                p8.Add(
                    "1. No signature is required for this computer printout."
                    + "\n2. Disputes concerning any charges must be raised within 7 days of the invoice date."
                    + "\n3. Interest of 3% per month will be charged on overdue accounts from the date of which payment was due to the date of actual\n    payment at a daily interest rate basis."
                    + "\n4. Payable amount is the Balance Due amount stated on the invoice at the time interval specify on the invoice"
                    + "\n5. Payment should be made payable to \"SuperHub Ltd.\" or deposit to our bank accounts below."
                    + "\n        Bank Name: HSBC Bank                           Name: Hang Seng Bank"
                    + "\n        Bank Code: 004                                         Bank Code: 024"
                    + "\n        Bank Account #: 191‐346477‐001                 Bank Account #: 294‐230875‐001"
                    + "\n        SWIFT Code: HSBCHKHHHKH                SWIFT Code: HASEHKHH"
                    + "\n    Please email the receipt to "
                );
                p8.Add(new Chunk("cs@superhub.com.hk", contentUnderlineFont));
                p8.Add(
                    " with invoice number on the payment receipt reference."
                    + "\n6. Credit card and autopay payment will be billed 2 working days before the due date."
                    + "\n7. Deposit payment will be refunded, if qualified and after settlement of all outstanding payment, within 1 month after accepted\n    termination of service."
                    + "\n8. Our marketplace "
                );
                p8.Add(new Chunk("\"store.superhub.com.hk\"", contentUnderlineFont));
                p8.Add(
                    " enables retrieving billing history and details. Customers shall take invoice sent to\n    their registered account as official document to arrange payment before due date."
                    + "\n9. Service suspension may be executed for overdue payment remains unsettled over 30 days and we will be disclaimed and not\n    liable for any loss due to such service suspension."
                    + "\n10. Please pay within 5 working days."
                );

                doc.Add(p1);
                doc.Add(p2);
                doc.Add(new Chunk("\n"));
                doc.Add(p3);
                doc.Add(new Chunk("\n"));
                doc.Add(p4);
                doc.Add(p5);
                doc.Add(new Chunk("\n"));
                doc.Add(p6);
                doc.Add(new Chunk("\n"));
                doc.Add(p7);
                doc.Add(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                doc.Add(p8);
                doc.Close();

                fs.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
