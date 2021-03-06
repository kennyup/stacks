﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Slalom.Stacks.Services.Registry;

namespace Slalom.Stacks.Documentation
{
    public class WordDocument : IDisposable
    {
        private readonly Body _body;

        private readonly WordprocessingDocument _document;
        private readonly string _path;

        public WordDocument(string path)
        {
            File.WriteAllBytes(path, File.ReadAllBytes(@"C:\source\Stacks\Core\src\Slalom.Stacks.Documentation\template.docx"));

            _path = path;
            _document = WordprocessingDocument.Open(path, true);
            _body = _document.MainDocumentPart.Document.Body = new Body();

            var sectionProps = new SectionProperties();
            var pageMargin = new PageMargin { Top = 808, Right = 1008U, Bottom = 1008, Left = 1008U, Header = 520U, Footer = 720U, Gutter = 0U };
            sectionProps.Append(pageMargin);
            _body.Append(sectionProps);
        }


        public void Append(IEnumerable<EndPointProperty> requestProperties)
        {
            if (requestProperties.Any())
            {
                var table = new DocumentTable(_document, 2500, 2500, 5000);
                table.AppendRow("Name", "Type", "Description");

                foreach (var property in requestProperties)
                {
                    table.AppendRow(property.Name, Type.GetType(property.Type).Name, property.Comments?.Value);
                }
            }
            else
            {
                this.Append("None");
            }
        }

        public DocumentTable AppendTable(params int[] columnWidths)
        {
            return new DocumentTable(_document, columnWidths);
        }

        public void Append(string text, string style = null)
        {
            var paragraph = new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(text)));

            if (style != null)
            {
                paragraph.ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId
                    {
                        Val = _document.GetStyleIdFromStyleName(style)
                    });
            }

            _body.AppendChild(paragraph);
        }

        public void Dispose()
        {
            _document.Dispose();
        }

        public void Open()
        {
            Process.Start(_path);
        }

        public void Save()
        {
            _document.MainDocumentPart.Document.Save();
        }
    }
}