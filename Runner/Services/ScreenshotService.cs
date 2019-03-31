﻿//   
// Copyright (c) Jesse Freeman, Pixel Vision 8. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) except for a few
// portions of the code. See LICENSE file in the project root for full 
// license information. Third-party libraries used by Pixel Vision 8 are 
// under their own licenses. Please refer to those libraries for details 
// on the license they use.
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christina-Antoinette Neofotistou @CastPixel
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
//

using System;
using PixelVision8.Engine;
using PixelVision8.Engine.Services;
using SharpFileSystem;

namespace PixelVision8.Runner.Services
{
    public class ScreenshotService : AbstractService
    {
        private bool active;

//        private ITextureFactory textureFactory;
        private readonly WorkspaceService workspace;

        public ScreenshotService(WorkspaceService workspace)
        {
            // TODO this needs to get teh workspace through the service
//            this.textureFactory = textureFactory;
            this.workspace = workspace;
        }

        private FileSystemPath screenshotDir
        {
            get
            {
                var fileSystem = workspace.fileSystem;
                try
                {
                    var directoryName = workspace.ReadBiosData("ScreenshotDir", "Screenshots") as string;

                    var path = FileSystemPath.Root.AppendDirectory("Tmp").AppendDirectory(directoryName);

                    try
                    {
                        if (fileSystem.Exists(FileSystemPath.Root.AppendDirectory("Workspace")))
                            path = FileSystemPath.Root.AppendDirectory("Workspace")
                                .AppendDirectory(directoryName);
                    }
                    catch
                    {
//                        Console.WriteLine("Screenshot Error: No workspace found.");
                    }

                    // Check to see if a screenshot directory exits
                    if (!fileSystem.Exists(path)) fileSystem.CreateDirectoryRecursive(path);

                    active = true;

                    return path;
                }
                catch
                {
//                    Console.WriteLine("Save Screenshot Error:\n"+e.Message);
                }

                return FileSystemPath.Root;
            }
        }

        public FileSystemPath GenerateScreenshotName()
        {
            return workspace.UniqueFilePath(screenshotDir.AppendFile("screenshot.png"));
        }

        public bool TakeScreenshot(IEngine engine)
        {
            throw new NotImplementedException();

//            var fileName = GenerateScreenshotName().Path;
//            
//            if (active == false)
//                return active;
//            
//            var pixels = engine.displayChip.pixels;
//
//            var displaySize = engine.gameChip.Display();
//
//            // Need to crop the image
//
//            // Create a tmporary texture data class
//            var tmpTD = new TextureData(engine.displayChip.width, engine.displayChip.height);
//
//            // Set the pixel data from the display
//            tmpTD.SetPixels(pixels);
//
//            // Copy just the visible pixles over
//            tmpTD.CopyPixels(ref pixels, 0, 0, displaySize.x, displaySize.y);
//
//            var imageExporter = new PNGWriter();
//
//            // We need to do this manually since the exporter could be active and we don't want to break it for a screenshot
//            var tmpExporter = new PixelDataExporter(fileName, pixels, displaySize.x, displaySize.y,
//                engine.colorChip.colors, imageExporter);
//            tmpExporter.CalculateSteps();
//
//            // Manually step through the exporter
//            while (tmpExporter.completed == false)
//                tmpExporter.NextStep();
//
//            try
//            {
//                workspace.SaveExporterFiles(new Dictionary<string, byte[]> {{tmpExporter.fileName, tmpExporter.bytes}});
//                
//                return true;
//            }
//            catch
//            {
////                Console.WriteLine("Take Screenshot Error:\n"+e.Message);
//                // TODO throw some kind of error?
//                return false;
//            }
        }
    }
}