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
using System.Xml.Serialization;

namespace BaseIndexer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
        }
        private List<string> _listOld;
        private Dictionary<int, string> _wordList;
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Start" + DateTime.Now);
            StreamReader s = new StreamReader(@"D:\Saideh\MirasText\MirasTextEditN2.txt");
            var charCount = new System.IO.StreamReader(@"D:\Saideh\MirasText\MirasTextEditN2.txt").ReadToEnd().Replace("\r\n", "\r").Length;
            _wordList = NextWord2(s, charCount);
            progressBar1.Minimum = 0;
            progressBar1.Maximum = _wordList.Count;
            s.Close();
            backgroundWorker1.RunWorkerAsync();
        }
        private int _wordIndex = -1;
        private Dictionary<int, string> NextWord2(StreamReader stremReader, int charCount)
        {
            var wordList = new Dictionary<int, string>();
            int c = stremReader.Peek();
            while (c != -1 && Char.IsWhiteSpace(Convert.ToChar(c)))
            {
                stremReader.Read();
                c = stremReader.Peek();
            }

            if (c == -1) return new Dictionary<int, string>();


            var word = "";
            var d = 0;
            while (true)
            {
                while (c != -1 && !Char.IsWhiteSpace(Convert.ToChar(c)))
                {
                    word += (Convert.ToChar(c));
                    _wordIndex++;
                    stremReader.Read();
                    c = stremReader.Peek();
                }
                if (word != "")
                {
                    wordList.Add(_wordIndex-word.Length+1, word);
                }
                if (c == 13)
                {
                    wordList.Add(_wordIndex-1, Environment.NewLine);
                }
                stremReader.Read();
                d++;
                c = stremReader.Peek();
                //برای فاصله ها به اندیس اضافه کن
                _wordIndex++;

                word = "";

                if (d > charCount)
                    break;
            }


            return wordList;
        }
        private List<string> NextWord(StreamReader stremReader, int charCount)
        {
            var wordList = new List<string>();
            int c = stremReader.Peek();
            while (c != -1 && Char.IsWhiteSpace(Convert.ToChar(c)))
            {
                stremReader.Read();
                c = stremReader.Peek();
            }

            if (c == -1) return new List<string>();


            var word = "";
            var d = 0;
            while (true)
            {
                while (c != -1 && !Char.IsWhiteSpace(Convert.ToChar(c)))
                {
                    word += (Convert.ToChar(c));
                    stremReader.Read();
                    c = stremReader.Peek();
                }
                if (word != "")
                {
                    wordList.Add(word);
                }
                if (c == 13)
                {
                    wordList.Add(Environment.NewLine);
                }
                stremReader.Read();
                d++;
                c = stremReader.Peek();


                word = "";

                if (d > charCount)
                    break;
            }


            return wordList;
        }
        private Dictionary<int, string> _zamir = new Dictionary<int, string>();
        private Dictionary<int, string> _marja = new Dictionary<int, string>();
        private Dictionary<int, List<int>> _db = new Dictionary<int, List<int>>();
        private bool FindIndex(int startPosition, int endPosition, bool isZamir)
        {

            var text = richTextBox1.Text;
            var wordIndex = GetWordPosition(startPosition);
            if (wordIndex == -1)
                return false;
            char[] charArr = text.ToCharArray();
            string result = "";

            for (var i = startPosition; i < endPosition; i++)
            {
                result += charArr[i];
            }

            if (result != "")
            {
                if (isZamir)
                {
                    //_zamir.Add(startPosition, result);
                    _zamir.Add(wordIndex, result);
                    //txtEnd.Text = result;
                }
                else
                {
                    var lastZamir = _zamir.Last();
                    if (_marja.Keys.Any(x => x == wordIndex))
                    {
                        //add to List
                        if (_db.TryGetValue(wordIndex, out var list))
                        {
                            list.Add(lastZamir.Key);
                            _db[wordIndex] = list;
                        }


                    }
                    else
                    {
                        _marja.Add(wordIndex, result);                        
                        _db.Add(wordIndex, new List<int> { lastZamir.Key });
                    }
                }
            }
            return true;

        }

        private int GetWordPosition(int startPosition)
        {
            int i = 0;
            foreach(KeyValuePair<int, string> item in _wordList)
            {
                if(startPosition == item.Key)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private void richTextBox1_SelectionChanged_1(object sender, EventArgs e)
        {
            var docStart = richTextBox1;

            var selectionStart = richTextBox1.SelectionStart;
            var selectionEnd = richTextBox1.SelectionLength;            
            txtStart.Text = selectionStart.ToString();
            txtEnd.Text = selectionEnd.ToString();
            _startPos = selectionStart;
            _endPos = selectionEnd;
        }
        private int _startPos;
        private int _endPos;
        private void txtFinish_Click(object sender, EventArgs e)
        {
            if (!FindIndex(_startPos, _startPos + _endPos, false))
                return;
            richTextBox1.SelectionStart = _startPos;
            richTextBox1.SelectionLength = _endPos ;
            richTextBox1.SelectionBackColor = Color.Pink;
            btnStart.Enabled = true;
            btnFinish.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!FindIndex(_startPos, _startPos + richTextBox1.SelectionLength, true))
                return;
            richTextBox1.SelectionStart = int.Parse(txtStart.Text);
            richTextBox1.SelectionLength = int.Parse(txtEnd.Text);
            richTextBox1.SelectionBackColor = Color.Yellow;
            btnStart.Enabled = false;
            btnFinish.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            var marjas = new List<Marja>();
            foreach (KeyValuePair<int, List<int>> item0 in _db)
            {                
                var zamirs = new List<Zamir>();
                var zamirIndexes = item0.Value;
                foreach (var z in zamirIndexes)
                {
                    zamirs.Add(
                        new Zamir()
                        {
                            Index = z,
                            Text = _zamir[z]
                        });
                }
                var marja = new Marja()
                {
                    Index = item0.Key,
                    Content = _marja[item0.Key],
                    Zamir = zamirs
                };
                marjas.Add(marja);
            }
            
          
            var export = new Export()
            {
                Marjas = new Marjas
                {
                    Marja = marjas
                }
            };

            new Helper().Serialize("Export.xml", export);
        }
        public Export Import(string filename)
        {
            Export export;
            XmlSerializer serializer = new XmlSerializer(typeof(Export));
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                export = (Export)serializer.Deserialize(fs);
            }

            return export;
        }
        private Export _export;
        private void btnImport_Click(object sender, EventArgs e)
        {
            _db.Clear();
            _zamir.Clear();
            _marja.Clear();            
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.FromArgb(255,255,255,255);
            _export = Import("Export.xml");
            foreach(var marja in _export.Marjas.Marja)
            {
                _marja.Add(marja.Index, marja.Content);
                var zamirs = new List<int>();
                foreach(var zamir in marja.Zamir)
                {
                    _zamir.Add(zamir.Index, zamir.Text);
                    zamirs.Add(zamir.Index);
                }
                _db.Add(marja.Index, zamirs);
            }
            SelectAllZamir(_zamir);
            SelectAllMarja(_marja);
        }
        private void SelectAllZamir(Dictionary<int, string> zamirs)
        {
            foreach(KeyValuePair<int, string> item in zamirs)
            {
                richTextBox1.SelectionStart = item.Key;
                richTextBox1.SelectionLength = item.Value.Length;
                richTextBox1.SelectionBackColor = Color.Yellow;
            }
        }
        private void SelectAllMarja(Dictionary<int, string> marjas)
        {
            foreach (KeyValuePair<int, string> item in marjas)
            {
                richTextBox1.SelectionStart = item.Key;
                richTextBox1.SelectionLength = item.Value.Length;
                richTextBox1.SelectionBackColor = Color.Pink;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            var index = 0;
            foreach (KeyValuePair<int, string> item in _wordList)
            {
                
                //richTextBox1.AppendText(_list[i] + " ");
                Action action = () => richTextBox1.AppendText(item.Value + " ");
                richTextBox1.Invoke(action);
                worker.ReportProgress(index++);
            }
            
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.Enabled = true;
            richTextBox1.Enabled = true;
        }
        int start = 0;
        int indexOfSearchText = 0;
        private void btnSearch_Click(object sender, EventArgs e)
        {
            int startindex = 0;
            int endindex = 0;
            start = 0;
            indexOfSearchText = 0;
            while (startindex + endindex < richTextBox1.Text.Length && startindex != -1)
            {
                

                if (txtSearch.Text.Length > 0)
                    startindex = FindMyText(txtSearch.Text, start, richTextBox1.Text.Length);

                // If string was found in the RichTextBox, highlight it
                if (startindex >= 0)
                {
                    // Set the highlight color as red
                    //richTextBox1.SelectionColor = Color.Red;
                    // Find the end index. End Index = number of characters in textbox
                    endindex = txtSearch.Text.Length;
                    // Highlight the search string
                    richTextBox1.Select(startindex, endindex);
                    if (richTextBox1.SelectionBackColor != Color.LightGreen &&
                        richTextBox1.SelectionBackColor != Color.Yellow &&
                        richTextBox1.SelectionBackColor != Color.Pink)
                    { richTextBox1.SelectionBackColor = Color.LightGreen; }
                    // mark the start position after the position of
                    // last search string
                    start = startindex + endindex;
                }
                //start = 0;
                //indexOfSearchText = 0;
            }
            
        }
        public int FindMyText(string txtToSearch, int searchStart, int searchEnd)
        {
            // Unselect the previously searched string
            //if (searchStart > 0 && searchEnd > 0 && indexOfSearchText >= 0)
            //{
            //    richTextBox1.Undo();
            //}

            // Set the return value to -1 by default.
            int retVal = -1;

            // A valid starting index should be specified.
            // if indexOfSearchText = -1, the end of search
            if (searchStart >= 0 && indexOfSearchText >= 0)
            {
                // A valid ending index
                if (searchEnd > searchStart || searchEnd == -1)
                {
                    // Find the position of search string in RichTextBox
                    indexOfSearchText = richTextBox1.Find(txtToSearch, searchStart, searchEnd, RichTextBoxFinds.None);
                    // Determine whether the text was found in richTextBox1.
                    if (indexOfSearchText != -1)
                    {
                        // Return the index to the specified search text.
                        retVal = indexOfSearchText;
                    }
                }
            }
            return retVal;
        }
    }
}
