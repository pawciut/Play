﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

class SongParser
{
    public static bool ParseSongFile(string path, Encoding enc = null)
    {
        Note lastNote = null; //Holds last parsed note. Get's reset on player change
        bool endFound = false; // True if end tag was found

        int player = 1;

        char[] trimChars = { ' ', ':' };
        char[] splitChars = { ' ' };

        Dictionary<ESongHeader, System.Object> headers = new Dictionary<ESongHeader, System.Object>();
        List<List<Sentence>> voicesSentences = new List<List<Sentence>>
        {
            new List<Sentence>(),
            new List<Sentence>(),
            new List<Sentence>()
        };

        try
        {
            using (StreamReader reader = TxtReader.GetFileStreamReader(path, enc))
            {
                bool finishedHeaders = false;
                string directory = new FileInfo(path).Directory.FullName;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == "" || line[0].Equals(" ")
                        || (finishedHeaders && line[0].Equals('#')))
                    {
                        Debug.Log(String.Format("Invalid linestart found in {0} :: \"{1}\". Aborting.", path, line);
                        return false;
                    }
                    if (!finishedHeaders && line[0].Equals('#'))
                    {
                        int pos = line.IndexOf(":", StringComparison.Ordinal);
                        string identifier = line.Substring(1, pos - 1).Trim().ToUpper();
                        if (identifier.Contains(" ") || identifier.Length < 2)
                        {
                            HandleParsingError("invalid file...", EParsingErrorSeverity.Critical);
                            continue;
                        }
                        string tag = line.Substring(pos + 1).Trim();

                        if (tag.Equals(string.Empty))
                        {
                            // invalid tag.
                            HandleParsingError("Invalid empty tag found", EParsingErrorSeverity.Minor);
                            continue;
                        }

                        if (identifier.Equals("ENCODING"))
                        {
                            if (tag.Equals("UTF8"))
                            {
                                tag = "UTF-8";
                            }
                            Encoding newEncoding = Encoding.GetEncoding(tag);
                            if (!newEncoding.Equals(reader.CurrentEncoding))
                            {
                                reader.Dispose();
                                return ParseSongFile(path, newEncoding);
                            }
                        }

                        identifier = ParseHeaderField(headers, directory, identifier, tag);
                    }
                    else
                    {
                        if (!finishedHeaders)
                        {
                            finishedHeaders = true;
                        }
                        ParseLyricsTxtLine(ref lastNote, ref endFound, ref player, trimChars, splitChars, ref line);
                    }
                }

                if (reader.EndOfStream && !finishedHeaders)
                {
                    HandleParsingError("Lyrics/Notes missing", EParsingErrorSeverity.Critical);
                }

                CheckMinimalRequiredHeaders(headers);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error reading song file" + e.Message);
            return false;
        }
        Song song = new Song(headers, voicesSentences, path);
        SongsManager.AddSongs(song);
        return true;
    }

    private static void CheckMinimalRequiredHeaders(Dictionary<ESongHeader, object> headers)
    {
        if (headers[ESongHeader.Title] == null)
        {
            HandleParsingError("Title tag missing", EParsingErrorSeverity.Critical);
        }

        if (headers[ESongHeader.Artist] == null)
        {
            HandleParsingError("Artist tag missing", EParsingErrorSeverity.Critical);
        }

        if (headers[ESongHeader.Mp3] == null)
        {
            HandleParsingError("MP3 tag missing", EParsingErrorSeverity.Critical);
        }

        if (headers[ESongHeader.Bpm] == null)
        {
            HandleParsingError("BPM tag missing", EParsingErrorSeverity.Critical);
        }
    }

