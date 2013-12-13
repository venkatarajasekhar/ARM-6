﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElementEditor
{
  public partial class Form2 : Form
  {
    public Form2()
    {
      InitializeComponent();
    }

    public String NewTabName
    {
      get { return this.textBox1.Text; }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (textBox1.Text == String.Empty)
      {
        MessageBox.Show("Введите имя", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      else
      {
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }
    private void button2_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
