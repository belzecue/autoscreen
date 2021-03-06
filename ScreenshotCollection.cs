﻿//////////////////////////////////////////////////////////
// Auto Screen Capture 2.0.8
// autoscreen.EditorCollection.cs
//
// Written by Gavin Kendall (gavinkendall@gmail.com)
// Thursday, 15 May 2008 - Friday, 5 January 2018

using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace autoscreen
{
    public static class ScreenshotCollection
    {
        private static XmlDocument xdoc = null;
        private static ArrayList m_screenshotList = new ArrayList();

        private const string XML_FILE_INDENT_CHARS = "   ";
        private const string XML_FILE_SCREENSHOT_NODE = "screenshot";
        private const string XML_FILE_SCREENSHOTS_NODE = "screenshots";
        private const string XML_FILE_ROOT_NODE = "autoscreen";

        private const string SCREENSHOT_INDEX = "index";
        private const string SCREENSHOT_DATE = "date";
        private const string SCREENSHOT_PATH = "path";
        private const string SCREENSHOT_SCREEN = "screen";
        private const string SCREENSHOT_FORMAT = "format";
        private const string SCREENSHOT_FILENAME = "filename";
        private const string SCREENSHOT_SLIDENAME = "slidename";
        private const string SCREENSHOT_XPATH = "/" + XML_FILE_ROOT_NODE + "/" + XML_FILE_SCREENSHOTS_NODE + "/" + XML_FILE_SCREENSHOT_NODE;

        public static void Add(Screenshot screenshot)
        {
            m_screenshotList.Add(screenshot);
        }

        public static Screenshot GetByIndex(int index)
        {
            return (Screenshot)m_screenshotList[index];
        }

        public static int Count
        {
            get { return m_screenshotList.Count; }
        }

        public static Screenshot GetBySlidename(string slidename, int screenNumber)
        {
            Screenshot screenshot = null;

            if (xdoc != null)
            {
                xdoc.LoadXml(Properties.Settings.Default.Screenshots);

                XmlNodeList xscreenshots = xdoc.SelectNodes(SCREENSHOT_XPATH + "[" + SCREENSHOT_SLIDENAME + " = '" + slidename + "' and " + SCREENSHOT_SCREEN + " = '" + screenNumber + "']");

                foreach (XmlNode xscreenshot in xscreenshots)
                {
                    XmlNodeReader xreader = new XmlNodeReader(xscreenshot);

                    while (xreader.Read())
                    {
                        if (xreader.IsStartElement() && xreader.Name.Equals(SCREENSHOT_INDEX))
                        {
                            xreader.Read();

                            if (!string.IsNullOrEmpty(xreader.Value))
                            {
                                screenshot = GetByIndex(Convert.ToInt32(xreader.Value));
                            }
                        }
                    }

                    xreader.Close();
                }
            }

            return screenshot;
        }

        public static ArrayList GetDates()
        {
            ArrayList dates = new ArrayList();

            if (xdoc != null)
            {
                XmlNodeList xdates = xdoc.SelectNodes(SCREENSHOT_XPATH + "/" + SCREENSHOT_DATE);

                foreach (XmlNode xdate in xdates)
                {
                    XmlNodeReader xreader = new XmlNodeReader(xdate);

                    while (xreader.Read())
                    {
                        if (xreader.IsStartElement() && xreader.Name.Equals(SCREENSHOT_DATE))
                        {
                            xreader.Read();

                            if (!string.IsNullOrEmpty(xreader.Value))
                            {
                                dates.Add(xreader.Value);
                            }
                        }
                    }

                    xreader.Close();
                }
            }

            return dates;
        }

        /// <summary>
        /// Loads the screenshots.
        /// </summary>
        public static void Load()
        {
            xdoc = new XmlDocument();
            xdoc.LoadXml(Properties.Settings.Default.Screenshots);

            XmlNodeList xscreenshots = xdoc.SelectNodes(SCREENSHOT_XPATH);

            foreach (XmlNode xscreenshot in xscreenshots)
            {
                Screenshot screenshot = new Screenshot();
                XmlNodeReader xreader = new XmlNodeReader(xscreenshot);

                while (xreader.Read())
                {
                    if (xreader.IsStartElement())
                    {
                        switch (xreader.Name)
                        {
                            case SCREENSHOT_INDEX:
                                xreader.Read();
                                screenshot.Index = string.IsNullOrEmpty(xreader.Value) ? 0 : Convert.ToInt32(xreader.Value);
                                break;

                            case SCREENSHOT_DATE:
                                xreader.Read();
                                screenshot.Date = xreader.Value;
                                break;

                            case SCREENSHOT_PATH:
                                xreader.Read();
                                screenshot.Path = xreader.Value;
                                break;

                            case SCREENSHOT_SCREEN:
                                xreader.Read();
                                screenshot.Screen = string.IsNullOrEmpty(xreader.Value) ? 0 : Convert.ToInt32(xreader.Value);
                                break;

                            case SCREENSHOT_FORMAT:
                                xreader.Read();
                                screenshot.Format = xreader.Value;
                                break;

                            case SCREENSHOT_FILENAME:
                                xreader.Read();
                                screenshot.Filename = xreader.Value;
                                break;

                            case SCREENSHOT_SLIDENAME:
                                xreader.Read();
                                screenshot.Slidename = xreader.Value;
                                break;
                        }
                    }
                }

                xreader.Close();

                if (!string.IsNullOrEmpty(screenshot.Date) &&
                    !string.IsNullOrEmpty(screenshot.Path) &&
                    screenshot.Screen > 0 &&
                    !string.IsNullOrEmpty(screenshot.Format) &&
                    !string.IsNullOrEmpty(screenshot.Filename) &&
                    !string.IsNullOrEmpty(screenshot.Slidename))
                {
                    Add(screenshot);
                }
            }
        }

        /// <summary>
        /// Saves the screenshots.
        /// </summary>
        public static void Save()
        {
            XmlWriterSettings xsettings = new XmlWriterSettings();
            xsettings.Indent = true;
            xsettings.CloseOutput = true;
            xsettings.CheckCharacters = true;
            xsettings.Encoding = Encoding.UTF8;
            xsettings.DoNotEscapeUriAttributes = true;
            xsettings.NewLineChars = Environment.NewLine;
            xsettings.IndentChars = XML_FILE_INDENT_CHARS;
            xsettings.NewLineHandling = NewLineHandling.Entitize;
            xsettings.ConformanceLevel = ConformanceLevel.Document;

            StringBuilder screenshots = new StringBuilder();

            using (XmlWriter xwriter = XmlWriter.Create(screenshots))
            {
                xwriter.WriteStartDocument();
                xwriter.WriteStartElement(XML_FILE_ROOT_NODE);
                xwriter.WriteStartElement(XML_FILE_SCREENSHOTS_NODE);

                foreach (object obj in m_screenshotList)
                {
                    Screenshot screenshot = (Screenshot)obj;

                    xwriter.WriteStartElement(XML_FILE_SCREENSHOT_NODE);
                    xwriter.WriteElementString(SCREENSHOT_INDEX, screenshot.Index.ToString());
                    xwriter.WriteElementString(SCREENSHOT_DATE, screenshot.Date);
                    xwriter.WriteElementString(SCREENSHOT_PATH, screenshot.Path);
                    xwriter.WriteElementString(SCREENSHOT_SCREEN, screenshot.Screen.ToString());
                    xwriter.WriteElementString(SCREENSHOT_FORMAT, screenshot.Format);
                    xwriter.WriteElementString(SCREENSHOT_FILENAME, screenshot.Filename);
                    xwriter.WriteElementString(SCREENSHOT_SLIDENAME, screenshot.Slidename);

                    xwriter.WriteEndElement();
                }

                xwriter.WriteEndElement();
                xwriter.WriteEndElement();
                xwriter.WriteEndDocument();

                xwriter.Flush();
                xwriter.Close();
            }

            Properties.Settings.Default.Screenshots = screenshots.ToString();
        }
    }
}
