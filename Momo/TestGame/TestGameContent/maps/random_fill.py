#!/bin/python

import xml.dom.minidom
import sys
import random

if len(sys.argv) < 4:
    print "Usage: random_fill.py filename layer numbers"

filenameArg = sys.argv[1]
layerArg = sys.argv[2]
tilesArg = sys.argv[3:]

doc = xml.dom.minidom.parse(filenameArg)

layerNodes = doc.getElementsByTagName("layer")

for layerNode in layerNodes:
	if(layerNode.attributes["name"].nodeValue == layerArg):
		dataNodes = layerNode.getElementsByTagName("data")
		for dataNode in dataNodes:
			childNodes = dataNode.childNodes
			for childNode in childNodes:
				if childNode.nodeType == childNode.TEXT_NODE:
					csv = childNode.nodeValue
					tiles = csv.split(",")
					newTiles = []
					for tile in tiles:
						if tile != "0":
							tile = tilesArg[random.randint(0, len(tilesArg)-1)]
						newTiles.append(tile)
					csv = ",".join(newTiles)
					childNode.nodeValue = csv

print doc.toprettyxml()