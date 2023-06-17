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

        async Task Convert()
        {
            void UpdateProgressBar(int value)
            {
                progressBar1.Invoke((MethodInvoker)(() =>
                {
                    progressBar1.Value = value;
                }));
            }
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoId);
            progressBar1.Visible = true;
            UpdateProgressBar(9);
            UpdateProgressBar(18);
            var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(videoId);
            UpdateProgressBar(27);
            var audioStreamInfo = streamInfoSet.GetAudioOnlyStreams().GetWithHighestBitrate();
            UpdateProgressBar(36);
            if (audioStreamInfo != null)
            {
                UpdateProgressBar(45);
                var fileName = $"{video.Title}.{audioStreamInfo.Container.Name}";
                UpdateProgressBar(54);
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), fileName);
                UpdateProgressBar(63);
                await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath);
                UpdateProgressBar(72);
                // Converter para MP3
                var mp3FilePath = Path.ChangeExtension(filePath, ".mp3");
                UpdateProgressBar(81);
                ConvertToMp3(filePath, mp3FilePath);
                UpdateProgressBar(90);
                File.Delete(filePath);
                UpdateProgressBar(100);
                richTextBox1.Visible = true;
                richTextBox1.Text = $"O áudio do vídeo '{video.Title}' foi baixado com sucesso em: {mp3FilePath}";
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
