using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SharpPcap;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using PacketDotNet;
using System;

namespace windowsform1
{
    public partial class packet
    {
        //
        //声明变量
        //
        public string time;
        public string source;
        public string destination;
        public string srcPort;
        public string desPort;
        public string protocol;
        public string information;
        public string color;
        public string data;
        public int index = 0;


        //
        //用于储存特定包的变量
        //
        public Packet temp;
        public RawCapture rawp;
        public PacketDotNet.EthernetPacket epac;//以太网层数据包
        public IPv4Packet ip4;
        public IPv6Packet ip6;

        /// <summary>
        ///层变量 
        ///链路层数据包包头
        /// </summary>
        public PacketDotNet.LinkLayers layer;

        /// <summary>
        /// 用来存储有关包内容的数组列表
        /// </summary>

        public ArrayList PacketInforArray = new ArrayList();
        public ArrayList EthernetInforArray = new ArrayList();
        public ArrayList IpInforArray = new ArrayList();
        public ArrayList IcmpInforArray = new ArrayList();
        public ArrayList IgmpInforArray = new ArrayList();
        public ArrayList TcpInforArray = new ArrayList();
        public ArrayList UdpInforArray = new ArrayList();
        public ArrayList ArpInforArray = new ArrayList();
        public ArrayList ApplicationInfor = new ArrayList();

        public ArrayList KeyWords = new ArrayList();

