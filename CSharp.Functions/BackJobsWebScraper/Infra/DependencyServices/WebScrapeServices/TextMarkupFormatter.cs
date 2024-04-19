﻿using AngleSharp;
using AngleSharp.Dom;

namespace BackJobsWebScraper.Infra
{
    public class TextMarkupFormatter : IMarkupFormatter
    {
        public string Text(ICharacterData text)
        {
            return text.Data;
        }

        public string LiteralText(ICharacterData text)
        {
            return "";
        }

        public string Comment(IComment comment)
        {
            return "";
        }

        public string Processing(IProcessingInstruction processing)
        {
            return "";
        }

        public string Doctype(IDocumentType doctype)
        {
            return "";
        }

        public string OpenTag(IElement element, bool selfClosing)
        {
            if (IsBlockLevelElement(element))
            {
                return "<br/>";
            }
            return IsListElement(element) ? " " : "";
        }

        public string CloseTag(IElement element, bool selfClosing)
        {
            if (IsBlockLevelElement(element) || element.TagName == "BR")
            {
                return "<br/>";
            }
            return "";
        }

        private bool IsListElement(IElement element)
        {
            switch (element.TagName)
            {
                case "LI":
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsBlockLevelElement(IElement element)
        {
            switch (element.TagName)
            {
                //case "ADDRESS":
                //case "ARTICLE":
                //case "ASIDE":
                //case "BLOCKQUOTE":
                //case "DETAILS":
                //case "DIALOG":
                //case "DD":
                //case "DIV":
                //case "DL":
                //case "FIELDSET":
                //case "FIGCAPTION":
                //case "FIGURE":
                //case "FOOTER":
                //case "FORM":
                case "H1":
                case "H2":
                case "H3":
                case "H4":
                case "H5":
                case "H6":
                //case "HEADER":
                //case "HGROUP":
                //case "HR":
                //case "LI":
                //case "MAIN":
                case "NAV":
                case "OL":
                case "P": //affects in p tag in li; maybe a way to check if it's parent is li
                //case "PRE":
                //case "SECTION":
                //case "TABLE":
                case "UL":
                    return true;

                default:
                    return false;
            }
        }
    }
}