using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

public class Program
{
    static async Task Main()
    {
        string videoId = "https://youtu.be/IpwHB2U3J1s"; // Substitua pelo ID do vídeo desejado

        var youtube = new YoutubeClient();
        var video = await youtube.Videos.GetAsync(videoId);

        var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(videoId);
        var audioStreamInfo = streamInfoSet.GetAudioOnly().WithHighestBitrate();
        if (audioStreamInfo != null)
        {
            var fileName = $"{video.Title}.{audioStreamInfo.Container.Name}";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), fileName);

            await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, filePath);

            Console.WriteLine($"O áudio do vídeo '{video.Title}' foi baixado com sucesso em: {filePath}");
        }
        else
        {
            Console.WriteLine($"Não foi possível encontrar uma stream de áudio para o vídeo '{video.Title}'.");
        }
    }
}

