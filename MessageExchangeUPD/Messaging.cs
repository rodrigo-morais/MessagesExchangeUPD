﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using UPDIntegration;
using System.Timers;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace MessageExchangeUPD
{
    public partial class Messaging : Form
    {
        private UdpConnector connector;
        private System.Timers.Timer verifyMessages;
        private UdpIntegration udpMessages;
        
        public Messaging()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string remoteIP = txtRemoteIP.Text;
            int remotePort = int.Parse(txtRemotePort.Text);
            int port = int.Parse(txtPort.Text);

            
            connector = new UdpConnector(remoteIP, remotePort, port);
            udpMessages = new UdpIntegration(connector);

            txtRemoteIP.Enabled = false;
            txtRemotePort.Enabled = false;
            txtPort.Enabled = false;
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            txtMessage.Enabled = true;
            btnSend.Enabled = true;

            verifyMessages = new System.Timers.Timer();
            verifyMessages.Interval = 500;
            verifyMessages.Enabled = true;

            verifyMessages.Elapsed += new ElapsedEventHandler(receive);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            udpMessages = null;
            connector.disconnect();
            
            txtRemoteIP.Enabled = true;
            txtRemotePort.Enabled = true;
            txtPort.Enabled = true;
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            txtMessage.Enabled = false;
            btnSend.Enabled = false;

            verifyMessages.Enabled = false;
            verifyMessages.Stop();
            verifyMessages = null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            udpMessages.send(txtMessage.Text);
            string header = udpMessages + ":" + txtPort.Text + " (Me) \r\n";
            txtConversation.AppendText(header);
            txtConversation.AppendText("  " + txtMessage.Text + " \r\n");
            txtMessage.Text = string.Empty;

            txtMessage.Focus();
        }

        public void receive(object source, ElapsedEventArgs e)
        {
            byte[] data = udpMessages.receive();
            string message = Encoding.UTF8.GetString(data, 0, data.Length);
            string header = txtRemoteIP.Text + ":" + txtRemotePort.Text + " (Other Computer) \r\n";
            txtConversation.Invoke((Action)(() => txtConversation.AppendText(header)));
            txtConversation.Invoke((Action)(() => txtConversation.AppendText("  " + message + " \r\n")));
        }

    }
}
