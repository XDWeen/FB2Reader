﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using FB2Library;
using FB2Library.Elements;
using FB2Library.Elements.Poem;
using Microsoft.Win32;

namespace Fb2LibReader
{
    public class BookInteraction
    {
        //global variable
        public FB2File Book;
        private List<string> BookText = new List<string>();
        private byte[] Images;

     
        protected virtual void PrepareTextItems(IEnumerable<IFb2TextItem> textItems)
        {
            foreach (var textItem in textItems)
            {
                PrepareTextItem(textItem);
            }
        }


        //record book strings to dictionary for read
        public virtual void PrepareTextItem(IFb2TextItem textItem)
        {
            if (textItem is CiteItem)
            {
                PrepareTextItems(((CiteItem)textItem).CiteData);
                return;
            }

            if (textItem is PoemItem)
            {
                var item = (PoemItem)textItem;
                AddTitle(item.Title);
                PrepareTextItems(item.Content);
                return;
            }

            if (textItem is SectionItem)
            {
                var item = (SectionItem)textItem;
                AddTitle(item.Title);
                PrepareTextItems(item.Content);
                return;
            }

            if (textItem is StanzaItem)
            {
                var item = (StanzaItem)textItem;
                AddTitle(item.Title);
                PrepareTextItems(item.Lines);
                return;
            }

            if (textItem is ParagraphItem
                || textItem is EmptyLineItem
                || textItem is TitleItem
                || textItem is SimpleText)
            {
                BookText.Add(textItem.ToString() + "\n");
                return;
            }

            if (textItem is ImageItem)
            {
                var item = (ImageItem)textItem;
                var key = item.HRef.Replace("#", string.Empty);

                if (Book.Images.ContainsKey(key))
                {
                    var data = Book.Images[key].BinaryData;
                    Images = data;
                }
                return;
            }

            if (textItem is DateItem)
            {
                BookText.Add(((DateItem)textItem).DateValue.ToString() + "\n");
                return;
            }

            if (textItem is EpigraphItem)
            {
                var item = (EpigraphItem)textItem;
                PrepareTextItems(item.EpigraphData);
                return;
            }

            throw new Exception(textItem.GetType().ToString());
        }




        protected virtual void AddTitle(TitleItem titleItem)
        {
            if (titleItem != null)
            {
                foreach (var title in titleItem.TitleData)
                {
                    BookText.Add(title.ToString() + "\n");
                }
            }
        }



        //add all book's content to Dictionary
        protected void BookToDictionary(FB2File file)
        {
            int sectionsCount = 0;
            Book = file;

            for (var i = 0; sectionsCount < Book.MainBody.Sections.Count() - 1; i++)
            {
                if (i == (Book.MainBody.Sections[sectionsCount].Content.Count()) - 1)
                {
                    ++sectionsCount;
                    i = 0;
                }

                PrepareTextItem(Book.MainBody.Sections[sectionsCount].Content[i]);
            }
        }


        //make book to read
        protected void ReadBook(FB2File file, int skip, int take)
        {
            BookToDictionary(file);
            var dictionaryDop = BookText.Skip(skip);
            BookText = dictionaryDop.Take(take).ToList();
        }

        //Return book's page to place on textblock
        public string GetPage(FB2File file, int skip, int take)
        {
            
            ReadBook(file, skip, take);
            var page = "";
            foreach (var str in BookText)
            {
                page += str;
            }
            return page;
        }


        //Open book


        //Open book
        public async Task OpenBook()
        {
            var openDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\Projects\\MyLibReader\\MyLibReader\\Solution\\Books",
            };
            openDialog.ShowDialog();
            var path = openDialog.FileName;
            Book = await ReadFb2FileStreamAsync(File.OpenRead(path));
            //return file;  Task<FB2File>
        }


        //Prepare book to open
        public async Task<FB2File> ReadFb2FileStreamAsync(Stream stream)
        {
            // setup
            var readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };
            var loadSettings = new XmlLoadSettings(readerSettings);

            var file = new FB2File();

            try
            {
                // reading
                file = await new FB2Reader().ReadAsync(stream, loadSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file : {ex.Message}");
            }

            return file;
        }

    }
}