        public packet(RawCapture pac)
        {
            temp = Packet.ParsePacket(pac.LinkLayerType, pac.Data);
            rawp = pac;
            DateTime time = pac.Timeval.Date;
            this.time = time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString() + ":" + time.Millisecond.ToString();
            this.destination = "";
            this.color = "";
            this.srcPort = "";
            this.desPort = "";
            this.source = "";
            this.protocol = "";
            this.information = "";
            this.data = "";
            this.layer = pac.LinkLayerType;
            PacketInforArray.Add("Total Length : " + temp.Bytes.Length.ToString() + "Bytes");

            KeyWords.Add(temp.Bytes.Length.ToString());

            if (this.layer == PacketDotNet.LinkLayers.Ethernet)
            {
                //
                //以太网层
                //
                epac = (PacketDotNet.EthernetPacket)temp;
                EthernetInforArray.Add("Ethernet II \n");
                EthernetInforArray.Add("Destination Hardware Address: " + epac.DestinationHwAddress.ToString() + "\n");
                EthernetInforArray.Add("Source Hardware Address: " + epac.SourceHwAddress.ToString() + "\n");
                EthernetInforArray.Add("Type of the ethernet: " + epac.Type.ToString() + "\n");

                KeyWords.Add("Ethernet".ToUpper());
                KeyWords.Add(epac.DestinationHwAddress.ToString().ToUpper());
                KeyWords.Add(epac.SourceHwAddress.ToString().ToUpper());
                KeyWords.Add(epac.Type.ToString().ToUpper());

                //
                //ip层
                //
                if (epac.Type.ToString() == "IpV4" || epac.Type.ToString() == "IpV6")
                {
                    //ipv4
                    if (epac.Type.ToString() == "IpV4")
                    {   
                        //record keywords
                        ip4 = (IPv4Packet)epac.Extract(typeof(IPv4Packet));
                        this.protocol = ip4.Protocol.ToString();
                        this.destination = ip4.DestinationAddress.ToString();
                        this.source = ip4.SourceAddress.ToString();
                        this.information = ip4.TotalLength.ToString() + "Bytes | id :" + ip4.Id.ToString() + "  |";

                        //Internet protocl messages
                        IpInforArray.Add("HeaderLength : " + (ip4.HeaderLength * 4).ToString() + "Bytes \n");
                        IpInforArray.Add("Version: " + ip4.Version.ToString() + "\n");
                        IpInforArray.Add("Type of Service: " + ip4.TypeOfService.ToString() + "\n");
                        IpInforArray.Add("Total Length: " + ip4.Bytes.Length.ToString() + "Bytes \n");
                        IpInforArray.Add("Identification: 0x" + Convert.ToString((Int32)ip4.Id, 16).ToUpper().PadLeft(4, '0') + "(" + ip4.Id.ToString() + ")\n");
                        IpInforArray.Add("Flags: 0x" + Convert.ToString(ip4.Bytes[6] / 32, 16).ToUpper().PadLeft(2, '0') + "\n");                                                             //
                        IpInforArray.Add("Fragment Offset: " + (Convert.ToInt32((ip4.Bytes[6] % 32) << 8) + Convert.ToInt32(ip4.Bytes[7])).ToString() + "\n");
                        IpInforArray.Add("TTL: " + ip4.TimeToLive.ToString() + "\n");
                        IpInforArray.Add("Protocol: " + ip4.Protocol.ToString() + " \n");
                        IpInforArray.Add("CheckSum: " + ip4.Checksum.ToString() + "\n");
                        IpInforArray.Add("Source IP Address: " + ip4.SourceAddress.ToString() + "\n");
                        IpInforArray.Add("Destination IP Address: " + ip4.DestinationAddress.ToString() + "\n");
                        IpInforArray.Add("Option: if any.\n");

                        KeyWords.Add(ip4.Protocol.ToString().ToUpper());
                        KeyWords.Add(ip4.Id.ToString().ToUpper());
                        KeyWords.Add(ip4.SourceAddress.ToString().ToUpper());
                        KeyWords.Add(ip4.DestinationAddress.ToString().ToUpper());


                        if (ip4.Protocol.ToString() == "ICMP") { icmpProtocol(); }
                        else if (ip4.Protocol.ToString() == "UDP") { udpProtocol(); }
                        else if (ip4.Protocol.ToString() == "TCP") { tcpProtocol(); }
                        else if (ip4.Protocol.ToString() == "IGMP") { igmpProtocol(); }
                        else { ;}

                    }
                    else if (epac.Type.ToString() == "IpV6")
                    {
                        ip6 = (IPv6Packet)epac.Extract(typeof(IPv6Packet));
                        this.protocol = ip6.Protocol.ToString();
                        this.destination = ip6.DestinationAddress.ToString();
                        this.source = ip6.SourceAddress.ToString();
                        this.information = ip6.TotalLength.ToString() + "Bytes ";


                        IpInforArray.Add("Version: " + ip6.Version.ToString() + "\n");
                        IpInforArray.Add("Traffic Class :  0x" + Convert.ToString(ip6.Bytes[0] % 16, 16).PadLeft(1, '0') + Convert.ToString((Int32)(ip6.Bytes[1] / 16), 16).PadLeft(1, '0') + "\n");
                        IpInforArray.Add("Flow Label:  0x" + Convert.ToString(ip6.Bytes[1] % 16, 16).PadLeft(1, '0') + Convert.ToString(ip6.Bytes[2], 16).PadLeft(2, '0') + Convert.ToString(ip6.Bytes[3], 16).PadLeft(2, '0') + "\n");
                        IpInforArray.Add("Payload Length: " + ip6.PayloadLength.ToString() + "\n");
                        IpInforArray.Add("Next Header: " + ip6.NextHeader.ToString() + "\n");
                        IpInforArray.Add("Hop Limit: " + ip6.HopLimit.ToString() + "\n");
                        IpInforArray.Add("Source Address: " + ip6.SourceAddress.ToString() + "\n");
                        IpInforArray.Add("Destination Address: " + ip6.DestinationAddress.ToString() + "\n");

                        KeyWords.Add(ip6.Protocol.ToString().ToUpper());
                        KeyWords.Add(ip6.SourceAddress.ToString().ToUpper());
                        KeyWords.Add(ip6.DestinationAddress.ToString().ToUpper());

                        if (ip6.Protocol.ToString() == "ICMP") { icmpProtocol(); }
                        else if (ip6.Protocol.ToString() == "UDP") { udpProtocol(); }
                        else if (ip6.Protocol.ToString() == "TCP") { tcpProtocol(); }
                        else if (ip6.Protocol.ToString() == "IGMP") { igmpProtocol(); }
                        else { ;}

                        KeyWords.Add(this.color.ToString().ToUpper());
                        KeyWords.Add(this.protocol.ToString().ToUpper());
                    }
                }
                else if (epac.Type.ToString() == "Arp") //分析arp报文
                {
                    var arppacket = (ARPPacket)epac.Extract(typeof(ARPPacket));

                    ArpInforArray.Add("HardwareAddressType: " + arppacket.HardwareAddressType.ToString() + "\n");
                    ArpInforArray.Add("ProtocolAddressType: " + arppacket.ProtocolAddressType.ToString() + "\n");
                    ArpInforArray.Add("HardwareAddressLength: " + arppacket.HardwareAddressLength.ToString());
                    ArpInforArray.Add("ProtocolAddressLength: " + arppacket.ProtocolAddressLength.ToString());
                    ArpInforArray.Add("Operation: " + arppacket.Operation.ToString());
                    ArpInforArray.Add("SenderHardwareAddress: " + arppacket.SenderHardwareAddress.ToString());
                    ArpInforArray.Add("SenderProtocolAddress: " + arppacket.SenderProtocolAddress.ToString());
                    ArpInforArray.Add("TargetHardwareAddress: " + arppacket.TargetHardwareAddress.ToString());
                    ArpInforArray.Add("TargetProtocolAddress: " + arppacket.TargetProtocolAddress.ToString());


                    this.color = "Salmon";

                    this.protocol = "ARP";
                    this.source = arppacket.SenderProtocolAddress.ToString();
                    this.destination = arppacket.TargetProtocolAddress.ToString();
                    this.information = arppacket.SenderProtocolAddress.ToString() + " want to get in touch with " + arppacket.TargetProtocolAddress.ToString();

                    KeyWords.Add(arppacket.SenderHardwareAddress.ToString().ToUpper());
                    KeyWords.Add(arppacket.SenderProtocolAddress.ToString().ToUpper());
                    KeyWords.Add(arppacket.TargetHardwareAddress.ToString().ToUpper());
                    KeyWords.Add(arppacket.TargetProtocolAddress.ToString().ToUpper());
                    KeyWords.Add(this.color);
                }


            }
        }

