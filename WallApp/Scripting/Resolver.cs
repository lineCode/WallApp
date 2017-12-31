﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WallApp.Scripting.Cs;

namespace WallApp.Scripting
{
    class Resolver
    {
        public static Dictionary<string, Module> Cache { get; private set; }

        static Resolver()
        {
            Cache = new Dictionary<string, Module>();
        }

        public static void LoadModules(string rootDirectory)
        {
            Cache.Clear();
            List<Module> modules = new List<Module>();

            string[] directories = Directory.GetDirectories(rootDirectory);
            foreach (var directory in directories)
            {
                var dir = directory.TrimEnd('\\') + "\\";
                var module = ScanDirectory(dir);
                if (module != null)
                {
                    modules.Add(module);
                    Cache.Add(dir + "manifest.xml", module);
                }
            }
        }

        private static Module ScanDirectory(string directory)
        {
            string manifestPath = directory + "manifest.xml";
            if (!File.Exists(manifestPath))
            {
                //TODO
            }
            return ScanManifest(manifestPath);
        }

        public static Module ScanManifest(string manifestFile)
        {
            var doc = XDocument.Load(manifestFile);
            var root = doc.Root;

            string sourceFile = "";
            string name = "";
            string description = "";
            int minWidth = 0;
            int minHeight = 0;
            int maxWidth = int.MaxValue;
            int maxHeight = int.MaxValue;
            bool allowsCustomEffects = false;

            foreach (var xElement in root.Elements())
            {
                if (xElement.Name == "source")
                {
                    sourceFile = xElement.Value;
                }
                else if (xElement.Name == "name")
                {
                    name = xElement.Value;
                }
                else if (xElement.Name == "description")
                {
                    description = xElement.Value;
                }
                else if (xElement.Name == "minwidth")
                {
                    if (!int.TryParse(xElement.Value, out minWidth))
                    {
                        //TODO: Warning
                    }
                }
                else if (xElement.Name == "minheight")
                {
                    if (!int.TryParse(xElement.Value, out minHeight))
                    {
                        //TODO: Warning
                    }
                }
                else if (xElement.Name == "maxwidth")
                {
                    if (!int.TryParse(xElement.Value, out maxWidth))
                    {
                        //TODO: Warning
                    }
                }
                else if (xElement.Name == "maxheight")
                {
                    if (!int.TryParse(xElement.Value, out maxHeight))
                    {
                        //TODO: Warning
                    }
                }
                else if (xElement.Name == "customeffects")
                {
                    if (!bool.TryParse(xElement.Value, out allowsCustomEffects))
                    {
                        //TODO: Warning
                    }
                }
            }

            if (string.IsNullOrEmpty(sourceFile) || string.IsNullOrEmpty(name))
            {
                //TODO: Exception
            }

            if (!File.Exists(sourceFile))
            {
                sourceFile = Path.GetDirectoryName(manifestFile).TrimEnd('\\') + '\\' + sourceFile;
                if (!File.Exists(sourceFile))
                {
                    //TODO
                }
            }

            string kind = Path.GetExtension(sourceFile).TrimStart('.');
            var module = Resolve(kind);
            module.Init(manifestFile, sourceFile, name, description, minWidth, minHeight, maxWidth, maxHeight, allowsCustomEffects);
            return module;
        }

        private static Module Resolve(string kind)
        {
            Module module = null;
            if (kind == "csx" || kind == "cs")
            {
                module = new CsModule();
            }
            return module;
        }
    }
}