    private static void ParseLyricsTxtLine(ref Note lastNote, ref bool endFound, ref int player, char[] trimChars, char[] splitChars, ref string line)
    {
        char tag = line[0];
        line = (line.Length >= 2 && line[1] == ' ') ? line.Substring(2) : line.Substring(1);

        int startBeat, length;
        switch (tag)
        {
            case 'E':
                endFound = true;
                break;
            case 'P':
                line = line.Trim(trimChars);

                if (!int.TryParse(line, out player))
                {
                    HandleParsingError("Wrong or missing number after \"P\"", EParsingErrorSeverity.Critical);
                }
                lastNote = null;
                break;
            case ':':
            case '*':
            case 'F':
                string[] noteData = line.Split(splitChars, 4);
                if (noteData.Length < 4)
                {
                    if (noteData.Length == 3)
                    {
                        HandleParsingError("Ignored note without text", EParsingErrorSeverity.Minor);
                        break;
                    }
                    HandleParsingError("Invalid note found", EParsingErrorSeverity.Critical);
                }
                int pitch;
                if (!int.TryParse(noteData[0], out startBeat)
                    || !int.TryParse(noteData[1], out length)
                    || !int.TryParse(noteData[2], out pitch))
                {
                    HandleParsingError("Invalid note found (non-numeric values)", EParsingErrorSeverity.Critical);
                    break;
                }
                string text = TxtReader.NormalizeWhiteSpaceForLoop(noteData[3]);
                if (text == "")
                {
                    HandleParsingError("Ignored note without text", EParsingErrorSeverity.Minor);
                    break;
                }

                ENoteType noteType;

                if (tag.Equals('*'))
                {
                    noteType = ENoteType.Golden;
                }
                else if (tag.Equals('F'))
                {
                    noteType = ENoteType.Freestyle;
                }
                else
                {
                    noteType = ENoteType.Normal;
                }

                lastNote = new Note(pitch, startBeat, length, text, noteType);
                break;
            case '-':
                string[] lineBreakData = line.Split(splitChars);
                if (lineBreakData.Length < 1)
                {
                    HandleParsingError("Invalid line break found (No beat)", EParsingErrorSeverity.Critical);
                }
                if (!int.TryParse(lineBreakData[0], out startBeat))
                {
                    HandleParsingError("Invalid line break found (Non-numeric value)", EParsingErrorSeverity.Critical);
                }

                if (lastNote != null && startBeat <= lastNote.m_startBeat + lastNote.m_length - 1)
                {
                    HandleParsingError("Line break is before previous note end. Adjusted.", EParsingErrorSeverity.Minor);
                    startBeat = lastNote.m_startBeat + lastNote.m_length;
                }

                if (startBeat < 1)
                {
                    HandleParsingError("Ignored line break because position is < 1", EParsingErrorSeverity.Minor);
                }
                break;
            default:
                HandleParsingError("Unexpected or missing character (" + tag + ")", EParsingErrorSeverity.Critical);
                break;
        }
    }