        /// <summary>
        /// icmp报文分析
        /// </summary>
        public void icmpProtocol()
        {
            if (epac.Type.ToString() == "IpV4" && ip4.Protocol.ToString() == "ICMP")
            {
                var icmppacket = (ICMPv4Packet)ip4.Extract(typeof(ICMPv4Packet));
                int LE = (icmppacket.ID / 256) + ((icmppacket.ID % 256) << 8);
                int sqLE = (icmppacket.Sequence / 256) + ((icmppacket.Sequence % 256) << 8);
                string data1 = "";

                IcmpInforArray.Add("Type: " + Convert.ToString(icmppacket.Bytes[0], 10) + ")\n");
                IcmpInforArray.Add("Code: " + Convert.ToString(icmppacket.Bytes[1], 10) + "\n");
                IcmpInforArray.Add("Checksum:  0x" + Convert.ToString(icmppacket.Checksum, 16).PadLeft(4, '0') + "\n");
                IcmpInforArray.Add("Identifier(BE): " + icmppacket.ID.ToString() + " (0x" + Convert.ToString(icmppacket.ID, 16).PadLeft(4, '0') + ")\n");
                IcmpInforArray.Add("Identifier(LE): " + LE.ToString() + " (0x" + Convert.ToString(LE, 16).PadLeft(4, '0') + ")\n");
                IcmpInforArray.Add("Sequence number(BE): " + icmppacket.Sequence.ToString() + " (0x" + Convert.ToString(icmppacket.Sequence, 16).PadLeft(4, '0') + ")\n");
                IcmpInforArray.Add("Sequence number(LE): " + sqLE.ToString() + " (0x" + Convert.ToString(sqLE, 16).PadLeft(4, '0') + ")\n");
                for (int i = icmppacket.Header.Length; i < icmppacket.Bytes.Length; i++)
                {
                    data1 = data1 + Convert.ToChar(icmppacket.Bytes[i]);
                }
                IcmpInforArray.Add("Data: " + data1 + "\n");

                KeyWords.Add(icmppacket.ID.ToString().ToUpper());
                KeyWords.Add(icmppacket.Sequence.ToString().ToUpper());


                this.color = "Gold";
                this.data = Encoding.UTF8.GetString(icmppacket.PayloadData);
                this.information = " id=" + icmppacket.ID.ToString() + ", seq=" + icmppacket.Sequence.ToString() + ", ttl=" + ip4.TimeToLive.ToString();
            }
            else if (epac.Type.ToString() == "IpV6" && ip6.Protocol.ToString() == "ICMP")
            {
                var icmppacket = (ICMPv6Packet)ip6.Extract(typeof(ICMPv6Packet));

                IcmpInforArray.Add("Type: " + icmppacket.Type.ToString() + "\n");
                IcmpInforArray.Add("Checksum: " + icmppacket.Checksum.ToString() + "\n");
                IcmpInforArray.Add("Code: " + icmppacket.Code.ToString() + "\n");
                IcmpInforArray.Add("Identifier: " + Convert.ToString(icmppacket.Bytes[4], 10) + "\n");

                KeyWords.Add(Convert.ToString(icmppacket.Bytes[4], 10).ToUpper());

                this.color = "Gold";
                this.information = icmppacket.Type.ToString() + "id = " + Convert.ToString(icmppacket.Bytes[4], 10);
            }
            else { ;}
        }

