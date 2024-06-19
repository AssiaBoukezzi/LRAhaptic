/// ----------------------------///
///     RICCA Aylen 2016        ///
///     aricca8@gmail.com       ///
///     Internship IBISC        ///
/// ----------------------------///

//---------------------------------------------------------------------------
// INCLUDES
//---------------------------------------------------------------------------
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using UnityEngine.SceneManagement;

/// <summary>
/// XML Manager class for IO operations
/// </summary>
public class XMLManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    private string pathToLogFolder = "Logs\\";

    /// <summary>
    /// 
    /// </summary>
    private int nextFd = 0;

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<int, XmlWriter> openDocuments = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int openLogFileForHapticsPosition(string name)
    {
        if (openDocuments == null)
            openDocuments = new Dictionary<int, XmlWriter>();

        // create write xml file and add it to the openDocuments
        int key = nextFd++;
        DateTime date = DateTime.Now;
        XmlWriter xmlWriter = XmlWriter.Create(pathToLogFolder + name + "_" + date.ToString("yy-MM-dd-HH-mm-ss") + ".xml");
        openDocuments.Add(key, xmlWriter);

        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("movements");

        xmlWriter.Flush();

        return key;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="attributes"></param>
    public void logPointEntry(int key, Dictionary<string, string> attributes)
    {
        XmlWriter xmlWriter = openDocuments[key];

        xmlWriter.WriteStartElement("point");

        if (attributes != null)
            foreach (var a in attributes)
            {
                xmlWriter.WriteAttributeString(a.Key, a.Value);
            }

        xmlWriter.WriteEndElement();

        xmlWriter.Flush();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="time"></param>
    /// <param name="position"></param>
    public void logEntry(int key, double time, Vector3 position)
    {
        Dictionary<string, string> attrs = new Dictionary<string, string>();
        attrs.Add("id", time.ToString());
        attrs.Add("x", position.x.ToString());
        attrs.Add("y", position.y.ToString());
        attrs.Add("z", position.z.ToString());
        attrs.Add("t", time.ToString());

        logPointEntry(key, attrs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void closeFileEntry(int key)
    {
        XmlWriter xmlWriter = openDocuments[key];
        xmlWriter.WriteEndElement();

        xmlWriter.Flush();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void openFileEntry(int key, int id)
    {
        XmlWriter xmlWriter = openDocuments[key];

        xmlWriter.WriteStartElement("movement");
        xmlWriter.WriteAttributeString("id", id.ToString());

        xmlWriter.Flush();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    public void closeFile(int key)
    {
        try
        {
            XmlWriter xmlWriter = openDocuments[key];
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            openDocuments.Remove(key);
        }
        catch (Exception) { }
    }
}
