using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClaPoetryCorpus.Definitions;
using ClaPoetryCorpus.Extractors;

namespace ClaPoetryCorpus.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TestExtractSongciFromFile();
        }

        private static void TestExtractSongciFromFile()
        {
            string filePath =
                @"C:\Personal\Projects\Repositoryies\Classical-Chinese-Poetry-Corpus\ClaPoetryCorpus\TestData\SongCiHtmls\0003.htm";
            string dirPath =
                @"C:\Personal\Projects\Repositoryies\Classical-Chinese-Poetry-Corpus\ClaPoetryCorpus\RawData\quansongci";
            string outputFile = @"C:\Personal\temp.txt";
            ClaPoetryCorpus.Extractors.SongCiExtractor extractor = new SongCiExtractor();

            List<SongCi> songcis = extractor.ExtractFromDirectory(dirPath);
            StreamWriter writer = new StreamWriter(outputFile);
            writer.WriteLine(songcis.Count);
            writer.WriteLine(string.Join("\r\n", songcis.Select(x => x.ToString())));
            writer.Flush();
            writer.Close();
        }
    }
}
