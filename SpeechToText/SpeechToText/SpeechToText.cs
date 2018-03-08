﻿using Newtonsoft;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace SpeechToText
{
    class speechtotext
    {
        string res = "";
        public Rootobject parse(string json)
        {
            Rootobject root = new Rootobject();
            //Console.WriteLine(".........................");
            Newtonsoft.Json.JsonConvert.PopulateObject(json, root);
            return root;
        }

        public string fromWaveFile(string path)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(
                           "b37a3344-f8ad-4817-ac8b-6e15635a1527:JyBxgO1QvWXw")));

                var content = new StreamContent(new FileStream(path, FileMode.Open));
                content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
                var response = client.PostAsync("https://stream.watsonplatform.net/speech-to-text/api/v1/recognize?interim_results=false", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(res);
                }
                string json = response.Content.ReadAsStringAsync().Result;
                return json;
            }
        }
        public static void GetResponses()
        {            
            for (int j = 0; ; j++)
            {
                try
                {
                    //Thread.Sleep(500);
                   // Console.WriteLine("reading from temp" + j.ToString());
                    if (File.Exists(@"D:\temp" + j.ToString() + ".wav"))
                    {
                        Rootobject rootobject = new Rootobject();
                        speechtotext speechtotext = new speechtotext();
                        string json = speechtotext.fromWaveFile(@"D:\temp" + j.ToString() + ".wav");
                        rootobject = speechtotext.parse(json);

                        for (int i = 0; i < rootobject.results.Length; i++)
                        {
                            //Console.WriteLine(rootobject.results[i].final);
                            Console.WriteLine(rootobject.results[i].alternatives[0].transcript.ToString());


                        }
                    }
                }
                catch
                {
                    j--; 
                }
            }
        }
        static int recordingFlag = 0;
        public static void recordInChunks()
        {
            for (int i = 0; recordingFlag!=0; i++)
            {
                //Console.WriteLine("writing on temp" + i.ToString());
                string filename = @"D:/temp" + i.ToString() + ".wav";
                AudioRecorder recorder = new AudioRecorder();
                recorder.startRecording(filename);
                System.Threading.Thread.Sleep(4500);
                //Console.WriteLine("Press any key to split");
                //Console.ReadKey();
                recorder.stopRecording();
            }
        }
        public static void Transcribe()
        {
            recordingFlag = 1;
            Thread recorderThread = new Thread(new ThreadStart(recordInChunks));            
            Thread responseThread = new Thread(new ThreadStart(GetResponses));
            recorderThread.Start();
            Thread.Sleep(5000);
            responseThread.Start();
        }
    }

}