using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using NAudio.Wave;

namespace YouTubeToMP3
{
    public partial class Form1 : Form
    {
        string videoId;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                Convert();
        }
        void UpdateProgressBar(int value)
        {
            progressBar1.Invoke((MethodInvoker)(() =>
            {
                progressBar1.Value = value;
            }));
        }
        async Task Convert()
        {
            var youtube = new YoutubeClient();
            richTextBox1.Visible = false;
            progressBar1.Visible = true;
            var video = await youtube.Videos.GetAsync(videoId);
            UpdateProgressBar(8);
            var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(videoId);
            UpdateProgressBar(16);
            var audioStreamInfo = streamInfoSet.GetAudioOnlyStreams().GetWithHighestBitrate();
            UpdateProgressBar(24);

            if (audioStreamInfo != null)
            {
                UpdateProgressBar(32);
                var tempPath = Path.GetTempPath();
                UpdateProgressBar(40);
                var fileName = $"{video.Title}.{audioStreamInfo.Container.Name}";
                UpdateProgressBar(48);
                var newFileName = fileName.Replace(".webm", ".mp3");
                UpdateProgressBar(56);
                var tempFile = Path.Combine(tempPath, fileName);
                UpdateProgressBar(64);
                var saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), newFileName);
                UpdateProgressBar(72);
                await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, tempFile);
                richTextBox1.Visible = true;
                richTextBox1.Text = $"O áudio do vídeo '{video.Title}' foi baixado e será convertido...";
                UpdateProgressBar(80);
                ConvertToMp3(tempFile, saveFilePath);
                UpdateProgressBar(90);
                File.Delete(tempFile);
                UpdateProgressBar(100);
                richTextBox1.Text = $"O áudio do vídeo '{video.Title}' foi baixado e convertido com sucesso em: {saveFilePath}";
                progressBar1.Visible = false;
                UpdateProgressBar(0);


            }
            else
            {
                richTextBox1.Visible = true;
                richTextBox1.Text = $"Não foi possível encontrar uma stream de áudio para o vídeo '{video.Title}'";
            }
        }
        private static void ConvertToMp3(string inputFilePath, string outputFilePath)
        {
            using (var reader = new MediaFoundationReader(inputFilePath))
            {
                MediaFoundationEncoder.EncodeToMp3(reader, outputFilePath);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            videoId = textBox1.Text;
        }
    }
}
