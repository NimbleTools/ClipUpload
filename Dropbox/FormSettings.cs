﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Dropbox
{
  public partial class FormSettings : Form
  {
    Dropbox mainClass;

    public FormSettings(Dropbox mainClass)
    {
      InitializeComponent();

      this.mainClass = mainClass;

      groupNoDropbox.Visible = !mainClass.DropboxInstalled;
      if (!mainClass.DropboxInstalled)
        return;

      textPath.Text = mainClass.dbPath;
      textHttp.Text = mainClass.dbHttp;

      int selIndex = 0;
      switch (mainClass.imageFormat.ToLower()) {
        case "png": selIndex = 0; break;
        case "jpg": selIndex = 1; break;
        case "gif": selIndex = 2; break;
      }
      comboFormat.SelectedIndex = selIndex;

      checkUseMD5.Checked = mainClass.useMD5;
      checkShortMD5.Checked = mainClass.shortMD5;
      numLength.Value = mainClass.length;

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

      if (textPath.Text == "") {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Dropbox\\host.db";
        if (File.Exists(path)) {
          string dropboxPath = "";

          try {
            using (StreamReader reader = new StreamReader(File.OpenRead(path))) {
              // new way of getting the user's dropbox path.
              // host.db contains a second line which is a base64 encoded string of the path.
              // don't ask me why, but they made config.db encrypted (config.dbx)
              reader.ReadLine(); // skip first line, random useless zeros
              dropboxPath = mainClass.base64Decode(reader.ReadLine());
            }
          } catch { }

          if (dropboxPath != "")
            textPath.Text = dropboxPath.Replace('\\', '/') + "/Public/";
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (mainClass.DropboxInstalled) {
        mainClass.settings.SetString("Path", textPath.Text.Replace('\\', '/') + (textPath.Text.EndsWith("/") || textPath.Text.EndsWith("\\") ? "" : "/"));
        mainClass.settings.SetString("Http", textHttp.Text);

        mainClass.settings.SetString("Format", comboFormat.Items[comboFormat.SelectedIndex].ToString());

        mainClass.settings.SetBool("UseMD5", checkUseMD5.Checked);
        mainClass.settings.SetBool("ShortMD5", checkShortMD5.Checked);

        mainClass.settings.SetInt("Length", (int)numLength.Value);

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
      }

      this.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void numShortMD5Count_ValueChanged(object sender, EventArgs e)
    {
      checkShortMD5.Checked = true;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      MessageBox.Show("ClipUpload can not automatically detect your Dropbox public link, so you'll have to give part of it yourself. To do that, make a dummy file (test.txt) in your public folder, right click it, click \"Dropbox\" -> \"Copy public link\", then paste the link in the field here. Remember to remove the \"test.txt\" from the field. You can now remove test.txt from your public folder again.", "Dropbox Public Link", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button4_Click(object sender, EventArgs e)
    {
      MessageBox.Show("This turns the uploaded image into a Jpeg instead of the usual format selected on the left. If the resulting filesize is larger than X amount of KB, it will use the given compression rate.", "Jpeg compression", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  }
}
