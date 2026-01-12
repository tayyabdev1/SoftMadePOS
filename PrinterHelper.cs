using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// 1. Helper Class for passing Data safely
public class ReceiptItem
{
    public string Name { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
}

public static class PrinterHelper
{
    // ==========================================
    // 1. LOW LEVEL WINDOWS DRIVER CODE (Do not touch)
    // ==========================================
    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOC_INFO_1 pDocInfo);

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct DOC_INFO_1
    {
        public string pDocName;
        public string pOutputFile;
        public string pDatatype;
    }

    // ==========================================
    // 2. ESC/POS COMMANDS (For Cutting & Formatting)
    // ==========================================
    private const string ESC = "\x1B";
    private const string GS = "\x1D";
    private const string CutPaper = GS + "V" + "\x41" + "\x00"; // Full Cut
    private const string InitPrinter = ESC + "@";
    private const string BoldOn = ESC + "E" + "\x01";
    private const string BoldOff = ESC + "E" + "\x00";
    private const string CenterAlign = ESC + "a" + "\x01";
    private const string LeftAlign = ESC + "a" + "\x00";

    // ==========================================
    // 3. MAIN PRINT FUNCTION (Call this from POS.razor)
    // ==========================================
    public static bool PrintReceipt(string printerName, string shopName, string address, List<ReceiptItem> items, decimal total, decimal discount)
    {
        StringBuilder sb = new StringBuilder();

        // --- 1. Header ---
        sb.Append(InitPrinter);
        sb.Append(CenterAlign);
        sb.Append(BoldOn + shopName + "\n" + BoldOff);
        sb.Append(address + "\n");
        sb.Append(DateTime.Now.ToString("dd-MM-yyyy hh:mm tt") + "\n");
        sb.Append("--------------------------------\n"); // 32 Dashes for 58mm

        // --- 2. Columns Headers ---
        sb.Append(LeftAlign);
        // Item (16) | Qty (5) | Price (9) = 32 Chars
        sb.Append($"{"Item".PadRight(16)}{"Qty".PadLeft(5)}{"Price".PadLeft(9)}\n");
        sb.Append("--------------------------------\n");

        // --- 3. Items Loop ---
        foreach (var item in items)
        {
            // Truncate name if too long (max 16 chars)
            string name = item.Name.Length > 16 ? item.Name.Substring(0, 16) : item.Name;

            // Format Line
            sb.Append($"{name.PadRight(16)}{item.Qty.ToString().PadLeft(5)}{item.Price.ToString().PadLeft(9)}\n");
        }
        sb.Append("--------------------------------\n");

        // --- 4. Totals ---
        sb.Append(RightAlign("Subtotal: " + (total + discount).ToString() + "\n"));
        sb.Append(RightAlign("Discount: " + discount.ToString() + "\n"));
        sb.Append(BoldOn + RightAlign("TOTAL: " + total.ToString() + "\n") + BoldOff);

        // --- 5. Footer ---
        sb.Append(CenterAlign);
        sb.Append("\nThank You!\n");
        sb.Append("Software by SoftMade Studio\n");
        sb.Append("\n\n"); // Feed lines
        sb.Append(CutPaper); // CUT THE PAPER!

        // Send to Printer
        return SendStringToPrinter(printerName, sb.ToString());
    }

    // Helper to align text right (for totals)
    private static string RightAlign(string text)
    {
        // 32 Chars total width for 58mm paper
        return text.PadLeft(32);
    }

    // ==========================================
    // 4. LOW LEVEL SENDER
    // ==========================================
    public static bool SendStringToPrinter(string printerName, string text)
    {
        IntPtr hPrinter = IntPtr.Zero;
        DOC_INFO_1 di = new DOC_INFO_1
        {
            pDocName = "POS Receipt",
            pDatatype = "RAW",
            pOutputFile = null
        };

        try
        {
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero)) return false;
            if (!StartDocPrinter(hPrinter, 1, ref di)) return false;
            if (!StartPagePrinter(hPrinter)) return false;

            // Convert string to Bytes (using code page 437 for legacy printers is safer, but UTF8 works for Epson T88V)
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            IntPtr pBytes = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, pBytes, bytes.Length);

            bool success = WritePrinter(hPrinter, pBytes, bytes.Length, out int written);

            Marshal.FreeHGlobal(pBytes);
            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
            return success;
        }
        finally
        {
            if (hPrinter != IntPtr.Zero) ClosePrinter(hPrinter);
        }
    }
}