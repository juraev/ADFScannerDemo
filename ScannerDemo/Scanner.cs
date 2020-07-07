using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WIA;
using System.Windows.Forms;
using System.IO;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace ScannerDemo
{
    class WIA_DPS_DOCUMENT_HANDLING_SELECT
    {
        public const uint FEEDER = 0x00000001;
        public const uint FLATBED = 0x00000002;
    }

    class WIA_DPS_DOCUMENT_HANDLING_STATUS
    {
        public const uint FEED_READY = 0x00000001;
    }

    class WIA_PROPERTIES
    {
        public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
        public const uint WIA_DIP_FIRST = 2;
        public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        //
        // Scanner only device properties (DPS)
        //
        public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
        public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
        public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
    }

    class WIA_ERRORS
    {
        public const uint BASE_VAL_WIA_ERROR = 0x80210000;
        public const uint WIA_ERROR_PAPER_EMPTY = BASE_VAL_WIA_ERROR + 3;
    }
    class Scanner
    {
        private readonly DeviceInfo _deviceInfo;
        private int resolution = 150;
        private int width_pixel = 1250;
        private int height_pixel = 1700;
        private int color_mode = 1;
        private string DeviceID;
        const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatGIF = "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
        const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";

        public Scanner(DeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }
 
        /// <summary>
        /// Scan a image with PNG Format
        /// </summary>
        /// <returns></returns>
        public Images ScanPNG()
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            Images images = new Images();

            // Select the scanner
            CommonDialogClass dlg = new CommonDialogClass(); 

            var item = device.Items[1];

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatPNG, true);

                if(scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;
                    images.add(imageFile);
                    // Return the imageFile
                    return images;
                }
            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode ==  0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }else if(errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }else
                {
                    MessageBox.Show("A non catched error occurred, check the console","Error",MessageBoxButtons.OK);
                }
            }

            return images;
        }

        /// <summary>
        /// Scan a image with JPEG Format
        /// </summary>
        /// <returns></returns>
        public Images ScanJPEG()
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            // Select the scanner
            CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];

            Images images = new Images();

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatJPEG, true);

                if (scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;
                    images.add(imageFile);

                    // Return the imageFile
                    return images;
                }
            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return images;
        }

        /// <summary>
        /// Scan a image with TIFF Format
        /// </summary>
        /// <returns> Images </returns>
        public Images ScanTIFF()
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            // Select the scanner
            CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];

            Images images = new Images();

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatTIFF, true);

                if (scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;

                    images.add(imageFile);

                    // Return the imageFile
                    return images;
                }
            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return images;
        }

        /// <summary>
        ///  Scan multi-paged document 
        /// </summary>
        /// <returns>Images</returns>
        public Images ADFScan()
        {

            //Choose Scanner
            CommonDialogClass class1 = new CommonDialogClass();
            Images images = new Images();
            Device d = class1.ShowSelectDevice(WiaDeviceType.UnspecifiedDeviceType, true, false);
            if (d != null)
            {
                this.DeviceID = d.DeviceID;
            }
            else
            {
                //no scanner chosen
                return null;
            }



            WIA.CommonDialog WiaCommonDialog = new CommonDialogClass();

            bool hasMorePages = true;
            int x = 0;
            int numPages = 0;
            ImageFile res = null;
            while (hasMorePages)
            {
                //Create DeviceManager
                DeviceManager manager = new DeviceManagerClass();
                Device WiaDev = null;
                foreach (DeviceInfo info in manager.DeviceInfos)
                {
                    if (info.DeviceID == this.DeviceID)
                    {
                        WIA.Properties infoprop = null;
                        infoprop = info.Properties;

                        //connect to scanner
                        WiaDev = info.Connect();

                        break;
                    }
                }

                //Start Scan

                WIA.ImageFile img = null;
                WIA.Item Item = WiaDev.Items[1] as WIA.Item;

                try
                {

                    img = (ImageFile)WiaCommonDialog.ShowTransfer(Item, wiaFormatJPEG, false);
                    res = img;


                    //process image:
                    //one would do image processing here
                    //

                    //Save to file
                    //string varimagefilename = "c:\\test" + x.tostring() + ".jpg";
                    //if (file.exists(varimagefilename))
                    //{
                    //    //file exists, delete it
                    //    file.delete(varimagefilename);
                    //}
                    //img.savefile(varimagefilename);

                    images.add(res);

                    numPages++;
                    img = null;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    Item = null;
                    //determine if there are any more pages waiting
                    Property documentHandlingSelect = null;
                    Property documentHandlingStatus = null;
                    foreach (Property prop in WiaDev.Properties)
                    {
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_SELECT)
                            documentHandlingSelect = prop;
                        if (prop.PropertyID == WIA_PROPERTIES.WIA_DPS_DOCUMENT_HANDLING_STATUS)
                            documentHandlingStatus = prop;


                    }

                    hasMorePages = false; //assume there are no more pages
                    if (documentHandlingSelect != null)
                    //may not exist on flatbed scanner but required for feeder
                    {
                        //check for document feeder
                        if ((Convert.ToUInt32(documentHandlingSelect.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_SELECT.FEEDER) != 0)
                        {
                            hasMorePages = ((Convert.ToUInt32(documentHandlingStatus.get_Value()) & WIA_DPS_DOCUMENT_HANDLING_STATUS.FEED_READY) != 0);
                        }
                    }
                    x++;
                }
            }
            return images;
        }

        /// <summary>
        /// Adjusts the settings of the scanner with the providen parameters.
        /// </summary>
        /// <param name="scannnerItem">Expects a </param>
        /// <param name="scanResolutionDPI">Provide the DPI resolution that should be used e.g 150</param>
        /// <param name="scanStartLeftPixel"></param>
        /// <param name="scanStartTopPixel"></param>
        /// <param name="scanWidthPixels"></param>
        /// <param name="scanHeightPixels"></param>
        /// <param name="brightnessPercents"></param>
        /// <param name="contrastPercents">Modify the contrast percent</param>
        /// <param name="colorMode">Set the color mode</param>
        private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        /// <summary>
        /// Declare the ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (string) this._deviceInfo.Properties["Name"].get_Value();
        }
    }
}
