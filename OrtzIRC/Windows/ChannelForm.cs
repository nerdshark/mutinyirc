﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OrtzIRC
{
    public partial class ChannelForm : Form
    {
        public ChannelForm()
        {
            InitializeComponent();
        }

        public void AppendLine(string line)
        {
            channelOutputBox.AppendLine(line);
        }
    }
}
