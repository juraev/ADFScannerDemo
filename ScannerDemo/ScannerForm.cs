using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIA;

namespace ScannerDemo
{
    public partial class ScannerForm : Form
    {
        private AsynchronousSocketListener mContext;
        public delegate void AddListItem();
        public AddListItem myDelegate;
        string name = "myscan";

        public ScannerForm()
        {
            InitializeComponent();
            myDelegate = new AddListItem(AddListItemMethod);
        }

        private void AddListItemMethod()
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListScanners();

            // Set start output folder TMP
            textBox1.Text = Path.GetTempPath();
        }

        public void SetContext(AsynchronousSocketListener context)
        {
            mContext = context;
        }

        private void ListScanners()
        {
            // Clear the ListBox.
            listBox1.Items.Clear();

            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();

            // Loop through the list of devices and add the name to the listbox
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Add the device only if it's a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                // Add the Scanner device to the listbox (the entire DeviceInfos object)
                // Important: we store an object of type scanner (which ToString method returns the name of the scanner)
                listBox1.Items.Add(
                    new Scanner(deviceManager.DeviceInfos[i])
                );
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(StartScanning).ContinueWith(result => TriggerScan());
        }

        private void TriggerScan()
        {
            Console.WriteLine("Image succesfully scanned");
        }

        public void StartScanning()
        {
            Scanner device = null;

            this.Invoke(new MethodInvoker(delegate ()
            {
                device = listBox1.SelectedItem as Scanner;
            }));

            if (device == null)
            {
                MessageBox.Show("You need to select first an scanner device from the list",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Images images = new Images();

            this.Invoke(new MethodInvoker(delegate ()
            {
                switch (3)
                {
                    case 0:
                        images = device.ScanPNG();
                        break;
                    case 1:
                        images = device.ScanJPEG();
                        break;
                    case 2:
                        images = device.ScanTIFF();
                        break;
                    case 3:
                        images = device.ADFScan();
                        break;
                }
            }));

            //// Save the doc
            string docExtension = ".pdf";

            var path = Path.Combine(textBox1.Text, name + docExtension);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            ImagesToPDFConverter converter = new ImagesToPDFConverter(images);

            if (images != null && images.size() > 0)
            {
                converter.getPDF(path.ToString());
                emitPath(true);
            } else
            {
                emitPath(false);
            }
            //pictureBox1.Image = new Bitmap(path);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                textBox1.Text = folderDlg.SelectedPath;
            }
        }

        void emitPath(bool success)
        {
            if(success)
                mContext.setDocumentPath(textBox1.Text + "\\" + name + ".pdf");
            else
                mContext.setDocumentPath("--Unsuccessfull: scanning is not finished--");
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
