﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PostHttp
{
  public partial class FormSettings : Form
  {
    PostHttp mainClass;

    public FormSettings(PostHttp mainClass)
    {
      InitializeComponent();

      this.mainClass = mainClass;

      int selIndex = 0;
      switch (mainClass.imageFormat.ToLower()) {
        case "png": selIndex = 0; break;
        case "jpg": selIndex = 1; break;
        case "gif": selIndex = 2; break;
      }
      comboFormat.SelectedIndex = selIndex;

      textURL.Text = mainClass.endpointURL;
      textName.Text = mainClass.endpointName;
      textAuthentication.Text = mainClass.endpointAuthorization;

      checkJpegCompression.Checked = mainClass.jpegCompression;
      numJpegCompressionFilesize.Value = mainClass.jpegCompressionFilesize;
      numJpegCompressionRate.Value = mainClass.jpegCompressionRate;

      {
        string[] parts = mainClass.shortCutDragModifiers.Split('+');
        foreach (string part in parts) {
          switch (part) {
            case "Ctrl": checkDragModCtrl.Checked = true; break;
            case "Alt": checkDragModAlt.Checked = true; break;
            case "Shift": checkDragModShift.Checked = true; break;
          }
        }
      }

      {
        string[] parts = mainClass.shortCutAnimModifiers.Split('+');
        foreach (string part in parts) {
          switch (part) {
            case "Ctrl": checkAnimModCtrl.Checked = true; break;
            case "Alt": checkAnimModAlt.Checked = true; break;
            case "Shift": checkAnimModShift.Checked = true; break;
          }
        }
      }

      {
        string[] parts = mainClass.shortCutPasteModifiers.Split('+');
        foreach (string part in parts) {
          switch (part) {
            case "Ctrl": checkPasteModCtrl.Checked = true; break;
            case "Alt": checkPasteModAlt.Checked = true; break;
            case "Shift": checkPasteModShift.Checked = true; break;
          }
        }
      }

      mainClass.PopulateKeysCombobox(comboDragKeys);
      mainClass.PopulateKeysCombobox(comboPasteKeys);

      comboDragKeys.SelectedItem = mainClass.shortCutDragKey;
      comboPasteKeys.SelectedItem = mainClass.shortCutPasteKey;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      mainClass.settings.SetString("Format", comboFormat.Items[comboFormat.SelectedIndex].ToString());
      mainClass.settings.SetString("EndpointURL", textURL.Text);
      mainClass.settings.SetString("EndpointName", textName.Text);
      mainClass.settings.SetString("EndpointAuthorization", textAuthentication.Text);

      mainClass.settings.SetBool("JpegCompression", checkJpegCompression.Checked);
      mainClass.settings.SetInt("JpegCompressionFilesize", (int)numJpegCompressionFilesize.Value);
      mainClass.settings.SetInt("JpegCompressionRate", (int)numJpegCompressionRate.Value);

      {
        string shortcutModifiers = "";
        if (checkDragModCtrl.Checked) shortcutModifiers += "+Ctrl";
        if (checkDragModAlt.Checked) shortcutModifiers += "+Alt";
        if (checkDragModShift.Checked) shortcutModifiers += "+Shift";
        shortcutModifiers = shortcutModifiers.Trim('+');

        mainClass.settings.SetString("ShortcutDragModifiers", shortcutModifiers);
        mainClass.settings.SetString("ShortcutDragKey", (string)comboDragKeys.SelectedItem != "None" ? (string)comboDragKeys.SelectedItem : "");
      }

      {
        string shortcutModifiers = "";
        if (checkAnimModCtrl.Checked) shortcutModifiers += "+Ctrl";
        if (checkAnimModAlt.Checked) shortcutModifiers += "+Alt";
        if (checkAnimModShift.Checked) shortcutModifiers += "+Shift";
        shortcutModifiers = shortcutModifiers.Trim('+');

        mainClass.settings.SetString("ShortcutAnimModifiers", shortcutModifiers);
        mainClass.settings.SetString("ShortcutAnimKey", (string)comboAnimKeys.SelectedItem != "None" ? (string)comboAnimKeys.SelectedItem : "");
      }

      {
        string shortcutModifiers = "";
        if (checkPasteModCtrl.Checked) shortcutModifiers += "+Ctrl";
        if (checkPasteModAlt.Checked) shortcutModifiers += "+Alt";
        if (checkPasteModShift.Checked) shortcutModifiers += "+Shift";
        shortcutModifiers = shortcutModifiers.Trim('+');

        mainClass.settings.SetString("ShortcutPasteModifiers", shortcutModifiers);
        mainClass.settings.SetString("ShortcutPasteKey", (string)comboPasteKeys.SelectedItem != "None" ? (string)comboPasteKeys.SelectedItem : "");
      }

      mainClass.settings.Save();

      mainClass.LoadSettings();

      this.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void button3_Click(object sender, EventArgs e)
    {
      MessageBox.Show("This turns the uploaded image into a Jpeg instead of the usual format selected on the left. If the resulting filesize is larger than X amount of KB, it will use the given compression rate.", "Jpeg compression", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  }
}
