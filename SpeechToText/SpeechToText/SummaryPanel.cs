﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SpeechToText
{
    public partial class SummaryPanel : UserControl
    {
        
        public SummaryPanel()
        {
            InitializeComponent();
            toolTip1.SetToolTip(pictureBox_anger , "Anger");
            toolTip1.SetToolTip(pictureBox_confidence , "Confidence");
            toolTip1.SetToolTip(pictureBox_disgust, "Disgust");
            toolTip1.SetToolTip(pictureBox_fear, "Fear");
            toolTip1.SetToolTip(pictureBox_happy, "Happy");
            toolTip1.SetToolTip(pictureBox_sad, "Sad");
        }

        private void SummaryPanel_Load(object sender, EventArgs e)
        {

        }
        public void uploadAction()
        {
            //uploading audio file
            openFileDialog1.Title = "Choose Audio file";
            openFileDialog1.Filter = "Wave audio (*.wav)|*.wav|Mp3 audio (*.mp3)|*.mp3";
            string filePath;
            string outputText;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox_summary.Text = "Please wait. we are working on the file.";
                filePath = openFileDialog1.FileName;
                //requesting response from server
                if (filePath.Substring(filePath.Length - 3) == "wav")
                    outputText = speechtotext.fromWaveFile(filePath);
                else if (filePath.Substring(filePath.Length - 3) == "mp3")
                    outputText = speechtotext.fromMp3File(filePath);
                else
                    outputText = speechtotext.fromflacFile(filePath);
                //parsing the json file and showing in textbox
                outputText = speechtotext.GetSummaryFromJSON(outputText);
                richTextBox_summary.Text = outputText;
                //analysing the tone of the user
                ToneSetterUpload(outputText);
            }
            else
            {

            }
        }
        private void ToneSetterRecord(string text)
        {
            try
            {
                ToneAnalyzerJSON obj = new ToneAnalyzerJSON();
                ToneAnalyser.Parse(obj, text);
                for (int i = 0; i < obj.document_tone.tones.Length; i++)
                {
                    if (obj.document_tone.tones[i].tone_id == "sadness")
                        label_sad.Text = ((int)(obj.document_tone.tones[i].score * 100)).ToString() + " %";
                    else if (obj.document_tone.tones[i].tone_id == "joy")
                        label_happy.Text = ((int)(obj.document_tone.tones[i].score * 100)).ToString() + " %";
                    else if (obj.document_tone.tones[i].tone_id == "fear")
                        label_fear.Text = ((int)(obj.document_tone.tones[i].score * 100)).ToString() + " %";
                    else if (obj.document_tone.tones[i].tone_id == "disgust")
                        label_disgust.Text = ((int)(obj.document_tone.tones[i].score * 100)).ToString() + " %";
                    else if (obj.document_tone.tones[i].tone_id == "anger")
                        label_angry.Text = ((int)(obj.document_tone.tones[i].score * 100)).ToString() + " %";

                    else if (obj.document_tone.tones[i].tone_id == "confident")
                        label_sad.Text = ((int)(obj.document_tone.tones[i].score * 100)).ToString() + " %";

                }
                Form1.outputText = text;
            }
            catch { }
        }
        private void ToneSetterUpload(string text)
        {
            try
            {
                ToneAnalyzerJSON obj = new ToneAnalyzerJSON();
                ToneAnalyser.Parse(obj, text);

                int[] a = { 0, 0, 0, 0, 0, 0 };
                foreach (var item in obj.sentences_tone)
                {
                    foreach (var i in item.tones)
                    {
                        if (i.tone_id == "joy")
                            a[0] += (int)(i.score * 100);
                        else if (i.tone_id == "anger")
                            a[1] += (int)(i.score * 100);
                        else if (i.tone_id == "sadness")
                            a[2] += (int)(i.score * 100);
                        else if (i.tone_id == "disgust")
                            a[3] += (int)(i.score * 100);
                        else if (i.tone_id == "fear")
                            a[4] += (int)(i.score * 100);
                        else if (i.tone_id == "confident")
                            a[5] += (int)(i.score * 100);
                    }
                }

                for (int i = 0; i < 6; i++)
                {
                    a[i] = a[i] / obj.sentences_tone.Length;
                }

                label_happy.Text = a[0].ToString() + " %";
                label_angry.Text = a[1].ToString() + " %";
                label_sad.Text = a[2].ToString() + " %";
                label_disgust.Text = a[3].ToString() + " %";
                label_fear.Text = a[4].ToString() + " %";
                label_confidence.Text = a[5].ToString() + " %";              

                Form1.outputText = text;
            }
            catch { }
        }
        private void SummaryPanel_VisibleChanged(object sender, EventArgs e)
        {
            textBox_translation.Text = "";
            richTextBox1.Text = "";
            if(Form1.summaryCaller=="uploadPanel")
            {
                uploadAction();
            }
            else if(Form1.summaryCaller=="recordingPanel")
            {
                richTextBox_summary.Text = Form1.outputText;
                ToneSetterRecord(Form1.outputText);
            }
        }

        private void button_saveToFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save Transcript";
            saveFileDialog1.Filter = "Text file (*.txt)|*.txt";
            string filePath;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog1.FileName;
                using (StreamWriter file = new StreamWriter(filePath))
                {
                    file.WriteLine(Form1.outputText);
                    file.WriteLine();
                    file.WriteLine("Translation :");
                    file.WriteLine(textBox_translation.Text);
                }
                
            }
        }
    }
}
