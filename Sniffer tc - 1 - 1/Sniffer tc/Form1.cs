using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SharpPcap;
using PacketDotNet;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace windowsform1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            combox1_Ini();
            combox2_Ini();

            deviceIsOpen = false;
        }

        private ICaptureDevice device;
        private int timeout = 2000;
        private string filter;
        private bool deviceIsOpen;

        public ArrayList reassemblyPac = new ArrayList();
        public ArrayList simpackets = new ArrayList();
        private ArrayList packets = new ArrayList();


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.start = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.stop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.save = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.filt = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.search = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.open = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // start
            // 
            this.start.AutoEllipsis = true;
            this.start.BackColor = System.Drawing.Color.YellowGreen;
            this.start.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start.Location = new System.Drawing.Point(935, 25);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(131, 105);
            this.start.TabIndex = 0;
            this.start.Text = "start";
            this.start.UseVisualStyleBackColor = false;
            this.start.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Georgia", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(320, 62);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(468, 23);
            this.textBox1.TabIndex = 1;
            // 
            // stop
            // 
            this.stop.BackColor = System.Drawing.Color.BurlyWood;
            this.stop.Font = new System.Drawing.Font("Cambria", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stop.Location = new System.Drawing.Point(1135, 38);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(84, 67);
            this.stop.TabIndex = 3;
            this.stop.Text = "stop";
            this.stop.UseVisualStyleBackColor = false;
            this.stop.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Cambria", 15F);
            this.label1.Location = new System.Drawing.Point(33, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Eth:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Cambria", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(147, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(752, 24);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.Text = "WLAN(default)";
            // 
            // save
            // 
            this.save.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.save.Font = new System.Drawing.Font("Cambria", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save.Location = new System.Drawing.Point(18, 167);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(81, 67);
            this.save.TabIndex = 8;
            this.save.Text = "save";
            this.save.UseVisualStyleBackColor = false;
            this.save.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.Font = new System.Drawing.Font("Cambria", 15F);
            this.label2.Location = new System.Drawing.Point(33, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "Filter:";
            // 
            // filt
            // 
            this.filt.BackColor = System.Drawing.Color.MintCream;
            this.filt.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold);
            this.filt.Location = new System.Drawing.Point(810, 54);
            this.filt.Name = "filt";
            this.filt.Size = new System.Drawing.Size(75, 38);
            this.filt.TabIndex = 10;
            this.filt.Text = "filt";
            this.filt.UseVisualStyleBackColor = false;
            this.filt.Click += new System.EventHandler(this.button4_Click);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.treeView1.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.Location = new System.Drawing.Point(423, 447);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(330, 274);
            this.treeView1.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label3.Font = new System.Drawing.Font("Cambria", 15F);
            this.label3.Location = new System.Drawing.Point(32, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 23);
            this.label3.TabIndex = 13;
            this.label3.Text = "search";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Georgia", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(147, 107);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(641, 23);
            this.textBox2.TabIndex = 14;
            // 
            // search
            // 
            this.search.BackColor = System.Drawing.Color.MintCream;
            this.search.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Bold);
            this.search.Location = new System.Drawing.Point(810, 103);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(75, 39);
            this.search.TabIndex = 15;
            this.search.Text = "search";
            this.search.UseVisualStyleBackColor = false;
            this.search.Click += new System.EventHandler(this.button5_Click);
            // 
            // listView1
            // 
            this.listView1.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listView1.BackColor = System.Drawing.SystemColors.Window;
            this.listView1.BackgroundImageTiled = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView1.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.ForeColor = System.Drawing.Color.Black;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(116, 148);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1103, 257);
            this.listView1.TabIndex = 16;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "index";
            this.columnHeader1.Width = 126;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "time";
            this.columnHeader2.Width = 164;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "source";
            this.columnHeader3.Width = 160;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "destination";
            this.columnHeader4.Width = 198;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "protocol";
            this.columnHeader5.Width = 136;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "information";
            this.columnHeader6.Width = 358;
            // 
            // open
            // 
            this.open.BackColor = System.Drawing.Color.Silver;
            this.open.Font = new System.Drawing.Font("Cambria", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open.Location = new System.Drawing.Point(18, 300);
            this.open.Name = "open";
            this.open.Size = new System.Drawing.Size(81, 64);
            this.open.TabIndex = 17;
            this.open.Text = "open";
            this.open.UseVisualStyleBackColor = false;
            this.open.Click += new System.EventHandler(this.open_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Georgia", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(147, 60);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(146, 25);
            this.comboBox2.TabIndex = 18;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.listView2.Font = new System.Drawing.Font("Cambria", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(771, 504);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(458, 217);
            this.listView2.TabIndex = 19;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.SelectedIndexChanged += new System.EventHandler(this.listView2_SelectedIndexChanged);
            this.listView2.Click += new System.EventHandler(this.listView2_Click);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "index";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "protocol";
            this.columnHeader8.Width = 98;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "id";
            this.columnHeader9.Width = 84;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "source";
            this.columnHeader10.Width = 100;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "destination";
            this.columnHeader11.Width = 116;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.button3.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(836, 421);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(315, 51);
            this.button3.TabIndex = 20;
            this.button3.Text = "IP Fragmentation and Reassembly";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(-6, -7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1315, 816);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Image = ((System.Drawing.Image)(resources.GetObject("label4.Image")));
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label4.Location = new System.Drawing.Point(-1, 409);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(418, 312);
            this.label4.TabIndex = 22;
            this.label4.Text = "Details:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Cambria", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(547, 421);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 23);
            this.label5.TabIndex = 23;
            this.label5.Text = "header";
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1231, 730);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.open);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.search);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.filt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.save);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.start);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Sniffer";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Load += new System.EventHandler(this.Sniffer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //
        //start the capture,use multiple thread to increase efficiency.
        //
        private void button1_Click(object sender, EventArgs e)
        {
            int d = comboBox1.SelectedIndex;
            if (d == -1) d = 4;
            var devices = CaptureDeviceList.Instance;
            this.device = devices[d];
            this.deviceIsOpen = true;
            this.listView1.Items.Clear();

            Thread newThread = new Thread(new ThreadStart(threadHandler));
            newThread.Start();

        }

        //
        //thread handler
        //
        private void threadHandler()
        {
            this.device.Open(DeviceMode.Promiscuous, timeout);
            this.device.Filter = this.filter;
            this.device.OnPacketArrival += new PacketArrivalEventHandler(packetarrive);
            this.device.StartCapture();
        }

        //
        //event handler when a new packet arrives.
        //
        public void packetarrive(object sender, CaptureEventArgs e)
        {
            ProcessContext(e.Packet);
        }

        //
        //display the packets onto the listview.
        //
        public void ProcessContext(RawCapture pac)
        {
            packet p = new packet(pac);
            packets.Add(p);
            p.index = (listView1.Items.Count + 1);
            ListViewItem item = new ListViewItem(new string[] { p.index.ToString(), p.time, p.source, p.destination, p.protocol, p.information });
            item.BackColor = Color.FromName(p.color);
            listView1.Items.Add(item);
        }

        //
        //To prevent the corss thread exception
        //
        private void Sniffer_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }


        //
        //Initialize the devicelist.
        //
        private void combox1_Ini()
        {
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                comboBox1.Items.Add("找不到网络设备");
            }
            else
            {
                foreach (ICaptureDevice dev in devices)
                {
                    string dev_name = dev.ToString();
                    dev_name = dev_name.Substring(dev_name.IndexOf("FriendlyName: "), 60);
                    dev_name = dev_name.Substring("FriendlyName: ".Length, dev_name.IndexOf('\n') - "FriendlyName: ".Length);
                    comboBox1.Items.Add(dev_name);
                }
            }
        }

        //
        //close the capture.
        //
        private void button2_Click(object sender, EventArgs e)
        {
            this.device.StopCapture();
            this.device.Close();
            this.deviceIsOpen = false;
        }

        //
        // save file
        //
        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog savefile1 = new SaveFileDialog();
            savefile1.InitialDirectory = Environment.CurrentDirectory;
            savefile1.Filter = "pcap files (*.pcap)|*.pcap";
            savefile1.AddExtension = true;
            savefile1.RestoreDirectory = true;

            if (savefile1.ShowDialog() == DialogResult.OK && savefile1.FileName.ToString() != "")
            {
                try
                {
                    string name = savefile1.FileName;
                    this.device.Open();
                    SharpPcap.LibPcap.CaptureFileWriterDevice captureFileWriter = new SharpPcap.LibPcap.CaptureFileWriterDevice((SharpPcap.LibPcap.LibPcapLiveDevice)this.device, name);
                    foreach (packet pac in this.packets)
                    {
                        captureFileWriter.Write(pac.rawp);
                    }
                    MessageBox.Show("success!");
                }
                catch (Exception)
                {
                    MessageBox.Show("fail!");
                }
            }
        }

        //
        //open file
        //
        private void open_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openfile = new OpenFileDialog();
            openfile.InitialDirectory = Environment.CurrentDirectory;
            openfile.Filter = "pcap files (*.pcap)|*.pcap";
            openfile.CheckFileExists = true;
            openfile.RestoreDirectory = true;

            if (openfile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string name = openfile.FileName;

                listView1.Items.Clear();
                this.packets = new ArrayList();

                SharpPcap.LibPcap.CaptureFileReaderDevice reader = new SharpPcap.LibPcap.CaptureFileReaderDevice(name);
                RawCapture rawp;

                try
                {
                    rawp = reader.GetNextPacket();
                    while (rawp != null)
                    {
                        packet temp = new packet(rawp);
                        packets.Add(temp);

                        temp.index = (listView1.Items.Count + 1);

                        ListViewItem item = new ListViewItem(new string[] { temp.index.ToString(), temp.time, temp.source, temp.destination, temp.protocol, temp.information });
                        item.BackColor = Color.FromName(temp.color);
                        listView1.Items.Add(item);


                        rawp = reader.GetNextPacket();
                    }
                    MessageBox.Show("success!");
                }
                catch (Exception)
                {
                    MessageBox.Show("fail!");
                }
            }
        }

        //
        //to show the details of a packet in treeview
        //
        private void listView1_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(this.listView1.SelectedItems[0].Text);
            packet pac = (packet)packets[index - 1];
            this.treeView1.Nodes.Clear();

            label4.Text = "layer:" + pac.layer + "\nepac:" + pac.epac + "\nhash:" + pac.GetHashCode() + "\ninformation:" + pac.information + "\ntime:" + pac.time+"\ndata:"+pac.data;


            if (pac.PacketInforArray.Count > 0)
            {
                TreeNode new1 = new TreeNode("Frame: ");
                for (int i = 0; i < pac.PacketInforArray.Count; i++)
                {
                    new1.Nodes.Add(pac.PacketInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new1);
            }

            if (pac.EthernetInforArray.Count > 0)
            {
                TreeNode new2 = new TreeNode("Ethernet:");
                for (int i = 0; i < pac.EthernetInforArray.Count; i++)
                {
                    new2.Nodes.Add(pac.EthernetInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new2);
            }

            if (pac.IpInforArray.Count > 0)
            {
                TreeNode new3 = new TreeNode("Internet Protocol:");
                for (int i = 0; i < pac.IpInforArray.Count; i++)
                {
                    new3.Nodes.Add(pac.IpInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new3);
            }

            if (pac.IcmpInforArray.Count > 0)
            {
                TreeNode new4 = new TreeNode("Internet Control Message Protocol: ");
                for (int i = 0; i < pac.IcmpInforArray.Count; i++)
                {
                    new4.Nodes.Add(pac.IcmpInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new4);
            }

            if (pac.UdpInforArray.Count > 0)
            {
                TreeNode new5 = new TreeNode("User Datagram Protocol: ");
                for (int i = 0; i < pac.UdpInforArray.Count; i++)
                {
                    new5.Nodes.Add(pac.UdpInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new5);
            }

            if (pac.TcpInforArray.Count > 0)
            {
                TreeNode new6 = new TreeNode("Transmission Control Protocol: ");
                for (int i = 0; i < pac.TcpInforArray.Count; i++)
                {
                    new6.Nodes.Add(pac.TcpInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new6);
            }

            if (pac.ArpInforArray.Count > 0)
            {
                TreeNode new7 = new TreeNode("Address Resolution Protocol: ");
                for (int i = 0; i < pac.ArpInforArray.Count; i++)
                {
                    new7.Nodes.Add(pac.ArpInforArray[i].ToString());
                }
                this.treeView1.Nodes.Add(new7);
            }

            if (pac.ApplicationInfor.Count > 0)
            {
                TreeNode new8 = new TreeNode("Application Layer Protocol: ");
                for (int i = 0; i < pac.ApplicationInfor.Count; i++)
                {
                    new8.Nodes.Add(pac.ApplicationInfor[i].ToString());
                }
                this.treeView1.Nodes.Add(new8);
            }
        }

        //
        //filter. using the winpcap filtering expression syntax
        //
        private void button4_Click(object sender, EventArgs e)
        {

            int item = comboBox2.SelectedIndex;
            Console.Write("{0}", item.ToString());
            this.filter = "";

            if (item != -1)
            {
                switch (item)
                {
                    case 0:
                        filter = "" + textBox1.Text;
                        break;
                    case 1:
                        filter = "dst port " + textBox1.Text;
                        break;
                    case 2:
                        filter = "src port " + textBox1.Text;
                        break;
                    case 3:
                        filter = "des host " + textBox1.Text;
                        break;
                    case 4:
                        filter = "src host " + textBox1.Text;
                        break;
                }
            }

            if (this.deviceIsOpen)
                this.device.Filter = filter;


        }

        //
        //Initialize the combox2, which is a part of the filter expression.
        //
        private void combox2_Ini()
        {
            comboBox2.Items.Add("protocol");
            comboBox2.Items.Add("destination port");
            comboBox2.Items.Add("source port");
            comboBox2.Items.Add("destination IP");
            comboBox2.Items.Add("source IP");
        }

        //
        //search.
        //
        private void button5_Click(object sender, EventArgs e)
        {
            string text = textBox2.Text;
            listView1.Items.Clear();

            if (text != "")
            {
                foreach (packet p in this.packets)
                {
                    for (int i = 0; i < p.KeyWords.Count; i++)
                    {
                        if (text.ToUpper() == p.KeyWords[i].ToString().ToUpper())
                        {
                            ListViewItem item = new ListViewItem(new string[] { p.index.ToString(), p.time, p.source, p.destination, p.protocol, p.information });
                            item.BackColor = Color.FromName(p.color);
                            listView1.Items.Add(item);
                            continue;
                        }
                    }
                }
            }
            else
            {
                foreach (packet p in this.packets)
                {
                    ListViewItem item = new ListViewItem(new string[] { p.index.ToString(), p.time, p.source, p.destination, p.protocol, p.information });
                    item.BackColor = Color.FromName("white");
                    listView1.Items.Add(item);
                    continue;
                }

            }
        }

        //
        //reassemble
        //show the reassembly packets in the listview2.
        //
        private void button3_Click_1(object sender, EventArgs e)
        {
            this.listView2.Items.Clear();
            if (this.deviceIsOpen)
                MessageBox.Show("Please close the capture first!");
            else
            {
                foreach (packet p in this.packets)
                {
                    if (p.epac.Type.ToString() == "IpV4")//get all the ip4 packets
                    {
                        simplePacket simpac = new simplePacket(p);
                        if (simpac.df == 0)
                            simpackets.Add(simpac);
                    }
                }
                foreach (simplePacket sp in this.simpackets)
                {
                    if (sp.offset == 0 && sp.mf == 1)
                    {   // find the first packet of  the same id packets
                        ArrayList arr = new ArrayList();
                        simplePacket lastpa = sp;
                        foreach (simplePacket simp in this.simpackets)
                        {
                            if (simp.id == sp.id && simp.src == sp.src && simp.des == sp.des)
                                arr.Add(simp); // get all the packets having the same id.
                            if (simp.id == sp.id && simp.src == sp.src && simp.des == sp.des && simp.mf == 0)
                                lastpa = simp; //gte the last packet.
                        }
                        int totallength = 0;
                        foreach (simplePacket s in arr)
                        {
                            totallength += s.length;
                        }
                        int a = lastpa.length + (lastpa.offset * 8);
                        if (totallength == a) // test if there is any packet lost.
                        {
                            while (sp.length != totallength)
                            {
                                foreach (simplePacket pac in arr)
                                {
                                    if ((pac.offset * 8) == sp.length)
                                    {
                                        sp.length += pac.length;
                                        sp.data += pac.data;
                                    }
                                }
                            }
                            reassemblyPac.Add(sp);
                        }
                    }
                }
                //
                //show the information in the treeview.
                //
                int i = 1;
                foreach (simplePacket sp in reassemblyPac)
                {
                    ListViewItem item = new ListViewItem(new string[] { i.ToString(), "IpV4", sp.id, sp.src, sp.des });
                    listView2.Items.Add(item);
                }
            }
        }

        //
        //when you choose a item in listview2, a messagebox will appear,to show the reassembled packets' content.
        //
        private void listView2_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(this.listView2.SelectedItems[0].Text);
            simplePacket pac = (simplePacket)reassemblyPac[index - 1];

            MessageBox.Show("id: " + pac.id + " \n" + "source address: " + pac.src + "\n" + "destination address: " + pac.des + "\n" + "length: " + pac.length + "\n" + "reassembly packet payloaddata: \n" + pac.data);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }




    }

    /// <summary>
    /// 新建类simplePacket
    /// 为了提高重组时的效率，将packet类中有关重组的信息抽取，在这个类中存储。分析重组时可减少对packet对象的分析。
    /// </summary>
    public partial class simplePacket
    {
        public string src;
        public string des;
        public string id;
        public int mf;
        public int df;
        public int length;
        public int offset;
        public string data = "";


        public simplePacket(packet p)
        {
            this.src = p.ip4.SourceAddress.ToString();
            this.des = p.ip4.DestinationAddress.ToString();
            this.id = p.ip4.Id.ToString();
            this.length = p.ip4.TotalLength - p.ip4.HeaderLength * 4;
            this.offset = p.ip4.FragmentOffset;
            this.mf = (p.ip4.Bytes[6] / 32) % 2;
            this.df = (p.ip4.Bytes[6] / 64) % 2;
            this.data = p.data;
        }
    }
}
