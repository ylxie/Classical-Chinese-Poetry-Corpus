using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using ClaPoetryCorpus.Definitions;
using HtmlAgilityPack;

namespace ClaPoetryCorpus.Extractors
{
    public class SongCiExtractor
    {
        private static readonly string AuthorXPath = null;

        private static readonly string TitleXPath = null;

        static SongCiExtractor()
        {
            AuthorXPath = "/html/body/table/tr/td/table/tr/td/table/tr/td/font/b/font/text()";
            TitleXPath = "/html/body/table/tr/td/table/tr/td/table/tr/td/p";
        }

        public List<SongCi> ExtractFromDirectory(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"{dirPath} doesn't exist!");
            }

            FileInfo[] htmlFiles = dir.GetFiles("*.htm*", SearchOption.AllDirectories);

            return htmlFiles.SelectMany(info => ExtractFromFile(info)).ToList();
        }

        public List<SongCi> ExtractFromFile(string filePath)
        {
            return ExtractFromFile(new FileInfo(filePath));
        }

        public List<SongCi> ExtractFromFile(FileInfo fileInfo)
        {
            HtmlDocument htmlDocument = new HtmlDocument();            
            htmlDocument.DetectEncodingAndLoad(fileInfo.FullName, true);

            HtmlNode rootNode = htmlDocument.DocumentNode;
            RemoveEmptyNodes(rootNode);

            return new List<SongCi>(ExtractSongCi(rootNode));
        }

        private string ExtractAuthor(HtmlNode rootNode)
        {
            HtmlNode authorNode = rootNode.SelectSingleNode(AuthorXPath);

            return authorNode != null ? NormalizeText(authorNode.InnerText) : null;
        }

        private IEnumerable<SongCi> ExtractSongCi(HtmlNode rootNode)
        {
            string author = ExtractAuthor(rootNode);

            if(string.IsNullOrEmpty(author))
            {
                yield break;
            }

            var titleNodes = rootNode.SelectNodes(TitleXPath);

            foreach (var titleNode in titleNodes)
            {
                if (IsSongCiTitle(titleNode))
                {
                    string fullBody = NormalizeText(titleNode.NextSibling.InnerText);

                    string[] parts = fullBody.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                                            .Where(x => !string.IsNullOrWhiteSpace(x))
                                            .ToArray();
                    string subTitle = string.Empty;
                    
                    if(parts.Length > 1 && parts[0].EndsWith(@"\\"))
                    {
                        subTitle = parts[0].Substring(0, parts[0].Length - 2);
                        parts = parts.Skip(1).ToArray();
                    }

                    string body = string.Join("\r\n", parts);

                    yield return new SongCi(author, NormalizeText(titleNode.InnerText), subTitle, body);
                }
            }            
        }

        private bool RemoveEmptyNodes(HtmlNode htmlNode)
        {
            if(htmlNode.HasChildNodes)
            {
                List<HtmlNode> emptyNodes = new List<HtmlNode>();
                foreach (HtmlNode node in htmlNode.ChildNodes)
                {
                    if (RemoveEmptyNodes(node))
                    {
                        emptyNodes.Add(node);
                    }
                }

                bool isThisEmpty = emptyNodes.Count == htmlNode.ChildNodes.Count;

                emptyNodes.ForEach(node => node.Remove());

                return isThisEmpty;
            }
            else
            {
                return string.IsNullOrWhiteSpace(HttpUtility.HtmlDecode(htmlNode.InnerText));
            }
        }

        private bool IsSongCiTitle(HtmlNode titleNode)
        {
            if (titleNode.NextSibling != null
                && titleNode.NextSibling.Name.ToLower() == "ul")
            {
                return true;
            }

            return false;
        }

        private string NormalizeText(string text)
        {
            return HttpUtility.HtmlDecode(text).Trim();            
        }
    }
}