    private static string ParseHeaderField(Dictionary<ESongHeader, object> headers, string directory, string identifier, string fieldValue)
    {
        switch (identifier)
        {
            case "ENCODING":
                // handled outside the switch. Nothing to do here
                break;
            case "TITLE":
                headers[ESongHeader.Title] = fieldValue.Trim();
                break;
            case "ARTIST":
                headers[ESongHeader.Artist] = fieldValue.Trim();
                break;
            case "CREATOR":
            case "AUTHOR":
            case "AUTOR":
                headers[ESongHeader.Creator] = fieldValue.Trim();
                break;
            case "MP3":
                if (File.Exists(Path.Combine(directory, fieldValue.Trim())))
                {
                    headers[ESongHeader.Mp3] = fieldValue.Trim();
                }
                else
                {
                    HandleParsingError("Can't find audio file: " + Path.Combine(directory, fieldValue), EParsingErrorSeverity.Critical);
                }
                break;
            case "BPM":
                float result;
                if (TryParse(fieldValue, out result))
                {
                    headers[ESongHeader.Bpm] = result;
                }
                else
                {
                    HandleParsingError("Invalid BPM value", EParsingErrorSeverity.Critical);
                }
                break;
            case "EDITION":
                if (fieldValue.Length > 1)
                {
                    headers[ESongHeader.Edition] = fieldValue.Trim();
                }
                else
                {
                    HandleParsingError("Invalid edition", EParsingErrorSeverity.Minor);
                }
                break;
            case "GENRE":
                if (fieldValue.Length > 1)
                {
                    headers[ESongHeader.Genre] = fieldValue.Trim();
                }
                else
                {
                    HandleParsingError("Invalid genre", EParsingErrorSeverity.Minor);
                }
                break;
            case "ALBUM":
                headers[ESongHeader.Edition] = fieldValue.Trim();
                break;
            case "YEAR":
                int num;
                if (fieldValue.Length == 4 && int.TryParse(fieldValue, out num) && num > 0)
                {
                    headers[ESongHeader.Year] = num;
                }
                else
                {
                    HandleParsingError("Invalid year", EParsingErrorSeverity.Minor);
                }
                break;
            case "LANGUAGE":
                if (fieldValue.Length > 1)
                {
                    headers[ESongHeader.Language] = fieldValue.Trim();
                }
                else
                {
                    HandleParsingError("Invalid language", EParsingErrorSeverity.Minor);
                }
                break;
            case "GAP":
                float resultGap;
                if (TryParse(fieldValue, out resultGap))
                {
                    headers[ESongHeader.Gap] = resultGap / 1000f;
                }
                else
                {
                    HandleParsingError("Invalid gap", EParsingErrorSeverity.Minor);
                }
                break;
            case "COVER":
                if (File.Exists(Path.Combine(directory, fieldValue)))
                {
                    headers[ESongHeader.Cover] = fieldValue;
                }
                else
                {
                    HandleParsingError("Can't find cover file: " + Path.Combine(directory, fieldValue), EParsingErrorSeverity.Minor);
                }
                break;
            case "BACKGROUND":
                if (File.Exists(Path.Combine(directory, fieldValue)))
                {
                    headers[ESongHeader.Background] = fieldValue;
                }
                else
                {
                    HandleParsingError("Can't find background file: " + Path.Combine(directory, fieldValue), EParsingErrorSeverity.Minor);
                }
                break;
            case "VIDEO":
                if (File.Exists(Path.Combine(directory, fieldValue)))
                {
                    headers[ESongHeader.Video] = fieldValue;
                }
                else
                {
                    HandleParsingError("Can't find video file: " + Path.Combine(directory, fieldValue), EParsingErrorSeverity.Minor);
                }
                break;
            case "VIDEOGAP":
                float resultVideoGap;
                if (TryParse(fieldValue, out resultVideoGap))
                {
                    headers[ESongHeader.Videogap] = resultVideoGap;
                }
                else
                {
                    HandleParsingError("Invalid videogap", EParsingErrorSeverity.Minor);
                }
                break;
            case "START":
                float resultStart;
                if (TryParse(fieldValue, out resultStart))
                {
                    headers[ESongHeader.Start] = resultStart;
                }
                else
                {
                    HandleParsingError("Invalid start", EParsingErrorSeverity.Critical);
                }
                break;
            case "END":
                float resultFinish;
                if (TryParse(fieldValue, out resultFinish))
                {
                    headers[ESongHeader.End] = resultFinish / 1000f;
                }
                else
                {
                    HandleParsingError("Invalid end", EParsingErrorSeverity.Critical);
                }
                break;
            
            default:
                if (identifier.StartsWith("DUETSINGER"))
                {
                    identifier = identifier.Substring(10);
                    if (!identifier.StartsWith("P"))
                    {
                        identifier = "P" + identifier;
                    }
                }
                if (!identifier.StartsWith("P"))
                {
                    HandleParsingError("Unknown tag: #" + identifier, EParsingErrorSeverity.Minor);
                }

                break;
        }

        return identifier;
    }

    private static bool TryParse<T>(string input, out T result, bool ignoreCase = false)
            where T : struct
    {
        result = default(T);
        try
        {
            result = (T)Enum.Parse(typeof(T), input, ignoreCase);
            return true;
        }
        catch (Exception e) 
        {
            Debug.Log(e);
        }

        return false;
    }

    private static bool TryParse(string input, out float result)
    {
        string inputAsWeWant = input.Replace(',', '.');
        return Single.TryParse(inputAsWeWant, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
    }

    private static void HandleParsingError(string errorMessage, EParsingErrorSeverity errorSeverity)
    {
        switch (errorSeverity)
        {
            case EParsingErrorSeverity.Critical:
                throw new SongParserException("Critical parsing error in file.\nInner error message: " + errorMessage);
            default:
                Debug.Log(errorMessage);
                break;

        }
    }

    enum EParsingErrorSeverity
    {
        Minor,
        Critical
    }
}

public class SongParserException : Exception 
{
    public SongParserException(string message)
        : base(message) 
    {
    }

    public SongParserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
