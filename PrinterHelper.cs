using System.Runtime.InteropServices;
using System.Text;

public static class PrinterHelper
{
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
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
                return false;

            if (!StartDocPrinter(hPrinter, 1, ref di))
                return false;

            if (!StartPagePrinter(hPrinter))
                return false;

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
            if (hPrinter != IntPtr.Zero)
                ClosePrinter(hPrinter);
        }
    }
}