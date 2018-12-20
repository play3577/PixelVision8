﻿//   
// Copyright (c) Jesse Freeman. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany

using System;
using System.Collections.Generic;
using System.Linq;
using PixelVisionRunner.Utils;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.Parsers
{
    public class TilemapJsonParser : JsonParser
	{
		protected IEngine target;
		
		public TilemapJsonParser(string jsonString, IEngine target) : base(jsonString)
		{
			this.target = target;
		}
		
		public override void CalculateSteps()
		{
			base.CalculateSteps();
			steps.Add(ConfigureTilemap);
		}

		public virtual void ConfigureTilemap()
		{

			var tilemapChip = target.tilemapChip;
			
			
			if (data.ContainsKey("layers"))
			{

				var layers = data["layers"] as List<object>;
				var tileSets = data["tilesets"] as List<object>;
				
				var total = layers.Count;

				for (int i = 0; i < total; i++)
				{
					try
					{
						var layer = layers[i] as Dictionary<string, object>;
						
	
						var layerType = (string)layer["type"];

						if (layerType == "tilelayer")
						{

							var tileSet = tileSets[i] as Dictionary<string, object>;


							var offset = (int) (long) tileSet["firstgid"];

							var columns = (int) (long) layer["width"];
							var rows = (int) (long) layer["height"];

							var rawLayerData = layer["data"] as List<object>;

							int[] dataValues = rawLayerData
								.Select(x => ((int) (long) x) - offset < -1 ? -1 : ((int) (long) x) - offset).ToArray();

							if (columns != tilemapChip.columns || rows > tilemapChip.rows)
							{

								// Create texture data that matches the memory of the tilemap chip
								var tmpPixelData = new TextureData(tilemapChip.columns, tilemapChip.rows);
								tmpPixelData.Clear();

								var jsonData = new TextureData(columns, rows);
								jsonData.Clear();
								jsonData.SetPixels(0, 0, columns, rows, dataValues);

								var tmpCol = columns > tilemapChip.columns ? tilemapChip.columns : columns;
								var tmpRow = rows > tilemapChip.rows ? tilemapChip.rows : rows;

								if (tmpCol > columns)
									tmpCol = columns;

								if (tmpRow > rows)
									tmpRow = rows;

								var tmpData = new int[tmpCol * tmpRow];

								jsonData.CopyPixels(ref tmpData, 0, 0, tmpCol, tmpRow);

								tmpPixelData.SetPixels(0, 0, tmpCol, tmpRow, tmpData);

								tmpPixelData.CopyPixels(ref dataValues, 0, 0, tmpPixelData.width, tmpPixelData.height);

//							var jsonMap = new TextureData(columns, rows);
//							jsonMap.SetPixels(0, 0, columns, rows, dataValues);
//							
//							
//							Debug.Log("Resize " + tilemapChip.columns +"x"+tilemapChip.rows + " " + columns + "x"+rows);
//							
//							var tmpPixelData = new TextureData(columns, rows);
//							tmpPixelData.Clear();
//
//							var totalData = dataValues.Length;
//							
//							for (int j = 0; j < totalData; j++)
//							{
//								var pos = target.gameChip.CalculatePosition(j, columns);
//								
//								
//								
//								
//								
//							}
//							tmpPixelData.SetPixels(0, 0, columns, rows, dataValues);
//							
//							Array.Resize(ref dataValues, tilemapChip.total);
//							
								tmpPixelData.CopyPixels(ref dataValues, 0, 0, tilemapChip.columns, tilemapChip.rows);
							}

							var layerName =
								(TilemapChip.Layer) Enum.Parse(typeof(TilemapChip.Layer), ((string) layer["name"]));

							Array.Copy(dataValues, tilemapChip.layers[(int) layerName], dataValues.Length);

						}
						else if (layerType == "objectgroup")
						{
							var tiles = layer["objects"] as List<object>;

							var totalTiles = tiles.Count;

							for (int j = 0; j < totalTiles; j++)
							{
								var tileData = tiles[j] as Dictionary<string, object>;
								
								var column = (int)Math.Floor(((float)(long) tileData["x"])/8);
								var row = (int)Math.Floor(((float)(long) tileData["y"])/8) - 1;

								var gID = (uint) (long) tileData["gid"];

								int spriteID = -1;
								var flipH = false;
								var flipV = false;

								ReadGID(gID, out spriteID, out flipH, out flipV);
								
//								Console.WriteLine(j + " Sprite "+spriteID+" Flip " + flipH + " " +flipV);
								
								var properties = tileData["properties"] as List<object>;

								int flagID = -1;
								int colorOffset = 0;
								
								for (int k = 0; k < properties.Count; k++)
								{
									var prop = properties[i] as Dictionary<string, object>;

									var propName = (string)prop["name"];
									
									if (propName  == "flagID")
									{
										flagID = (int) (long) prop["value"];
									}else if (propName == "colorOffset")
									{
										colorOffset = (int) (long) prop["value"];
									}
								}
								
								tilemapChip.UpdateTileAt((spriteID - 1), column, row, flagID, colorOffset, flipH, flipV);
							}
						}
						// TODO need to make sure that the layer is the same size as the display chip

						// TODO copy the tilemap data over to layer correctly

					}
					catch (Exception e)
					{
						// Just igonre any layers that don't exist
						throw new Exception("Unable to parse 'tilemap.json' file. It may be corrupt. Try deleting it and creating a new one.");
					}
					

				}
				
			}
			
			currentStep++;

		}
		
		public void ReadGID(uint gid, out int id, out bool flipH, out bool flipV)
		{
        
			// Starts with 0, 31 in a uint
        
			// Create mask by subtracting the bits you don't want

			var idMask = (1 << 30) - 1;

			id = (int)(gid & idMask);

			var hMask = 1 << 31;

			flipH = (hMask & gid) != 0;
        
			var vMask = 1 << 30;

			flipV = (vMask & gid) != 0;
        
		}
	}
}