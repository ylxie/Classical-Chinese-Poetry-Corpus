using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaPoetryCorpus.Definitions
{
    public class SongCi
    {
        public SongCi(string author, string title, string body)
        {
            this.Author = author;
            this.Title = title;
            this.Body = body;
        }

        public string Author { get; private set; }

        public string Title { get; private set; }

        public string Body { get; private set; }

        public override string ToString()
        {
            return $"Author: {Author}\r\nTitle: {Title}\r\nBody: {Body}";
        }
    }
}