        /// <summary>
        /// igmp报文分析
        /// </summary>
        public void igmpProtocol()
        {
            var igmppacket = (IGMPv2Packet)temp.Extract(typeof(IGMPv2Packet));

            IgmpInforArray.Add("Type: " + igmppacket.Type.ToString() + "\n");
            IgmpInforArray.Add("MaxResponseTime: " + igmppacket.MaxResponseTime.ToString() + "\n");
            IgmpInforArray.Add("Checksum: 0x" + Convert.ToString(igmppacket.Checksum, 16).PadLeft(4, '0') + "\n");
            IgmpInforArray.Add("GroupAddress: " + igmppacket.GroupAddress.ToString() + "\n");

            this.color = "Khaki";
            this.data = Encoding.UTF8.GetString(igmppacket.PayloadData);
            this.information = "Type: " + igmppacket.Type.ToString() + ";  GroupAddress" + igmppacket.GroupAddress.ToString();

            KeyWords.Add(igmppacket.Type.ToString().ToUpper());
            KeyWords.Add(igmppacket.GroupAddress.ToString().ToUpper());

        }

        /// <summary>
        /// tcp报文分析
        /// 分析了端口，但目前只能分析http头部和部分Telnet协议报文。
        /// </summary>
        public void tcpProtocol()
        {
            var tcppacket = (TcpPacket)temp.Extract(typeof(TcpPacket));

            TcpInforArray.Add("Source Port: " + tcppacket.SourcePort.ToString() + "\n");
            TcpInforArray.Add("Destination Port: " + tcppacket.DestinationPort.ToString() + "\n");
            TcpInforArray.Add("Sequence Number: " + tcppacket.SequenceNumber.ToString() + "\n");
            TcpInforArray.Add("Acknowledge Number: " + tcppacket.AcknowledgmentNumber.ToString() + "\n");
            TcpInforArray.Add("Offset: " + tcppacket.DataOffset.ToString() + "\n");
            TcpInforArray.Add("URG: " + tcppacket.Urg.ToString() + "\n");
            TcpInforArray.Add("ACK: " + tcppacket.Ack.ToString() + "\n");
            TcpInforArray.Add("PSH: " + tcppacket.Psh.ToString() + "\n");
            TcpInforArray.Add("RST: " + tcppacket.Rst.ToString() + "\n");
            TcpInforArray.Add("SYN: " + tcppacket.Syn.ToString() + "\n");
            TcpInforArray.Add("FIN: " + tcppacket.Fin.ToString() + "\n");
            TcpInforArray.Add("Window Size: " + tcppacket.WindowSize.ToString() + "\n");
            TcpInforArray.Add("CheckSum: " + tcppacket.Checksum.ToString() + "\n");
            TcpInforArray.Add("Urgent Point: " + tcppacket.UrgentPointer.ToString() + "\n");

            this.color = "MistyRose";
            this.data = Encoding.UTF8.GetString(tcppacket.PayloadData);
            this.information = tcppacket.SourcePort.ToString() + "->" + tcppacket.DestinationPort.ToString() + ", seq = " + tcppacket.SequenceNumber.ToString() + ", ack num = " + tcppacket.AcknowledgmentNumber.ToString() + ", win = " + tcppacket.WindowSize.ToString();
            this.srcPort = tcppacket.SourcePort.ToString();
            this.desPort = tcppacket.DestinationPort.ToString();


            KeyWords.Add(tcppacket.DestinationPort.ToString().ToUpper());
            KeyWords.Add(tcppacket.SourcePort.ToString().ToUpper());

            //http
            //
            if (tcppacket.DestinationPort == 80 || tcppacket.SourcePort == 80)
            {
                var httpData = tcppacket.PayloadData;
                string headertext = "";
                string datatext = "";
                string bytetext = "";
                foreach (byte i in httpData)
                {
                    bytetext += Convert.ToString(i, 16).ToUpper().PadLeft(2, '0');
                }

                if (bytetext.IndexOf("0D0A0D0A") >= 0)
                {
                    headertext = System.Text.Encoding.Default.GetString(httpData);
                    headertext = headertext.Substring(0, headertext.IndexOf("\r\n\r\n"));

                    if (headertext.IndexOf("HTTP") == 0 || headertext.IndexOf("GET") == 0 || headertext.IndexOf("POST") == 0)
                    {
                        //0D0A0D0A------\r\n\r\n
                        datatext = bytetext.Substring(bytetext.IndexOf("0D0A0D0A") + "0D0A0D0A".Length, bytetext.Length - bytetext.IndexOf("0D0A0D0A") - "0D0A0D0A".Length);
                    }
                    else
                    {
                        datatext = bytetext;
                    }
                }
                else
                {
                    datatext = bytetext;
                }

                if (headertext.IndexOf("HTTP") == 0 || headertext.IndexOf("GET") == 0 || headertext.IndexOf("POST") == 0)
                {
                    this.protocol = "HTTP";
                    this.color = "PaleGreen";
                    ApplicationInfor.Add("HEADER: " + headertext);
                    ApplicationInfor.Add("DATA: " + datatext);
                    if (headertext.IndexOf("OK") > 0)
                    {
                        this.information = headertext.Substring(0, headertext.IndexOf("OK") + "OK".Length);
                    }

                    if (headertext.IndexOf("HTTP") > 0)
                    {
                        this.information = headertext.Substring(0, headertext.IndexOf("HTTP") + "HTTP/1.1".Length);
                    }


                }

            }

            if (tcppacket.DestinationPort == 23 || tcppacket.SourcePort == 23)
            {
                this.protocol = "Telnet";
                this.color = "Beigi";
                this.information = "Telnet (uncompleted)";

                var telnetData = tcppacket.PayloadData;
                string sRecieved = Encoding.GetEncoding("utf-8").GetString(telnetData, 0, telnetData.Length);
                string m_strLine = "";
                for (int i = 0; i < telnetData.Length; i++)
                {
                    Char ch = Convert.ToChar(telnetData[i]);
                    switch (ch)
                    {
                        case '\r':
                            m_strLine += Convert.ToString("\r\n");
                            break;
                        case '\n':
                            break;
                        default:
                            m_strLine += Convert.ToString(ch);
                            break;

                    }
                }

                this.ApplicationInfor.Add("Telnet Protocol.");
                this.ApplicationInfor.Add("Data: " + m_strLine);
            }

            if (tcppacket.DestinationPort == 21 || tcppacket.SourcePort == 21)
            {
                this.protocol = "FTP";
                this.color = "Ivory";
                ApplicationInfor.Add("It's a FTP protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            if (tcppacket.DestinationPort == 443 || tcppacket.SourcePort == 443)
            {
                this.protocol = "SSL";
                this.color = "LightGoldenrodYellow";
                ApplicationInfor.Add("It's a SSL protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            if (tcppacket.SourcePort == 53 || tcppacket.DestinationPort == 53)
            {
                this.protocol = "DNS";
                this.color = "Turquoise";
                ApplicationInfor.Add("It's a DNS protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            KeyWords.Add(this.protocol);
        }

        /// <summary>
        /// udp报文分析
        /// </summary>
        public void udpProtocol()
        {
            var udppacket = (UdpPacket)temp.Extract(typeof(UdpPacket));

            UdpInforArray.Add("Source port: " + udppacket.SourcePort.ToString() + "\n");
            UdpInforArray.Add("Destination port: " + udppacket.DestinationPort.ToString() + "\n");
            UdpInforArray.Add("Length: " + udppacket.Length.ToString() + "\n");
            UdpInforArray.Add("Checksum: " + udppacket.Checksum.ToString() + "\n");

            int check = udppacket.CalculateUDPChecksum();
            if (check == udppacket.Checksum) UdpInforArray.Add("[valid packet, save]");
            else UdpInforArray.Add("[invalid packet, drop]");

            this.color = "LightYellow";
            this.data = Encoding.UTF8.GetString(udppacket.PayloadData);
            this.information = udppacket.SourcePort.ToString() + " -> " + udppacket.DestinationPort.ToString();
            this.desPort = udppacket.DestinationPort.ToString();
            this.srcPort = udppacket.SourcePort.ToString();

            KeyWords.Add(udppacket.SourcePort.ToString().ToUpper());
            KeyWords.Add(udppacket.DestinationPort.ToString().ToUpper());

            if (udppacket.DestinationPort == 53 || udppacket.SourcePort == 53)
            {
                this.protocol = "DNS";
                this.color = "BlanchedAlmond";
                ApplicationInfor.Add("It's a dns protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            if (udppacket.SourcePort == 161 || udppacket.SourcePort == 162 || udppacket.DestinationPort == 161 || udppacket.DestinationPort == 162)
            {
                this.protocol = "SNMP";
                this.color = "LightYellow";
                ApplicationInfor.Add("It's a SNMP protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            if (udppacket.DestinationPort == 67 || udppacket.SourcePort == 67 || udppacket.DestinationPort == 68 || udppacket.SourcePort == 68)
            {
                this.protocol = "BOOTP";
                this.color = "MediumSpringGreen";
                ApplicationInfor.Add("It's a BOOTP protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            if (udppacket.SourcePort == 137 || udppacket.SourcePort == 138 || udppacket.DestinationPort == 137 || udppacket.DestinationPort == 138 || udppacket.SourcePort == 139 || udppacket.DestinationPort == 139)
            {
                this.protocol = "NetBIOS";
                this.color = "Pink";
                ApplicationInfor.Add("It's a NETBIOS protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            if (udppacket.DestinationPort == 1900 || udppacket.SourcePort == 1900)
            {
                this.protocol = "SSDP";
                this.color = "SkyBlue";
                ApplicationInfor.Add("It's a SSDP protocol packet.");
                ApplicationInfor.Add("This version of sniffer can't transfer the bytes into human-being language.Please wait the next version.");
            }

            this.KeyWords.Add(this.protocol);
        }
    }
}
